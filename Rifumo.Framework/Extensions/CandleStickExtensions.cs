using System;

namespace Rifumo.Framework.Extensions
{
    public static class CandleStickExtensions
    {
        public static bool IsBullishCandle(this CandleStick candleStick)
        {
            return candleStick.Close > candleStick.Open;
        }

        public static bool IsBearishCandle(this CandleStick candleStick)
        {
            return candleStick.Close < candleStick.Open;
        }

        public static bool IsDoji(this CandleStick candleStick, int pointPow)
        {
            if (Math.Abs(candleStick.Open - candleStick.Close) * pointPow < 0.6) return (true);
            return (false);
        }

        public static bool IsSmallLowerShadow(this CandleStick candleStick)
        {
            return candleStick.GetLowerShadowHeight < candleStick.GetAllHeight / 5.0;
        }

        public static bool IsSmallUpperShadow(this CandleStick candleStick)
        {
            return candleStick.GetUpperShadowHeight < candleStick.GetAllHeight / 5.0;
        }

        public static bool IsBodyPositionHigher(this CandleStick candleStick)
        {
            double bodyLength = candleStick.High - candleStick.Low;
            double bodyPosition = 0.4;                                 //Body position in Candle (e.g. top/bottom 40%)

            if (1 - (Math.Min(candleStick.Open, candleStick.Close) - candleStick.Low) / bodyLength < bodyPosition) return true;
            return false;
        }

        public static bool IsBodyPositionLower(this CandleStick candleStick)
        {
            double bodyLength = candleStick.High - candleStick.Low;
            double bodyPosition = 0.4;                                 //Body position in Candle (e.g. top/bottom 40%)

            if (1 - (candleStick.High - Math.Max(candleStick.Open, candleStick.Close)) / bodyLength < bodyPosition) return true;
            return false;
        }

        public static bool IsBullishPinBar(this CandleStick candleStick)
        {
            /*
                    ___
                   |   |
                   |___|
                     |
                     |
                     |
                     |
                     |

           Criteria
           1. The lower shadow should be at least two times the length of the body.
           2. The real body is at the upper end of the trading range. The color of the body is not
           important although a white body should have slightly more bullish implications.
           3. There should be no upper shadow or a very small upper shadow.
           The following day needs to confirm the Hammer signal with a strong bullish day   
          */
            if (candleStick.IsBodyPositionHigher())
            {
                double bodySize = candleStick.GetBodyHeight;
                bool smallUpperShadow = candleStick.IsSmallUpperShadow;
                bool lowerShadowAtLeastTwiceBodySize = candleStick.GetLowerShadowHeight > (2.0 * bodySize);

                if (smallUpperShadow && lowerShadowAtLeastTwiceBodySize)
                {
                    return true;
                }
            }
            return false;

        }

        public static bool IsBearishPinBar(this CandleStick candleStick)
        {
            /*
                   |
                   |
                   |
                   |
                   |
                  ____
                 |    |
                 |____|  

               Criteria
               1. The upper shadow should be at least two times the length of the body.
               2. The real body is at the lower end of the trading range. The color of the body is not important, although a white body should have slightly more bullish implications.
               3. There should be no lower shadow, or a very small lower shadow.
              */
            if (candleStick.IsBodyPositionLower())
            {
                double bodySize = candleStick.GetBodyHeight;
                bool smallLowerShadow = candleStick.IsSmallLowerShadow;
                bool upperShadowAtLeastTwiceBodySize = candleStick.GetUpperShadowHeight > (2.0 * bodySize);

                if (smallLowerShadow && upperShadowAtLeastTwiceBodySize)
                {
                    return true;
                }
            }
            return false;
        }

        public static string Emit(this CandleStick candleStick)
        {
            var candleProps = $"{candleStick.TickDateTime.ToString()}, bodySize: {candleStick.GetBodyHeight}, " +
                $"smallLowerShadow: {candleStick.IsSmallLowerShadow}, upperShadowAtLeastTwiceBodySize: {candleStick.GetUpperShadowHeight} > {(2.0*candleStick.GetBodyHeight)}";
            return candleProps;
        }
    }
}
