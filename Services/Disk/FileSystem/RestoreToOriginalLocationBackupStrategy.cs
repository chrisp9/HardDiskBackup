using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper.IO;

namespace Services.Disk.FileSystem
{
    public class RestoreToOriginalLocationBackupStrategy : IBackupStrategy
    {
        private IErrorLogger _errorLogger;
        private IBackupFileSystem2 _backupFileSystem;

        public RestoreToOriginalLocationBackupStrategy(
            IErrorLogger errorLogger,
            IBackupFileSystem2 backupFileSystem)
        {
            _errorLogger = errorLogger;
            _backupFileSystem = backupFileSystem;
        }

        public async Task Restore(
            ExistingBackup existingBackup, 
            Action<IFileInfoWrap> onFileRestore)
        {
            var directory = existingBackup.BackupDirectory.Directory.FullName;
            var index = directory.LastIndexOf(Constants.DiskBackupApp);

            var targetRestoreLocation = directory.Substring(index);

            _errorLogger.SubscribeToErrors();

            await Task.Run(() =>
            {
                _backupFileSystem.Copy(
                    existingBackup.BackupDirectory.Directory,
                    targetRestoreLocation,
                    onFileRestore);
            });

            _errorLogger.UnsubscribeFromErrors();
        }
    }
}