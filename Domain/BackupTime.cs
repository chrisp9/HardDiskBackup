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
        private TimeSpan _time;

        public BackupTime(TimeSpan dateTime)
        {
            _time = dateTime;
        }

        public BackupTime(int hours, int minutes, int seconds)
        {
            _time = new TimeSpan(hours, minutes, seconds);
        }

        public override bool Equals(object obj)
        {
            var other = obj as BackupTime;
            if (other == null) return false;

            return _time == other._time;
        }

        public static bool operator < (BackupTime a, BackupTime b) 
        {
            return a._time < b._time;
        }

        public static bool operator >(BackupTime a, BackupTime b)
        {
            return a._time > b._time;
        }

        public override int GetHashCode()
        {
            return _time.GetHashCode();
        }

        public int Hours { get { return _time.Hours; } }
        public int Minutes { get { return _time.Minutes; } }
        public int Seconds { get { return _time.Seconds; } }
    }
}
