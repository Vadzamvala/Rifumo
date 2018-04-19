using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Rifumo.Framework;
using Rifumo.Framework.Extensions;

namespace TrendlineBreakoutUnitTests
{
    [TestClass]
    public class PinBarTests
    {
        [TestMethod]
        public void IsBullishPinBar_Should_ReturnTrue()
        {
            var pinBarCandleList = new List<CandleStick>()
            {
                new CandleStick("USDCAD",new DateTime(2018,01,08,03,00,00),1.23880,1.23892,1.23774,1.23883), // Bullish
                new CandleStick("USDCAD",new DateTime(2018,01,08,13,00,00),1.24032,1.24124,1.23865,1.24105), // Bullish
                new CandleStick("USDCAD",new DateTime(2018,01,09,09,00,00),1.24105,1.24205,1.23971,1.24167), // Bullish
                new CandleStick("USDCAD",new DateTime(2018,01,10,13,00,00),1.24385,1.24430,1.24266,1.24408), // Bullish
                new CandleStick("USDCAD",new DateTime(2018,01,12,17,00,00),1.25106,1.25147,1.24921,1.25099), // Bullish
                new CandleStick("USDCAD",new DateTime(2018,01,15,12,00,00),1.24196,1.24257,1.24042,1.24213),  // Bullish

                new CandleStick("EURUSD",new DateTime(2018,03,02,09,00,00),1.22619,1.22662,1.22506,1.22624),
                new CandleStick("EURUSD",new DateTime(2018,03,02,11,00,00),1.22734,1.22809,1.22615,1.22751),
                new CandleStick("EURUSD",new DateTime(2018,03,08,13,00,00),1.23779,1.23782,1.23679,1.23776),
                new CandleStick("EURUSD",new DateTime(2018,03,13,16,00,00),1.23681,1.23806,1.23393,1.23739),
                new CandleStick("EURUSD",new DateTime(2017,03,14,17,00,00),1.23623,1.23672,1.23464,1.23604),
                new CandleStick("EURUSD",new DateTime(2017,03,16,10,00,00),1.23174,1.23214,1.23058,1.23173),  //Doji type
                new CandleStick("EURUSD",new DateTime(2017,03,16,19,00,00),1.22905,1.22905,1.22765,1.22881),  //Hammer type
            };

            var numberValidations = pinBarCandleList.Count;
            var numberPassed = 0;
            foreach (var nextCandle in pinBarCandleList)
            {
                if (nextCandle.IsBullishPinBar())
                {
                    numberPassed++;
                }
                else
                {
                    Console.WriteLine($"Failed: {nextCandle.Emit()}");
                }
            }
            Assert.IsTrue(numberPassed == numberValidations);
        }

        public void IsBearishPinBar_ShoudReturnTrue()
        {
            var pinBarCandleList = new List<CandleStick>()
            {
                new CandleStick("USDCAD",new DateTime(2018,01,10,15,00,00),1.24261,1.24404,1.24228,1.24302), // Bearish
                new CandleStick("USDCAD",new DateTime(2018,01,09,17,00,00),1.24564,1.24772,1.24511,1.24596), // Bearish
                new CandleStick("USDCAD",new DateTime(2018,01,10,17,00,00),1.24826,1.24834,1.24627,1.24792), // Bearish
                new CandleStick("USDCAD",new DateTime(2018,01,11,23,00,00),1.25260,1.25607,1.25124,1.25143), // Bearish
                new CandleStick("USDCAD",new DateTime(2018,01,12,18,00,00),1.25098,1.25331,1.25038,1.25176), // Bearish
                new CandleStick("EURNZD",new DateTime(2018,01,15,18,00,00),1.67685,1.68081,1.67603,1.67750)  // Bearish
            };
        }
    }
}
