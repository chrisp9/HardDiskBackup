using System;
using SystemWrapper;

namespace TestHelpers
{
    public class DateTimeWrapFactory
    {
        public static DateTimeWrap CreateDateTimeWrap(DateTime time)
        {
            return new DateTimeWrap(time);
        }
    }
}