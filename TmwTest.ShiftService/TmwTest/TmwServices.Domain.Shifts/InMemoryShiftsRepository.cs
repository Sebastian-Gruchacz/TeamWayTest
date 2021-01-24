using System.Linq;

namespace TmwServices.Domain.Shifts
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using TmwServices.Domain.Shifts.Model;

    /// <summary>
    /// Implements <see cref="IShiftsRepository"/> as in-memory, demo implementation
    /// </summary>
    /// <seealso cref="TmwServices.Domain.Shifts.IShiftsRepository" />
    public class InMemoryShiftsRepository : IShiftsRepository
    {
        private readonly ILogger<InMemoryShiftsRepository> _logger;

        private readonly ConcurrentDictionary<Guid, List<Shift>> _shiftsDataBase = 
            new ConcurrentDictionary<Guid, List<Shift>>();

        public InMemoryShiftsRepository(ILogger<InMemoryShiftsRepository> logger)
        {
            _logger = logger;
        }

        public async Task<Shift[]> GetUserShiftsAsync(Guid workerId, DateTime startDateTime, DateTime endDateTime)
        {
            if (workerId == Guid.Empty)
            {
                throw new ArgumentException("Worker Identifier cannot be empty", nameof(workerId));
            }

            var workerShifts = GetOrCreateWorkerShifts(workerId);
            Shift[] result; 
            lock (workerShifts)
            {
                result = ExtractBoundedShifts(workerShifts, startDateTime, endDateTime).Result;
            }

            return await Task.FromResult(result);
        }

        public async Task<Shift> TryInsertBoundedShiftAsync(Shift shift, DateTime startDateTime, DateTime endDateTime)
        {
            if (shift == null)
            {
                throw new ArgumentNullException(nameof(shift));
            }

            if (shift.WorkerId == Guid.Empty)
            {
                throw new ArgumentException("Worker Identifier cannot be empty", nameof(shift));
            }

            // trusting that all bounds and logic was tested on service level, to avoid duplicate checks...

            var workerShifts = GetOrCreateWorkerShifts(shift.WorkerId);
            Shift result = null;
            lock (workerShifts)
            {
                var existingEntriesInConflict = ExtractBoundedShifts(workerShifts, startDateTime, endDateTime).Result;
                if (!existingEntriesInConflict.Any())
                {
                    shift.ShiftId = Guid.NewGuid();
                    workerShifts.Add(shift);
                    result = shift;
                }
            }

            return await Task.FromResult(result);
        }

        private static async Task<Shift[]> ExtractBoundedShifts(List<Shift> workerShifts, DateTime startDateTime, DateTime endDateTime)
        {
            return await Task.FromResult(workerShifts.Where(shift =>
                    (shift.StartUtc >= startDateTime && shift.StartUtc <= endDateTime) ||
                    (shift.EndUtc >= startDateTime && shift.EndUtc <= endDateTime))
                .ToArray());
        }

        private List<Shift> GetOrCreateWorkerShifts(Guid workerId)
        {
            return _shiftsDataBase.GetOrAdd(workerId, guid => new List<Shift>());
        }

    }
}