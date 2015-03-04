using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper;

namespace Domain
{
    public interface INextBackupDateTimeFactory
    {
        NextBackupDateTime Create(BackupDate date, BackupTime time);
    }

    public class NextBackupDateTimeFactory : INextBackupDateTimeFactory
    {
        public NextBackupDateTime Create(BackupDate date, BackupTime time)
        {
            return new NextBackupDateTime(
                new DateTime(
                        date.Day,
                        date.Month, 
                        date.Year, 
                        time.Hours, 
                        time.Minutes, 
                        time.Seconds));
        }
    }
}
