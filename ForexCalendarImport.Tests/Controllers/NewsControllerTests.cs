using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ForexCalendar.Api.Controllers;
using ForexCalendar.Domain.Models;

namespace ForexCalendarImport.Controllers.Tests
{
    [TestClass]
    public class CalendarQueryUnitTests
    {
        [TestMethod]
        public async Task CalendarSelectAsync_ShouldReturnWeeklyEvents()
        {
            // Arrange
            var controller = new NewsController();

            //NFP feb
            var numberEvents = 67;
            var eventDate = new DateTime(2018, 02, 02);
            var currencyPair = "USD";

            // Act
            var response = await controller.GetWeeklyNewsAsync(currencyPair, eventDate, ImpactType.High) as OkNegotiatedContentResult<List<ForexCalendarEvent>>;

            // Assert
            Assert.IsTrue(numberEvents == response.Content.Count);
        }

        [TestMethod]
        public async Task CalendarSelectAsync_ShouldReturnNotFound()
        {
            // Arrange
            var controller = new NewsController();
            var eventDate = new DateTime(2018, 02, 03);     //Sat
            var currencyPair = "ZAR";

            // Act
            var response = await controller.GetWeeklyNewsAsync(currencyPair, eventDate, ImpactType.High) as OkNegotiatedContentResult<List<ForexCalendarEvent>>;

            // Assert
            Assert.IsInstanceOfType(response, typeof(NotFoundResult));
        }
    }
}
