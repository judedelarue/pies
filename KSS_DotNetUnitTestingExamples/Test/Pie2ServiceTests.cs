using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Serilog;
using Services;
using Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

using static Services.Helpers.StatusCodesHttp;

namespace Test
{
    /// <summary>
    /// More comprehensive unit tests  for a pie service.
    /// </summary>
    [TestClass]
    public class Pie2ServiceTests
    {
        private Mock<IPastryService> _mockPastryService = new Mock<IPastryService>();
        private Mock<IFillingService> _mockFillingService = new Mock<IFillingService>();
        private Mock<IPie2DataService> _mockDataService = new Mock<IPie2DataService>();
        private Mock<ILogger> _mockLogger = new Mock<ILogger>();
        private Mock<INowAdapter> _mockNowService = new Mock<INowAdapter>();

        private Pie2Service uut;
        private int validPastryQuantity = 50;
        private int validFillingQuantity = 50;
        private DateTime fixedNow = new DateTime(2010, 1, 2, 08, 14, 3);
        private string someValidFlavour = "cherry";

        [TestInitialize]
        public void Setup()
        {
            // Runs before each test.

            //uut is short for unit under test. Some people use sut (system under test)
            uut = new Pie2Service(_mockDataService.Object, _mockPastryService.Object, _mockFillingService.Object, _mockLogger.Object, _mockNowService.Object);
        }

        [TestMethod]
        [DataRow("   ")]
        [DataRow("invalid")]
        [DataRow("")]
        [DataRow(null)]
        [DataTestMethod]
        public void GetPieRecord_returns_BadRequest_pie_for_invalid_flavour(string inputFlavour)
        {
            PieRecord expectedResult = getPieRecord(inputFlavour);
            SetUpMocks(expectedResult, inputFlavour);
            // We have to recreate the Array here. If the list of recognised flavours was moved to the data service then we wouldn't need to do this.
            // Our uut service would be cleaner and our test less brittle
            string[] RecognisedFlavours = { "Cherry", "Apple", "Cheese" };

            var actualResult = uut.GetPieRecord(inputFlavour).GetAwaiter().GetResult();

            // Verify for serilog calls fails for null. Hence inputFlavour is It.IsAny<string>()
            // CodeInfo contains the line number of the calling code so can change.  The line number has no affect on functionality hence it is It.IsAny<string>()
            _mockLogger.Verify(p => p.Warning("Flavour {Flavour} not allowed. Allowed flavours {RecognisedFlavours}. {CodeInfo}",
                It.IsAny<string>(), RecognisedFlavours, It.IsAny<string>()), Times.Once());

            Assert.AreEqual(BadRequest, actualResult.StatusCodeHttp);
            Assert.IsNull(actualResult.PieRecord);
        }

        [TestMethod]
        public void GetPieRecord_attempts_pastry_order_if_needed()
        {
            PieRecord expectedResult = getPieRecord(someValidFlavour);
            SetUpMocks(expectedResult, someValidFlavour);
            _mockPastryService.Setup(p => p.Get(It.IsAny<int>())).Throws<ArgumentException>();

            var actualResult = uut.GetPieRecord(someValidFlavour).GetAwaiter().GetResult();

            _mockLogger.Verify(p => p.Information("Ordering pastry. {CodeInfo}", It.IsAny<string>()), Times.Once());
            _mockPastryService.Verify(p => p.Order(), Times.Once());
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(49)]
        [DataRow(51)]
        [DataRow(-50)]  //if I changed this to 50 then this test should fail - Red/green testing
        [DataTestMethod]
        public void GetPieRecord_returns_InternalServerError_and_logs_error_for_unexpected_pastry_quantity(int invalid)
        {
            PieRecord expectedResult = getPieRecord(someValidFlavour);
            SetUpMocks(expectedResult, someValidFlavour);
            //overwrite this setup for this test
            _mockPastryService.Setup(p => p.Get(validPastryQuantity)).Returns(invalid);

            var actualResult = uut.GetPieRecord(someValidFlavour).GetAwaiter().GetResult();

            _mockLogger.Verify(p => p.Error("Pastry required {Required}. Pastry received {Received}. {CodeInfo}", validPastryQuantity, invalid, It.IsAny<string>()), Times.Once());
            Assert.AreEqual(InternalServerError, actualResult.StatusCodeHttp);
            Assert.IsNull(actualResult.PieRecord);
        }

        [TestMethod]
        public void GetPieRecord_attempts_filling_order_if_incorrect_filling_quantity_returned()
        {
            PieRecord expectedResult = getPieRecord(someValidFlavour);
            SetUpMocks(expectedResult, someValidFlavour);
            _mockFillingService.Setup(p => p.Get(someValidFlavour, It.IsAny<int>())).Throws<ArgumentException>();

            var actualResult = uut.GetPieRecord(someValidFlavour).GetAwaiter().GetResult();

            _mockLogger.Verify(p => p.Information("Ordering {Flavour} filling. {CodeInfo}", someValidFlavour, It.IsAny<string>()), Times.Once());
            _mockFillingService.Verify(p => p.Order(someValidFlavour), Times.Once());
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(49)]
        [DataRow(51)]
        [DataRow(-50)]  //if I changed this to 50 then this test should fail - Red/green testing
        [DataTestMethod]
        public void GetPieRecord_returns_InternalServerError_and_logs_error_for_unexpected_filling_quantity(int invalid)
        {
            PieRecord expectedResult = getPieRecord(someValidFlavour);
            SetUpMocks(expectedResult, someValidFlavour);
            //overwrite this setup for this test
            _mockFillingService.Setup(p => p.Get(someValidFlavour, validFillingQuantity)).Returns(invalid);

            var actualResult = uut.GetPieRecord(someValidFlavour).GetAwaiter().GetResult();

            _mockLogger.Verify(p => p.Error("{Flavour} filling required {Required}. Filling received {Received}. {CodeInfo}", someValidFlavour, validFillingQuantity, invalid, It.IsAny<string>()), Times.Once());
            Assert.AreEqual(InternalServerError, actualResult.StatusCodeHttp);
            Assert.IsNull(actualResult.PieRecord);
        }

