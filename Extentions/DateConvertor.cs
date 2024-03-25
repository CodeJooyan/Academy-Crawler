using System.Globalization;

namespace CrawlerService.Extentions
{
    public static class DateConvertor
    {
        public static DateTime? ToShamsiByString(this string? date)
        {
            if (date is null) return null;

            var dateArray = date.Split("/");
            int year = int.Parse(dateArray[0]);
            int month = int.Parse(dateArray[1]);
            int day = int.Parse(dateArray[2]);

            return new DateTime(year, month, day);
        }

        public static string? GetPersianDayOfWeek(this DateTime? date)
        {
            if (date is null) return null;

            int year = date.Value.Year;
            int month = date.Value.Month;
            int day = date.Value.Day;

            PersianCalendar persianCalendar = new PersianCalendar();

            DateTime persianDate = new DateTime(year, month, day, persianCalendar);

            string dayOfWeek = persianDate.DayOfWeek.ToString().ToPersianDayOfWeek();

            return dayOfWeek;
        }

        public static string ToPersianDayOfWeek(this string dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case "Saturday":
                    return "شنبه";
                case "Sunday":
                    return "یکشنبه";
                case "Monday":
                    return "دوشنبه";
                case "Tuesday":
                    return "سه شنبه";
                case "Wednesday":
                    return "چهارشنبه";
                case "Thursday":
                    return "پنجشنبه";
                case "Friday":
                    return "جمعه";
            }
            return "بدون تاریخ";
        }
    }
}