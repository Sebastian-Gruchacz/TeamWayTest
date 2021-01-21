using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TmwServices.Core;
using TmwServices.Domain.Shifts.Model;

namespace TmwServices.Domain.Shifts
{
    public class ShiftsService : IShiftsService
    {
        private readonly IOptions<ShiftRulesConfiguration> _configuration;
        private readonly ILogger<ShiftsService> _logger;

        public ShiftsService(IOptions<ShiftRulesConfiguration> configuration, ILogger<ShiftsService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ActionResponse<Shift>> TryRegisterShiftAsync(Shift shift)
        {
            throw new NotImplementedException();
        }
    }

    public class ShiftRulesConfiguration
    {
    }

    public interface IShiftsService
    {
        /// <summary>
        /// Tries to register a new shift
        /// </summary>
        /// <param name="shift"></param>
        /// <returns></returns>
        Task<ActionResponse<Shift>> TryRegisterShiftAsync(Shift shift);
    }
}
