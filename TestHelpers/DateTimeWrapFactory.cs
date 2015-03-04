using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
