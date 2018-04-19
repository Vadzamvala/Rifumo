using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rifumo.Framework
{
    public struct CandleStruct
    {
        public double Open;
        public double High;
        public double Low;
        public double Close;
        public DateTime TickDateTime;
        public string Symbol; 
    }

    public class CandleStick
    {
        public CandleStick(CandleStruct _candleData)
        {
            Open = _candleData.Open;
            High = _candleData.High;
            Low = _candleData.Low;
            Close = _candleData.Close;
            TickDateTime = _candleData.TickDateTime;
            Symbol = _candleData.Symbol;
        }

        public CandleStick(string _symbol, DateTime _candleDt, double _open, double _high, double _low, double _close)
        {
            Open = _open;
            High = _high;
            Low = _low;
            Close = _close;
            TickDateTime = _candleDt;
            Symbol = _symbol;
        }

        public DateTime TickDateTime { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }

        public string Symbol { get; set; }

        public double GetUpperShadowHeight
        {
            get { return (Math.Abs(High - Math.Max(Close, Open))); }
        }

        public double GetLowerShadowHeight
        {
            get { return Math.Abs(Math.Min(Close, Open) - Low); }
        }

        public double GetBodyHeight
        {
            get { return (Math.Abs(Close - Open)); }
        }

        public double GetAllHeight
        {
            get { return (Math.Abs(High - Low)); }
        }

        public double GetLowCloseOpen
        {
            get { return (Math.Min(Close, Open)); }
        }

        public double GetHighCloseOpen
        {
            get { return (Math.Max(Close, Open)); }
        }

        public bool IsSmallLowerShadow
        {
            get { return (GetLowerShadowHeight < (GetAllHeight / 5.0)); }
        }

        public bool IsSmallUpperShadow
        {
            get { return (GetUpperShadowHeight < (GetAllHeight / 5.0)); }
        }
    }
}
