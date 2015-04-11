using Domain;
using Registrar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Factories
{
    public interface IExistingBackupFactory
    {
        IEnumerable<ExistingBackup> Create(BackupRootDirectory directory);
    }

    [Register(LifeTime.SingleInstance)]
    public class ExistingBackupFactory : IExistingBackupFactory
    {
        public IEnumerable<ExistingBackup> Create(BackupRootDirectory directory)
        {
            var directories = directory.Directory.GetDirectories();

            foreach (var dir in directories)
            {
                var backupDateTime = DateTime.ParseExact(dir.Name, "yyyy-MM-dd_HH.mm.ss", null);
                var timestampedDir = new TimestampedBackupRoot(dir);

                yield return new ExistingBackup(
                    new BackupDate(backupDateTime), 
                    new BackupTime(backupDateTime.TimeOfDay), 
                    timestampedDir);
            }
        }
    }
}