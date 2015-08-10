using Domain;
using System;
using System.Threading.Tasks;
using SystemWrapper.IO;

namespace Services.Disk.FileSystem
{
    public class RestoreToOriginalLocationBackupStrategy : IBackupStrategy
    {
        private readonly IErrorLogger _errorLogger;
        private readonly IBackupFileSystem _backupFileSystem;

        public RestoreToOriginalLocationBackupStrategy(
            IErrorLogger errorLogger,
            IBackupFileSystem backupFileSystem)
        {
            _errorLogger = errorLogger;
            _backupFileSystem = backupFileSystem;
        }

        public async Task Restore(
            ExistingBackup existingBackup, 
            Action<IFileInfoWrap> onFileRestore)
        {
            var directory = existingBackup.BackupDirectory.Directory.FullName;
            var index = directory.LastIndexOf(Constants.DiskBackupApp, StringComparison.Ordinal);

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