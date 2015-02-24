using SystemWrapper.IO;

namespace Services.Disk
{
    public interface IBackupDirectoryValidator
    {
        bool IsValidDirectory(string path);
    }

    public class BackupDirectoryValidator : IBackupDirectoryValidator
    {
        private IDirectoryWrap _directoryWrap;

        public BackupDirectoryValidator(IDirectoryWrap directoryWrap) 
        {
            _directoryWrap = directoryWrap;
        }

        public bool IsValidDirectory(string path)
        {
            return _directoryWrap.Exists(path) 
                && (path.Contains("/") || path.Contains(@"\"));
        }
    }
}
