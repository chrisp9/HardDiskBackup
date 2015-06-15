using Domain;
using Registrar;
using Services.Factories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SystemWrapper.IO;

namespace Services.Disk.FileSystem
{
    public enum FileSystemStatus
    {
        Idle,
        Calculating,
        Copying
    }

    public interface IBackupFileSystem
    {
        void Copy(IDirectory source, IDirectory destination, Action<IFileInfoWrap> onFileCopied);

        Task Copy(IEnumerable<IDirectory> backupDirectories, Action<IFileInfoWrap> onFileCopied);

        Task Restore(ExistingBackup existingBackup, Action<IFileInfoWrap> onFileRestored);

        Task Delete(ExistingBackup existingBackup, Action onDeleteComplete);

        Task<long> CalculateTotalSize(params IDirectory[] backupDirectories);

        void Target(BackupRootDirectory directory);

        event PropertyChangedEventHandler PropertyChanged;
    }

    [Register(LifeTime.Transient)]
    public class BackupFileSystem : INotifyPropertyChanged, IBackupFileSystem
    {
        private BackupRootDirectory _backupRootDirectory;
        private IDirectoryWrap _directoryWrap;
        private IDirectoryFactory _directoryFactory;
        private IFileWrap _fileWrap;
        private ITimestampedBackupRootProvider _timestampedRootProvider;
        private ISafeActionPerformer _safeActionLogger;
        private IErrorLogger _errorLogger;

        public BackupFileSystem(
            IDirectoryWrap directoryWrap,
            IFileWrap fileWrap,
            IDirectoryFactory directoryFactory,
            ITimestampedBackupRootProvider timestampedRootProvider,
            ISafeActionPerformer safeActionPerformer,
            IErrorLogger errorLogger)
        {
            _directoryWrap = directoryWrap;
            _fileWrap = fileWrap;
            _directoryFactory = directoryFactory;
            _timestampedRootProvider = timestampedRootProvider;
            _safeActionLogger = safeActionPerformer;
            _errorLogger = errorLogger;
        }

        public void Target(BackupRootDirectory directory)
        {
            _backupRootDirectory = directory;
        }

        public async Task<long> CalculateTotalSize(params IDirectory[] directories)
        {
            var size = await Task.Run(() => CalculateSize(directories));
            return size;
        }

        /// <summary>
        /// Copies the provided BackupDirectories from source to backup medium.
        /// </summary>
        /// <param name="backupDirectories">The directories included in the backup</param>
        /// <param name="onFileCopied">A callback which executes after each file is copied</param>
        /// <returns>A task which allows the operation to be awaited</returns>
        public async Task Copy(IEnumerable<IDirectory> backupDirectories, Action<IFileInfoWrap> onFileCopied)
        {
            _errorLogger.SubscribeToErrors();

            var timestampedRoot = _timestampedRootProvider.CreateTimestampedBackup(_backupRootDirectory);

            await Task.Run(() =>
            {
                foreach (var backupDirectory in backupDirectories)
                    Copy(backupDirectory, timestampedRoot, onFileCopied);
            });

            _errorLogger.UnsubscribeFromErrors();
        }

        public async Task Restore(ExistingBackup existingBackup, Action<IFileInfoWrap> onFileRestore)
        {
            var directory = existingBackup.BackupDirectory.Directory.FullName;
            var index  = directory.LastIndexOf(Constants.DiskBackupApp);

            var targetRestoreLocation = directory.Substring(index);

            _errorLogger.SubscribeToErrors();

            await Task.Run(() =>
            {
                Copy(existingBackup.BackupDirectory,
                    BackupDirectoryFactory.Create(targetRestoreLocation), 
                    onFileRestore);
            });

            _errorLogger.UnsubscribeFromErrors();
        }

        public async Task Delete(ExistingBackup existingBackup, Action onDeleteComplete)
        {
            var toDelete = existingBackup.BackupDirectory.Directory;
            await Task.Run(() => toDelete.Delete(true));
            onDeleteComplete();
        }

        private long CalculateSize(IEnumerable<IDirectory> directories)
        {
            var currentSize = 0L;

            foreach (var directory in directories)
                currentSize += CalculateSize(directory.Directory);

            return currentSize;
        }

        private long CalculateSize(IDirectoryInfoWrap directory)
        {
            long currentSize = 0L;

            var fis = _safeActionLogger.SafeGet(() => directory.GetFiles());

            foreach (var fi in fis)
            {
                currentSize += fi.Length;
            }
            // Add subdirectory sizes.

            var dis = _safeActionLogger.SafeGet(() => directory.GetDirectories());

            foreach (var di in dis)
            {
                currentSize += CalculateSize(di);
            }

            return (currentSize);
        }

        // Recursively copy source -> destination
        public void Copy(IDirectory source, IDirectory destination, Action<IFileInfoWrap> onFileCopied)
        {
            var files = _safeActionLogger.SafeGet(() => source.Directory.GetFiles());
            var directories = _safeActionLogger.SafeGet(() => source.Directory.GetDirectories());

            var mirroredRoot = CreateMirroredDirectory(source, destination);

            foreach (var file in files)
            {
                var newFilePath = Path.Combine(mirroredRoot.ToString(), file.Name);
                _safeActionLogger.InvokeSafely(() =>
                {
                    _fileWrap.Copy(file.FullName, newFilePath);
                    var newFile = new FileInfoWrap(Path.Combine(mirroredRoot.ToString(), file.Name).ToString());
                    newFile.Attributes &= ~FileAttributes.ReadOnly;
                });
                onFileCopied(file);
            }

            foreach (var directory in directories)
            {
                var backupDirectory = new BackupDirectory(directory);
                Copy(backupDirectory, destination, onFileCopied);
            }
        }

        private MirroredDirectory CreateMirroredDirectory(IDirectory directory, IDirectory destination)
        {
            var path = directory.ToString();
            var backupRootPath = destination.ToString();

            var mirroredPath = Path.Combine(backupRootPath, path.Replace(":", ""));

            // This will do nothing if the directory already exists,
            // however the application will crash here if the directory
            // cannot be created due to permissions.
            // TODO: catch and return a sensible error to  the user?
            var mapped = _directoryWrap.CreateDirectory(mirroredPath);
            return _directoryFactory.GetMirroredDirectoryFor(mirroredPath);
        }

        private string ReplaceRootWith(string path, string newRoot)
        {
            var endPart = path; //TODO: Will this be flaky?
            return Path.Combine(newRoot, endPart);
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}