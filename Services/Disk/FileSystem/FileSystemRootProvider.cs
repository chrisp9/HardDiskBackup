using Domain;
using Registrar;
using System.IO;

namespace Services.Disk.FileSystem
{
    public interface IFileSystemRootProvider
    {
        string GetFileSystemRoot(IDriveInfoWrap driveInfoWrap);
    }

    [Register(LifeTime.Transient)]
    public class FileSystemRootProvider : IFileSystemRootProvider
    {
        public string GetFileSystemRoot(IDriveInfoWrap driveInfoWrap)
        {
            var root =
                driveInfoWrap
                .RootDirectory
                .FullName;

            return Path.Combine(root, Constants.DiskBackupApp).ToString();
        }
    }
}