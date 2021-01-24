namespace TmwServices.Domain.Shifts
{
    using System;
    using System.Threading.Tasks;

    using TmwServices.Domain.Shifts.Model;

    /// <summary>
    /// Represents shifts persistence repository
    /// </summary>
    public interface IShiftsRepository
    {
        /// <summary>
        /// Gets the worker shifts.
        /// </summary>
        /// <param name="workerId">The worker identifier.</param>
        /// <param name="startDateTime">The start date time.</param>
        /// <param name="endDateTime">The end date time.</param>
        /// <returns></returns>
        Task<Shift[]> GetWorkerShiftsAsync(Guid workerId, DateTime startDateTime, DateTime endDateTime);

        /// <summary>
        /// Tries to insert shift within bounded time.
        /// </summary>
        /// <param name="shift">The shift.</param>
        /// <param name="startDateTime">The start date time.</param>
        /// <param name="endDateTime">The end date time.</param>
        /// <returns></returns>
        Task<Shift> TryInsertBoundedShiftAsync(Shift shift, DateTime startDateTime, DateTime endDateTime);
    }
}