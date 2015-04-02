using Domain;
using Registrar;
using Services.Factories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
        Task Copy(IEnumerable<BackupDirectory> backupDirectories);
        Task<long> CalculateTotalSize(IEnumerable<BackupDirectory> backupDirectories);
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
        private ISafeActionPerformer _safeActionPerformer;
        
        public BackupFileSystem(
            IDirectoryWrap directoryWrap,
            IFileWrap fileWrap,
            IDirectoryFactory directoryFactory,
            ITimestampedBackupRootProvider timestampedRootProvider,
            ISafeActionPerformer safeActionPerformer)
        {
            _directoryWrap = directoryWrap;
            _fileWrap = fileWrap;
            _directoryFactory = directoryFactory;
            _timestampedRootProvider = timestampedRootProvider;
            _safeActionPerformer = safeActionPerformer;
        }

        public void Target(BackupRootDirectory directory)
        {
            _backupRootDirectory = directory;
        }

        public async Task<long> CalculateTotalSize(IEnumerable<BackupDirectory> directories)
        {
            var size = await Task.Run(() => CalculateSize(directories));
            return size;
        }

        public async Task Copy(IEnumerable<BackupDirectory> backupDirectories)
        {
            var timestampedRoot = _timestampedRootProvider.CreateTimestampedBackup(_backupRootDirectory);

            await Task.Run(() =>
            {
                foreach (var backupDirectory in backupDirectories)
                    Copy(backupDirectory, timestampedRoot);
            });
        }

        private long CalculateSize(IEnumerable<BackupDirectory> directories)
        {
            var currentSize = 0L;

            foreach (var directory in directories)
                currentSize += CalculateSize(directory.Directory);

            return currentSize;
        }

        private long CalculateSize(IDirectoryInfoWrap directory)
        {
            long currentSize = 0L;

            var fis = _safeActionPerformer.SafeGet(() => directory.GetFiles());

            foreach (var fi in fis)
            {
                currentSize += fi.Length;
            }
            // Add subdirectory sizes.

            var dis = _safeActionPerformer.SafeGet(() => directory.GetDirectories());

            foreach (var di in dis)
            {
                currentSize += CalculateSize(di);
            }

            return (currentSize);
        }

        // Recursively copy source -> destination
        private void Copy(BackupDirectory source, TimestampedBackupRoot destination)
        {
            var files = _safeActionPerformer.SafeGet(() => source.Directory.GetFiles());
            var directories = _safeActionPerformer.SafeGet(() => source.Directory.GetDirectories());
            
            var mirroredRoot = CreateMirroredDirectory(source, destination);

            foreach (var file in files)
            {
                _fileWrap.Copy(file.FullName, Path.Combine(mirroredRoot.ToString(), file.Name));
            }

            foreach (var directory in directories)
            {
                var backupDirectory = new BackupDirectory(directory);
                Copy(backupDirectory, destination);
            }
        }

        private MirroredDirectory CreateMirroredDirectory(BackupDirectory directory, TimestampedBackupRoot destination)
        {
            var path = directory.ToString();
            var backupRootPath = destination.ToString();

            var mirroredPath = ReplaceRootWith(path, backupRootPath);

            // This will do nothing if the directory already exists,
            // however the application will crash here if the directory
            // cannot be created due to permissions.
            // TODO: catch and return a sensible error to  the user?
            var mapped = _directoryWrap.CreateDirectory(mirroredPath);
            return _directoryFactory.GetMirroredDirectoryFor(mirroredPath);
        }

        private string ReplaceRootWith(string path, string newRoot)
        {
            var endPart = path.Substring(3); //TODO: Will this be flaky?
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
