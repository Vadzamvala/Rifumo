using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rifumo.Framework;

namespace TrendlineBreakoutUnitTests
{
    [TestClass]
    public class PriceRangeTests
    {
        int rangePercentageThreshold = 5;

        [TestMethod]
        public void IsLowPriceWithinRange_ShouldReturn_True()
        {
            //Arrange
            var candleCloseDt = new DateTime(2018, 03, 09, 15, 00, 00);
            var candle = GetCandle("EURUSD", candleCloseDt, 1.2294, 1.2313, 1.2272, 1.2293);
            var trendlineValue = 1.2269;

            var candleStickHelper = new CandlestickHelper(candle.Symbol, 5);

            //Act
            var isCandleStickInRange = candleStickHelper.IsWithinRange(candle.Low, trendlineValue, rangePercentageThreshold);

            //Assert
            Assert.IsTrue(isCandleStickInRange == false);
        }

        [TestMethod]
        public void IsLowPriceWithinRange_ShouldReturn_False()
        {
            //Arrange
            var candleCloseDt = new DateTime(2018, 03, 13, 10, 00, 00);
            var candle = GetCandle("EURUSD", candleCloseDt, 1.2318, 1.2334, 1.2314, 1.2333);
            var trendlineValue = 1.2305;

            var candleStickHelper = new CandlestickHelper(candle.Symbol, 5);
            //Act
            var isCandleStickInRange = candleStickHelper.IsWithinRange(candle.Low, trendlineValue, rangePercentageThreshold);
            //Assert
            Assert.IsTrue(isCandleStickInRange == false);
        }

        [TestMethod]
        public void IsHighPriceWithinRange_ShouldReturn_True()
        {
            //Arrange
            //2018,03,08,11,00,00
            //0.94434,0.94512,0.94423,0.94504

            var candleCloseDt = new DateTime(2018, 03, 08, 12, 00, 00);
            var candle = GetCandle("USDCHF", candleCloseDt, 0.945, 0.9455, 0.9445, 0.9454);
            var trendlineValue = 0.945;

            var candleStickHelper = new CandlestickHelper(candle.Symbol, 5);

            //Act
            var isCandleStickInRange = candleStickHelper.IsWithinRange(candle.High, trendlineValue, rangePercentageThreshold);

            //Assert
            Assert.IsTrue(isCandleStickInRange == true);
        }

        [TestMethod]
        public void IsHighPriceWithinRange_ShouldReturn_False()
        {
            //Arrange
            var candleCloseDt = new DateTime(2018, 03, 07, 16, 00, 00);
            var candle = GetCandle("USDCHF", candleCloseDt, 0.94084, 0.94265, 0.94248, 0.95000);
            var trendlineValue = 0.94369;

            var candleStickHelper = new CandlestickHelper(candle.Symbol, 5);

            //Act
            var isCandleStickInRange = candleStickHelper.IsWithinRange(candle.High, trendlineValue, rangePercentageThreshold);

            //Assert
            Assert.IsTrue(isCandleStickInRange == false);


        }

        [TestMethod]
        public void IsPriceAboveTrendline()
        {
            //Arrange
            //Act
            //Assert
        }

        [TestMethod]
        public void IsPriceBelowTrendline()
        {
            //Arrange
            //Act
            //Assert
        }

        #region Helpers
        private CandleStick GetCandle(string symbol, DateTime candleDateTime,double open, double high, double low, double close)
        {
            CandleStruct candleData = new CandleStruct()
            {
                Open = open,
                High = high,
                Low = low,
                Close = close,
                Symbol = symbol,
                TickDateTime = candleDateTime
            };
            return new CandleStick(candleData);
        }
        #endregion
    }
}
