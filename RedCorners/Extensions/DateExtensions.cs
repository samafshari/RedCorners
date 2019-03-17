using System;
using System.Collections.Generic;
using System.Text;

namespace RedCorners
{
    public static class DateExtensions
    {
        static readonly DateTime BaseDate = new DateTime(year: 2019, month: 3, day: 11);

        public static int ToWeekNumber(this DateTime date)
        {
            return (int)((date - BaseDate).TotalDays / 7);
        }

        public static DateTime GetFirstDayOfWeek(this int weeknumber)
        {
            var days = weeknumber * 7;
            return BaseDate.AddDays(days);
        }

        public static DateTime GetLastDayOfWeek(this int weeknumber)
        {
            return (weeknumber + 1).GetFirstDayOfWeek();
        }
    }
}
