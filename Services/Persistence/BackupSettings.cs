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
        IBackupDateTime NextBackup { get; set; }
        IEnumerable<BackupDirectory> BackupDirectories { get; set; }
    }

    public class BackupSettings : IBackupSettings
    {
        public IBackupDateTime NextBackup { get; set; }
        public IEnumerable<BackupDirectory> BackupDirectories { get; set; }

        public BackupSettings(IBackupDateTime nextBackup, IEnumerable<BackupDirectory> backupDirectories)
        {
            NextBackup = nextBackup;
            BackupDirectories = backupDirectories;
        }
    }
}
