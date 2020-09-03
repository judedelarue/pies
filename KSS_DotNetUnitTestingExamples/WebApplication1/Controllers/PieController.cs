using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Dto;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [Route("[controller]")]
    public class PieController : ControllerBase
    {
        private readonly IPieService _pieService;

        public PieController(IPieService pieService)
        {
            _pieService = pieService;
        }

        [HttpGet("[action]/flavour/{flavour}")]
        public async Task<ActionResult<PieRecord>> Get(string flavour)
        {
            var pieRecord = await _pieService.GetPieRecord(flavour).ConfigureAwait(false);
            return Ok(pieRecord);
        }
    }
}