namespace Dealogic.ServiceBus.Azure.ApplicationInsights
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.ServiceBus;

    /// <summary>
    /// Service bus telemetry helper extensions
    /// </summary>
    public static class ServiceBusTelemetryHelperExtensions
    {
        /// <summary>
        /// Opens a dependency telemetry scope for message sending. If the action returns, the scope
        /// will be autmatically ended. If action runs without error, success flag will be set to
        /// true. If the action throws an exception it will be logged under the scope and the success
        /// code will be false.
        /// </summary>
        /// <param name="serviceBusTelemetryHelper">The service bus telemetry helper.</param>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="targetEntity">The target entity.</param>
        /// <param name="sendAction">The send action.</param>
        /// <exception cref="System.ArgumentNullException">sendAction is null</exception>
        /// <exception cref="System.ArgumentException">Target entity is null or empty. - targetEntity</exception>
        /// <exception cref="ArgumentNullException">sendAction is null</exception>
        /// <exception cref="ArgumentException">Target entity is null or empty. - targetEntity</exception>
        public static void SendMessage(this IServiceBusTelemetryHelper serviceBusTelemetryHelper, Message brokeredMessage, string targetEntity, Action sendAction)
        {
            if (brokeredMessage == null)
            {
                return;
            }

            if (sendAction == null)
            {
                throw new ArgumentNullException(nameof(sendAction));
            }

            if (string.IsNullOrWhiteSpace(targetEntity))
            {
                throw new ArgumentException("Target entity is null or empty.", nameof(targetEntity));
            }

            using (var scope = serviceBusTelemetryHelper.BeginMessageSendingScope(brokeredMessage, targetEntity))
            {
                try
                {
                    sendAction();
                    scope.SetSuccess(true);
                }
                catch (Exception ex)
                {
                    if (serviceBusTelemetryHelper.TrackException)
                    {
                        serviceBusTelemetryHelper.TelemetryClient.TrackException(ex);
                    }

                    scope.SetSuccess(false);
                    throw;
                }
            }
        }

        /// <summary>
        /// Opens a dependency telemetry scope for async message sending. If the action returns, the
        /// scope will be autmatically ended. If action runs without error, success flag will be set
        /// to true. If the action throws an exception it will be logged under the scope and the
        /// success code will be false.
        /// </summary>
        /// <param name="serviceBusTelemetryHelper">The service bus telemetry helper.</param>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="targetEntity">The target entity.</param>
        /// <param name="sendAction">The send action.</param>
        /// <returns>Task reference.</returns>
        /// <exception cref="System.ArgumentNullException">sendAction</exception>
        /// <exception cref="System.ArgumentException">Target entity is null or empty. - targetEntity</exception>
        /// <exception cref="ArgumentNullException">sendAction is null</exception>
        /// <exception cref="ArgumentException">Target entity is null or empty. - targetEntity</exception>
        public static async Task SendMessageAsync(this IServiceBusTelemetryHelper serviceBusTelemetryHelper, Message brokeredMessage, string targetEntity, Func<Task> sendAction)
        {
            if (brokeredMessage == null)
            {
                return;
            }

            if (sendAction == null)
            {
                throw new ArgumentNullException(nameof(sendAction));
            }

            if (string.IsNullOrWhiteSpace(targetEntity))
            {
                throw new ArgumentException("Target entity is null or empty.", nameof(targetEntity));
            }

            using (var scope = serviceBusTelemetryHelper.BeginMessageSendingScope(brokeredMessage, targetEntity))
            {
                try
                {
                    await sendAction().ConfigureAwait(false);
                    scope.SetSuccess(true);
                }
                catch (Exception ex)
                {
                    if (serviceBusTelemetryHelper.TrackException)
                    {
                        serviceBusTelemetryHelper.TelemetryClient.TrackException(ex);
                    }

                    scope.SetSuccess(false);
                    throw;
                }
            }
        }

        /// <summary>
        /// Opens a dependency telemetry scope for async message sending. If the action returns, the
        /// scope will be autmatically ended. If action runs without error, success flag will be set
        /// to true. If the action throws an exception it will be logged under the scope and the
        /// success code will be false.
        /// </summary>
        /// <param name="serviceBusTelemetryHelper">The service bus telemetry helper.</param>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="targetEntity">The target entity.</param>
        /// <param name="sendAction">The send action.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task reference.</returns>
        /// <exception cref="System.ArgumentNullException">sendAction</exception>
        /// <exception cref="System.ArgumentException">Target entity is null or empty. - targetEntity</exception>
        /// <exception cref="ArgumentNullException">sendAction is null</exception>
        /// <exception cref="ArgumentException">Target entity is null or empty. - targetEntity</exception>
        public static async Task SendMessageAsync(this IServiceBusTelemetryHelper serviceBusTelemetryHelper, Message brokeredMessage, string targetEntity, Func<CancellationToken, Task> sendAction, CancellationToken cancellationToken)
        {
            if (brokeredMessage == null)
            {
                return;
            }

            if (sendAction == null)
            {
                throw new ArgumentNullException(nameof(sendAction));
            }

            if (string.IsNullOrWhiteSpace(targetEntity))
            {
                throw new ArgumentException("Target entity is null or empty.", nameof(targetEntity));
            }

            using (var scope = serviceBusTelemetryHelper.BeginMessageSendingScope(brokeredMessage, targetEntity))
            {
                try
                {
                    await sendAction(cancellationToken).ConfigureAwait(false);
                    scope.SetSuccess(true);
                }
                catch (Exception ex)
                {
                    if (serviceBusTelemetryHelper.TrackException)
                    {
                        serviceBusTelemetryHelper.TelemetryClient.TrackException(ex);
                    }

                    scope.SetSuccess(false);
                    throw;
                }
            }
        }

        /// <summary>
        /// Sends the with telemetry asynchronous.
        /// </summary>
        /// <param name="queueClient">The queue client.</param>
        /// <param name="message">The message.</param>
        /// <param name="serviceBusTelemetryHelper">The service bus telemetry helper.</param>
        /// <returns>Task reference.</returns>
        public static async Task SendWithTelemetryAsync(this IQueueClient queueClient, Message message, IServiceBusTelemetryHelper serviceBusTelemetryHelper)
        {
            if (message == null)
            {
                return;
            }

            if (serviceBusTelemetryHelper == null)
            {
                throw new ArgumentNullException(nameof(serviceBusTelemetryHelper));
            }

            using (var scope = serviceBusTelemetryHelper.BeginMessageSendingScope(message, queueClient.QueueName))
            {
                try
                {
                    await queueClient.SendAsync(message).ConfigureAwait(false);
                    scope.SetSuccess(true);
                }
                catch (Exception ex)
                {
                    if (serviceBusTelemetryHelper.TrackException)
                    {
                        serviceBusTelemetryHelper.TelemetryClient.TrackException(ex);
                    }

                    scope.SetSuccess(false);
                    throw;
                }
            }
        }

        /// <summary>
        /// Sends the with telemetry asynchronous.
        /// </summary>
        /// <param name="queueClient">The queue client.</param>
        /// <param name="messages">The messages.</param>
        /// <param name="serviceBusTelemetryHelper">The service bus telemetry helper.</param>
        /// <returns>Task reference</returns>
        public static async Task SendWithTelemetryAsync(this IQueueClient queueClient, IEnumerable<Message> messages, IServiceBusTelemetryHelper serviceBusTelemetryHelper)
        {
            if (messages?.Any() == false)
            {
                return;
            }

            if (serviceBusTelemetryHelper == null)
            {
                throw new ArgumentNullException(nameof(serviceBusTelemetryHelper));
            }

            using (var scope = serviceBusTelemetryHelper.BeginMessageSendingScope(messages, queueClient.QueueName))
            {
                try
                {
                    await queueClient.SendAsync(messages.ToList()).ConfigureAwait(false);
                    scope.SetSuccess(true);
                }
                catch (Exception ex)
                {
                    if (serviceBusTelemetryHelper.TrackException)
                    {
                        serviceBusTelemetryHelper.TelemetryClient.TrackException(ex);
                    }

                    scope.SetSuccess(false);
                    throw;
                }
            }
        }

        /// <summary>
        /// Sends the message asynchronous.
        /// </summary>
        /// <param name="serviceBusTelemetryHelper">The service bus telemetry helper.</param>
        /// <param name="message">The message.</param>
        /// <param name="targetEntity">The target entity.</param>
        /// <param name="sendAction">The send action.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">sendAction is null</exception>
        /// <exception cref="ArgumentException">Target entity is null or empty. - targetEntity</exception>
        public static async Task SendMessageAsync(this IServiceBusTelemetryHelper serviceBusTelemetryHelper, Message message, string targetEntity, Func<Message, Task> sendAction)
        {
            if (message == null)
            {
                return;
            }

            if (sendAction == null)
            {
                throw new ArgumentNullException(nameof(sendAction));
            }

            if (string.IsNullOrWhiteSpace(targetEntity))
            {
                throw new ArgumentException("Target entity is null or empty.", nameof(targetEntity));
            }

            using (var scope = serviceBusTelemetryHelper.BeginMessageSendingScope(message, targetEntity))
            {
                try
                {
                    await sendAction(message).ConfigureAwait(false);
                    scope.SetSuccess(true);
                }
                catch (Exception ex)
                {
                    if (serviceBusTelemetryHelper.TrackException)
                    {
                        serviceBusTelemetryHelper.TelemetryClient.TrackException(ex);
                    }

                    scope.SetSuccess(false);
                    throw;
                }
            }
        }

        /// <summary>
        /// Sends the messages asynchronous.
        /// </summary>
        /// <param name="serviceBusTelemetryHelper">The service bus telemetry helper.</param>
        /// <param name="messages">The messages.</param>
        /// <param name="targetEntity">The target entity.</param>
        /// <param name="sendAction">The send action.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">sendAction</exception>
        /// <exception cref="ArgumentException">Target entity is null or empty. - targetEntity</exception>
        public static async Task SendMessagesAsync(this IServiceBusTelemetryHelper serviceBusTelemetryHelper, IEnumerable<Message> messages, string targetEntity, Func<IList<Message>, Task> sendAction)
        {
            if (messages?.Any() == false)
            {
                return;
            }

            if (sendAction == null)
            {
                throw new ArgumentNullException(nameof(sendAction));
            }

            if (string.IsNullOrWhiteSpace(targetEntity))
            {
                throw new ArgumentException("Target entity is null or empty.", nameof(targetEntity));
            }

            using (var scope = serviceBusTelemetryHelper.BeginMessageSendingScope(messages, targetEntity))
            {
                try
                {
                    await sendAction(messages.ToList()).ConfigureAwait(false);
                    scope.SetSuccess(true);
                }
                catch (Exception ex)
                {
                    if (serviceBusTelemetryHelper.TrackException)
                    {
                        serviceBusTelemetryHelper.TelemetryClient.TrackException(ex);
                    }

                    scope.SetSuccess(false);
                    throw;
                }
            }
        }

        /// <summary>
        /// Opens a Request scope for an action processing the message. If the action returns, the
        /// scope will be autmatically ended. If action runs without error, success flag will be set
        /// to true. If the action throws an exception it will be logged under the scope and the
        /// success code will be false.
        /// </summary>
        /// <param name="serviceBusTelemetryHelper">The service bus telemetry helper.</param>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="entityPath">The entity path.</param>
        /// <param name="messageProcessingAction">The message processing action.</param>
        /// <exception cref="System.ArgumentException">Entity path is null or empty. - entityPath</exception>
        /// <exception cref="System.ArgumentNullException">messageProcessingAction</exception>
        /// <exception cref="ArgumentNullException">messageProcessingAction is null</exception>
        public static void ProcessMessage(this IServiceBusTelemetryHelper serviceBusTelemetryHelper, Message brokeredMessage, string entityPath, Action messageProcessingAction)
        {
            if (brokeredMessage == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(entityPath))
            {
                throw new ArgumentException("Entity path is null or empty.", nameof(entityPath));
            }

            if (messageProcessingAction == null)
            {
                throw new ArgumentNullException(nameof(messageProcessingAction));
            }

            using (var scope = serviceBusTelemetryHelper.BeginMessageProcessingScope(brokeredMessage, entityPath))
            {
                try
                {
                    messageProcessingAction();
                    scope.SetSuccess(true);
                }
                catch (Exception ex)
                {
                    if (serviceBusTelemetryHelper.TrackException)
                    {
                        serviceBusTelemetryHelper.TelemetryClient.TrackException(ex);
                    }

                    scope.SetSuccess(false);
                    throw;
                }
            }
        }

        /// <summary>
        /// Opens a Request scope for an async action processing the message. If the action returns,
        /// the scope will be autmatically ended. If action runs without error, success flag will be
        /// set to true. If the action throws an exception it will be logged under the scope and the
        /// success code will be false.
        /// </summary>
        /// <param name="serviceBusTelemetryHelper">The service bus telemetry helper.</param>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="entityPath">The entity path.</param>
        /// <param name="messageProcessingAction">The message processing action.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task reference.</returns>
        /// <exception cref="System.ArgumentException">Entity path is null or empty. - entityPath</exception>
        /// <exception cref="System.ArgumentNullException">messageProcessingAction</exception>
        /// <exception cref="ArgumentNullException">messageProcessingAction</exception>
        public static async Task ProcessMessageAsync(this IServiceBusTelemetryHelper serviceBusTelemetryHelper, Message brokeredMessage, string entityPath, Func<CancellationToken, Task> messageProcessingAction, CancellationToken cancellationToken)
        {
            if (brokeredMessage == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(entityPath))
            {
                throw new ArgumentException("Entity path is null or empty.", nameof(entityPath));
            }

            if (messageProcessingAction == null)
            {
                throw new ArgumentNullException(nameof(messageProcessingAction));
            }

            using (var scope = serviceBusTelemetryHelper.BeginMessageProcessingScope(brokeredMessage, entityPath))
            {
                try
                {
                    await messageProcessingAction(cancellationToken).ConfigureAwait(false);
                    scope.SetSuccess(true);
                }
                catch (Exception ex)
                {
                    if (serviceBusTelemetryHelper.TrackException)
                    {
                        serviceBusTelemetryHelper.TelemetryClient.TrackException(ex);
                    }

                    scope.SetSuccess(false);
                    throw;
                }
            }
        }

        /// <summary>
        /// Opens a Request scope for an async action processing the message. If the action returns,
        /// the scope will be autmatically ended. If action runs without error, success flag will be
        /// set to true. If the action throws an exception it will be logged under the scope and the
        /// success code will be false.
        /// </summary>
        /// <param name="serviceBusTelemetryHelper">The service bus telemetry helper.</param>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="entityPath">The entity path.</param>
        /// <param name="messageProcessingAction">The message processing action.</param>
        /// <returns>Task reference.</returns>
        /// <exception cref="System.ArgumentException">Entity path is null or empty. - entityPath</exception>
        /// <exception cref="System.ArgumentNullException">messageProcessingAction</exception>
        /// <exception cref="ArgumentNullException">messageProcessingAction</exception>
        public static async Task ProcessMessageAsync(this IServiceBusTelemetryHelper serviceBusTelemetryHelper, Message brokeredMessage, string entityPath, Func<Task> messageProcessingAction)
        {
            if (brokeredMessage == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(entityPath))
            {
                throw new ArgumentException("Entity path is null or empty.", nameof(entityPath));
            }

            if (messageProcessingAction == null)
            {
                throw new ArgumentNullException(nameof(messageProcessingAction));
            }

            using (var scope = serviceBusTelemetryHelper.BeginMessageProcessingScope(brokeredMessage, entityPath))
            {
                try
                {
                    await messageProcessingAction().ConfigureAwait(false);
                    scope.SetSuccess(true);
                }
                catch (Exception ex)
                {
                    if (serviceBusTelemetryHelper.TrackException)
                    {
                        serviceBusTelemetryHelper.TelemetryClient.TrackException(ex);
                    }

                    scope.SetSuccess(false);
                    throw;
                }
            }
        }

        /// <summary>
        /// Sends the with telemetry asynchronous.
        /// </summary>
        /// <param name="topicClient">The topic client.</param>
        /// <param name="message">The message.</param>
        /// <param name="serviceBusTelemetryHelper">The service bus telemetry helper.</param>
        /// <returns>Task reference.</returns>
        /// <exception cref="ArgumentNullException">serviceBusTelemetryHelper is null</exception>
        public static async Task SendWithTelemetryAsync(this ITopicClient topicClient, Message message, IServiceBusTelemetryHelper serviceBusTelemetryHelper)
        {
            if (message == null)
            {
                return;
            }

            if (serviceBusTelemetryHelper == null)
            {
                throw new ArgumentNullException(nameof(serviceBusTelemetryHelper));
            }

            using (var scope = serviceBusTelemetryHelper.BeginMessageSendingScope(message, topicClient.TopicName))
            {
                try
                {
                    await topicClient.SendAsync(message).ConfigureAwait(false);
                    scope.SetSuccess(true);
                }
                catch (Exception ex)
                {
                    if (serviceBusTelemetryHelper.TrackException)
                    {
                        serviceBusTelemetryHelper.TelemetryClient.TrackException(ex);
                    }

                    scope.SetSuccess(false);
                    throw;
                }
            }
        }

        /// <summary>
        /// Sends the with telemetry asynchronous.
        /// </summary>
        /// <param name="topicClient">The topic client.</param>
        /// <param name="messages">The messages.</param>
        /// <param name="serviceBusTelemetryHelper">The service bus telemetry helper.</param>
        /// <returns>Task reference</returns>
        /// <exception cref="ArgumentNullException">serviceBusTelemetryHelper is null</exception>
        public static async Task SendWithTelemetryAsync(this ITopicClient topicClient, IEnumerable<Message> messages, IServiceBusTelemetryHelper serviceBusTelemetryHelper)
        {
            if (messages?.Any() == false)
            {
                return;
            }

            if (serviceBusTelemetryHelper == null)
            {
                throw new ArgumentNullException(nameof(serviceBusTelemetryHelper));
            }

            using (var scope = serviceBusTelemetryHelper.BeginMessageSendingScope(messages, topicClient.TopicName))
            {
                try
                {
                    await topicClient.SendAsync(messages.ToList()).ConfigureAwait(false);
                    scope.SetSuccess(true);
                }
                catch (Exception ex)
                {
                    if (serviceBusTelemetryHelper.TrackException)
                    {
                        serviceBusTelemetryHelper.TelemetryClient.TrackException(ex);
                    }

                    scope.SetSuccess(false);
                    throw;
                }
            }
        }
    }
}