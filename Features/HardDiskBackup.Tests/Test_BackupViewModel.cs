using Domain;
using Domain.Scheduling;
using HardDiskBackup.ViewModel;
using Moq;
using NUnit.Framework;
using Services;
using Services.Disk;
using Services.Disk.FileSystem;
using Services.Factories;
using Services.Scheduling;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
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

            _mockBackupFileSystem.Verify(x => x.Copy(
                _mockDirectoryInfoWrap.Object, 
                _mirroredDirectoryName, 
                It.IsAny<Action<IFileInfoWrap>>()), Times.Once());
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
                .Callback<IDirectoryInfoWrap, string, Action<IFileInfoWrap>>((x, y, z) => Assert.AreEqual(0, _sut.BytesCopiedSoFar))
                .Returns(Task.FromResult(Result.Success()));

            await _subscriptionAction(_mockDriveInfoWrap.Object);

            Assert.AreEqual(0L, _sut.BytesCopiedSoFar);
        }

        [Test]
        public async void We_are_in_error_state_if_copy_reports_errors()
        {
            SetupSut();

            _mockBackupFileSystem.Setup(x => x.Copy(
                    It.IsAny<IDirectoryInfoWrap>(),
                    It.IsAny<string>(),
                    It.IsAny<Action<IFileInfoWrap>>()))
                .Callback<IDirectoryInfoWrap, string, Action<IFileInfoWrap>>((x, y, z) => Assert.AreEqual(0, _sut.BytesCopiedSoFar))
                .Returns(Task.FromResult(Result.Fail(new Exception())));

            await _subscriptionAction(_mockDriveInfoWrap.Object);

            Assert.IsTrue(_sut.HasErrors);
            Assert.AreEqual("Completed with errors", _sut.Status);
            Assert.AreEqual(Colors.Red.R, _sut.LabelColor.Color.R);
            Assert.AreEqual(Colors.Red.G, _sut.LabelColor.Color.G);
            Assert.AreEqual(Colors.Red.B, _sut.LabelColor.Color.B);
        }

        public async void Status_is_initially_waiting()
        {
            SetupSut();

            Assert.AreEqual(true, _sut.ProgressBarIsIndeterminate);
            Assert.AreEqual("Waiting for backup device to be plugged in...", _sut.Status);

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
            _mockBackupFileSystem = new Mock<IBackupFileSystem>();
            _mockBackupDirectoryFactory = new Mock<IDirectoryFactory>();
            _mockDriveNotifier = new Mock<IDriveNotifier>();
            _mockDriveInfoWrap = new Mock<IDriveInfoWrap>();
            _mockDirectoryInfoWrap = new Mock<IDirectoryInfoWrap>();

            _mockDirectoryInfoWrap.Setup(x => x.FullName).Returns(_directoryName);

            _backupRootDirectory = new BackupRootDirectory(_mockDirectoryInfoWrap.Object);
            _mockDirectoryInfoWrap.Setup(x => x.FullName).Returns(_directoryName);

            _backupDirectories = new[] { new BackupDirectory(_mockDirectoryInfoWrap.Object) }.ToArray();

            _mockDirectoryInfoWraps = new[] { _mockDirectoryInfoWrap.Object }.ToArray();
            _mockTimestampedDirectory = new Mock<IDirectoryInfoWrap>();

            _timestampedBackupRoot = new TimestampedBackupRoot(_mockTimestampedDirectory.Object);
            _mockTimestampedBackupRootProvider = new Mock<ITimestampedBackupRootProvider>();

            _mockTimestampedBackupRootProvider.Setup(x => x.CreateTimestampedBackup(It.IsAny<BackupRootDirectory>()))
                .Returns(_timestampedBackupRoot);

            _mockTimestampedDirectory.Setup(x => x.FullName).Returns(_mirroredDirectoryName);

            _mockResultFormatter = new Mock<IResultFormatter>();

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

                .Callback<IDirectoryInfoWrap, string, Action<IFileInfoWrap>>((x, y, z) => z(_mockFileInfoWrap.Object))
                .Returns(Task.FromResult(Result.Success()));
        }

        public void SetupSut()
        {
            _sut = new BackupViewModel(
                _mockDriveNotifier.Object,
                _mockBackupScheduleService.Object,
                _mockBackupDirectoryFactory.Object,
                _mockBackupFileSystem.Object,
                _mockTimestampedBackupRootProvider.Object, 
                _mockResultFormatter.Object);

            _mirroredDirectoryInfoWrap = new Mock<IDirectoryInfoWrap>();
            _mirroredDirectoryInfoWrap.Setup(x => x.FullName).Returns(_mirroredDirectoryName);

            // Jesus...
            _mockBackupScheduleService.Setup(x => x.NextBackup).Returns(
                BackupDirectoriesAndSchedule.Create(
                _backupDirectories,
                new DailyBackupSchedule(Mock.Of<INextBackupDateTimeFactory>(),
                    Mock.Of<IDateTimeProvider>(),
                    new BackupTime(TimeSpan.Parse("20:00:00")))));
        }

        private Mock<ITimestampedBackupRootProvider> _mockTimestampedBackupRootProvider;
        private Mock<IDirectoryInfoWrap> _mirroredDirectoryInfoWrap;
        private Mock<IResultFormatter> _mockResultFormatter;
        
        private BackupViewModel _sut;
        private Func<IDriveInfoWrap, Task> _subscriptionAction;
        private string _directoryName = @"c:\testDir";
        private string _mirroredDirectoryName = @"e:\backupApp\c\testDir";

        private Mock<IFileInfoWrap> _mockFileInfoWrap;
        private BackupRootDirectory _backupRootDirectory;
        private BackupDirectory[] _backupDirectories;
        private Mock<IBackupScheduleService> _mockBackupScheduleService;
        private Mock<IBackupFileSystem> _mockBackupFileSystem;
        private Mock<IDirectoryFactory> _mockBackupDirectoryFactory;
        private Mock<IDriveNotifier> _mockDriveNotifier;
        private Mock<IDriveInfoWrap> _mockDriveInfoWrap;
        private Mock<IDirectoryInfoWrap> _mockDirectoryInfoWrap;

        private TimestampedBackupRoot _timestampedBackupRoot;
        private Mock<IDirectoryInfoWrap> _mockTimestampedDirectory;

        private IDirectoryInfoWrap[] _mockDirectoryInfoWraps;
    }
}