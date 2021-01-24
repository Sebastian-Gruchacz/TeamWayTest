namespace TmwServices.ShiftsService.ViewModel
{
    using System;

    using TmwServices.Domain.Shifts.Model;

    /// <summary>
    /// View model for a Worker Shift
    /// </summary>
    public class ShiftViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShiftViewModel"/> class.
        /// </summary>
        /// <remarks>To be used by serializers mainly</remarks>
        public ShiftViewModel()
        {
            
        }   

        /// <summary>
        /// Initializes a new instance of the <see cref="ShiftViewModel"/> class.
        /// </summary>
        /// <param name="shift">The shift model.</param>
        public ShiftViewModel(Shift shift)
        {
            this.ShiftId = shift.ShiftId;
            this.WorkerId = shift.WorkerId;
            this.StartUtc = shift.StartUtc;
            this.EndUtc = shift.EndUtc;
            this.TimeZoneId = shift.TimeZone.Id;
        }

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
        /// Gets or sets the time zone standard name.
        /// </summary>
        /// <remarks>The most precise and consistent specification</remarks>
        public string TimeZoneId { get; set; }

        /// <summary>
        /// Converts view model to domain class
        /// </summary>
        public Shift AsDomainClass()
        {
            return new Shift
            {
                ShiftId = this.ShiftId,
                WorkerId = this.WorkerId,
                StartUtc = this.StartUtc,
                EndUtc = this.EndUtc,
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById(this.TimeZoneId)
            };
        }
    }
}