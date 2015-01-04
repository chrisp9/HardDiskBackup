using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Services.Disk;
using Services;
using Moq;
using TestHelpers;
using System.IO;
using Domain;
using Queries;

namespace Services.Tests
{
    public class Test_DiskService
    {
        [Test]
        public void Ready_removable_disks_are_returned_by_the_service()
        {
            // Arrange
            var builder = new FakeDriveInfoBuilder();

            var removableDisk = builder.WithDriveType(DriveType.Removable).Build();
            var fixedDisk = builder.WithDriveType(DriveType.Fixed).Build();
            
            var driveInfos = new[] 
            {
                removableDisk,
                fixedDisk
            };

            IDriveInfoService sut = SetupSut(driveInfos);

            // Act
            var result = sut.GetDrives().Single();

            // Assert
            Assert.AreEqual(removableDisk, result);
        }

        [Test]
        public void Non_ready_removable_disks_are_not_returned_by_the_service()
        {
            // Arrange
            var builder = new FakeDriveInfoBuilder();

            var removableDisk = builder.WithDriveType(DriveType.Removable)
                .WithIsReady(false)
                .Build();

            var fixedDisk = builder.WithDriveType(DriveType.Fixed).Build();

            var driveInfos = new[] 
            {
                removableDisk,
                fixedDisk
            };

            IDriveInfoService sut = SetupSut(driveInfos);

            // Act
            var result = sut.GetDrives();

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        [TestCase(DriveType.CDRom)]
        [TestCase(DriveType.Fixed)]
        [TestCase(DriveType.Network)]
        [TestCase(DriveType.NoRootDirectory)]
        [TestCase(DriveType.Ram)]
        [TestCase(DriveType.Unknown)]
        public void Non_Removable_disks_are_not_returned_by_the_service(DriveType driveType)
        {
            // Arrange
            var builder = new FakeDriveInfoBuilder();

            var removableDisk = builder.WithDriveType(DriveType.Fixed).Build();
            var otherDriveType = builder.WithDriveType(driveType).Build();

            var driveInfos = new[] 
            {
                removableDisk,
                otherDriveType
            };

            IDriveInfoService sut = SetupSut(driveInfos);

            // Act
            var result = sut.GetDrives();

            // Assert
            Assert.IsEmpty(result);
        }

        private IDriveInfoService SetupSut(IEnumerable<IDriveInfoWrap> driveInfos)
        {
            var mockDriveInfo = new Mock<IDriveInfoQuery>();

            mockDriveInfo
                .Setup(x => x.GetDrives())
                .Returns(driveInfos);

            return new DriveInfoService(mockDriveInfo.Object);
        }

    }
}
