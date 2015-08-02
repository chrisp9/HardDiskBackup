using Domain;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Services.Disk.FileSystem;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace HardDiskBackup.ViewModel
{
    public class RestoreBackupViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public FormattedExistingBackup FormattedExistingBackup { get; private set; }
        public RelayCommand RestoreBackupCommand;

        public long TotalBytesToCopy
        {
            get { return _totalBytesToCopy; }
            set { _totalBytesToCopy = value; OnPropertyChanged(); }
        }

        public long BytesCopiedSoFar
        {
            get { return _bytesCopiedSoFar; }
            set { _bytesCopiedSoFar = value; OnPropertyChanged(); }
        }

        private long _totalBytesToCopy;
        private long _bytesCopiedSoFar;

        private IBackupFileSystem _backupFileSystem;
        private RestoreToOriginalLocationBackupStrategy _originalLocationStrat;

        public RestoreBackupViewModel(
            FormattedExistingBackup backup,
            IBackupFileSystem backupFileSystem,
            RestoreToOriginalLocationBackupStrategy originalLocationStrat)
        {
            FormattedExistingBackup = backup;
            _backupFileSystem = backupFileSystem;
            _originalLocationStrat = originalLocationStrat;

            RestoreBackupCommand = new RelayCommand(
                async () =>
                {
                    var result = await Task.Run(() => 
                        _backupFileSystem.CalculateTotalSize(backup.ExistingBackup.BackupDirectory.Directory));

                    TotalBytesToCopy = result.Value;

                    await _originalLocationStrat.Restore(backup.ExistingBackup, 
                        file => { BytesCopiedSoFar += file.Length; });
                },
                () => true);
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChangedHandler;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
