using System.Windows.Input;
using Domain;

namespace HardDiskBackup.ViewModel
{
    public interface IFirstRunViewModel
    {
        string this[string columnName] { get; }

        ICommand AddPathCommand { get; }
        BackupDirectoriesAndSchedule Backup { get; }
        IBackupDirectoryModel BackupDirectoryModel { get; set; }
        string DirectoryPath { get; set; }
        string Error { get; }
        ICommand RemovePathCommand { get; }
        ICommand ScheduleBackupCommand { get; }
        SetScheduleViewModel SetScheduleViewModel { get; set; }
    }
}