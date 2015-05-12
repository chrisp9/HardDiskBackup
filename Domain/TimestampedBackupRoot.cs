using System;
using SystemWrapper.IO;

namespace Domain
{
    public class TimestampedBackupRoot : IDirectory
    {
        public IDirectoryInfoWrap Directory { get; private set; }

        public TimestampedBackupRoot(IDirectoryInfoWrap directory)
        {
            if (directory == null)
                throw new ArgumentNullException("You passed a null directory when instantiating a BackupDirectory");

            Directory = directory;
        }

        public override string ToString()
        {
            return Directory.FullName.ToLower().Replace('/', '\\');
        }
    }
}