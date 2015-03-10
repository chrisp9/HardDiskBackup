using Domain;
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
        void CreateMirroredDirectory(BackupDirectory directory);
    }

    public class BackupFileSystem
    {
        private BackupRootDirectory _backupRootDirectory;
        private IDirectoryWrap _directoryWrap;

        public BackupFileSystem(
            BackupRootDirectory directory,
            IDirectoryWrap directoryWrap)
        {
            _backupRootDirectory = directory;
            _directoryWrap = directoryWrap;
        }

        public void CreateMirroredDirectory(BackupDirectory directory)
        {
            var path = directory.ToString();
            var backupRootPath = _backupRootDirectory.ToString();
            
            var mirroredPath = ReplaceRootWith(path, backupRootPath);
            _directoryWrap.CreateDirectory(mirroredPath);
        }

        private string ReplaceRootWith(string path, string newRoot)
        {
            var endPart = path.Substring(3);
            return Path.Combine(newRoot, endPart);
        }

    }
}
