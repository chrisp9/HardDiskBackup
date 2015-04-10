using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class BackupRootEventArgs : EventArgs
    {
        public BackupRootDirectory Directory { get; private set; }

        public BackupRootEventArgs(BackupRootDirectory directory)
        {
            Directory = directory;
        }
    }
}
