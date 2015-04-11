using Domain.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class BackupDirectoriesAndSchedule
    {
        public IEnumerable<BackupDirectory> BackupDirectories { get; private set; }
        public BackupSchedule BackupSchedule { get; private set; }

        private BackupDirectoriesAndSchedule(
            IEnumerable<BackupDirectory> backupDirectories,
            BackupSchedule nextBackupDateTime)
        {
            BackupDirectories = backupDirectories;
            BackupSchedule = nextBackupDateTime;
        }

        public static BackupDirectoriesAndSchedule Create(
            IEnumerable<BackupDirectory> backupDirectories, 
            BackupSchedule nextBackupDateTime) 
        {
            return new BackupDirectoriesAndSchedule(backupDirectories, nextBackupDateTime);
        }
    }
}