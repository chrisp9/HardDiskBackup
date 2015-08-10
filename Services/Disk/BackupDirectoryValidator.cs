using Domain;
using Registrar;
using SystemWrapper.IO;

namespace Services.Disk
{
    public interface IBackupDirectoryValidator
    {
        ValidationResult CanAdd(string backupDirectory);
    }

    public enum ValidationResult
    {
        Success,
        InvalidPath,
        PathAlreadyExists
    }

    [Register(LifeTime.Transient)]
    public class BackupDirectoryValidator : IBackupDirectoryValidator
    {
        private readonly IDirectoryWrap _directoryWrap;
        private readonly IBackupDirectoryModel _backupDirectoryModel;

        public BackupDirectoryValidator(IDirectoryWrap directoryWrap, IBackupDirectoryModel model)
        {
            _directoryWrap = directoryWrap;
            _backupDirectoryModel = model;
        }

        public ValidationResult CanAdd(string backupDirectory)
        {
            if (backupDirectory == null
                || !_directoryWrap.Exists(backupDirectory)
                || backupDirectory.Contains(".")
                || (!backupDirectory.Contains("/")
                && !backupDirectory.Contains(@"\")))
                return ValidationResult.InvalidPath;

            return _backupDirectoryModel.IsSubdirectoryOfExisting(backupDirectory)
                ? ValidationResult.PathAlreadyExists
                : ValidationResult.Success;
        }

        public string Error => string.Empty;

        public string this[string columnName]
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}