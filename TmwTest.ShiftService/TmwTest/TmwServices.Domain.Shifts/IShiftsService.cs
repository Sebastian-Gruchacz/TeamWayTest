namespace TmwServices.Domain.Shifts
{
    using System;
    using System.Threading.Tasks;

    using TmwServices.Core;
    using TmwServices.Domain.Shifts.Model;

    public interface IShiftsService
    {
        /// <summary>
        /// Tries to register a new shift
        /// </summary>
        /// <param name="shift"></param>
        Task<ActionResponse<Shift>> TryRegisterShiftAsync(Shift shift);

        /// <summary>
        /// Gets Worker's list of shifts that happen at least partially starting from given date
        /// </summary>
        /// <param name="workerId"></param>
        /// <param name="startDateTime"></param>
        Task<Shift[]> GetWorkerShiftsAsync(Guid workerId, DateTime startDateTime);

        /// <summary>
        /// Gets Worker's list of shifts that happen at least partially between given dates
        /// </summary>
        /// <param name="workerId"></param>
        /// <param name="startDateTime"></param>
        /// <param name="endDateTime"></param>
        Task<Shift[]> GetWorkerShiftsAsync(Guid workerId, DateTime startDateTime, DateTime endDateTime);
    }
}