namespace TmwServices.Domain.Shifts.Configuration
{
    using TmwServices.Core.Configuration;

    /// <summary>
    /// Specifies configuration of the queues used in the service
    /// </summary>
    public class QueuesSubsystemConfiguration
    {
        public const string SectionName = @"QueuesConfiguration";

        /// <summary>
        /// Gets or sets a value indicating whether Shifts Registration queue monitoring is enabled.
        /// </summary>
        public bool MonitoringEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Registration raising events is enabled.
        /// </summary>
        public bool RaiseEventsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the configuration of the queue where shifts registration request come into the service.
        /// </summary>
        public AzureQueueConfiguration ShiftsRegistrationsSource { get; set; }

        /// <summary>
        /// Gets or sets the configuration of the queue where events about completed registrations land
        /// </summary>
        public AzureQueueConfiguration ShiftsRegistrationSuccessEvents { get; set; }

        /// <summary>
        /// Gets or sets the configuration of the queue where events about failed registrations land
        /// </summary>
        public AzureQueueConfiguration ShiftsRegistrationFailedEvents { get; set; }
    }
}
