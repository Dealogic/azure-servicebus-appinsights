namespace Dealogic.ServiceBus.Azure.ApplicationInsights
{
    using System.Collections.Generic;
    using Microsoft.ApplicationInsights;
    using Microsoft.Azure.ServiceBus;

    /// <summary>
    /// Service Bus telemetry helper interface
    /// </summary>
    public interface IServiceBusTelemetryHelper
    {
        /// <summary>
        /// Gets the telemetry client.
        /// </summary>
        /// <value>The telemetry client.</value>
        TelemetryClient TelemetryClient { get; }

        /// <summary>
        /// Gets or sets a value indicating whether [track exception].
        /// </summary>
        /// <value><c>true</c> if [track exception]; otherwise, <c>false</c>.</value>
        /// <remarks>The default value is <c>true</c></remarks>
        bool TrackException { get; set; }

        /// <summary>
        /// Begins the message processing scope.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="entityPath">The entity path.</param>
        /// <returns>Message processing scope.</returns>
        IMessageScope BeginMessageProcessingScope(Message brokeredMessage, string entityPath);

        /// <summary>
        /// Begins the message sending scope.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="entityPath">The entity path.</param>
        /// <returns>Message sending scope.</returns>
        IMessageScope BeginMessageSendingScope(Message brokeredMessage, string entityPath);

        /// <summary>
        /// Begins the batch message sending scope.
        /// </summary>
        /// <param name="messages">The messages.</param>
        /// <param name="entityPath">The entity path.</param>
        /// <returns>Message scope</returns>
        IMessageScope BeginMessageSendingScope(IEnumerable<Message> messages, string entityPath);
    }
}