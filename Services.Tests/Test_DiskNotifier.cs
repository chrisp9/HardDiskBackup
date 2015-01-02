using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Microsoft.Reactive.Testing;
using Services.Disk;
using Domain;
using TestHelpers;
using System.IO;

namespace Services.Tests
{
    public class Test_DiskNotifier
    {
        private TestScheduler _testScheduler;

        // These tests do not check that the service only returns removable disks

        [Test]
        public void If_a_disk_is_added_after_subscribing_then_this_is_detected()
        {
            // Arrange
            var builder = new FakeDriveInfoBuilder();
            IDriveInfoWrap fixedDisk = builder.WithDriveType(DriveType.Fixed).Build();
            IDriveInfoWrap removableDisk = builder.WithDriveType(DriveType.Removable).Build();

            var initialDriveList = new[] { fixedDisk };
            var finalDriveList = new[] { fixedDisk, removableDisk };

            Mock<IDiskService> diskService = CreateMockDiskService(initialDriveList);

            _testScheduler = new TestScheduler();
            var sut = new DriveNotifier(_testScheduler, diskService.Object);

            IDriveInfoWrap result = null;
            sut.Subscribe((driveInfo) => { result = driveInfo; });

            diskService.Setup(x => x.GetDrives()).Returns(finalDriveList);

            // Act
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(2).Ticks);

            // Assert
            Assert.AreEqual(removableDisk, result);
        }

        [Test]
        public void If_a_disk_exists_before_subscribing_then_this_is_not_detected()
        {
            // Arrange
            var builder = new FakeDriveInfoBuilder();
            IDriveInfoWrap fixedDisk = builder.WithDriveType(DriveType.Fixed).Build();
            IDriveInfoWrap removableDisk = builder.WithDriveType(DriveType.Removable).Build();

            var finalDriveList = new[] { fixedDisk, removableDisk };

            Mock<IDiskService> diskService = CreateMockDiskService(finalDriveList);

            var scheduler = new TestScheduler();
            var sut = new DriveNotifier(scheduler, diskService.Object);

            IDriveInfoWrap result = null;
            sut.Subscribe((driveInfo) => { result = driveInfo; });

            // Act
            scheduler.AdvanceBy(TimeSpan.FromSeconds(2).Ticks);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void If_no_disks_are_returned_by_the_diskService_then_no_detection_occurs()
        {
            // Arrange
            var builder = new FakeDriveInfoBuilder();

            var driveList = Enumerable.Empty<IDriveInfoWrap>();
            Mock<IDiskService> diskService = CreateMockDiskService(driveList);

            var scheduler = new TestScheduler();
            var sut = new DriveNotifier(scheduler, diskService.Object);

            IDriveInfoWrap result = null;
            sut.Subscribe((driveInfo) => { result = driveInfo; });

            // Act
            scheduler.AdvanceBy(TimeSpan.FromSeconds(2).Ticks);

            // Assert
            Assert.IsNull(result);
        }

        private Mock<IDiskService> CreateMockDiskService(IEnumerable<IDriveInfoWrap> drives) 
        {
            var mockDiskService = new Mock<IDiskService>();
            mockDiskService.Setup(x => x.GetDrives()).Returns(drives);

            return mockDiskService;
        }
    }
}
