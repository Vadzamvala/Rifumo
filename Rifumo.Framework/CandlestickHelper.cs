using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rifumo.Framework
{
    public class CandlestickHelper
    {
        #region Properties
        private string symbol = "";
        public string Symbol
        {
            get { return symbol; }
            set
            {
                symbol = value;
                Point = GetPoint();
                Multiplier = GetMultipler();
            }
        }

        private int brokerDigits = 5;
        public int BrokerDigits
        {
            get { return brokerDigits; }
            private set { brokerDigits = value; }
        }

        private double _symbolPoint = 0.0001;
        public double Point
        {
            get { return _symbolPoint; }
            private set { _symbolPoint = value; }
        }

        private int multiplier = 10000;
        public int Multiplier
        {
            get { return multiplier; }
            private set { multiplier = value; }
        }
        #endregion

        private double _averageCandleBodySize;
        private double _powerCoef;
        private double _pointPow;

        public CandlestickHelper(string symbol, int digits)
        {
            Symbol = symbol;
            BrokerDigits = digits;
        }

        public bool IsHigher(CandleStick currentCandle, CandleStick shift2, CandleStick shift3, CandleStick shift4)
        {
            if (currentCandle.High >= shift2.High + 2.0 * Point && currentCandle.High >= shift3.High + 2.0 * Point && currentCandle.High >= shift4.High + 2.0 * Point) return true;
            return false;
        }

        public bool IsBodyHigher(CandleStick currentCandle, CandleStick shift2, CandleStick shift3)
        {
            if (currentCandle.Close > shift2.GetHighCloseOpen + 2.0 * Point && currentCandle.Close > shift3.GetHighCloseOpen + 2.0 * Point) return true;
            return false;
        }

        public bool IsLower(CandleStick shift1, CandleStick shift2, CandleStick shift3)
        {
            if (shift1.Low + 2.0 * Point < shift2.Low && shift1.Low + 2.0 * Point < shift3.Low && shift1.Low + 2.0 * Point < shift3.Low) return true;
            return false;
        }

        public bool AlmostSameBodyHeight(CandleStick shift, CandleStick shift1)
        {
            if (Math.Abs(shift.GetBodyHeight - shift1.GetBodyHeight) * GetMultipler() < 5.0) return true;
            return false;
        }

        public bool IsSignificantCandle(CandleStick candleStick, double averageCandleSize, bool bodyHeightOnly = true)
        {
            if (!bodyHeightOnly)
            {
                return candleStick.GetAllHeight > averageCandleSize;
            }
            return candleStick.GetBodyHeight > averageCandleSize;
        }

        public bool IsWithinRange(double x, double y, int threshold)
        {
            double percentageAmt = ((Math.Abs(x - y)/Point)*100);
            if (percentageAmt > 100)
            {
                percentageAmt = percentageAmt / 10;
            }

            if (percentageAmt >= 1 && percentageAmt <= threshold)
            {
                return true;
            }

            return false;
        }

        #region Helpers
        private int GetMultipler()
        {
            if (GetPoint() == 0.0001) return 10000;
            return 1000;
        }
        private double GetPoint()
        {
            double __point = 0.0001;
            if (Symbol.Substring(2, 3) == "JPY")
            {
                __point = 0.001;
            }
            return __point;
        }

        #endregion
    }
}
