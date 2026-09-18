using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebHostStreaming.Helpers;
using WebHostStreaming.Providers;

namespace WebHostStreaming.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly IErrorLogsProvider _errorLogsProvider;

        public LogsController(IErrorLogsProvider errorLogsProvider)
        {
            _errorLogsProvider = errorLogsProvider;
        }

        [HttpGet("errors")]
        public IActionResult GetErrorLogs()
        {
            return Ok(_errorLogsProvider.GetErrorLogs());
        }

        [HttpDelete("errors")]
        public IActionResult DeleteErrorLogs()
        {
            _errorLogsProvider.DeleteErrorLogs();

            return NoContent();
        }

        [HttpGet("live")]
        public ServerSentEventsResult<string> GetLiveLogs(CancellationToken cancellationToken)
        {
            return TypedResults.ServerSentEvents(AppLogger.GetLogs(cancellationToken), eventType: "live_logs");
        }
    }
}