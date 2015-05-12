using Newtonsoft.Json;
using System;
using SystemWrapper.IO;

namespace Domain
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BackupDirectory : IDirectory
    {
        [JsonProperty]
        public IDirectoryInfoWrap Directory { get; private set; }

        public BackupDirectory(IDirectoryInfoWrap directory)
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