namespace TmwServices.Domain.Shifts.Implementation
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using TmwServices.Core;
    using TmwServices.Domain.Shifts.Configuration;
    using TmwServices.Domain.Shifts.Model;


    /// <inheritdoc cref="IShiftsService"/>
    public class ShiftsService : IShiftsService
    {
        private readonly ShiftRulesConfiguration _configuration;
        private readonly ILogger<ShiftsService> _logger;
        private readonly IShiftsRepository _shiftsRepository;
        private readonly IShiftEventsEmitterService _eventsEmitterService;

        /// <summary>Initializes a new instance of the <see cref="ShiftsService" /> class.</summary>
        /// <param name="configuration">The shifts rules configuration.</param>
        /// <param name="shiftsRepository">The sifts persisting repository</param>
        /// <param name="eventsEmitterService"></param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ShiftsService(IOptions<ShiftRulesConfiguration> configuration,
            IShiftsRepository shiftsRepository,
            IShiftEventsEmitterService eventsEmitterService,
            ILogger<ShiftsService> logger)
        {
            _configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _shiftsRepository = shiftsRepository ?? throw new ArgumentNullException(nameof(shiftsRepository));
            _eventsEmitterService = eventsEmitterService ?? throw new ArgumentNullException(nameof(eventsEmitterService));
        }

        /// <inheritdoc cref="IShiftsService"/>
        public async Task<ActionResponse<Shift>> TryRegisterShiftAsync(Shift shift)
        {
            if (shift == null)
            {
                throw new ArgumentNullException(nameof(shift));
            }

            string validationResultMessage = ValidateShift(shift);
            if (!string.IsNullOrWhiteSpace(validationResultMessage))
            {
                _logger.LogTrace("Shift validation rejection: {0}.", JObject.FromObject(shift).ToString(Formatting.None));
                await _eventsEmitterService.ShiftRejectedAsync(shift);
                return await Task.FromResult(new ActionResponse<Shift>(HttpStatusCode.BadRequest, validationResultMessage));
            }

            if (shift.ShiftId != Guid.Empty)
            {
                _logger.LogTrace("Shift duplicate request rejection: {0}.", JObject.FromObject(shift).ToString(Formatting.None));
                await _eventsEmitterService.ShiftRejectedAsync(shift);
                //throw new ArgumentException(@"Cannot register shift that is already registered.", nameof(shift));
                return await Task.FromResult(new ActionResponse<Shift>(HttpStatusCode.BadRequest, @"Cannot register shift that is already registered."));
            }

            // get existing shifts of worker, that might potentially get in conflict
            var bufferedStart = shift.StartUtc.AddHours(_configuration.MinShiftsGape * (-1));
            var bufferedEnd = shift.EndUtc.AddHours(_configuration.MinShiftsGape);

            var conflictingShifts = await GetWorkerShiftsAsync(shift.WorkerId, bufferedStart, bufferedEnd);
            if (conflictingShifts.Any())
            {
                _logger.LogTrace("Shift conflict rejection: {0}.", JObject.FromObject(shift).ToString(Formatting.None));
                await _eventsEmitterService.ShiftConflictedAsync(shift);
                return await Task.FromResult(new ActionResponse<Shift>(HttpStatusCode.Conflict, @"Rejected because of conflict with already booked shift(s)."));
            }

            var insertedShift = await _shiftsRepository.TryInsertBoundedShiftAsync(shift, bufferedStart, bufferedEnd);
            if (insertedShift != null)
            {
                await _eventsEmitterService.ShiftRegisteredAsync(insertedShift);
                return new ActionResponse<Shift>(insertedShift, HttpStatusCode.Created);
            }

            _logger.LogTrace("Shift insert conflict: {0}.", JObject.FromObject(shift).ToString(Formatting.None));
            await _eventsEmitterService.ShiftConflictedAsync(shift);
            return new ActionResponse<Shift>(HttpStatusCode.Conflict, "Insertion conflict");
        }

        /// <inheritdoc cref="IShiftsService"/>
        public async Task<Shift[]> GetWorkerShiftsAsync(Guid workerId, DateTime startDateTime)
        {
            return await GetWorkerShiftsAsync(workerId, startDateTime, DateTime.MaxValue);
        }

        /// <inheritdoc cref="IShiftsService"/>
        public async Task<Shift[]> GetWorkerShiftsAsync(Guid workerId, DateTime startDateTime, DateTime endDateTime)
        {
            return await _shiftsRepository.GetWorkerShiftsAsync(workerId, startDateTime, endDateTime);
        }

        /// <summary>
        /// Static validation of the shift definition
        /// </summary>
        /// <param name="shift"></param>
        /// <returns>NULL if shift is correct or error message.</returns>
        private string ValidateShift(Shift shift)
        {
            double shiftLength = (shift.EndUtc - shift.StartUtc).TotalHours;
            if (shiftLength < _configuration.MinShiftLength)
            {
                return $"The shift cannot be shorter than {_configuration.MinShiftLength} hours.";
            }

            if (shiftLength > _configuration.MaxShiftLength)
            {
                return $"The shift cannot be longer than {_configuration.MaxShiftLength} hours.";
            }

            if (shift.WorkerId == Guid.Empty)
            {
                return $"The shift must provide Worker's identifier in the correct format.";
            }

            var start = TimeZoneInfo.ConvertTimeFromUtc(shift.StartUtc, shift.TimeZone);
            var end = TimeZoneInfo.ConvertTimeFromUtc(shift.EndUtc, shift.TimeZone);

            if (!_configuration.AllowDayOverlap && start.Date != end.Date)
            {
                return $"The shift must be entirely contained within one day.";
            }

            return null;
        }
    }
}
