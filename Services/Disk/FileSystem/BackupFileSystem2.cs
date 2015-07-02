using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper.IO;

namespace Services.Disk.FileSystem
{
    public interface IBackupFileSystem2
    {
        Task<Result> Copy(
            IDirectoryInfoWrap source,
            string destination,
            Action<IFileInfoWrap> onFileCopied);

        Task<Result<long>> CalculateTotalSize(IDirectoryInfoWrap directory);

        Task<Result> Delete(IDirectoryInfoWrap directory, Action onDeleteComplete);
    }

    public class BackupFileSystem2 : IBackupFileSystem2
    {
        private IDirectoryCopier _directoryCopier;
        private IDirectoryDeleter _directoryDeleter;

        public BackupFileSystem2(
            IDirectoryCopier directoryCopier,
            IDirectoryDeleter directoryDeleter)
        {
            _directoryCopier = directoryCopier;
            _directoryDeleter = directoryDeleter;
        }

        public Task<Result> Copy(IDirectoryInfoWrap source, string destination, Action<IFileInfoWrap> onFileCopied)
        {
            return _directoryCopier.CopySafe(source, destination, onFileCopied);
        }

        public Task<Result<long>> CalculateTotalSize(IDirectoryInfoWrap directory)
        {
            return _directoryCopier.CalculateTotalSizeSafe(directory);
        }

        public Task<Result> Delete(IDirectoryInfoWrap directory, Action onDeleteComplete)
        {
            return _directoryDeleter.Delete(directory, onDeleteComplete);
        }
    }
}
