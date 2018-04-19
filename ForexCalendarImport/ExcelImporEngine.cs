using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Data;

using Spire.Xls;

using ForexCalendar.Data;
using ForexCalendar.Data.Extensions;

namespace ForexCalendarImport
{
    public class ExcelImporEngine
    {
        #region Fields
        private readonly string _calendarNameRegexprPattern = "^Calendar-d{2}-d{2}-d{4}";
        private readonly string _processedFileFolderName = "processed";
        private readonly string _erroredFileFolderName = "errored";
        #endregion

        #region Properties
        public string ImportFolderPath { get; set; }
        public int NumberFiles { get; set; }
        public int ImportedFiles { get; set; }
        #endregion

        #region Contructor
        public ExcelImporEngine(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                if (!Directory.Exists(Path.Combine(folderPath, _processedFileFolderName)))
                {
                    Directory.CreateDirectory(Path.Combine(folderPath, _processedFileFolderName));
                }

                if (!Directory.Exists(Path.Combine(folderPath, _erroredFileFolderName)))
                {
                    Directory.CreateDirectory(Path.Combine(folderPath, _erroredFileFolderName));
                }
                this.ImportFolderPath = folderPath;
            }
            else
            {
                throw new Exception(string.Format("Invalid folder: {0}. Please specify a valid file", folderPath));
            }
        }

        private string GetFilename(string filepath)
        {
            if (File.Exists(filepath))
            {
                return Path.GetFileName(filepath);
            }
            return string.Empty;
        }
        #endregion

        #region public methods
        public bool ImportFiles()
        {
            var importFiles = Directory.GetFiles(this.ImportFolderPath, "*.xls");
            this.NumberFiles = importFiles.Length;

            var successfullyImported = 0;
            foreach (var file in importFiles)
            {
                var destFile = Path.Combine(this.ImportFolderPath, _processedFileFolderName, Path.GetFileName(file));
                if (ImportFile(file))
                {
                    successfullyImported++;
                    File.Move(file, destFile);
                }
                else
                {
                    System.Diagnostics.Debugger.Break();

                    destFile = Path.Combine(this.ImportFolderPath, _erroredFileFolderName, Path.GetFileName(file));
                    File.Move(file, destFile);
                }
            }
            this.ImportedFiles = successfullyImported;

            return successfullyImported == this.NumberFiles;
        }
        #endregion

        #region Helpers
        private bool ImportFile(string filePath)
        {
            try
            {
                var importWorkBook = new Workbook();
                importWorkBook.LoadFromFile(filePath);

                var importedWorkSheet = importWorkBook.Worksheets[0];
                var importDataTable = importedWorkSheet.ExportDataTable();
                var dataTableName = importDataTable.TableName;
                var extractedDate = "";
                if (Regex.IsMatch(dataTableName, _calendarNameRegexprPattern))
                {
                    extractedDate = dataTableName.Replace("Calendar-", "");
                }

                var extractedYear = int.Parse(dataTableName.Substring(dataTableName.LastIndexOf("-") + 1, 4));

                var rowsAffected = 0;
                var rowCounter = 0;
                var lastParsedDate = "";
                foreach (DataRow row in importDataTable.Rows)
                {
                    rowCounter++;
                    if (rowCounter < 12)
                    {
                        continue;
                    }

                    if (!IsValidCalendarEvent(row))
                    {
                        if (!string.IsNullOrEmpty(row[0].ToString()))
                        {
                            lastParsedDate = DateTimeFromString(row[0].ToString(), extractedYear);
                        }
                        continue;
                    }

                    if (row.ItemArray.Length == 7)
                    {
                        var nextEvent = CalendarEventFromDataRow(row, lastParsedDate, ref extractedYear);
                        if (!string.IsNullOrEmpty(row[0].ToString()))
                        {
                            lastParsedDate = nextEvent.EventDate.ToString("yyyy-MM-dd hh:mm:ss");
                        }

                        if (nextEvent.Insert() <= 0)
                        {
                            System.Diagnostics.Debugger.Break();
                        }
                        rowsAffected++;
                    }
                }
                //return (importDataTable.Rows.Count - rowsAffected) > 12;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


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

        private CalendarEvent CalendarEventFromDataRow(DataRow row, string eventDate, ref int eventYear)
        {
            try
            {
                EventYearOverride(eventDate, row[0].ToString(), ref eventYear);

                var parsedEvent = new CalendarEvent();
                parsedEvent.EventName = row[2].ToString();
                parsedEvent.CurrencyPair = row[1].ToString();
                parsedEvent.EventDate = ToDate(string.IsNullOrEmpty(row[0].ToString()) ? eventDate : row[0].ToString(), eventYear);
                parsedEvent.EventTime = ToTime(parsedEvent.EventDate, row[3].ToString());
                parsedEvent.Forecast = row[5].ToString();
                parsedEvent.Previous = row[6].ToString();

                return parsedEvent;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void EventYearOverride(string lastParsedDate, string inputDate, ref int eventYear)
        {
            if (string.IsNullOrEmpty(lastParsedDate)) return;
            if (string.IsNullOrEmpty(inputDate)) return;

            DateTime lastParsedDt;
            if (DateTime.TryParse(lastParsedDate, out lastParsedDt))
            {
                if (lastParsedDt.Month == 12 && lastParsedDt.Day == 31)
                {
                    eventYear++;
                }
            }
        }

        private DateTime ToTime(DateTime datePortionOnly, string inputTime)
        {
            var hh = int.Parse(inputTime.Substring(0, 2));
            var mm = int.Parse(inputTime.Substring(3, 2));
            var ss = 00;

            var returnDate = new DateTime(datePortionOnly.Year, datePortionOnly.Month, datePortionOnly.Day, hh, mm, ss);
            return returnDate;
        }

        private DateTime ToDate(string inputDate, int year)
        {
            inputDate = DateTimeFromString(inputDate, year);

            DateTime parsed;
            if (DateTime.TryParse(inputDate, out parsed))
            {
                return parsed;
            }
            System.Diagnostics.Debugger.Break();

            return DateTime.MinValue;
        }

        private string DateTimeFromString(string inputDate, int year)
        {
            var datePattern = @"^(?:\d{1}|\d{2})\-([aA-zZ][aA-zZ][aA-zZ])\s([aA-zZ][aA-zZ][aA-zZ])";
            if (Regex.IsMatch(inputDate, datePattern))
            {
                inputDate = ParseDate(inputDate, year);
            }
            return inputDate;
        }

        private string ParseDate(string inputDate, int year)
        {
            switch (inputDate.Length)
            {
                case 10:
                    inputDate = String.Concat(FromMonthName(inputDate.Substring(3, 3)), "-", inputDate.Substring(0, 2), "-", year.ToString());
                    break;
                case 9:
                    inputDate = String.Concat(FromMonthName(inputDate.Substring(2, 3)), "-", inputDate.Substring(0, 1), "-", year.ToString());
                    break;
                default:
                    System.Diagnostics.Debugger.Break();
                    break;
            }

            return inputDate;
        }

        private double ToDouble(string inputString)
        {
            double parsed;
            if (Double.TryParse(inputString, out parsed))
            {
                return parsed;
            }
            return 0.00;
        }

        private bool IsValidCalendarEvent(DataRow row)
        {
            return !string.IsNullOrEmpty(row[1].ToString()) && !string.IsNullOrEmpty(row[2].ToString()) && !string.IsNullOrEmpty(row[3].ToString());
        }

        #endregion
    }
}
