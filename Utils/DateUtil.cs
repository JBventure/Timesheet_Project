using System.Globalization;
using Timesheet_Project.ViewModel;
using Timesheet_Project.Utils; 

namespace Timesheet_Project.Utils
{
    public static class DateUtil
    {
        public static List<TimesheetDays> indexMonth(int year, int month)
        {
            var date = new DateTime(year, month, 1);
            int first_week = System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                Convert.ToDateTime(date), System.Globalization.CalendarWeekRule.FirstFullWeek, System.Globalization.DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);//date.DayOfYear / 7;//CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday);
            DateTime start_date = FirstDateOfWeekISO8601(date.Year, first_week);


            int last_week = System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                new DateTime(year, month, DateTime.DaysInMonth(date.Year, date.Month)), System.Globalization.CalendarWeekRule.FirstFullWeek, System.Globalization.DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);//CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(new DateTime(year, month, DateTime.DaysInMonth(date.Year,date.Month)), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday);

            if (first_week > last_week)
            {
                start_date = date;
            }

            DateTime end_date = FirstDateOfWeekISO8601(new DateTime(year, month, DateTime.DaysInMonth(date.Year, date.Month)).Year, last_week).AddDays(6);

            List<TimesheetDays> days = new List<TimesheetDays>();

            for (var day = start_date; day <= end_date; day = day.AddDays(1))
            {
                TimesheetDays timeday = new TimesheetDays();
                timeday.DayOfMonth = day.Day;
                timeday.DayOfWeek = day.DayOfWeek.ToString();
                timeday.dayOfWeekIndex = (int)day.DayOfWeek;
                timeday.Date = day;
                timeday.WeekOfMonth = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(day, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday);
                days.Add(timeday);
            }
            return days;
        }

        public static DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Sunday - jan1.DayOfWeek;

            DateTime firstSunday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstSunday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday);

            var weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }
            var result = firstSunday.AddDays(weekNum * 7);
            return result;
        }

        public static DateTime FirstDateOfWeekISO8601Monday(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Sunday - jan1.DayOfWeek;

            DateTime firstSunday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstSunday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }
            var result = firstSunday.AddDays(weekNum * 7);
            return result;
        }

        public static int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }

    static class DateTimeExtensions
    {
        static GregorianCalendar _gc = new GregorianCalendar();
        public static int GetWeekOfMonth(this DateTime time)
        {
            DateTime first = new DateTime(time.Year, time.Month, 1);
            return time.GetWeekOfYear() - first.GetWeekOfYear();
        }

        static int GetWeekOfYear(this DateTime time)
        {
            return _gc.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }
    }
}
