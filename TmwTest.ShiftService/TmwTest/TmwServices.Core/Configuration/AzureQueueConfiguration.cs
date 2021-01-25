namespace TmwServices.Core.Configuration
{
    /// <summary>
    /// Configuration of single queue
    /// </summary>
    public class AzureQueueConfiguration
    {
        /// <summary>
        /// Gets or sets the identifier of the queue.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the service bus access key.
        /// </summary>
        public string ServiceBusAccessKey { get; set; }

        /// <summary>
        /// Gets or sets the name of the queue as in the ServiceBus.
        /// </summary>
        public string QueueName { get; set; }
    }
}
