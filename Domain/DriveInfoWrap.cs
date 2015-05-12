using System;
using System.IO;
using SystemWrapper.IO;

namespace Domain
{
    public interface IDriveInfoWrap
    {
        long AvailableFreeSpace { get; }

        string DriveFormat { get; }

        DriveType DriveType { get; }

        bool IsReady { get; }

        string Name { get; }

        IDirectoryInfoWrap RootDirectory { get; }

        long TotalFreeSpace { get; }

        long TotalSize { get; }

        string VolumeLabel { get; set; }
    }

    public class DriveInfoWrap : IDriveInfoWrap, IEquatable<DriveInfoWrap>
    {
        private readonly DriveInfo _driveInfo;

        public DriveInfoWrap(DriveInfo driveInfo)
        {
            if (driveInfo == null)
                throw new ArgumentNullException("DriveInfo");

            _driveInfo = driveInfo;
        }

        public long AvailableFreeSpace
        {
            get
            {
                return _driveInfo.AvailableFreeSpace;
            }
        }

        public string DriveFormat
        {
            get
            {
                return _driveInfo.DriveFormat;
            }
        }

        public DriveType DriveType
        {
            get
            {
                return _driveInfo.DriveType;
            }
        }

        public bool IsReady
        {
            get
            {
                return _driveInfo.IsReady;
            }
        }

        public string Name
        {
            get
            {
                return _driveInfo.Name;
            }
        }

        public IDirectoryInfoWrap RootDirectory
        {
            get
            {
                return new DirectoryInfoWrap(_driveInfo.RootDirectory);
            }
        }

        public long TotalFreeSpace
        {
            get
            {
                return _driveInfo.TotalFreeSpace;
            }
        }

        public long TotalSize
        {
            get
            {
                return _driveInfo.TotalSize;
            }
        }

        public string VolumeLabel
        {
            get
            {
                return _driveInfo.VolumeLabel;
            }

            set
            {
                _driveInfo.VolumeLabel = value;
            }
        }

        public bool Equals(DriveInfoWrap other)
        {
            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return AvailableFreeSpace.GetHashCode()
                    ^ DriveFormat.GetHashCode()
                    ^ DriveType.GetHashCode()
                    ^ IsReady.GetHashCode()
                    ^ Name.GetHashCode()
                    ^ TotalFreeSpace.GetHashCode()
                    ^ TotalSize.GetHashCode()
                    ^ VolumeLabel.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as DriveInfoWrap;
            if (other == null) return false;

            return AvailableFreeSpace == other.AvailableFreeSpace
                && DriveFormat == other.DriveFormat
                && DriveType == other.DriveType
                && IsReady == other.IsReady
                && Name == other.Name
                && TotalFreeSpace == other.TotalFreeSpace
                && TotalSize == other.TotalSize
                && VolumeLabel == other.VolumeLabel;
        }
    }
}