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

        public FormattedExistingBackup(ExistingBackup existingBackup)
        {
            ExistingBackup = existingBackup;
        }
    }
}
