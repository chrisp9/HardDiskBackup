using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper;

namespace Domain
{
    public class NextBackupDateTime
    {
        public IDateTimeWrap DateTime { get; private set; }

        public NextBackupDateTime(DateTime dateTime)
        {
            DateTime = new DateTimeWrap(dateTime);
        }

        public override string ToString()
        {
            return DateTime.ToString();
        }

        public string ToString(string format)
        {
            return DateTime.ToString(format);
        }
    }
}