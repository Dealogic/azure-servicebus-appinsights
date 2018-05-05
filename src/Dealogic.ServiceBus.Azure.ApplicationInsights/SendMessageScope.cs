namespace Dealogic.ServiceBus.Azure.ApplicationInsights
{
    using System;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.ApplicationInsights.Extensibility.Implementation;

    /// <summary>
    /// Message scope for message sending.
    /// </summary>
    public class SendMessageScope : IMessageScope
    {
        /// <summary>
        /// The success result code
        /// </summary>
        public const string SuccessResultCode = "0";

        /// <summary>
        /// The failure result code
        /// </summary>
        public const string FailureResultCode = "1";

        private readonly TelemetryClient telemetryClient;
        private readonly IOperationHolder<DependencyTelemetry> operationHolder;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendMessageScope"/> class.
        /// </summary>
        /// <param name="telemetryClient">The telemetry client.</param>
        /// <param name="operationHolder">The operation holder.</param>
        /// <exception cref="ArgumentNullException">telemetryClient or operationHolder is null</exception>
        public SendMessageScope(TelemetryClient telemetryClient, IOperationHolder<DependencyTelemetry> operationHolder)
        {
            this.telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
            this.operationHolder = operationHolder ?? throw new ArgumentNullException(nameof(operationHolder));
        }

        /// <inheritdoc/>
        public OperationTelemetry Operation => this.operationHolder.Telemetry;

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <inheritdoc/>
        public void SetSuccess(bool success)
        {
            this.operationHolder.Telemetry.Success = success;
            this.operationHolder.Telemetry.ResultCode = success ? SuccessResultCode : FailureResultCode;
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
                GC.SuppressFinalize(this);
            }
        }
    }
}