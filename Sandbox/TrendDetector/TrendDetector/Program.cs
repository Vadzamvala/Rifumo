using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendDetector
{
    class Program
    {
        static void Main(string[] args)
        {
            //var timeSeries = GetTimeSeries();
            //GetTrend(timeSeries);

            /*
              BUYS
              ----
                EURUSD: 2017.12.13 16:00 BUY (x = 1.17467, Y (VAL6; US) = 1.1746)
                EURUSD: 2017.12.13 17:00 BUY (x = 1.17483, Y (VAL6; US) = 1.1747)
                USDJPY: 2017.12.12 20:00 BUY (x = 112.461, Y (VAL6; US) = 112.4259)
                USDCAD: 2017.12.12 15:00 BUY (x = 1.28192, Y (VAL6; US) = 1.2818)
                USDCAD: 2017.12.14 03:00 BUY (x = 1.27990, Y (VAL6; US) = 1.2799)
                GOLD: 2017.12.13 14:00 BUY (x = 1240.12, Y (VAL6; US) = 1240.8774)

                x=1.17467; y=1.1746;
                x=1.17483; y=1.1747;
                x=112.461; y=112.4259;
                x=1.28192; y=1.2818;
                x=1.27990; y=1.2799;
                x=1240.12; y=1240.8774;
                x=1.28915; y=1.2893;

               SELLS
                -----
                USDCAD: 2017.12.12 19:00 SELL (x = 1.28915, Y (VAL3; LR) = 1.2893)             

            double x = 0; double y = 0;
            var n = 10;

            x = 1.17467; y = 1.1746;
            Console.WriteLine($"WithinRange = {WithinRange(x, y, n)}");
            x = 1.17483; y = 1.1747;
            Console.WriteLine($"WithinRange = {WithinRange(x, y, n)}");
            x = 112.461; y = 112.4259;
            Console.WriteLine($"(JPY) WithinRange = {WithinRange(x, y, n)}");
            x = 1.28192; y = 1.2818;
            Console.WriteLine($"WithinRange = {WithinRange(x, y, n)}");
            x = 1.27990; y = 1.2799;
            Console.WriteLine($"WithinRange = {WithinRange(x, y, n)}");
            x = 1240.12; y = 1240.8774;
            Console.WriteLine($"WithinRange = {WithinRange(x, y, n)}");
            x = 1.28915; y = 1.2893;
            Console.WriteLine($"WithinRange = {WithinRange(x, y, n)}");

            //*** Test Control (should fail) ***

            //x = 1.28915; y = 1.2893;
            //Console.WriteLine($"WithinRange = {WithinRange(x, y, n)}");
            //x = 1.28915; y = 1.2893;
            //Console.WriteLine($"WithinRange = {WithinRange(x, y, n)}");
            */

            //var tester = new PinBarTester();
            //tester.ExecuteBackTesting();

            //struct Point p1 = {1, 1}, q1 = {10, 1};
            //struct Point p2 = {1, 2}, q2 = {10, 2};

            //DoIntersect(p1, q1, p2, q2)? cout << "Yes\n": cout << "No\n";

            //p1 = {10, 0}, q1 = {0, 10};
            //p2 = {0, 0}, q2 = {10, 10};
            //DoIntersect(p1, q1, p2, q2)? cout << "Yes\n": cout << "No\n";

            //p1 = {-5, -5}, q1 = {0, 0};
            //p2 = {1, 1}, q2 = {10, 10};
            //DoIntersect(p1, q1, p2, q2)? cout << "Yes\n": cout << "No\n";

            var montName = "Jan";
            Console.WriteLine(montName, " => ", FromMonthName(montName));
            montName = "Feb";
            Console.WriteLine(montName, " => ", FromMonthName(montName));
            montName = "Mar";
            Console.WriteLine(montName, " => ", FromMonthName(montName));
            montName = "April";
            Console.WriteLine(montName, " => ", FromMonthName(montName));
            montName = "May";
            Console.WriteLine(montName, " => ", FromMonthName(montName));

            Console.ReadKey();
        }

        static void GetTrend(double[] timeSeries)
        {
               int trend = 0;
               int total = timeSeries.Length;
               double checkSum = 0;
   
               //create the sequence
               /*for(int i=0;i<total;i++)
                 {
                     timeSeries[i]=iCustom(NULL,0,afiStoch,Len,Filter,0,shift+i);
                 }*/
    
                for(int i=0;i<total;i++)
                 {
                    checkSum = timeSeries[i];
                     if (timeSeries[i]>checkSum)
                     {
                        trend++;
                     }
                     else if (timeSeries[i] < checkSum)
                {
                        trend--;
                     }
                 }
        }
        static double[] GetTimeSeries()
        {
            var timeSeries = new List<double>()
            {
                //Buy as sequence is decreasing
                13.0172, 9.8130, 7.0865, 3.7672, 1.8171
            };

            return timeSeries.ToArray();
        }
        static bool AboutEqual(double x, double y)
        {
            double epsilon = Math.Max(Math.Abs(x), Math.Abs(y)) * 1E-15;
            return Math.Abs(x - y) <= epsilon;
        }
        static bool WithinRange(double x, double y, int percentage)
        {
            //Range = (Abs(x-y)*n)/100 <= n
            double percentageAmt = IsGold(x) ? (Math.Abs(x - y)*percentage) : ((Math.Abs(x - y) / GetPoint(x)) / percentage);

            if (IsGold(x))
            {
                return (percentageAmt >= (100-percentage));
            }
            else
            {
                if (percentageAmt <= percentage)
                {
                    return true;
                }
            }
            return false;
        }
        static double GetPoint(double x)
        {
            if (IsJPYPair(x)) return 0.001;
            return  0.00001;
        }
        static bool IsJPYPair(double inputValue)
        {
            var asString = inputValue.ToString();
            var pos = asString.IndexOf('.');
            return pos==3;
        }
        static bool IsGold(double inputValue)
        {
            var asString = inputValue.ToString();
            var pos = asString.IndexOf('.');
            return pos == 3;
        }


        static string FromMonthName(string inputMonth)
        {
            string[] monthNames = { "", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

            if (inputMonth.Length>3)
            {
                inputMonth = inputMonth.Substring(0, 3);
            }

            var matchedAtPosition = monthNames.Select((Value, Index) => new { Value, Index })
                        .Where(pair => monthNames.Any(target => pair.Value.Contains(inputMonth)))
                        .Select(pair => pair.Index)
                        .FirstOrDefault();


            string monthNo = (matchedAtPosition+1 < 10 ? string.Concat("0", matchedAtPosition.ToString()) : matchedAtPosition.ToString());
            return monthNo;
        }
    }
}
