using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StochTrend
{
    class Program
    {
        enum Trend
        {
            OP_BUY,
            OP_SELL,
            Ranging
        }
        static void Main(string[] args)
        {
            var buyValues = new List<double>() { 5.2067, 11.7658, 22.8533, 49.4974, 80.9580  };
            var sellValues = new List<double>() { 11.9118, 34.1677, 55.9229, 69.7134, 81.3958  };
            var acceptRange = 5;
            var symbol = "EURUSD";

            //Console.WriteLine($"Trend = {GetTrend(buyValues)}");
            //Console.WriteLine($"Trend = {GetTrend(sellValues)}");

            //SELLS
            double x = 1.17918, y = 1.17810;    //EURUSD (x= high; y = lowerResistance (val2): 2017.12.12 10:00)
            Console.WriteLine($"IsInRange - {IsWithinRange(x, y, acceptRange, symbol)}");

            x = 1.17905; y = 1.17810;    //EURUSD (x= high; y = lowerResistance (val2): 2017.12.12 11:00)
            Console.WriteLine($"IsInRange - {IsWithinRange(x,y,acceptRange,symbol)}");




            Console.ReadKey();
        }

        private static string Symbol()
        {
            return "EURUSD";
        }

        private static double GetPoint(string symbol)
        {
            if (symbol.Substring(0,5)=="USDJPY")
            {
                return 0.001;
            }
            return 0.0001;
        }

        static Trend GetTrend(List<double> stochValues)
        {
            int total = stochValues.Count;
            int trend=0, accessor=0;
            for (int i = 0; i <= total; i++)
            {
                if (i==0 || i>=total)
                {
                    accessor = i;
                }
                else
                {
                    accessor = (i+1);
                }

                if (stochValues[i]>stochValues[accessor])
                    trend++;
                else
                    trend--;
            }

            if (trend > 0) return Trend.OP_BUY;
            if (trend < 0) return Trend.OP_SELL;

            return Trend.Ranging;
        }

        static bool IsWithinRange(double x, double y, int acceptablePercentage, string symbol)
        {
            double point = GetPoint(symbol);
            if ((Math.Abs(x - y) / point) / 100 <= acceptablePercentage) return true;
            return false;
        }
    }
}
