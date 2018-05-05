namespace Dealogic.ServiceBus.Azure.ApplicationInsights
{
    using Microsoft.ApplicationInsights.Extensibility.Implementation;

    /// <summary>
    /// No messages scope
    /// </summary>
    public sealed class NoScope : IMessageScope
    {
        /// <summary>
        /// Gets the operation.
        /// </summary>
        /// <value>The operation.</value>
        public OperationTelemetry Operation => null;

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly", Justification = "Nothing to dispose")]
        public void Dispose()
        {
        }

        /// <inheritdoc/>
        public void SetSuccess(bool success)
        {
        }
    }
}