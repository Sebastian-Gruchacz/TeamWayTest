namespace TmwServices.Domain.Shifts.Model
{
    using System;

    /// <summary>
    /// Shift model
    /// </summary>
    public class Shift
    {
        /// <summary>
        /// Identifier of the Shift
        /// </summary>
        public Guid ShiftId { get; set; }

        /// <summary>
        /// External Worker's identifier.
        /// </summary>
        public Guid WorkerId { get; set; }

        /// <summary>
        /// Shift start
        /// </summary>
        public DateTime StartUtc { get; set; }

        /// <summary>
        /// Shift End
        /// </summary>
        public DateTime EndUtc { get; set; }

        /// <summary>
        /// Time zone at which Worker's shift was created and will be (potentially) targeted despite of the server / service time zone
        /// </summary>
        /// <remarks>Would not be needed in single-site application. Yet usage of micro-services suggests more global / distributed usage.</remarks>
        public TimeZoneInfo TimeZone { get; set; }
    }
}