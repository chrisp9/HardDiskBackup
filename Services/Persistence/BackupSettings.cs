using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace Services
{
    public interface IBackupSettings 
    {
        NextBackupDateTime NextBackup { get; set; }
        IEnumerable<BackupDirectory> BackupDirectories { get; set; }
    }

    public class BackupSettings : IBackupSettings
    {
        public NextBackupDateTime NextBackup { get; set; }
        public IEnumerable<BackupDirectory> BackupDirectories { get; set; }

        public BackupSettings(NextBackupDateTime nextBackup, IEnumerable<BackupDirectory> backupDirectories)
        {
            NextBackup = nextBackup;
            BackupDirectories = backupDirectories;
        }
    }
}
