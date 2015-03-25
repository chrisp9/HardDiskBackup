using Registrar;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Domain
{
    public interface IBackupDirectoryModel : INotifyPropertyChanged
    {
        ReadOnlyCollection<BackupDirectory> BackupDirectories { get; }
        void Add(BackupDirectory backupDirectory);
        void Remove(BackupDirectory backupDirectory);

        bool IsSubdirectoryOfExisting(string backupDirectoryPath);
    }

    [Register(Scope.SingleInstance)]
    public class BackupDirectoryModel : IBackupDirectoryModel
    {
        public ReadOnlyCollection<BackupDirectory> BackupDirectories
        {
            get
            {
                return new ReadOnlyCollection<BackupDirectory>(
                    _backupDirectories.ToArray());
            }
        }

        private IList<BackupDirectory> _backupDirectories;

        public BackupDirectoryModel()
        {
            _backupDirectories = new List<BackupDirectory>();
        }

        public void Add(BackupDirectory backupDirectory)
        {
            var subPaths = ExistingSubdirectoriesOf(backupDirectory);
            subPaths
                .ToList()
                .ForEach(x => _backupDirectories.Remove(x));

            _backupDirectories.Add(backupDirectory);
            OnPropertyChanged();
        }

        public void Remove(BackupDirectory backupDirectory)
        {
            _backupDirectories.Remove(backupDirectory);
            OnPropertyChanged();
        }

        public bool IsSubdirectoryOfExisting(string backupDirectory)
        {
            var newPath = backupDirectory.ToLower().Replace('/','\\');

            return _backupDirectories
                .Where(x => newPath.Contains(x.ToString()))
                .Any();
        }

        private IEnumerable<BackupDirectory> ExistingSubdirectoriesOf(BackupDirectory backupDirectory)
        {
            return _backupDirectories
                .Where(x => x.ToString().Contains(backupDirectory.ToString()));
        }

        private void OnPropertyChanged()
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs("BackupDirectories"));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
