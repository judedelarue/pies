using Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface IPieService
    {
        Task<PieRecord> GetPieRecord(string flavour);
    }

    public class PieService : IPieService
    {
        private readonly IPieDataService _pieDataService;
        private readonly IPastryService _pastryService;
        private readonly IFillingService _fillingService;

        public PieService()
        {
            _pieDataService = new PieDataService();
            _pastryService = new PastryService();
            _fillingService = new FillingService();
        }

        public async Task<PieRecord> GetPieRecord(string flavour)
        {
            int pastry = _pastryService.Get(50);
            int filling = _fillingService.Get(flavour, 50);
            Pie pie = await _pieDataService.BakePie(flavour, pastry, filling);
            List<Pie> pieAudit = await _pieDataService.GetPieAudit();
            return new PieRecord
            {
                MostRecent = pie,
                PieAudit = pieAudit
            };
        }
    }
}