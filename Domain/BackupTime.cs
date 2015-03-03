using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper;

namespace Domain
{
    public class BackupTime
    {
        private IDateTimeWrap _dateTimeWrap;

        public BackupTime(IDateTimeWrap dateTimeWrap)
        {
            _dateTimeWrap = dateTimeWrap;
        }

        public int Hours { get { return _dateTimeWrap.Hour; } }
        public int Minutes { get { return _dateTimeWrap.Minute; } }
        public int Seconds { get { return _dateTimeWrap.Second; } }
    }
}
