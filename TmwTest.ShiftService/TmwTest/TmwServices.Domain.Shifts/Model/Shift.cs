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
        /// <remarks>TODO: Perhaps should be GUID as well to better support cross system uniqueness.
        /// TODO: Optionally could replace GUID's with other types of subsystem-wide unique numbers.</remarks>
        public int ShiftId { get; set; }

        /// <summary>
        /// External user's identifier.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Shift start
        /// </summary>
        public DateTime StartUtc { get; set; }

        /// <summary>
        /// Shift End
        /// </summary>
        public DateTime EndUtc { get; set; }

        /// <summary>
        /// Time zone at which user's shift was created and will be (potentially) targeted despite of the server / service time zone
        /// </summary>
        /// <remarks>Would not be needed in single-site application. Yet usage of micro-services suggests more global / distributed usage.</remarks>
        public TimeZoneInfo TimeZone { get; set; }
    }
}