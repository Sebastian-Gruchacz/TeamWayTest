using System.Net;
using Newtonsoft.Json.Linq;
using TmwServices.Core;

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
        /// List all Shifts available to the Worker
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
        /// Tries to book a Shift for a Worker
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> BookShift(ShiftViewModel newShift)
        {
            Shift shift = newShift.AsDomainClass();

            var inserted = await _shiftsService.TryRegisterShiftAsync(shift);
            if (inserted.IsSuccess)
            {
                return new JsonResult(new ShiftViewModel(inserted.Value));
            }
            
            return ProcessError(inserted);
        }

        private IActionResult ProcessError(ActionResponse result)
        {
            switch (result.Code)
            {
                case (int) HttpStatusCode.BadRequest:
                {
                    return BadRequest(result.ErrorMessage);
                }
                case (int)HttpStatusCode.Conflict:
                {
                    return Conflict(result.ErrorMessage);
                }
                default:
                {
                    return Problem(result.ErrorMessage);
                }
            }
        }
    }
}
