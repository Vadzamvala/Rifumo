using System;
using System.Collections.Generic;

namespace Rifumo.Framework
{
    public abstract class AbstractCandleStickTester
    {
        #region Fields
        private readonly CandlestickHelper _helper;
        #endregion

        #region Constructor
        public AbstractCandleStickTester() { }

        public AbstractCandleStickTester(string symbol, int digits)
        {
            _helper = new CandlestickHelper(symbol, digits);
        }
        #endregion

        public abstract void ExecuteBackTesting();

        public abstract void GenerateTickData();

    }
}
