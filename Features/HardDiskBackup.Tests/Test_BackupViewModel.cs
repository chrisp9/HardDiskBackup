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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemWrapper.IO;

namespace HardDiskBackup.Tests
{
    public class Test_BackupViewModel
    {
        [Test]
        public void Subscription_to_drive_notifier_occurs_on_construction()
        {
            SetupSut();

            _mockDriveNotifier.Verify(x =>
                x.Subscribe(It.IsAny<Func<IDriveInfoWrap, Task>>()), Times.Once());
        }

        [Test]
        public void BackupRootDirectory_is_retrieved_from_factory_when_new_drive_is_observed()
        {
            SetupSut();

            _subscriptionAction(_mockDriveInfoWrap.Object);

            _mockBackupDirectoryFactory.Verify(x => x.GetBackupRootDirectoryForDrive(_mockDriveInfoWrap.Object), Times.Once());
        }

        [Test]
        public void Calculate_total_size_is_called()
        {
            SetupSut();

            _subscriptionAction(_mockDriveInfoWrap.Object);

            _mockBackupFileSystem.Verify(x => x.CalculateTotalSize(_mockDirectoryInfoWrap.Object), Times.Once());
        }

        [Test]
        public async void Copy_is_called()
        {
            SetupSut();

            await _subscriptionAction(_mockDriveInfoWrap.Object);

            _mockBackupFileSystem.Verify(x => x.Copy(_mockDirectoryInfoWrap.Object, "", It.IsAny<Action<IFileInfoWrap>>()), Times.Once());
        }

        [Test]
        public void Bytes_copied_so_far_is_initially_zero()
        {
            SetupSut();

            Assert.AreEqual(0L, _sut.BytesCopiedSoFar);
        }

        [Test]
        public void Progress_bar_is_initially_indeterminate()
        {
            SetupSut();

            Assert.IsTrue(_sut.ProgressBarIsIndeterminate);
        }

        [Test]
        public async void Progress_bar_becomes_determinate()
        {
            SetupSut();

            await _subscriptionAction(_mockDriveInfoWrap.Object);

            Assert.IsFalse(_sut.ProgressBarIsIndeterminate);
        }

        [Test]
        public async void Bytes_copied_so_far_is_updated_after_a_file_is_copied()
        {
            SetupSut();

            await _subscriptionAction(_mockDriveInfoWrap.Object);

            Assert.AreEqual(5L, _sut.BytesCopiedSoFar);
        }

        [Test]
        public async void Bytes_copied_so_far_is_zero_before_copy_occurs()
        {
            SetupSut();

            _mockBackupFileSystem.Setup(x => x.Copy(
                    It.IsAny<IDirectoryInfoWrap>(), 
                    It.IsAny<string>(),
                    It.IsAny<Action<IFileInfoWrap>>()))
                .Callback<IDirectoryInfoWrap, string, Action<IFileInfoWrap>>((x, y, z) => Assert.AreEqual(0, _sut.BytesCopiedSoFar));

            await _subscriptionAction(_mockDriveInfoWrap.Object);

            Assert.AreEqual(0L, _sut.BytesCopiedSoFar);
        }

        [Test]
        public async void Bytes_to_copy_is_updated_after_calculating_total_size()
        {
            SetupSut();

            await _subscriptionAction(_mockDriveInfoWrap.Object);

            Assert.AreEqual(5L, _sut.TotalBytesToCopy);
        }

        [SetUp]
        public void SetUp()
        {
            _mockFileInfoWrap = new Mock<IFileInfoWrap>();
            _mockFileInfoWrap.Setup(x => x.Length).Returns(5L);

            _mockBackupScheduleService = new Mock<IBackupScheduleService>();
            _mockBackupFileSystem = new Mock<IBackupFileSystem2>();
            _mockBackupDirectoryFactory = new Mock<IDirectoryFactory>();
            _mockDriveNotifier = new Mock<IDriveNotifier>();
            _mockDriveInfoWrap = new Mock<IDriveInfoWrap>();
            _mockDirectoryInfoWrap = new Mock<IDirectoryInfoWrap>();

            _backupRootDirectory = new BackupRootDirectory(_mockDirectoryInfoWrap.Object);
            _backupDirectories = new[] { new BackupDirectory(_mockDirectoryInfoWrap.Object) }.ToArray();

            _mockDirectoryInfoWraps = new[] { _mockDirectoryInfoWrap.Object }.ToArray();

            _mockBackupDirectoryFactory
                .Setup(x => x.GetBackupRootDirectoryForDrive(_mockDriveInfoWrap.Object))
                .Returns(_backupRootDirectory);

            _mockDriveNotifier.Setup(x => x.Subscribe(It.IsAny<Func<IDriveInfoWrap, Task>>()))
                .Callback<Func<IDriveInfoWrap, Task>>(x => _subscriptionAction = x);

            _mockBackupFileSystem.Setup(x => x.CalculateTotalSize(It.IsAny<IDirectoryInfoWrap>()))
                .Returns(Task.FromResult(Result<long>.Success(5L)));

            _mockBackupFileSystem.Setup(x => x.Copy(
                    It.IsAny<IDirectoryInfoWrap>(),
                    It.IsAny<string>(), 
                    It.IsAny<Action<IFileInfoWrap>>()))

                .Callback<IEnumerable<IDirectory>, Action<IFileInfoWrap>>((x, y) => y(_mockFileInfoWrap.Object));
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
                BackupDirectoriesAndSchedule.Create(
                _backupDirectories,
                new DailyBackupSchedule(Mock.Of<INextBackupDateTimeFactory>(),
                    Mock.Of<IDateTimeProvider>(),
                    new BackupTime(TimeSpan.Parse("20:00:00")))));
        }

        private BackupViewModel _sut;
        private Func<IDriveInfoWrap, Task> _subscriptionAction;

        private Mock<IFileInfoWrap> _mockFileInfoWrap;
        private BackupRootDirectory _backupRootDirectory;
        private BackupDirectory[] _backupDirectories;
        private Mock<IBackupScheduleService> _mockBackupScheduleService;
        private Mock<IBackupFileSystem2> _mockBackupFileSystem;
        private Mock<IDirectoryFactory> _mockBackupDirectoryFactory;
        private Mock<IDriveNotifier> _mockDriveNotifier;
        private Mock<IDriveInfoWrap> _mockDriveInfoWrap;
        private Mock<IDirectoryInfoWrap> _mockDirectoryInfoWrap;

        private IDirectoryInfoWrap[] _mockDirectoryInfoWraps;
    }
}