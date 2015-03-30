using Registrar;
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

    [Register(LifeTime.SingleInstance)]
    public class NextBackupDateTimeFactory : INextBackupDateTimeFactory
    {
        public NextBackupDateTime Create(BackupDate date, BackupTime time)
        {
            return new NextBackupDateTime(
                new DateTime(
                        date.Year,
                        date.Month, 
                        date.Day, 
                        time.Hours, 
                        time.Minutes, 
                        time.Seconds));
        }
    }
}