        [TestMethod]
        public void GetPieRecord_returns_InternalServerError_and_logs_error_for_no_pie()
        {
            PieRecord expectedResult = getPieRecord(someValidFlavour);
            expectedResult.MostRecent = null;
            SetUpMocks(expectedResult, someValidFlavour);

            var actualResult = uut.GetPieRecord(someValidFlavour).GetAwaiter().GetResult();

            _mockLogger.Verify(p => p.Error("{Flavour} Pie not made. {CodeInfo}", someValidFlavour, It.IsAny<string>()), Times.Once());
            Assert.AreEqual(InternalServerError, actualResult.StatusCodeHttp);
            Assert.IsNull(actualResult.PieRecord);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataTestMethod]
        public void GetPieRecord_returns_Ok_and_logs_error_for_no_pie_audit(string emptyType)
        {
            PieRecord expectedResult = getPieRecord(someValidFlavour);
            expectedResult.PieAudit = emptyType == null ? null : new List<Pie>();
            SetUpMocks(expectedResult, someValidFlavour);

            var actualResult = uut.GetPieRecord(someValidFlavour).GetAwaiter().GetResult();

            _mockLogger.Verify(p => p.Error("No pie audit. {CodeInfo}", It.IsAny<string>()), Times.Once());
            Assert.AreEqual(Ok, actualResult.StatusCodeHttp);
            Assert.AreEqual(expectedResult.PieAudit?.Count(), actualResult.PieRecord.PieAudit?.Count());
        }

        [TestMethod]
        [DataRow("CHERRY   ", "cherry")]
        [DataRow("  apPle", "apple")]
        [DataRow("cheese ", "cheese")]
        [DataTestMethod]
        public void GetPieRecord_returns_expected_pie_for_valid_flavour(string inputFlavour, string expectedFlavour)
        {
            PieRecord expectedResult = getPieRecord(expectedFlavour);
            //Change the casing of some flavours - should be equal even if casing is different
            expectedResult.PieAudit[4].Flavour = expectedResult.PieAudit[4].Flavour.ToLower();
            expectedResult.PieAudit[5].Flavour = expectedResult.PieAudit[5].Flavour.ToUpper();
            SetUpMocks(expectedResult, expectedFlavour);

            var actualResult = uut.GetPieRecord(inputFlavour).GetAwaiter().GetResult();

            Assert.AreEqual(expectedFlavour, actualResult.PieRecord.MostRecent.Flavour);
            // Our NowAdapter lets us test that both pie and audit have a date time and it is the same date time
            Assert.AreEqual(fixedNow.ToString("T"), actualResult.PieRecord.MostRecent.LastMadeOn);
            Assert.AreEqual(fixedNow.ToString("T"), actualResult.PieRecord.PieAudit.First().LastMadeOn);
            Assert.IsNotNull(actualResult.PieRecord);
            Assert.AreEqual(expectedResult.PieAudit.Count, actualResult.PieRecord.PieAudit.Count);

            //Check each pie in the audit record is correct
            for(var i = 0;i < expectedResult.PieAudit.Count;i++)
            {
                Assert.IsTrue(expectedResult.PieAudit[i].Equals(actualResult.PieRecord.PieAudit[i]));
            }
            Assert.AreEqual(1, actualResult.PieRecord.PieAudit.Where(p => p.Flavour == expectedFlavour).Count());
        }

        private PieRecord getPieRecord(string flavour)
        {
            Pie expectedPie = new Pie
            {
                Flavour = flavour,
                LastMadeOn = fixedNow.ToString("T")
            };
            List<Pie> expectedPieAudit = new List<Pie> { expectedPie };
            for (var i = 0; i < 5; i++)
            {
                Pie pie = new Pie
                {
                    Flavour = flavour + i, 
                    LastMadeOn = fixedNow.AddMinutes(-i).ToString("T")
                };
                expectedPieAudit.Add(pie);
            }
            return new PieRecord
            {
                MostRecent = expectedPie,
                PieAudit = expectedPieAudit
            };
        }

        private void SetUpMocks(PieRecord expectedResult, string expectedFlavour)
        {
            _mockPastryService.Setup(p => p.Get(validPastryQuantity)).Returns(validPastryQuantity);
            _mockFillingService.Setup(p => p.Get(expectedFlavour, validFillingQuantity)).Returns(validFillingQuantity);
            _mockNowService.Setup(p => p.Now()).Returns(fixedNow);
            _mockDataService.Setup(p => p.BakePie(expectedFlavour, validPastryQuantity, validFillingQuantity, fixedNow)).ReturnsAsync(expectedResult.MostRecent);
            _mockDataService.Setup(p => p.GetPieAudit()).ReturnsAsync(expectedResult.PieAudit);
        }
    }
}