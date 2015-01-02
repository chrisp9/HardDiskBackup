using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper.IO;
using Moq;
using Domain;

namespace TestHelpers
{
    public class FakeDriveInfoBuilder
    {
        private long _availableFreeSpace = 12000000;
        private string _driveFormat = "Format";
        private DriveType _driveType = DriveType.Removable;
        private bool _isReady = true;
        private string _name = "TestDisk";
        private IDirectoryInfoWrap _rootDirectory = Mock.Of<IDirectoryInfoWrap>();
        private long _totalFreeSpace = 13000000;
        private long _totalSize = 300000000;
        private string _volumeLabel = "TestLabel";

        public FakeDriveInfoBuilder WithAvailableFreeSpace(long space)
        {
            _availableFreeSpace = space;
            return this;
        }

        public FakeDriveInfoBuilder WithDriveFormat(string format)
        {
            _driveFormat = format;
            return this;
        }

        public FakeDriveInfoBuilder WithDriveType(DriveType driveType)
        {
            _driveType = driveType;
            return this;
        }

        public FakeDriveInfoBuilder WithIsReady(bool isReady)
        {
            _isReady = isReady;
            return this;
        }

        public FakeDriveInfoBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public FakeDriveInfoBuilder WithRootDirectory(IDirectoryInfoWrap directoryInfo)
        {
            _rootDirectory = directoryInfo;
            return this;
        }

        public FakeDriveInfoBuilder WithTotalFreeSpace(long space)
        {
            _totalFreeSpace = space;
            return this;
        }

        public FakeDriveInfoBuilder WithTotalSize(long size)
        {
            _totalSize = size;
            return this;
        }

        public FakeDriveInfoBuilder WithVolumeLabel(string volumeLabel)
        {
            _volumeLabel = volumeLabel;
            return this;
        }

        public IDriveInfoWrap Build()
        {
            return new FakeDriveInfoWrap(
                _availableFreeSpace, 
                _driveFormat, 
                _driveType, 
                _isReady, 
                _name, 
                _rootDirectory, 
                _totalFreeSpace, 
                _totalSize, 
                _volumeLabel);
        }



    }
}
