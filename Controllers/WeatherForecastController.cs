using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SysUtility;
using SysUtility.Config.Interfaces;
using SysUtility.Config.Models;

namespace WeghingSystemCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWritableOptions<AppConfig> _options;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWritableOptions<AppConfig> options)
        {
            _logger = logger;
            _options = options;
            _logger.LogInformation("SAMPLE");
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            _options.Update(opt =>
            {
                opt.DefaultDBConnectionKey = 1;
            });

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            }).AsQueryable()
            .ToArray();
        }
    }
}
