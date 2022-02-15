using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TestsBaza.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api")]
    public class WeatherForecastController : Controller
    {

        public WeatherForecastController()
        {
        }

        [HttpGet("/alltests")]
        public IEnumerable<WeatherForecast> GetAllTests()
        {
            return null;
        }

    }
}