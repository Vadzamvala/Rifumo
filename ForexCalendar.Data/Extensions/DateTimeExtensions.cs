using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForexCalendar.Data.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime FromStringDate(this DateTime dateTime, string eventDate)
        {
            DateTime parsed;
            if (DateTime.TryParse(eventDate, out parsed))
            {
                return parsed;
            }
            return DateTime.MinValue;
        }

        public static DateTime FromStringTime(this DateTime dateTime, string eventTime)
        {
            var eventHour = int.Parse(eventTime.Substring(0, 2));
            var eventMinute = int.Parse(eventTime.Substring(3, 2));
            try
            {
                return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, eventHour, eventMinute, 0);
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }
    }
}
