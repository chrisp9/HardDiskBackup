using Domain;
using Registrar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper.IO;

namespace Services.Disk.FileSystem
{
    public interface IBackupFileSystem
    {
        Task<Result> Copy(
            IDirectoryInfoWrap source,
            string destination,
            Action<IFileInfoWrap> onFileCopied);

        Task<Result<long>> CalculateTotalSize(IDirectoryInfoWrap directory);

        Task<Result> Delete(IDirectoryInfoWrap directory, Action onDeleteComplete);
    }

    [Register(LifeTime.Transient)]
    public class BackupFileSystem : IBackupFileSystem
    {
        private IDirectoryCopier _directoryCopier;
        private IDirectoryDeleter _directoryDeleter;

        public BackupFileSystem(
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
