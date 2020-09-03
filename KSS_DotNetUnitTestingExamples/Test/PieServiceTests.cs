using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Test
{
    [Ignore]
    [TestClass]
    public class PieServiceTests

    {
        [TestMethod]
        public void GetPie_cherry()
        {
            PieService pieService = new PieService();
            Pie pie = new Pie
            {
                Flavour = "cherry",
                LastMadeOn = DateTime.Now.ToString("T")
            };
            PieRecord expectedResult = new PieRecord
            {
                MostRecent = pie,
                PieAudit = new List<Pie> { pie }
            };

            var actualResult = pieService.GetPieRecord("cherry").GetAwaiter().GetResult();

            Assert.AreEqual("cherry", actualResult.MostRecent.Flavour);
            Assert.AreEqual(DateTime.Now.ToString("T"), actualResult.MostRecent.LastMadeOn);
            Assert.AreEqual(1, actualResult.PieAudit.Count);
            Assert.AreEqual(1, actualResult.PieAudit.Where(p => p.Flavour == "cherry").Count());
        }
    }
}