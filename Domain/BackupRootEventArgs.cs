using System;

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