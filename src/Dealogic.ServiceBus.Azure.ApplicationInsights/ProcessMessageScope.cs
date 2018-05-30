namespace Dealogic.ServiceBus.Azure.ApplicationInsights
{
    using System;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.ApplicationInsights.Extensibility.Implementation;

    /// <summary>
    /// Scope for message processing.
    /// </summary>
    public class ProcessMessageScope : IMessageScope
    {
        /// <summary>
        /// The success status code
        /// </summary>
        public const string SuccessStatusCode = "0";

        /// <summary>
        /// The failure status code
        /// </summary>
        public const string FailureStatusCode = "1";

        private readonly TelemetryClient telemetryClient;
        private readonly IOperationHolder<RequestTelemetry> operationHolder;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessMessageScope"/> class.
        /// </summary>
        /// <param name="telemetryClient">The telemetry client.</param>
        /// <param name="operationHolder">The operation holder.</param>
        /// <exception cref="ArgumentNullException">telemetryClient or operationHolder is null</exception>
        public ProcessMessageScope(TelemetryClient telemetryClient, IOperationHolder<RequestTelemetry> operationHolder)
        {
            this.telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
            this.operationHolder = operationHolder ?? throw new ArgumentNullException(nameof(operationHolder));
        }

        /// <inheritdoc/>
        public OperationTelemetry Operation => operationHolder.Telemetry;

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <inheritdoc/>
        public virtual void SetSuccess(bool success)
        {
            this.Operation.Success = success;
            this.operationHolder.Telemetry.ResponseCode = success ? SuccessStatusCode : FailureStatusCode;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        /// unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.telemetryClient.StopOperation(this.operationHolder);
                this.operationHolder.Dispose();
            }
        }
    }
}