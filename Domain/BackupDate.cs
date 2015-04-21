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
        private DateTime _date;

        public BackupDate(int year, int month, int day)
        {
            _date = new DateTime(year, month, day);
        }

        public BackupDate(DateTime date)
        {
            _date = date;
        }

        public override bool Equals(object obj)
        {
            var other = obj as BackupDate;
            if (other == null) return false;

            return _date == other._date;
        }

        public override string ToString()
        {
            return _date.ToString("dd/MM/yyyy");
        }

        public override int GetHashCode()
        {
            return _date.GetHashCode();
        }

        public int Day { get { return _date.Day; } }
        public int Month { get { return _date.Month; } }
        public int Year { get { return _date.Year; } }
    }
}
