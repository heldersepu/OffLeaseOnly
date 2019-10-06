using System;

namespace OffLeaseOnly
{
    public static class DateTimeExtension
    {
        public static double Diff(this DateTime? value)
        {
            if (value == null) return 0;
            return Math.Round((DateTime.Now - (DateTime)value).TotalMilliseconds);
        }
    }
}