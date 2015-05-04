using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
