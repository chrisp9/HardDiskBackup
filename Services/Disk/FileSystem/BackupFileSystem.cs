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
    }

    public class BackupFileSystem
    {
        private BackupRootDirectory _backupRootDirectory;
        private IDirectoryWrap _directoryWrap;
        private IDictionary<BackupDirectory, MirroredDirectory> _mappings;
        private IDirectoryFactory _directoryFactory;

        public BackupFileSystem(
            BackupRootDirectory directory,
            IDirectoryWrap directoryWrap,
            IDirectoryFactory directoryFactory)
        {
            _backupRootDirectory = directory;
            _directoryWrap = directoryWrap;
            _directoryFactory = directoryFactory;
            _mappings = new Dictionary<BackupDirectory, MirroredDirectory>();
        }

        public void CreateMirroredDirectories(IEnumerable<BackupDirectory> backupDirectories)
        {
            foreach (var directory in backupDirectories)
                _mappings.Add(directory, CreateMirroredDirectory(directory));
        }

        public void Copy(IEnumerable<BackupDirectory> backupDirectories)
        {
            foreach (var backupDirectory in backupDirectories)
                PerformCopy(backupDirectory, _mappings[backupDirectory]);
        }

        // Recursively copy source -> destination
        private void PerformCopy(BackupDirectory source, MirroredDirectory target)
        {
            var files = source.Directory.GetFiles();
            var directories = source.Directory.GetDirectories();

            foreach (var file in files)
                file.CopyTo(target.ToString());

            foreach (var directory in directories)
            {
                var backupDirectory = new BackupDirectory(directory);
                var mirroredDirectory = CreateMirroredDirectory(backupDirectory);
                PerformCopy(backupDirectory, mirroredDirectory);
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
            var endPart = path.Substring(3);
            return Path.Combine(newRoot, endPart);
        }
    }
}
