using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper.IO;

namespace Domain
{
    public class ExistingBackup
    {
        public BackupDate BackupDate { get; private set; }
        public BackupTime BackupTime { get; private set; }
        public TimestampedBackupRoot BackupDirectory { get; private set; }
        public long SizeInBytes { get; private set; }

        public ExistingBackup(
            BackupDate backupDate,
            BackupTime backupTime,
            TimestampedBackupRoot directory)
        {
            BackupDate = backupDate;
            BackupTime = backupTime;
            BackupDirectory = directory;
            //SizeInBytes = directory.Directory;
        }
    }
}
