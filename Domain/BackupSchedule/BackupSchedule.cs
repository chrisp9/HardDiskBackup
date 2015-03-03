using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.BackupSchedule
{
    public abstract class BackupSchedule
    {
        protected IDateTimeProvider DateTimeProvider { get; private set; }
        protected INextBackupDateTimeFactory NextBackupDateTimeFactory { get; private set; }
        protected BackupTime BackupTime { get; private set; }
        public abstract NextBackupDateTime CalculateNextBackupDateTime();

        protected BackupSchedule(
            IDateTimeProvider provider, 
            INextBackupDateTimeFactory factory,
            BackupTime backupTime)
        {
            DateTimeProvider = provider;
            NextBackupDateTimeFactory = factory;
            BackupTime = backupTime;
        }
    }
}
