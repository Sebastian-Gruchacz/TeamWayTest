using System.Threading.Tasks;
using TmwServices.Domain.Shifts.Model;

namespace TmwServices.Domain.Shifts
{
    public interface IShiftEventsEmitterService
    {
        Task ShiftConflictedAsync(Shift shift);
        Task ShiftRegisteredAsync(Shift insertedShift);
        Task ShiftRejectedAsync(Shift shift);
    }
}