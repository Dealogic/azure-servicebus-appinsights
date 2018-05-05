namespace Dealogic.ServiceBus.Azure.ApplicationInsights
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.Azure.ServiceBus;

    /// <summary>
    /// Service bus telemetry helper
    /// </summary>
    public class ServiceBusTelemetryHelper : IServiceBusTelemetryHelper
    {
        /// <summary>
        /// The root identifier name
        /// </summary>
        public const string RootIdName = "RootId";

        /// <summary>
        /// The parent identifier name
        /// </summary>
        public const string ParentIdName = "ParentId";

        /// <summary>
        /// The dequeue operation name
        /// </summary>
        public const string DequeueOperationName = "Dequeue";

        /// <summary>
        /// The enqueue operation name
        /// </summary>
        public const string EnqueueOperationName = "Enqueue";

        private readonly TelemetryClient telemetryClient;
        private readonly Uri endpointUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBusTelemetryHelper"/> class. A new
        /// default telemetry client will be used.
        /// </summary>
        /// <param name="serviceBusConnectionString">The service bus connection string.</param>
        public ServiceBusTelemetryHelper(string serviceBusConnectionString)
            : this(serviceBusConnectionString, new TelemetryClient())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBusTelemetryHelper"/> class.
        /// </summary>
        /// <param name="serviceBusConnectionString">The service bus connection string.</param>
        /// <param name="telemetryClient">The telemetry client.</param>
        /// <exception cref="ArgumentException">
        /// Service Bus connection string is null or empty. - serviceBusConnectionString
        /// </exception>
        public ServiceBusTelemetryHelper(string serviceBusConnectionString, TelemetryClient telemetryClient)
        {
            if (string.IsNullOrWhiteSpace(serviceBusConnectionString))
            {
                throw new ArgumentException("Service Bus connection string is null or empty.", nameof(serviceBusConnectionString));
            }

            this.telemetryClient = telemetryClient ?? new TelemetryClient();
            var connectionStringBuilder = new ServiceBusConnectionStringBuilder(serviceBusConnectionString);
            this.endpointUri = new Uri(connectionStringBuilder.Endpoint);
        }

        /// <inheritdoc/>
        public TelemetryClient TelemetryClient => telemetryClient;

        /// <inheritdoc/>
        public bool TrackException { get; set; } = true;

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)", Justification = "Constant values")]
        public IMessageScope BeginMessageSendingScope(Message brokeredMessage, string entityPath)
        {
            if (brokeredMessage == null)
            {
                return new NoScope();
            }

            if (string.IsNullOrWhiteSpace(entityPath))
            {
                throw new ArgumentException("Entity path is null or empty.", nameof(entityPath));
            }

            var operation = this.telemetryClient.StartOperation<DependencyTelemetry>($"{EnqueueOperationName} {entityPath}");
            operation.Telemetry.Type = "Queue";
            operation.Telemetry.Target = $"{this.endpointUri.Host}/{entityPath}";
            operation.Telemetry.Data = EnqueueOperationName;
            operation.Telemetry.Properties.Add(nameof(Message.MessageId), brokeredMessage.MessageId);

            brokeredMessage.UserProperties.Add(ParentIdName, operation.Telemetry.Id);
            brokeredMessage.UserProperties.Add(RootIdName, operation.Telemetry.Context.Operation.Id);

            return new SendMessageScope(this.telemetryClient, operation);
        }

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)", Justification = "Constant values")]
        public IMessageScope BeginMessageProcessingScope(Message brokeredMessage, string entityPath)
        {
            if (brokeredMessage == null)
            {
                return new NoScope();
            }

            if (string.IsNullOrWhiteSpace(entityPath))
            {
                throw new ArgumentException("Entity path is null or empty.", nameof(entityPath));
            }

            var rootId = brokeredMessage.UserProperties.TryGetValue(RootIdName, out object r) ? (string)r : null;
            var parentId = brokeredMessage.UserProperties.TryGetValue(ParentIdName, out object p) ? (string)p : null;

            var requestTelemetry = new RequestTelemetry
            {
                Name = $"{DequeueOperationName} {entityPath}"
            };

            if (rootId != null)
            {
                requestTelemetry.Context.Operation.Id = rootId;
            }

            if (parentId != null)
            {
                requestTelemetry.Context.Operation.ParentId = parentId;
            }

            requestTelemetry.Properties.Add(nameof(Message.MessageId), brokeredMessage.MessageId);
            requestTelemetry.Url = new Uri($"{this.endpointUri.Host}/{entityPath}", UriKind.Relative);

            var operation = this.telemetryClient.StartOperation(requestTelemetry);

            return new ProcessMessageScope(this.telemetryClient, operation);
        }

        /// <summary>
        /// Begins the batch message sending scope.
        /// </summary>
        /// <param name="messages">The messages.</param>
        /// <param name="entityPath">The entity path.</param>
        /// <returns>Message scope</returns>
        /// <exception cref="ArgumentException">Entity path is null or empty. - entityPath</exception>
        public IMessageScope BeginMessageSendingScope(IEnumerable<Message> messages, string entityPath)
        {
            if (messages?.Any() == false)
            {
                return new NoScope();
            }

            if (string.IsNullOrWhiteSpace(entityPath))
            {
                throw new ArgumentException("Entity path is null or empty.", nameof(entityPath));
            }

            var operation = this.telemetryClient.StartOperation<DependencyTelemetry>($"{EnqueueOperationName} {entityPath}");
            operation.Telemetry.Type = "Queue";
            operation.Telemetry.Target = $"{this.endpointUri.Host}/{entityPath}";
            operation.Telemetry.Data = EnqueueOperationName;

            foreach (var brokeredMessage in messages)
            {
                brokeredMessage.UserProperties.Add(ParentIdName, operation.Telemetry.Id);
                brokeredMessage.UserProperties.Add(RootIdName, operation.Telemetry.Context.Operation.Id);
            }

            return new SendMessageScope(this.telemetryClient, operation);
        }
    }
}