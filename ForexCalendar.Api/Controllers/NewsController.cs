using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using System.Net;
using System.Net.Http;

using System.Threading.Tasks;
using System.Web.Http;
using System.Runtime.Caching;

using ForexCalendar.Data;
using ForexCalendar.Data.Extensions;
using ForexCalendar.Domain.Models;

namespace ForexCalendar.Api.Controllers
{
    [RoutePrefix("api/news")]
    public class NewsController : ApiController
    {
        // GET api/news
        [AllowAnonymous]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        #region FF Factory Xml
        // GET api/news/AsForexFactoryXml/2018-02-02/0
        [AllowAnonymous, Route("AsForexFactoryXml/{eventDate}/{impact}")]
        public IHttpActionResult GetWeeklyNewsAsyncAsXml(DateTime eventDate, ImpactType impact = ImpactType.Unspecified)
        {
            var calendarEvent = CalendarEventFromParams(eventDate);
            var result = FetchCalendarEventAsResponseMessage(impact, calendarEvent);
            return HandleProcessResult(result);
        }

        // GET api/news/AsForexFactoryXml/USD/2018-02-02/3
        [AllowAnonymous, Route("AsForexFactoryXml/{currencyPair}/{eventDate}/{impact}")]
        public IHttpActionResult GetWeeklyNewsAsyncAsXml(string currencyPair, DateTime eventDate, ImpactType impact = ImpactType.Unspecified)
        {
            var calendarEvent = CalendarEventFromParams(currencyPair, eventDate);
            var result = FetchCalendarEventAsResponseMessage(impact, calendarEvent);
            return HandleProcessResult(result);
        }
        #endregion

        // GET api/news/EURUSD/2018-02-02/3
        [AllowAnonymous, Route("{currencyPair}/{eventDate}/{impact}")]
        public async Task<IHttpActionResult> GetWeeklyNewsAsync(string currencyPair, DateTime eventDate, ImpactType impact)
        {
            return await Task.FromResult(GetWeeklyNews(currencyPair, eventDate, impact));
        }

        // GET api/news/EURUSD/2018-02-02/3/13:30
        [AllowAnonymous, Route("{currencyPair}/{eventDate}/{impact}/{eventTime}")]
        public IHttpActionResult GetIsNewsTime(string currencyPair, string eventDate, ImpactType impact, string eventTime)
        {
            if (!IsValidCurrencyPair(currencyPair))
            {
                return BadRequest($"'{currencyPair}'. is not a valid CurrencyPair!!!!");
            }

            var calendarEvent = new CalendarEvent();
            calendarEvent.CurrencyPair = currencyPair;
            calendarEvent.EventDate.FromStringDate(eventDate);
            calendarEvent.EventTime.FromStringTime(eventTime);

            var newsTime = false;

            return Ok(newsTime);
        }

        // POST api/news
        public IHttpActionResult Post([FromBody]string value)
        {
            return Ok("Updated successfully");
        }

        // PUT api/news/5
        public IHttpActionResult Put(int id, [FromBody]string value)
        {
            return Ok("Updated");
        }

        // DELETE api/news/5
        public IHttpActionResult Delete(int id)
        {
            return Ok("Purged");
        }

        #region helpers
        private HttpResponseMessage FetchCalendarEventAsResponseMessage(ImpactType impact, CalendarEvent calendarEvent)
        {
            var weeklyEvents = FetchCalendarEvents(impact, calendarEvent);
            var exportEventsList = calendarEvent.ToExportEventModelList(weeklyEvents.ToList());
            var xmlFormatter = Configuration.Formatters.XmlFormatter;

            var result = Request.CreateResponse(HttpStatusCode.OK, exportEventsList, xmlFormatter);
            result.Headers.Add("Accept", "application/xml");
            result.Headers.Add("Access-Control-Allow-Origin", "*");
            return result;
        }

        private bool IsValidCurrencyPair(string currencyPair)
        {
            return currencyPair.Length == 3;
        }

        private IHttpActionResult GetWeeklyNews(string currencyPair, DateTime eventDate, ImpactType impact)
        {
            if (!IsValidCurrencyPair(currencyPair))
            {
                return BadRequest("{currencyPair}. is not a valid CurrencyPair!!!!");
            }

            var calendarEvent = CalendarEventFromParams(currencyPair, eventDate);

            try
            {
                var weeklyEvents = FetchCalendarEvents(impact, calendarEvent);
                if (weeklyEvents.Any())
                {
                    return Ok(weeklyEvents);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private CalendarEvent CalendarEventFromParams(string currencyPair, DateTime eventDate)
        {
            var calendarEvent = new CalendarEvent();
            calendarEvent.CurrencyPair = currencyPair;
            calendarEvent.EventDate = eventDate;
            return calendarEvent;
        }

        private CalendarEvent CalendarEventFromParams(DateTime eventDate)
        {
            var calendarEvent = new CalendarEvent();
            calendarEvent.EventDate = eventDate;
            return calendarEvent;
        }

        private IEnumerable<CalendarEvent> FetchCalendarEvents(ImpactType impact, CalendarEvent calendarEvent)
        {
            var cacheItemKey = $"{ConfigurationManager.AppSettings["CachedCalendarEventsKeyName"]}_{calendarEvent.EventDate.ToShortDateString()}_{impact}";
            var cacheExpiryInMinutes = Convert.ToInt16(ConfigurationManager.AppSettings["CacheExpirationInMinutes"]);
            var cachedEvents = MemoryCache.Default;
            var weeklyEventsData = cachedEvents.Get(cacheItemKey) as IEnumerable<CalendarEvent>;
            if (weeklyEventsData != null)
            {
                return weeklyEventsData;
            }

            var weeklyEventsDataFromDb = calendarEvent.FindWeeklyEvents(impact);
            var cachePolicy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(cacheExpiryInMinutes) };
            cachedEvents.Add(cacheItemKey, weeklyEventsDataFromDb, cachePolicy);

            return weeklyEventsDataFromDb;
        }

        private IHttpActionResult HandleProcessResult(HttpResponseMessage result)
        {
            if (result.IsSuccessStatusCode)
            {
                return ResponseMessage(result);
            }
            else
            {
                return BadRequest( string.Concat(result.StatusCode, "=>",result.RequestMessage.Content.ReadAsStringAsync()) );
            }
        }
        #endregion
    }
}