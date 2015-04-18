using Domain;
using Registrar;
using Services.Disk.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Factories
{
    public interface IExistingBackupsFactory
    {
        Task<ExistingBackup[]> Create(BackupRootDirectory directory);
    }

    [Register(LifeTime.SingleInstance)]
    public class ExistingBackupsFactory : IExistingBackupsFactory
    {
        private IBackupFileSystem _backupFileSystem;

        public ExistingBackupsFactory(IBackupFileSystem backupFileSystem)
        {
            _backupFileSystem = backupFileSystem;
        }

        public async Task<ExistingBackup[]> Create(BackupRootDirectory directory)
        {
            var directories = directory.Directory.GetDirectories();
            var existingBackups = new List<ExistingBackup>();

            foreach (var dir in directories)
            {
                var backupDateTime = DateTime.ParseExact(dir.Name, "yyyy-MM-dd_HH.mm.ss", null);
                var timestampedDir = new TimestampedBackupRoot(dir);

                var size = await _backupFileSystem.CalculateTotalSize(timestampedDir);

                existingBackups.Add(new ExistingBackup(
                    new BackupDate(backupDateTime), 
                    new BackupTime(backupDateTime.TimeOfDay), 
                    timestampedDir,
                    _backupFileSystem.CalculateTotalSize(timestampedDir).Result)); //TODO: Will this be too slow?
            }

            return existingBackups.ToArray();
        }
    }
}