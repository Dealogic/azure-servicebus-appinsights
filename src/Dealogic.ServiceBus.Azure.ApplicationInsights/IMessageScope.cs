namespace Dealogic.ServiceBus.Azure.ApplicationInsights
{
    using System;
    using Microsoft.ApplicationInsights.Extensibility.Implementation;

    /// <summary>
    /// Message scope interface
    /// </summary>
    /// <seealso cref="System.IDisposable"/>
    public interface IMessageScope : IDisposable
    {
        /// <summary>
        /// Gets the operation. This will return the associated telemetry.
        /// </summary>
        /// <value>The operation.</value>
        OperationTelemetry Operation { get; }

        /// <summary>
        /// Sets the success of the scope.
        /// </summary>
        /// <param name="success">if set to <c>true</c> [success].</param>
        void SetSuccess(bool success);
    }
}