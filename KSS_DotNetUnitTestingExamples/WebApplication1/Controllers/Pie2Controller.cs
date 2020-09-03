using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Dto;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("[controller]")]
    public class Pie2Controller : ControllerBase
    {
        private readonly IPie2Service _pieService;

        public Pie2Controller(IPie2Service pieService)
        {
            _pieService = pieService;
        }

        /// <summary>
        /// This endpoint does 3 things because this makes it easier to demo the units tests
        /// It:
        /// 1. Creates the new Pie
        /// 2. Returns the created pie
        /// 3. Returns a sorted record of all the pies made that day
        /// This is a poor design - aim for each endpoint to do 1 thing only unless it helps performance
        /// </summary>
        /// <param name="flavour"></param>
        /// <returns></returns>
        [HttpGet("[action]/flavour/{flavour}")]
        [HttpGet("[action]/{flavour?}")]
        public async Task<ActionResult<PieRecord>> Get(string flavour)
        {
            var pieResponse = await _pieService.GetPieRecord(flavour).ConfigureAwait(false);
            return StatusCode(pieResponse.StatusCodeHttp, pieResponse.PieRecord);
        }
    }
}