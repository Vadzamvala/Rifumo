using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ForexCalendar.Data;
using ForexCalendar.Data.Extensions;
using ForexCalendar.Domain.Models;

namespace ForexCalendarImport.Tests
{
    [TestClass]
    public class ImportCalendarTests
    {
        [TestMethod]
        public void Invalid_ImportFileFile_Should_Throw_Exception()
        {
            //arrange
            //act
            //assert
        }

        [TestMethod]
        public void FromMonthName_ShouldReturn_Success()
        {
            string[] monthNames = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

            string monthName = "Jan";

            var matchedAtPosition = monthNames.Select((Value, Index) => new { Value, Index })
                        .Where(pair => monthNames.Any(target => pair.Value.Contains(monthName)))
                        .Select(pair => pair.Index)
                        .FirstOrDefault();

            Assert.IsTrue(matchedAtPosition == 1);
        }

        [TestMethod]
        public void ImportFiles_ShouldReturnSuccess()
        {
            //arrange
            var importFolderPath = @"C:\Personal\Forex\FFCalendar Downloads";
            var importUtility = new ExcelImporEngine(importFolderPath);
            //act
            var importResult = importUtility.ImportFiles();
            //assert
            Assert.IsTrue(importResult == true);
        }

        [TestMethod]
        public void GetWeeklyCalendar_ShouldReturnEvents()
        {
            //arrange

            //High impact NFP
            var numberEvents = 16;
            var impact = ImpactType.High;
            var calendarEvent = new CalendarEvent();
            calendarEvent.CurrencyPair = "USD";
            calendarEvent.EventDate = new DateTime(2018, 02, 02);

            //act
            var weeklyEvents = calendarEvent.FindWeeklyEvents(impact);

            //assert
            Assert.IsTrue(weeklyEvents != null && numberEvents.Equals(weeklyEvents.Count()));

        }

        #region Helpers
        private string FromMonthName(string inputMonth)
        {
            /*string[] monthNames = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

            if (inputMonth.Length > 3)
            {
                inputMonth = inputMonth.Substring(0, 3);
            }

            var matchedAtPosition = monthNames.Select((Value, Index) => new { Value, Index })
                        .Where(pair => monthNames.Any(target => pair.Value.Contains(inputMonth)))
                        .Select(pair => pair.Index)
                        .FirstOrDefault(); */

            var matchedAtPosition = 0;
            switch (inputMonth)
            {
                case "Jan":
                    matchedAtPosition = 1;
                    break;
                case "Feb":
                    matchedAtPosition = 2;
                    break;
                case "Mar":
                    matchedAtPosition = 3;
                    break;
                case "Apr":
                    matchedAtPosition = 4;
                    break;
                case "May":
                    matchedAtPosition = 5;
                    break;
                case "Jun":
                    matchedAtPosition = 6;
                    break;
                case "Jul":
                    matchedAtPosition = 7;
                    break;
                case "Aug":
                    matchedAtPosition = 8;
                    break;
                case "Sep":
                    matchedAtPosition = 9;
                    break;
                case "Oct":
                    matchedAtPosition = 10;
                    break;
                case "Nov":
                    matchedAtPosition = 11;
                    break;
                case "Dec":
                    matchedAtPosition = 12;
                    break;
                default:
                    break;
            }

            string monthNo = (matchedAtPosition < 10 ? string.Concat("0", matchedAtPosition.ToString()) : matchedAtPosition.ToString());
            return monthNo;
        }
        #endregion
    }
}
