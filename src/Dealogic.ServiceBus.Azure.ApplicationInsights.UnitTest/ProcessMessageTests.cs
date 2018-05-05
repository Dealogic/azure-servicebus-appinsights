namespace Dealogic.ServiceBus.Azure.ApplicationInsights.UnitTest
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    [TestClass]
    public class ProcessMessageTests
    {
        public const string FakeConnectionString = "Endpoint=sb://fake.servicebus.windows.net/";

        [TestMethod]
        public async Task ProcessMessage_Success()
        {
            var mockMessage = new Message();
            var telemetryHelper = Substitute.ForPartsOf<ServiceBusTelemetryHelper>(FakeConnectionString);

            await telemetryHelper.ProcessMessageAsync(mockMessage, "FakeEntityPath", token =>
            {
                return Task.CompletedTask;
            }, CancellationToken.None).ConfigureAwait(false);

            telemetryHelper.Received().BeginMessageProcessingScope(mockMessage, "FakeEntityPath");
        }

        [TestMethod]
        public async Task BeginEndMessageProcessScope_Success()
        {
            var mockMessage = new Message();
            var applicationInsightsHelper = new ServiceBusTelemetryHelper(FakeConnectionString);

            using (var scope = applicationInsightsHelper.BeginMessageProcessingScope(mockMessage, "FakeEntityPath"))
            {
                // Simulate some work
                await Task.Delay(50).ConfigureAwait(false);
                scope.SetSuccess(true);
            }
        }

        [TestMethod]
        public void ProcessMessage_NullMessage_NoScopeReturned()
        {
            var applicationInsightsHelper = new ServiceBusTelemetryHelper(FakeConnectionString);
            var scope = applicationInsightsHelper.BeginMessageProcessingScope(null, "FakeEntityPath");

            Assert.IsInstanceOfType(scope, typeof(NoScope));
        }

        [TestMethod]
        public void SendMessage_NullMessage_NoScopeReturned()
        {
            var applicationInsightsHelper = new ServiceBusTelemetryHelper(FakeConnectionString);
            var scope = applicationInsightsHelper.BeginMessageSendingScope((Message)null, "FakeEntityPath");

            Assert.IsInstanceOfType(scope, typeof(NoScope));
        }
    }
}