using Domain;
using Microsoft.Reactive.Testing;
using Moq;
using NUnit.Framework;
using Services.Disk;
using System;
using System.IO;
using System.Linq;
using TestHelpers;

namespace Services.Tests
{
    public class Test_DiskNotifier
    {
        // These tests do not check that the service only returns removable disks
        private IDriveInfoWrap _fixedDisk;

        private IDriveInfoWrap _removableDisk;
        private Mock<IDriveInfoService> _mockDiskService;
        private TestScheduler _testScheduler;

        [Test]
        public void If_a_disk_is_added_after_subscribing_then_this_is_detected()
        {
            // Arrange
            var sut = SetupSut();

            var initialDriveList = new[] { _fixedDisk };
            var finalDriveList = new[] { _fixedDisk, _removableDisk };

            _mockDiskService.Setup(x => x.GetDrives()).Returns(initialDriveList);

            IDriveInfoWrap result = null;
            sut.Subscribe((driveInfo) => { result = driveInfo; return null; });

            _mockDiskService.Setup(x => x.GetDrives()).Returns(finalDriveList);

            // Act
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(2).Ticks);

            // Assert
            Assert.AreEqual(_removableDisk, result);
        }

        [Test]
        public void If_a_disk_is_added_after_unsubscribing_then_this_is_not_detected()
        {
            // Arrange
            var sut = SetupSut();

            var initialDriveList = new[] { _fixedDisk };
            var finalDriveList = new[] { _fixedDisk, _removableDisk };

            _mockDiskService.Setup(x => x.GetDrives()).Returns(initialDriveList);

            IDriveInfoWrap result = null;
            sut.Subscribe((driveInfo) => { result = driveInfo; return null; });
            sut.Unsubscribe();

            _mockDiskService.Setup(x => x.GetDrives()).Returns(finalDriveList);

            // Act
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(2).Ticks);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void If_a_disk_exists_before_subscribing_then_this_is_not_detected()
        {
            // Arrange
            var finalDriveList = new[] { _fixedDisk, _removableDisk };

            var sut = SetupSut();
            _mockDiskService.Setup(x => x.GetDrives()).Returns(finalDriveList);

            IDriveInfoWrap result = null;
            sut.Subscribe((driveInfo) => { result = driveInfo; return null; });

            // Act
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(2).Ticks);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void If_no_disks_are_returned_by_the_diskService_then_no_detection_occurs()
        {
            // Arrange
            var driveList = Enumerable.Empty<IDriveInfoWrap>();

            var sut = SetupSut();

            IDriveInfoWrap result = null;
            sut.Subscribe((driveInfo) => { result = driveInfo; return null; });

            // Act
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(2).Ticks);

            // Assert
            Assert.IsNull(result);
        }

        public DriveNotifier SetupSut()
        {
            _testScheduler = new TestScheduler();
            _mockDiskService = new Mock<IDriveInfoService>();
            var _builder = new FakeDriveInfoBuilder();
            _fixedDisk = _builder.WithDriveType(DriveType.Fixed).Build();
            _removableDisk = _builder.WithDriveType(DriveType.Removable).Build();

            return new DriveNotifier(_testScheduler, _mockDiskService.Object);
        }
    }
}