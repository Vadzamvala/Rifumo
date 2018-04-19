using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForexCalendar.Domain.Models
{
    public class Event
    {
        public string Title;       //<title>Building Consents m/m</title>
        public string Country;     //<country>NZD</country>
        public string Date;        //<![CDATA[07-30-2017]]>
        public string Time;        //<![CDATA[10:45pm]]>
        public string Impact;      //<![CDATA[Low]]>
        public string Forecast;    //<forecast/> or <forecast><![CDATA[1.6%]]></forecast>
        public string Previous;    //<![CDATA[7.0%]]>
    }
}