using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
