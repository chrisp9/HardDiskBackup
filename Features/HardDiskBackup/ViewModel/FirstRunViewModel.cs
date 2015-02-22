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

namespace HardDiskBackup
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    /// 

    public class FirstRunViewModel : ViewModelBase, IDataErrorInfo
    {
        public IEnumerable<BackupDirectory> BackupDirectories { get; private set; }
        public ICommand AddPathCommand { get; private set; }
        public string DirectoryPath { get; set; }

        private IDateTimeProvider _dateTimeProvider;
        private IPersistedOptions _persistedOptions;
        private IBackupDirectoryService _backupDirectoryService;

        public FirstRunViewModel(
            IDateTimeProvider dateTimeProvider,
            IPersistedOptions persistedOptions,
            IBackupDirectoryService backupDirectoryService)
        {
            _dateTimeProvider = dateTimeProvider;
            _persistedOptions = persistedOptions;
            _backupDirectoryService = backupDirectoryService;

            AddPathCommand = new RelayCommand(() => { }, () => { return _backupDirectoryService.IsValidPath(DirectoryPath); });
        }

        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public string this[string columnName]
        {
            get 
            {
                if (columnName != "DirectoryPath")
                    throw new Exception("FirstRunViewModel only supports validation for DirectoryPath, but you tried to validate: " + columnName);

                return _backupDirectoryService.IsValidPath(DirectoryPath) ? null : "Invalid Path";
            }
        }
    }
}
