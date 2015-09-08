using System;

namespace Composite.News
{
    static class DateTimeUtils
    {
        public static bool IsValidDate(int year, int month, int day)
        {
            if (year < DateTime.MinValue.Year || year > DateTime.MaxValue.Year)
                return false;

            if (month < 1 || month > 12)
                return false;

            return day > 0 && day <= DateTime.DaysInMonth(year, month);
        }
    }
}
