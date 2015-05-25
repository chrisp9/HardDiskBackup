using System;
using System.ComponentModel;

namespace Domain
{
    public class FormattedExistingBackup : INotifyPropertyChanged
    {
        public ExistingBackup ExistingBackup;

        public string DateTime
        {
            get
            {
                return ExistingBackup.BackupDate.ToString() + " " + ExistingBackup.BackupTime;
            }
        }

        public string Size
        {
            get
            {
                return Math.Round(ExistingBackup.SizeInBytes / 1024m / 1024m).ToString();
            }
        }

        public bool DeleteIsInProgress
        {
            get { return _deleteIsInProgress; }
            set { _deleteIsInProgress = value; OnPropertyChanged("DeleteIsInProgress"); }
        }

        public bool RestoreIsInProgress
        {
            get { return _restoreIsInProgress; }
            set { _restoreIsInProgress = value; OnPropertyChanged("RestoreIsnProgress");
            }
        }

        private bool _restoreIsInProgress;
        private bool _deleteIsInProgress;

        public FormattedExistingBackup(ExistingBackup existingBackup)
        {
            ExistingBackup = existingBackup;
        }

        private void OnPropertyChanged(string propertyName = "")
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}