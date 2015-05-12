using Domain;
using System.IO;
using SystemWrapper.IO;

namespace TestHelpers
{
    public class FakeDriveInfoWrap : IDriveInfoWrap
    {
        public FakeDriveInfoWrap(
            long availableFreeSpace,
            string driveFormat,
            DriveType driveType,
            bool isReady,
            string name,
            IDirectoryInfoWrap rootDirectory,
            long totalFreeSpace,
            long totalSize,
            string volumeLabel)
        {
            AvailableFreeSpace = availableFreeSpace;
            DriveFormat = driveFormat;
            DriveType = driveType;
            IsReady = isReady;
            Name = name;
            RootDirectory = rootDirectory;
            TotalFreeSpace = totalFreeSpace;
            TotalSize = totalSize;
            VolumeLabel = volumeLabel;
        }

        public long AvailableFreeSpace
        {
            get;
            private set;
        }

        public string DriveFormat
        {
            get;
            private set;
        }

        public DriveType DriveType
        {
            get;
            private set;
        }

        public bool IsReady
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public IDirectoryInfoWrap RootDirectory
        {
            get;
            private set;
        }

        public long TotalFreeSpace
        {
            get;
            private set;
        }

        public long TotalSize
        {
            get;
            private set;
        }

        public string VolumeLabel
        {
            get;
            set;
        }
    }
}