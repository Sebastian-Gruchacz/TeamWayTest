using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TmwServices.Domain.Shifts;

namespace TmwServices.ShiftsService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShiftsController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<ShiftsController> _logger;
        private readonly IShiftsService _shiftsService;

        public ShiftsController(ILogger<ShiftsController> logger, IShiftsService shiftsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _shiftsService = shiftsService ?? throw new ArgumentNullException(nameof(shiftsService));
        }

        /// <summary>
        /// List Shifts available to the User
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
