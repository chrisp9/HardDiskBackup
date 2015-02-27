using System;
using Domain;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Services;
using Services.Disk;
using Services.Factories;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using Services.Persistence;

namespace HardDiskBackup
{
    public class FirstRunViewModel : ViewModelBase, IDataErrorInfo, INotifyPropertyChanged
    {
        public ICommand AddPathCommand { get; private set; }
        public ICommand RemovePathCommand { get; private set; }
        public string DirectoryPath { get; set; }
        public IBackupDirectoryModel BackupDirectoryModel { get; private set; }

        private IDateTimeProvider _dateTimeProvider;
        private IJsonSerializer _jsonSerializer;
        private IBackupDirectoryValidator _backupDirectoryValidator;
        private IBackupDirectoryFactory _backupDirectoryFactory;

        public FirstRunViewModel(
            IDateTimeProvider dateTimeProvider,
            IJsonSerializer jsonSerializer,
            IBackupDirectoryValidator backupDirectoryValidator,
            IBackupDirectoryFactory backupDirectoryFactory,
            IBackupDirectoryModel backupDirectoryModel)
        {
            _dateTimeProvider = dateTimeProvider;
            _jsonSerializer = jsonSerializer;
            _backupDirectoryValidator = backupDirectoryValidator;
            _backupDirectoryFactory = backupDirectoryFactory;
            BackupDirectoryModel = backupDirectoryModel;

            AddPathCommand = new RelayCommand(
                () => 
                    {
                        var backupDirectory = _backupDirectoryFactory.Create(DirectoryPath);
                        BackupDirectoryModel.Add(backupDirectory); 
                    },
                () => { return _backupDirectoryValidator.CanAdd(DirectoryPath) == ValidationResult.Success; });

            RemovePathCommand = new RelayCommand<BackupDirectory>(
                (item) => { BackupDirectoryModel.Remove(item); },
                _      => { return true; });
        }

        public string Error
        {
            // This isn't supported by WPF but can be invoked externally e.g. by Snoop.
            get { return string.Empty; }
        }

        public string this[string columnName] // Check if directory is a subdirectory of.
        {
            get 
            {
                if (columnName != "DirectoryPath")
                    throw new InvalidOperationException("FirstRunViewModel only supports validation for DirectoryPath, but you tried to validate: " + columnName);

                // TODO: Move to validator?
                switch (_backupDirectoryValidator.CanAdd(DirectoryPath))
                {
                    case ValidationResult.InvalidPath:
                        return "This path is not valid";
                    case ValidationResult.PathAlreadyExists:
                        return "You cannot add this path because it is already included in the backup";
                    case ValidationResult.Success:
                        return null;
                    default:
                        throw new ArgumentException("Invalid ValidationResult");
                }
            }
        }
    }
}