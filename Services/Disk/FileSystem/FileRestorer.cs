using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Disk.FileSystem
{
    public interface IFileRestorer
    {
        Task RestoreToOriginalLocation(ExistingBackup existingBackup);
        Task RestoreToDesktop(ExistingBackup existingBackup);
    }

    public class FileRestorer : IFileRestorer
    {
        private RestoreToOriginalLocationBackupStrategy _originalLocStrategy;

        public FileRestorer(RestoreToOriginalLocationBackupStrategy strategy) 
        {
            _originalLocStrategy = strategy;
        }

        public async Task RestoreToOriginalLocation(ExistingBackup existingBackup)
        {
            await _originalLocStrategy.Restore(existingBackup);
        }

        public async Task RestoreToDesktop(ExistingBackup existingBackup)
        {
            throw new NotImplementedException();
        }
    }
}
