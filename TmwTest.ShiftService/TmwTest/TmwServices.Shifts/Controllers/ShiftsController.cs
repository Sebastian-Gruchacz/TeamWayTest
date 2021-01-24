namespace TmwServices.ShiftsService.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    using TmwServices.Domain.Shifts;
    using TmwServices.Domain.Shifts.Model;

    using TmwServices.ShiftsService.ViewModel;

    /// <summary>
    /// Provides <see cref="Shift"/> control service
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ApiController]
    [Route("[controller]")]
    public class ShiftsController : ControllerBase
    {
        private readonly ILogger<ShiftsController> _logger;
        private readonly IShiftsService _shiftsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShiftsController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="shiftsService">The shifts service.</param>
        public ShiftsController(ILogger<ShiftsController> logger, IShiftsService shiftsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _shiftsService = shiftsService ?? throw new ArgumentNullException(nameof(shiftsService));
        }

        /// <summary>
        /// List all Shifts available to the User
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<ShiftViewModel>> GetAllUserShifts(Guid userId)
        {
            return (await _shiftsService.GetWorkerShiftsAsync(userId, DateTime.MinValue, DateTime.MaxValue))
                ?.Select(shift => new ShiftViewModel(shift))
                .ToArray();
        }

        /// <summary>
        /// Tries to book a shift
        /// </summary>
        [HttpPost]
        public async Task<ShiftViewModel> BookShift(ShiftViewModel newShift)
        {
            Shift shift = newShift.AsDomainClass();

            var inserted = await _shiftsService.TryRegisterShiftAsync(shift);
            if (inserted.IsSuccess)
            {
                return await Task.FromResult(new ShiftViewModel(inserted.Value));
            }

            return null;
        }

        // TODO: change the api to better return error data with IActionResult? 
    }
}
