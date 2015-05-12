using Registrar;
using System;

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