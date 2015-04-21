using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardDiskBackup.ViewModel
{
    public class FormattedExistingBackup
    {
        private ExistingBackup _existingBackup;

        public string DateTime
        {
            get
            {
                return _existingBackup.BackupDate.ToString() + " " + _existingBackup.BackupTime;
            }
        }

        public string Size
        {
            get
            {
                return Math.Round(_existingBackup.SizeInBytes / 1024m / 1024m).ToString();
            }
        }

        public FormattedExistingBackup(ExistingBackup existingBackup)
        {
            _existingBackup = existingBackup;
        }
    }
}
