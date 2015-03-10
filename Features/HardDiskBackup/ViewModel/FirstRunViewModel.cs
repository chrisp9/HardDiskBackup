using Domain;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Services.Disk;
using Services.Factories;
using Services.Persistence;
using Services.Scheduling;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace HardDiskBackup
{
    public class FirstRunViewModel : ViewModelBase, IDataErrorInfo, INotifyPropertyChanged
    {
        public ICommand AddPathCommand { get; private set; }
        public ICommand RemovePathCommand { get; private set; }
        public ICommand ScheduleBackupCommand { get; private set; }

        public Backup Backup { get; private set; }

        public string DirectoryPath { get; set; }
        public IBackupDirectoryModel BackupDirectoryModel { get; private set; }

        private IDateTimeProvider _dateTimeProvider;
        private IJsonSerializer _jsonSerializer;
        private IBackupDirectoryValidator _backupDirectoryValidator;
        private IBackupDirectoryFactory _backupDirectoryFactory;
        private ISetScheduleModel _setScheduleModel;

        public FirstRunViewModel( // TODO: Factor out commands.
            IDateTimeProvider dateTimeProvider,
            IJsonSerializer jsonSerializer,
            IBackupDirectoryValidator backupDirectoryValidator,
            IBackupDirectoryFactory backupDirectoryFactory,
            IBackupDirectoryModel backupDirectoryModel,
            ISetScheduleModel setScheduleModel)
        {
            _dateTimeProvider = dateTimeProvider;
            _jsonSerializer = jsonSerializer;
            
            _backupDirectoryValidator = backupDirectoryValidator;
            _backupDirectoryFactory = backupDirectoryFactory;
            _setScheduleModel = setScheduleModel;

            BackupDirectoryModel = backupDirectoryModel;

            // Yeah, this looks horrible and needs to be changed.
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

            ScheduleBackupCommand = new RelayCommand(
                () =>
                    {
                        var schedule = _setScheduleModel.CreateSchedule();
                        var directories = BackupDirectoryModel.BackupDirectories;

                        Backup = Backup.Create(directories, schedule);
                    },
                () =>
                    {
                        return backupDirectoryModel.BackupDirectories.Count > 0
                            && _setScheduleModel.IsScheduleValid();
                    });
        }

        public string Error
        {
            // This isn't supported by WPF but can be invoked externally e.g. by Snoop.
            get { return string.Empty; }
        }

        public string this[string columnName]
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