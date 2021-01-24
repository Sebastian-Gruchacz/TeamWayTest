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
        Task<Shift[]> GetUserShiftsAsync(Guid workerId, DateTime startDateTime, DateTime endDateTime);

        Task<Shift> TryInsertBoundedShiftAsync(Shift shift, DateTime startDateTime, DateTime endDateTime);
    }
}