# Dealogic Azure Service Bus Applicaiton Insights Extensions

By sending messages to a queue or topic this extension helps to standardise the usage of operation ids. 
On sending a message a Dependency call telemetry will be registered, on receiving a message a Request telemetry will registered.

## Content

* [Send a message](#send-a-message)
* [Process a message](#process-a-message)
* [Use message scope explicitly](#use-message-scope)
* [Implementation notes](#implementation-notes)

### <a id="send-a-message" /> Send a message to a Service Bus entity

Use the `SendMessageAsync` extension method (or the sync version) on the `ServiceBusTelemetryHelper` instance.
If any exception happens during the call of the deledate it will be tracked for the operation as well.

```csharp
var queueName = "YOUR QUEUE NAME";
var serviceBusConnectionString = "YOUR CONNECTION STRING";

var client = new QueueClient(serviceBusConnectionString, queueName);
var aiHelper = new ServiceBusTelemetryHelper(serviceBusConnectionString);
var brokeredMessage = new Message("some object");

await aiHelper.SendMessageAsync(brokeredMessage, queueName, () => client.SendAsync(brokeredMessage)).ConfigureAwait(false);
```

### <a id="process-a-message" /> Processing a received message from a Service Bus entity

Use the `ProcessMessageAsync` extension method (or the sync version) on the `ServiceBusTelemetryHelper` instance
If any exception happens during the call of the deledate it will be tracked for the operation as well.

```csharp
var queueName = "YOUR QUEUE NAME";
var serviceBusConnectionString = "YOUR CONNECTION STRING";

var client = new QueueClient(serviceBusConnectionString, queueName);
var aiHelper = new ServiceBusTelemetryHelper(serviceBusConnectionString);
client.RegisterMessageHandler((message) => aiHelper.ProcessMessageAsync(message, queueName, YourAsyncProcessDelegate), MessageHandlerOptions);
```

### <a id="use-message-scope" /> Use message scope explicitly

You can use message processing or sending scopes explicitly from the `ServiceBusTelemetryHelper` instance, but in that case
the scope has to be managed manually. On dispose it will close the telemetry operation. Before doing that make sure to call the `SetSuccess` method
to indicate the result of the operation.

## <a id="implementation-notes" /> Implementaiton notes
- The `ServiceBusTelemetryHelper` should be reused,
- If no `TelemetryClient` is supplied when creating the instance a default will be created,
- If custom telemetry data should be passed to the message scope explicit scope usage is recommended,
- MessageId will be automatically added as custom telemetry,
- Upon message sending two properties will be added to the message's properties:
  - **RootId**: The ID of the root operation where the scope is opened in,
  - **ParentId**: The ID of the parent operation where the scope is opened in.

## Contribution

The packages uses VSTS pipeline for build and release. The versioning is done by GitVersion.
From all feature (features) branches a new pre-release pacakges will be automatically released.
**After releasing a stable version, the version Tag has to be added to the code with the released version number.**
