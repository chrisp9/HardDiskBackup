using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Disk.FileSystem
{
    public class RestoreToOriginalLocationBackupStrategy : IBackupStrategy
    {
        public async Task Restore(ExistingBackup existingBackup)
        {
            var timestampedBackupRoot = existingBackup.BackupDirectory;

           // timestampedBackupRoot.Directory;

        }
    }
}
