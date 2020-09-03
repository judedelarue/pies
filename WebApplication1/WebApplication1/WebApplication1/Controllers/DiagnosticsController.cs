using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    public class DiagnosticsController : ControllerBase
    {
        // We can use the nice Serilog type format instead of Microsoft ILogger<someclass> here
        // Serilog will support the ILogger<someclass> as well if you must
        private ILogger _logger;

        public DiagnosticsController(ILogger logger)
        {
            _logger = logger;
        }

        [HttpGet("[action]")]
        public ActionResult<string> Ping()
        {
            // These are just so I can see them in the Seq tool
            var Jude = "hey jude";
            var someint = 5;
            _logger.Error("Some error: {Jude}", Jude);
            _logger.Warning("Some warning: {someint}", someint);

            _logger.Information("diagnostics controller log");
            return Ok("Pong");
        }
    }
}