using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public interface IBackupDirectoryModel : INotifyPropertyChanged
    {
        void Add(BackupDirectory backupDirectory);
        void Remove(BackupDirectory backupDirectory);

        bool IsSubdirectoryOfExisting(string backupDirectoryPath);
    }

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
                .Where(x => newPath.Contains(x.Directory.FullName.ToLower()))
                .Any();
        }

        private IEnumerable<BackupDirectory> ExistingSubdirectoriesOf(BackupDirectory backupDirectory)
        {
            var newPath = backupDirectory.Directory.FullName.ToLower();

            return _backupDirectories
                .Where(x => x.Directory.FullName.ToLower().Contains(newPath));
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
