using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Domain;
using Services;
using Services.Disk;
using System.ComponentModel;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace HardDiskBackup
{
    public class FirstRunViewModel : ViewModelBase, IDataErrorInfo, INotifyPropertyChanged
    {
        public ObservableCollection<BackupDirectory> BackupDirectories { get; private set; }
        public ICommand AddPathCommand { get; private set; }
        public string DirectoryPath { get; set; }

        private IDateTimeProvider _dateTimeProvider;
        private IPersistedOptions _persistedOptions;
        private IBackupDirectoryService _backupDirectoryService;
        private IBackupDirectoryValidator _backupDirectoryValidator;

        public FirstRunViewModel(
            IDateTimeProvider dateTimeProvider,
            IPersistedOptions persistedOptions,
            IBackupDirectoryService backupDirectoryService,
            IBackupDirectoryValidator backupDirectoryValidator)
        {
            _dateTimeProvider = dateTimeProvider;
            _persistedOptions = persistedOptions;
            _backupDirectoryService = backupDirectoryService;
            _backupDirectoryValidator = backupDirectoryValidator;
            BackupDirectories = new ObservableCollection<BackupDirectory>();

            AddPathCommand = new RelayCommand(
                () => 
                {          
                    BackupDirectories.Add(
                        _backupDirectoryService.GetDirectoryFor(DirectoryPath));
                },
                () => { return Validate(); });
        }

        public string Error
        {
            // WPF does not use this implementation. Best to fail fast if this property is accessed.
            get { throw new NotImplementedException(); }
        }

        public string this[string columnName]
        {
            get 
            {
                if (columnName != "DirectoryPath")
                    throw new InvalidOperationException("FirstRunViewModel only supports validation for DirectoryPath, but you tried to validate: " + columnName);

                return Validate() ? null : "This path is not valid";
            }
        }

        private bool Validate()
        {
            return _backupDirectoryValidator.IsValidDirectory(DirectoryPath);
        }
    }
}