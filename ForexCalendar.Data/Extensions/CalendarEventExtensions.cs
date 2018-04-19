using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using AutoMapper;
using Dapper;
using ForexCalendar.Domain.Models;

namespace ForexCalendar.Data.Extensions
{
    public static class CalendarEventExtensions
    {
        static CalendarEventExtensions()
        {
            Mapper.Initialize(x =>
            {
                x.CreateMap<CalendarEvent, Event>()
                    .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.EventName))
                    .ForMember(dest => dest.Date, opt => opt.MapFrom(src => DecorateWithCData(ToForexFactoryDate(src))))
                    .ForMember(dest => dest.Time, opt => opt.MapFrom(src => DecorateWithCData(GetEventTime(src))))
                    .ForMember(dest => dest.Impact, opt => opt.MapFrom(src => DecorateWithCData(src.Impact)))
                    .ForMember(dest => dest.Forecast, opt => opt.MapFrom(src => DecorateWithCData(src.Forecast)))
                    .ForMember(dest => dest.Previous, opt => opt.MapFrom(src => DecorateWithCData(src.Previous)))
                    .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.CurrencyPair));
            });
        }

        public static Event ToExportEventModel(this CalendarEvent model)
        {
            var exportEvent = Mapper.Map<Event>(model);
            return exportEvent;
        }

        public static List<Event> ToExportEventModelList(this CalendarEvent model, List<CalendarEvent> sourceItems)
        {
            return Mapper.Map<List<CalendarEvent>, List<Event>>(sourceItems);
        }

        public static List<CalendarEvent> ReadAll(this CalendarEvent calendarEvent)
        {
            using (var db = GetConnection())
            {
                var result = db.Query<CalendarEvent>(SqlText.Calendar_Select);
                return result.Any() ? result.ToList() : EmptyCalendarEvents();
            }
        }

        public static IEnumerable<CalendarEvent> FindWeeklyEvents(this CalendarEvent calendarEvent, ImpactType impact = ImpactType.Unspecified)
        {
            using (var db = GetConnection())
            {
                var result = db.Query<CalendarEvent>(SqlText.Calendar_Search_Sproc,
                        new
                        {
                            EventDate = calendarEvent.EventDate,
                            ImpactType = (int)impact,
                            CurrencyPair = DefaultOrEmptyCurrencyPair(calendarEvent)
                        },
                    commandType: System.Data.CommandType.StoredProcedure);

                return result.Any() ? result.ToList() : EmptyCalendarEvents();
            }


        }

        public static CalendarEvent FindEventById(this CalendarEvent calendarEvent, int calendarID)
        {
            using (var db = GetConnection())
            {
                var result = db.Query<CalendarEvent>(SqlText.Calendar_Select_ByID);
                return result.FirstOrDefault();
            }
        }

        public static CalendarEvent FindEventByDate(this CalendarEvent calendarEvent, string currencyPair, DateTime eventDate, ImpactType impact, DateTime? inputTime)
        {
            using (var db = GetConnection())
            {
                var result = db.Query<CalendarEvent>(SqlText.Calendar_Search_Sproc,
                    new
                    {
                        EventDate = eventDate,
                        ImpactType = (int)impact,
                        CurrencyPair = DefaultOrEmptyCurrencyPair(currencyPair)
                    },
                    commandType: System.Data.CommandType.StoredProcedure);
                return result.FirstOrDefault();
            }
        }

        public static bool IsEventTime(this CalendarEvent calendarEvent, string currencyPair, DateTime eventDate, DateTime eventTime, ImpactType impact, int? secondsFromNewsRelease)
        {
            using (var db = GetConnection())
            {
                var result = db.Query<CalendarEvent>(SqlText.Calendar_Select_IsEventTime,
                    new
                    {
                        EventDate = eventDate,
                        ImpactType = (int)impact,
                        CurrencyPair = DefaultOrEmptyCurrencyPair(currencyPair)
                    });
                bool hasUpcomingRelease = false;
                if (secondsFromNewsRelease.HasValue && result.Any())
                {
                    hasUpcomingRelease = result.Any(e => e.EventTime >= DateTime.Now && (e.EventTime - DateTime.Now).Seconds <= secondsFromNewsRelease);
                }
                return hasUpcomingRelease;
            }
        }

        public static long Insert(this CalendarEvent calendarEventModel)
        {
            using (var db = GetConnection())
            {
                var affectedRows = db.Execute(SqlText.Calendar_Insert,
                        new
                        {
                            EventName = calendarEventModel.EventName,
                            EventDate = calendarEventModel.EventDate,
                            EventTime = calendarEventModel.EventTime,
                            CurrencyPair = calendarEventModel.CurrencyPair,
                            ForeCast = calendarEventModel.Forecast,
                            Previous = calendarEventModel.Previous,
                            Impact = calendarEventModel.Impact
                        });
                return affectedRows;
            }
        }

        #region Helpers
        private static string DefaultOrEmptyCurrencyPair(CalendarEvent calendarEvent)
        {
            return DefaultOrEmptyPair(calendarEvent.CurrencyPair);
        }

        private static string DefaultOrEmptyCurrencyPair(string calendarEvent)
        {
            return DefaultOrEmptyPair(calendarEvent);
        }

        private static string DefaultOrEmptyPair(string currencyPair)
        {
            return string.IsNullOrEmpty(currencyPair) ? "" : currencyPair;
        }
        private static string GetEventTime(CalendarEvent src)
        {
            return src.EventTime.ToString("hh:mmtt");
        }

        private static string ToForexFactoryDate(CalendarEvent src)
        {
            return src.EventDate.ToString("MM-dd-yyyy");
        }

        private static string DecorateWithCData(string inputValue)
        {
            if (string.IsNullOrEmpty(inputValue))
            {
                return inputValue;
            }

            var returnValue = $"<![CDATA[{inputValue}]]>";
            return returnValue;
        }

        private static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionFromDbConfig());
        }

        private static List<CalendarEvent> EmptyCalendarEvents()
        {
            return new List<CalendarEvent>();
        }

        private static string ConnectionFromDbConfig()
        {
            //return ConfigurationManager.ConnectionStrings[0].ConnectionString;
            return @"Integrated Security=SSPI;Persist Security Info=False;database=FFCalendar;Data Source=NB-26506\SQLEXPRESS;Connect Timeout=0;Pooling=true";
        }

        private static string ProviderNameFromDbConfig()
        {
            //return ConfigurationManager.ConnectionStrings[0].ProviderName;
            return "System.Data.SqlClient";
        }
        #endregion
    }
}