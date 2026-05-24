using System;

namespace Utils.Extension_Methods
{
    public static class DateTimeExtensions
    {
        public static string ToMinutesAndSeconds(this DateTime dateTime)
        {
            return dateTime.Minute.ToString("00") + ":" + dateTime.Second.ToString("00");
        }
        
        public static string ToMinutesAndSeconds(this TimeSpan dateTime)
        {
            return dateTime.Minutes.ToString("00") + ":" + dateTime.Seconds.ToString("00");
        }

        public static string ToMinutesAndSecondsFromUtcNow(this DateTime dateTime, DateTime utcNow)
        {
            var diff = dateTime > utcNow ? dateTime - utcNow : utcNow - dateTime;
            return Math.Abs(diff.Minutes).ToString("00") + ":" + Math.Abs(diff.Seconds).ToString("00");
        }
    }
}