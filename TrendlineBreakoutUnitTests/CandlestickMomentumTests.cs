using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TrendlineBreakoutUnitTests
{
    [TestClass]
    public class CandlestickMomentumTests
    {
        [TestMethod]
        public void CandleStick_ShouldReturn_UpwardMomentum()
        {
            /*
             * How do you see momentum in a candlestick?  Well, you need two things:
             * ---------------------------------------------------------------------
                • the length of the candlestick (how long a candlestick is)
                • and the opening and closing prices.

                - Momentum Is A Product Of Time And Price
                    - an increase in momentum happens when price increase (or decreases) 
                        very quickly in a short period of time

                - Candle pulls back to 50% of its range

                /*
                 Decreasing Momentum
                 ----------------------------
                 - Price heading up to resistance/support
                 - Bullish candlestick start to decrease (candlesticks get shorter in length)
                 - Culminates in a PinBar / Hammer / Dojis

                 Candlestic order of strength
                 ----------------------------
                 1. Shaven/Mabaruzo 
                 2. PinBar
                 3. Upper & Lower Shadows nearly equal, nearly equal to the body
                    3.1. Body is twice the size of the Upper / Lower Shadow
                 4. Doji
             */


        }

        [TestMethod]
        public void CandleStick_ShouldReturn_NormalUpwardMomentum()
        {

        }

        [TestMethod]
        public void CandleStick_ShouldReturn_WeakUpwardMomentum()
        {

        }

        [TestMethod]
        public void BullsLosingMomentum_ShouldReturn_True()
        {

        }

        [TestMethod]
        public void BearsLosingMomentum_ShouldReturn_True()
        {
            
        }

    }
}
