using Domain;
using System;

namespace TestHelpers
{
    public class FakeDateTimeProvider : IDateTimeProvider
    {
        public FakeDateTimeProvider(
            int day,
            int month,
            int year,
            int hour,
            int minute,
            int second)
        {
            Now = new DateTime(year, month, day, hour, minute, second);
        }

        public FakeDateTimeProvider(string dateTime)
        {
            Now = DateTime.Parse(dateTime);
        }

        public DateTime Now { get; private set; }
    }
}