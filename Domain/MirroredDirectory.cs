using System;
using SystemWrapper.IO;

namespace Domain
{
    public class MirroredDirectory : IDirectory
    {
        public IDirectoryInfoWrap Directory { get; private set; }

        public MirroredDirectory(IDirectoryInfoWrap directory)
        {
            if (directory == null)
                throw new ArgumentNullException("You passed a null directory when instantiating a MirroredDirectory");

            Directory = directory;
        }

        public override string ToString()
        {
            return Directory.FullName.ToLower().Replace('/', '\\');
        }
    }
}