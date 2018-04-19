using System;

namespace ForexCalendar.Data
{
    public class CalendarEvent
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime EventTime { get; set; }
        public string CurrencyPair { get; set; }
        public string Forecast { get; set; }
        public string Previous { get; set; }
        public string Impact { get; set; }
        public int WeeksDifference { get; set; }
    }
}
