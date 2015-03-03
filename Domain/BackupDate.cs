using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper;

namespace Domain
{
    public class BackupDate
    {
        private IDateTimeWrap _date;

        public BackupDate(IDateTimeWrap date)
        {
            _date = date;
        }

        public int Day { get { return _date.Day; } }
        public int Month { get { return _date.Month; } }
        public int Year { get { return _date.Year; } }
    }
}
