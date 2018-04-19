using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TrendlineBreakoutUnitTests
{
    enum MarketTrend
    {
        UpTrend,
        Downtrend,
        Sideways
    }

    [TestClass]
    public class LinearRegressionTests
    {
        private readonly string Symbol = "EURUSD";

        [TestMethod]
        public void LinearRegression_ShouldReturnUpTrend()
        {
            //Ascending
            var highs = new List<double>() { 1.24110, 1.24047, 1.23979, 1.23945, 1.23941, 1.23963, 1.23929, 1.23992, 1.23987, 1.24054 };
            var lows = new List<double>() { 1.23998, 1.23896, 1.23882, 1.23882, 1.23896, 1.23880, 1.23876, 1.23837, 1.23835, 1.23898 };

            double rSquared;
            double yIntercept;
            double slope;

            LinearRegression(highs.ToArray(), lows.ToArray(), 0, highs.Count, out rSquared, out yIntercept, out slope);

            Assert.IsTrue(slope * GetPoint() > 0);
        }

        [TestMethod]
        public void LinearRegression_ShouldReturnDownTrend()
        {
            //Descending
            var highs = new List<double>() { 1.22956, 1.22847, 1.22765, 1.22860, 1.22764, 1.22636, 1.22619, 1.22543, 1.22473, 1.22442 };
            var lows = new List<double>() { 1.22792, 1.22622, 1.22622, 1.22689, 1.22446, 1.22524, 1.22484, 1.22448, 1.22390, 1.22395 };

            double rSquared;
            double yIntercept;
            double slope;

            LinearRegression(highs.ToArray(), lows.ToArray(), 0, highs.Count, out rSquared, out yIntercept, out slope);

            Assert.IsTrue((slope * GetPoint()) < 0);
        }

        /// <summary>
        /// Fits a line to a collection of (x,y) points.
        /// </summary>
        /// <param name="xVals">The x-axis values.</param>
        /// <param name="yVals">The y-axis values.</param>
        /// <param name="inclusiveStart">The inclusive inclusiveStart index.</param>
        /// <param name="exclusiveEnd">The exclusive exclusiveEnd index.</param>
        /// <param name="rsquared">The r^2 value of the line.</param>
        /// <param name="yintercept">The y-intercept value of the line (i.e. y = ax + b, yintercept is b).</param>
        /// <param name="slope">The slop of the line (i.e. y = ax + b, slope is a).</param>
        public static void LinearRegression(double[] xVals, double[] yVals,
                                            int inclusiveStart, int exclusiveEnd,
                                            out double rsquared, out double yintercept,
                                            out double slope)
        {
            Console.WriteLine(xVals.Length == yVals.Length);

            double sumOfX = 0;
            double sumOfY = 0;
            double sumOfXSq = 0;
            double sumOfYSq = 0;
            double ssX = 0;
            double ssY = 0;
            double sumCodeviates = 0;
            double sCo = 0;
            double count = exclusiveEnd - inclusiveStart;

            for (int ctr = inclusiveStart; ctr < exclusiveEnd; ctr++)
            {
                double x = xVals[ctr];
                double y = yVals[ctr];
                sumCodeviates += x * y;
                sumOfX += x;
                sumOfY += y;
                sumOfXSq += x * x;
                sumOfYSq += y * y;
            }
            ssX = sumOfXSq - ((sumOfX * sumOfX) / count);
            ssY = sumOfYSq - ((sumOfY * sumOfY) / count);
            double RNumerator = (count * sumCodeviates) - (sumOfX * sumOfY);
            double RDenom = (count * sumOfXSq - (sumOfX * sumOfX))
             * (count * sumOfYSq - (sumOfY * sumOfY));
            sCo = sumCodeviates - ((sumOfX * sumOfY) / count);

            double meanX = sumOfX / count;
            double meanY = sumOfY / count;
            double dblR = RNumerator / Math.Sqrt(RDenom);
            rsquared = dblR * dblR;
            yintercept = meanY - ((sCo / ssX) * meanX);
            slope = sCo / ssX;
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
    }
}
