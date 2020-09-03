using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using Services.Dto;
using System;
using System.Text.Json;

namespace Tests
{
    [TestClass]
    public class PieServiceTests
    {
        [DataRow( "cherry", "cherry")]
        [DataRow( "apple", "apple")]
        [DataRow( "cheese", "cheese")]
        [DataTestMethod]
        public void GetPie_returns_expected_pie(string inputFlavour, string expectedFlavour)
        {
            PieService pieService = new PieService();
            var actualResult = pieService.GetPie(inputFlavour).GetAwaiter().GetResult();
            Pie returnedPie = JsonSerializer.Deserialize<Pie>(actualResult);

            Assert.AreEqual(expectedFlavour, returnedPie.Flavour);


        }
        [TestMethod]
        public void jude ()
        {
            Assert.IsTrue(2 < 1);
        }
    }
}
