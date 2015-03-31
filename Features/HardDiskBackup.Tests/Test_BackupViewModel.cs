using Domain;
using Domain.Scheduling;
using HardDiskBackup.ViewModel;
using Moq;
using NUnit.Framework;
using Services.Disk;
using Services.Disk.FileSystem;
using Services.Factories;
using Services.Scheduling;
using System;
using SystemWrapper.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HardDiskBackup.Tests
{
    public class Test_BackupViewModel
    {

        [Test]
        public void Subscription_to_drive_notifier_occurs_on_construction()
        {
            SetupSut();

            _mockDriveNotifier.Verify(x => 
                x.Subscribe(It.IsAny<Action<IDriveInfoWrap>>()), Times.Once());
        }

        [Test]
        public void BackupRootDirectory_is_retrieved_from_factory_when_new_drive_is_observed()
        {
            Action<IDriveInfoWrap> subscriptionAction = null;
            _mockDriveNotifier.Setup(x => x.Subscribe(It.IsAny<Action<IDriveInfoWrap>>()))
                .Callback<Action<IDriveInfoWrap>>(x => subscriptionAction = x);

            SetupSut();

            subscriptionAction(_mockDriveInfoWrap.Object);

            _mockBackupDirectoryFactory.Verify(x => x.GetBackupRootDirectoryForDrive(_mockDriveInfoWrap.Object), Times.Once());
        }

        [Test]
        public void BackupFileSystem_is_targeted_at_root_directory_when_new_drive_is_observed()
        {
            Action<IDriveInfoWrap> subscriptionAction = null;
            _mockDriveNotifier.Setup(x => x.Subscribe(It.IsAny<Action<IDriveInfoWrap>>()))
                .Callback<Action<IDriveInfoWrap>>(x => subscriptionAction = x);

            SetupSut();

            subscriptionAction(_mockDriveInfoWrap.Object);

            _mockBackupFileSystem.Verify(x => x.Target(_backupRootDirectory), Times.Once());
        }

        [Test]
        public void Calculate_total_size_is_called()
        {
            Action<IDriveInfoWrap> subscriptionAction = null;
            _mockDriveNotifier.Setup(x => x.Subscribe(It.IsAny<Action<IDriveInfoWrap>>()))
                .Callback<Action<IDriveInfoWrap>>(x => subscriptionAction = x);

             SetupSut();

             subscriptionAction(_mockDriveInfoWrap.Object);

            _mockBackupFileSystem.Verify(x => x.CalculateTotalSize(_backupDirectories), Times.Once());
        }

        [Test]
        public void Copy_is_called()
        {
            Action<IDriveInfoWrap> subscriptionAction = null;
            _mockDriveNotifier.Setup(x => x.Subscribe(It.IsAny<Action<IDriveInfoWrap>>()))
                .Callback<Action<IDriveInfoWrap>>(x => subscriptionAction = x);

            SetupSut();

            subscriptionAction(_mockDriveInfoWrap.Object);

            _mockBackupFileSystem.Verify(x => x.Copy(_backupDirectories), Times.Once());
        }

        [SetUp]
        public void SetUp()
        {
            _mockBackupScheduleService = new Mock<IBackupScheduleService>();
            _mockBackupFileSystem = new Mock<IBackupFileSystem>();
            _mockBackupDirectoryFactory = new Mock<IDirectoryFactory>();
            _mockDriveNotifier = new Mock<IDriveNotifier>();
            _mockDriveInfoWrap = new Mock<IDriveInfoWrap>();
            _mockDirectoryInfoWrap = new Mock<IDirectoryInfoWrap>();

            _backupRootDirectory = new BackupRootDirectory(_mockDirectoryInfoWrap.Object);
            _backupDirectories = new[] { new BackupDirectory(_mockDirectoryInfoWrap.Object) }.ToList();

            _mockBackupDirectoryFactory
                .Setup(x => x.GetBackupRootDirectoryForDrive(_mockDriveInfoWrap.Object))
                .Returns(_backupRootDirectory);
        }

        public void SetupSut()
        {
            _sut = new BackupViewModel(
                _mockDriveNotifier.Object,
                _mockBackupScheduleService.Object,
                _mockBackupDirectoryFactory.Object,
                _mockBackupFileSystem.Object);

            // Jesus...
           _mockBackupScheduleService.Setup(x => x.NextBackup).Returns(
               Backup.Create(
               _backupDirectories, 
               new DailyBackupSchedule(Mock.Of<INextBackupDateTimeFactory>(), 
                   Mock.Of<IDateTimeProvider>(), 
                   new BackupTime(TimeSpan.Parse("20:00:00")))));
        }

        private BackupViewModel _sut;
        private BackupRootDirectory _backupRootDirectory;
        private IEnumerable<BackupDirectory> _backupDirectories;
        private Mock<IBackupScheduleService> _mockBackupScheduleService = new Mock<IBackupScheduleService>();
        private Mock<IBackupFileSystem> _mockBackupFileSystem = new Mock<IBackupFileSystem>();
        private Mock<IDirectoryFactory> _mockBackupDirectoryFactory = new Mock<IDirectoryFactory>();
        private Mock<IDriveNotifier> _mockDriveNotifier = new Mock<IDriveNotifier>();
        private Mock<IDriveInfoWrap> _mockDriveInfoWrap = new Mock<IDriveInfoWrap>();
        private Mock<IDirectoryInfoWrap> _mockDirectoryInfoWrap = new Mock<IDirectoryInfoWrap>();
    }
}
