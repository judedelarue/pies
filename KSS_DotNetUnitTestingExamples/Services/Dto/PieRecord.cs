using System.Collections.Generic;

namespace Services.Dto
{
    public class PieRecord
    {
        public Pie MostRecent { get; set; }

        public List<Pie> PieAudit { get; set; }

    }
}