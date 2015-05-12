using Domain;
using Microsoft.Reactive.Testing;
using Moq;
using NUnit.Framework;
using Services.Disk;
using System;
using System.IO;
using System.Linq;
using SystemWrapper.IO;
using TestHelpers;

namespace Services.Tests
{
    public class Test_ExistingBackupsPoller
    {
        [Test]
        public void Adding_a_disk_containing_backups_causes_onAdded_to_execute()
        {
            // Arrange
            var initialDriveList = Enumerable.Empty<IDriveInfoWrap>();
            var finalDriveList = new[] { _removableDisk };

            _mockDiskService.Setup(x => x.GetDrives()).Returns(initialDriveList);

            var hasBeenCalled = false;
            _sut.Subscribe(_ => hasBeenCalled = true, _ => { });

            _mockDiskService.Setup(x => x.GetDrives()).Returns(finalDriveList);
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(2).Ticks);

            Assert.IsTrue(hasBeenCalled);
        }

        [Test]
        public void Adding_a_disk_which_does_not_contain_backups_means_OnAdded_does_not_execute()
        {
            // Arrange
            var initialDriveList = Enumerable.Empty<IDriveInfoWrap>();
            var finalDriveList = new[] { _removableDisk };

            _mockDiskBackupDirectory.Setup(x => x.FullName).Returns("e:\\unrelatedStuff");
            _mockDiskService.Setup(x => x.GetDrives()).Returns(initialDriveList);

            var hasBeenCalled = false;
            _sut.Subscribe(_ => hasBeenCalled = true, _ => { });

            _mockDiskService.Setup(x => x.GetDrives()).Returns(finalDriveList);
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(2).Ticks);

            Assert.IsFalse(hasBeenCalled);
        }

        [Test]
        public void Removing_a_disk_containing_backups_causes_onRemoved_to_execute()
        {
            // Arrange
            var finalDriveList = Enumerable.Empty<IDriveInfoWrap>();
            var initialDriveList = new[] { _removableDisk };

            _mockDiskBackupDirectory.Setup(x => x.FullName).Returns("e:\\unrelatedStuff");
            _mockDiskService.Setup(x => x.GetDrives()).Returns(initialDriveList);

            var hasBeenCalled = false;
            _sut.Subscribe(_ => { }, _ => hasBeenCalled = true);

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(2).Ticks);

            _mockDiskService.Setup(x => x.GetDrives()).Returns(finalDriveList);

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(2).Ticks);

            Assert.IsFalse(hasBeenCalled);
        }

        [Test]
        public void Removing_a_disk_which_does_not_contain_backups_does_not_cause_onRemoved_to_execute()
        {
            // Arrange
            var finalDriveList = Enumerable.Empty<IDriveInfoWrap>();
            var initialDriveList = new[] { _removableDisk };

            _mockDiskService.Setup(x => x.GetDrives()).Returns(initialDriveList);

            var hasBeenCalled = false;
            _sut.Subscribe(_ => { }, _ => hasBeenCalled = true);

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(2).Ticks);

            _mockDiskService.Setup(x => x.GetDrives()).Returns(finalDriveList);

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(2).Ticks);

            Assert.IsTrue(hasBeenCalled);
        }

        [SetUp]
        public void Setup()
        {
            _testScheduler = new TestScheduler();
            _mockDiskService = new Mock<IDriveInfoService>();

            var mockRootDirectory = new Mock<IDirectoryInfoWrap>();
            mockRootDirectory.Setup(x => x.FullName).Returns("e:\\");

            _mockDiskBackupDirectory = new Mock<IDirectoryInfoWrap>();
            _mockDiskBackupDirectory.Setup(x => x.FullName).Returns("e:\\DiskBackupApp");

            mockRootDirectory.Setup(x => x.GetDirectories()).Returns(new[] { _mockDiskBackupDirectory.Object });

            var builder = new FakeDriveInfoBuilder();
            _removableDisk = builder.WithDriveType(DriveType.Removable)
                .WithRootDirectory(mockRootDirectory.Object).Build();

            _sut = new ExistingBackupsPoller(_testScheduler, _mockDiskService.Object);
        }

        private ExistingBackupsPoller _sut;
        private Mock<IDirectoryInfoWrap> _mockDiskBackupDirectory;

        private TestScheduler _testScheduler;
        private Mock<IDriveInfoService> _mockDiskService;
        private IDriveInfoWrap _removableDisk;
    }
}