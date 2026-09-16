using Microsoft.AspNetCore.Mvc;
using WebHostStreaming.Providers;

namespace WebHostStreaming.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ErrorLogsController : ControllerBase
    {
        private readonly IErrorLogsProvider _errorLogsProvider;

        public ErrorLogsController(IErrorLogsProvider errorLogsProvider)
        {
            _errorLogsProvider = errorLogsProvider;
        }

        [HttpGet]
        public IActionResult GetErrorLogs()
        {
            return Ok(_errorLogsProvider.GetErrorLogs());
        }

        [HttpDelete]
        public IActionResult DeleteErrorLogs()
        {
            _errorLogsProvider.DeleteErrorLogs();

            return NoContent();
        }
    }
}