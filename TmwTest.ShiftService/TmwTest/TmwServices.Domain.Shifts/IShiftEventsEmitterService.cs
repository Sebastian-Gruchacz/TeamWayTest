namespace TmwServices.Domain.Shifts
{
    using System.Threading.Tasks;

    using TmwServices.Domain.Shifts.Model;

    /// <summary>
    /// Sends asynchronous event messages about registration events
    /// </summary>
    public interface IShiftEventsEmitterService
    {
        /// <summary>
        /// Reports that requested shift conflicted with already registered shift
        /// </summary>
        /// <param name="shift">The shift that was rejected.</param>
        Task ShiftConflictedAsync(Shift shift);

        /// <summary>
        /// Reports that new Shift booking succeeded
        /// </summary>
        /// <param name="insertedShift">The newly inserted shift.</param>
        Task ShiftRegisteredAsync(Shift insertedShift);

        /// <summary>
        /// Reports that requested shift is invalid
        /// </summary>
        /// <param name="shift">The shift that was rejected.</param>
        /// <param name="rejectionReason">The rejection reason.</param>
        Task ShiftRejectedAsync(Shift shift, string rejectionReason);
    }
}