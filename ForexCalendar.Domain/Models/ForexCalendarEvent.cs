using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForexCalendar.Domain.Models
{
    public class ForexCalendarEvent
    {
        DateTime EventDate;
        DateTime EventTime;
        CurrencyTypeEnum Currency;
        double Forecast;
        double Previous;
    }
}
