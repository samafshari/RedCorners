using RedCorners.Models;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace RedCorners
{
    public static class DateExtensions
    {
        public static DateTimeOffset GetDayStartUniversalTime(TimeZoneInfo tz, DateTimeOffset? reference = null)
        {
            var utc = reference ?? DateTimeOffset.Now;
            return GetDayStartUniversalTime(tz, GetLocalTime(tz, utc));
        }

        public static DateTime GetLocalTime(TimeZoneInfo tz, DateTimeOffset utc)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utc.UtcDateTime, tz);
        }

        public static DateTimeOffset GetUniversalTime(TimeZoneInfo tz, DateTime local)
        {
            return TimeZoneInfo.ConvertTimeToUtc(new DateTime(
                year: local.Year, month: local.Month, day: local.Day,
                hour: local.Hour, minute: local.Minute, second: local.Second), tz);
        }

        public static DateTimeOffset GetDayStartUniversalTime(TimeZoneInfo tz, DateTime dt)
        {
            var offset = tz.GetUtcOffset(dt);
            return new DateTimeOffset(year: dt.Year, month: dt.Month, day: dt.Day, hour: 0, minute: 0, second: 0, millisecond: 0, offset);
        }

        public static DateTime GetDateInTimeZone(TimeZoneInfo tz, DateTime dt)
        {
            var dto = GetDayStartUniversalTime(tz, dt);
            return dto.Date;
        }

        public static TimeSpan SnapToNearest5Minute(this TimeSpan value)
        {
            return SnapToNearestNMinute(value, 5);
        }

        public static TimeSpan SnapToNearestNMinute(this TimeSpan value, int step)
        {
            value = value.WithoutSeconds();
            var minute = step * (int)(0.5 + value.Minutes / (double)step);
            return minute > 60
                ? value.Add(TimeSpan.FromMinutes(-value.Minutes)).Add(TimeSpan.FromHours(1))
                : value.Add(TimeSpan.FromMinutes(-value.Minutes)).Add(TimeSpan.FromMinutes(minute));
        }

        public static TimeSpan SnapToNearestNMinuteCeil(this TimeSpan value, int step)
        {
            value = value.WithoutSeconds();
            var minute = (int)step * (int)Math.Ceiling((double)value.Minutes / (double)step);
            return minute > 60
                ? value.Add(TimeSpan.FromMinutes(-value.Minutes)).Add(TimeSpan.FromHours(1))
                : value.Add(TimeSpan.FromMinutes(-value.Minutes)).Add(TimeSpan.FromMinutes(minute));
        }

        public static TimeSpan SnapToNearestNMinuteFloor(this TimeSpan value, int step)
        {
            value = value.WithoutSeconds();
            var minute = (int)step * (int)((double)value.Minutes / (double)step);
            return minute > 60
                ? value.Add(TimeSpan.FromMinutes(-value.Minutes)).Add(TimeSpan.FromHours(1))
                : value.Add(TimeSpan.FromMinutes(-value.Minutes)).Add(TimeSpan.FromMinutes(minute));
        }

        public static DateTimeOffset WithoutSeconds(this DateTimeOffset dt)
        {
            return dt - TimeSpan.FromSeconds(dt.TimeOfDay.Seconds) - TimeSpan.FromMilliseconds(dt.TimeOfDay.Milliseconds);
        }

        public static DateTime WithoutSeconds(this DateTime dt)
        {
            return dt - TimeSpan.FromSeconds(dt.TimeOfDay.Seconds) - TimeSpan.FromMilliseconds(dt.TimeOfDay.Milliseconds);
        }

        public static TimeSpan WithoutSeconds(this TimeSpan ts)
        {
            return new TimeSpan((int)ts.TotalHours, ts.Minutes, 0);
        }

        public static string ToHourMinuteString(this TimeSpan ts)
        {
            var totalHours = (int)ts.TotalHours;
            if (ts < TimeSpan.Zero)
            {
                ts = ts.Duration();
            }
            return totalHours + ts.ToString("'h'mm");
        }

        public static DateTimeOffset GetPreviousDay(TimeZoneInfo tz, DateTimeOffset dt)
        {
            var local = GetLocalTime(tz, dt);
            var yesterday = local.AddDays(-1);
            var utc = GetUniversalTime(tz, yesterday);
            return utc;
        }

        public static DateTimeOffset GetNextDay(TimeZoneInfo tz, DateTimeOffset dt)
        {
            var local = GetLocalTime(tz, dt);
            var yesterday = local.AddDays(1);
            var utc = GetUniversalTime(tz, yesterday);
            return utc;
        }

        public static DateTimeOffset GetBeginningOfMonth(TimeZoneInfo tz, DateTimeOffset dt)
        {
            var nowInFirstDay = dt.AddDays(1 - dt.Day);
            return GetDayStartUniversalTime(tz, nowInFirstDay);
        }

        public static DateTimeOffset GetPreviousDayOfWeek(DateTimeOffset dt, DayOfWeek targetDayOfWeek, bool includeToday = true)
        {
            if (dt.DayOfWeek == targetDayOfWeek)
                return includeToday ? dt : dt.AddDays(-7);
            var dow = dt.DayOfWeek;
            if (dow < targetDayOfWeek) dow += 7;
            var delta = dow - targetDayOfWeek;
            return dt.AddDays(-delta);
        }

        public static DateTimeOffset GetNextDayOfWeek(DateTimeOffset dt, DayOfWeek targetDayOfWeek, bool includeToday = true)
        {
            if (dt.DayOfWeek == targetDayOfWeek)
                return includeToday ? dt : dt.AddDays(7);
            return GetPreviousDayOfWeek(dt.AddDays(7), targetDayOfWeek);
        }

        public static Interval<DateTimeOffset> GetMonthRange(this TimeZoneInfo tz, bool endIsRef, DateTimeOffset? reference = null, bool inclusiveEnd = false)
        {
            var utc = reference ?? DateTimeOffset.Now;
            var utcEnd = TimeZoneInfo.ConvertTime(utc, tz);
            var utcStart = GetBeginningOfMonth(tz, utcEnd);
            if (!endIsRef) utcEnd = utcStart.AddMonths(1);
            if (!inclusiveEnd) utcEnd = utcEnd.AddTicks(-1);
            return new Interval<DateTimeOffset>(utcStart, utcEnd);
        }

        public static Interval<DateTimeOffset> GetPreviousMonthRange(this TimeZoneInfo tz, DateTimeOffset? reference = null, bool inclusiveEnd = false)
        {
            var utc = reference ?? DateTimeOffset.Now;
            var utcEnd = TimeZoneInfo.ConvertTime(utc, tz);
            utcEnd = GetBeginningOfMonth(tz, utcEnd).AddTicks(-1);
            var utcStart = GetBeginningOfMonth(tz, utcEnd);
            if (inclusiveEnd) utcEnd = utcEnd.AddTicks(1);
            var range = new Interval<DateTimeOffset>(utcStart, utcEnd);
            return range;
        }

        public static Interval<DateTimeOffset> GetWeekRange(this TimeZoneInfo tz, DayOfWeek firstDayOfWeek, bool endIsRef, DateTimeOffset? reference = null, bool inclusiveEnd = false)
        {
            var utc = reference ?? DateTimeOffset.Now;
            var utcEnd = TimeZoneInfo.ConvertTime(utc, tz);
            var utcStart = GetPreviousDayOfWeek(utcEnd, firstDayOfWeek);
            if (!endIsRef) utcEnd = utcStart.AddDays(7);
            if (!inclusiveEnd) utcEnd = utcEnd.AddTicks(-1);
            var range = new Interval<DateTimeOffset>(utcStart, utcEnd);
            return range;
        }

        public static Interval<DateTimeOffset> GetCalendarWeeksViewRange(this TimeZoneInfo tz, DayOfWeek firstDayOfWeek, int weeksSkip, int weeksTake, DateTimeOffset? reference = null)
        {
            var utc = reference ?? DateTimeOffset.Now;
            var weekStart = GetPreviousDayOfWeek(utc, firstDayOfWeek);
            if (weeksSkip != 0)
                weekStart = weekStart.AddDays(7 * weeksSkip);
            var end = weekStart.AddDays(7 * weeksTake).AddTicks(-1);
            return new Interval<DateTimeOffset>(weekStart, end);
        }

        public static string GetDateString(DateTime dt, bool forceDropYear = false)
        {
            var culture = CultureInfo.CurrentCulture.DateTimeFormat;
            var format = culture.ShortDatePattern
                .Replace("y", "")
                .Replace("Y", "");

            var dateString = dt.ToString(format);
            if (dt.Year != DateTime.Now.Year && !forceDropYear) return dateString;
            dateString = String.Join(culture.DateSeparator, dateString
                .Split(new[] { culture.DateSeparator }, StringSplitOptions.RemoveEmptyEntries));
            return dateString;
        }
    }
}
