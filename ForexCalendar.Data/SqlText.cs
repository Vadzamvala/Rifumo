using System;

namespace ForexCalendar.Data
{
    public static class SqlText
    {
        public static string Calendar_Select = "SELECT * FROM CalendarEvent;";
        public static string Calendar_Select_ByID = "SELECT * FROM CalendarEvent WHERE Id = @CalendarEventId;";
        public static string Calendar_Select_ByDate = "SELECT * FROM CalendarEvent WHERE CurrencyPair=@CurrencyPair AND EventDate = @EventDate;";
        public static string Calendar_Select_ByDateAndTime = "SELECT * FROM CalendarEvent WHERE CurrencyPair=@CurrencyPair AND EventDate = @EventDate AND EventTime = @EventTime;";
        public static string Calendar_Select_IsEventTime = "SELECT * FROM CalendarEvent WHERE CurrencyPair=@CurrencyPair AND EventDate = @EventDate";

        public static string Calendar_Insert = @"INSERT CalendarEvent(EventName,EventDate,EventTime,CurrencyPair,Forecast,Previous,Impact) VALUES (@EventName,@EventDate,@EventTime,@CurrencyPair,@ForeCast,@Previous,@Impact);";

        public static string Calendar_Update_EventDateAndTime = "UPDATE CalendarEvent SET EventDate = @EventDate, EventTime = @EventTime WHERE Id = @CalendarEventId";

        public static string Calendar_Delete = "DELETE FROM CalendarEvent WHERE Id = @CalendarEventId";

        //Stored Procs
        public static readonly string Calendar_Search_Sproc= "GetCalendarEventDataForMT4";
    }
}
