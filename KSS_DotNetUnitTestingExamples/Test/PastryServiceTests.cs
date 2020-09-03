using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Serilog;
using Services;
using System;

namespace Test
{
    [TestClass]
    public class PastryServiceTests
    {
        [TestMethod]
        public void GetPastry_returns_50_with_enough_stock()
        {
            var uut = new PastryService();

            var result = uut.Get(50);
            Assert.AreEqual(50, result);
        }

        [TestMethod]
        public void GetPastry_throws_with_insufficient_stock()
        {
            var uut = new PastryService();
            UseUpStock(uut);

            var ex = Assert.ThrowsException<ArgumentException>(() => uut.Get(50));


            // We don't usually care what is in an exception message so adding the assert below just makes the test brittle
           // Assert.AreEqual("Don't do this", ex.Message);
        }

        public void Order_adds_stock_as_expected()
        {
            var uut = new PastryService();
            UseUpStock(uut);

            uut.Order();

            for (var i = 0; i < 10; i++)
            {
                var result = uut.Get(50);
                Assert.AreEqual(50, result);
            }
        }

        private void UseUpStock(PastryService uut)
        {
            for (var i = 0; i < 10; i++)
            {
                uut.Get(50);
            }
        }
    }
}