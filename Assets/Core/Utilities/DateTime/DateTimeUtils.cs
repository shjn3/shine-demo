using System;
namespace Shine.Utils
{
    public class DateTimeUtils
    {
        public static bool IsToday(DateTime dataTime)
        {
            var now = DateTime.Now;
            return now.Year == dataTime.Year && now.Month == dataTime.Month && now.Day == dataTime.Day;
        }
    }
}