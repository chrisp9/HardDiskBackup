using Domain;
using Services.Factories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper.IO;

namespace Services.Disk.FileSystem
{
    public interface IBackupFileSystem
    {
        void Copy(IEnumerable<BackupDirectory> backupDirectories);
        long CalculateTotalSize(IEnumerable<BackupDirectory> backupDirectories);
    }

    public class BackupFileSystem
    {
        private BackupRootDirectory _backupRootDirectory;
        private IDirectoryWrap _directoryWrap;
        private IDirectoryFactory _directoryFactory;
        private IFileWrap _fileWrap;

        public BackupFileSystem(
            BackupRootDirectory directory,
            IDirectoryWrap directoryWrap,
            IFileWrap fileWrap,
            IDirectoryFactory directoryFactory)
        {
            _backupRootDirectory = directory;
            _directoryWrap = directoryWrap;
            _directoryFactory = directoryFactory;
            _fileWrap = fileWrap;
        }

        public void Copy(IEnumerable<BackupDirectory> backupDirectories)
        {
            foreach (var backupDirectory in backupDirectories)
                Copy(backupDirectory);
        }

        public long CalculateTotalSize(IEnumerable<BackupDirectory> directories)
        {
            var currentSize = 0L;
            // Add file sizes.

            foreach (var directory in directories)
            {
                currentSize += CalculateSize(directory.Directory);
            }

            return currentSize;
        }

        private long CalculateSize(IDirectoryInfoWrap directory)
        {
            long currentSize = 0L;
            var fis = directory.GetFiles();
            foreach (var fi in fis)
            {
                currentSize += fi.Length;
            }
            // Add subdirectory sizes.
            var dis = directory.GetDirectories();
            foreach (var di in dis)
            {
                currentSize += CalculateSize(di);
            }
            return (currentSize);
        }


        // Recursively copy source -> destination
        private void Copy(BackupDirectory source)
        {
            var files = source.Directory.GetFiles();
            var directories = source.Directory.GetDirectories();

            foreach (var directory in directories)
            {
                var backupDirectory = new BackupDirectory(directory);
                var mirroredDirectory = CreateMirroredDirectory(backupDirectory);

                foreach(var file in directory.GetFiles()) 
                {
                    _fileWrap.Copy(file.FullName, Path.Combine(mirroredDirectory.ToString(), file.Name));
                }

                Copy(backupDirectory);
            }
        }

        private MirroredDirectory CreateMirroredDirectory(BackupDirectory directory)
        {
            var path = directory.ToString();
            var backupRootPath = _backupRootDirectory.ToString();

            var mirroredPath = ReplaceRootWith(path, backupRootPath);

            // This will do nothing if the directory already exists,
            // however the application will crash here if the directory
            // cannot be created due to permissions.
            // TODO: catch and return a sensible error to  the user?
            var mapped = _directoryWrap.CreateDirectory(mirroredPath);
            return _directoryFactory.CreateMirroredDirectory(mirroredPath);
        }

        private string ReplaceRootWith(string path, string newRoot)
        {
            var endPart = path.Substring(3); //TODO: Will this be flaky?
            return Path.Combine(newRoot, endPart);
        }
    }
}
