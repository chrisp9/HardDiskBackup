using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class BackupDateTime
    {
        public DateTime DateTime { get; private set; }

        public BackupDateTime(DateTime dateTime)
        {
            DateTime = dateTime;
        }
    }
}
