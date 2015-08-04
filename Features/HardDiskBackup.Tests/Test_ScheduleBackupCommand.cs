using Domain;
using GalaSoft.MvvmLight.Messaging;
using HardDiskBackup.Commands;
using HardDiskBackup.View;
using HardDiskBackup.ViewModel;
using Moq;
using NUnit.Framework;
using Services;
using Services.Persistence;
using Services.Scheduling;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using SystemWrapper.IO;

namespace HardDiskBackup.Tests
{
    public class Test_ScheduleBackupCommand
    {
        private ScheduleBackupCommand _sut;
        private Mock<ISetScheduleModel> _mockSetScheduleModel;
        private Mock<IBackupScheduleService> _mockBackupScheduleService;
        private Mock<IBackupDirectoryModel> _mockBackupDirectoryModel;
        private Mock<IDispatcher> _mockDispatcher;
        private Mock<IWindowPresenter<BackupViewModel, IBackupView>> _mockPresenter;
        private Mock<IJsonSerializer> _mockJsonSerializer;
        private Messenger _messenger;

        [SetUp]
        public void Setup()
        {
            _mockSetScheduleModel = new Mock<ISetScheduleModel>();
            _mockBackupScheduleService = new Mock<IBackupScheduleService>();
            _mockBackupDirectoryModel = new Mock<IBackupDirectoryModel>();
            _mockDispatcher = new Mock<IDispatcher>();
            _mockPresenter = new Mock<IWindowPresenter<BackupViewModel, IBackupView>>();
            _mockJsonSerializer = new Mock<IJsonSerializer>();
            _messenger = new Messenger();

            _sut = new ScheduleBackupCommand(
                _mockSetScheduleModel.Object,
                _mockBackupScheduleService.Object,
                _mockBackupDirectoryModel.Object,
                _mockDispatcher.Object,
                _messenger,
                _mockJsonSerializer.Object);
        }

        [Test]
        public void Command_can_execute()
        {
            SetIsScheduleValid(true);
            SetHasDirectories(true);

            Assert.IsTrue(_sut.CanExecute(null));
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void Command_cannot_execute(bool hasDirectories, bool isScheduleValid)
        {
            SetIsScheduleValid(isScheduleValid);
            SetHasDirectories(hasDirectories);

            Assert.IsFalse(_sut.CanExecute(null));
        }

        [Test]
        public void Execute_calls_CreateSchedule()
        {
            // Act
            _sut.Execute(null);

            // Assert
            _mockSetScheduleModel.Verify(x => x.CreateSchedule(), Times.Once());
        }

        [Test]
        public void Execute_gets_directories()
        {
            // Act
            _sut.Execute(null);

            // Assert
            _mockBackupDirectoryModel.VerifyGet(x => x.BackupDirectories, Times.Once());
        }

        [Test]
        public void Message_is_dispatched_when_backup_time_is_reached()
        {
            var messageHasBeenReceived = false;
            _messenger.Register<Messages>(this, m => 
            {
                if (m == Messages.PerformBackup)
                    messageHasBeenReceived = true;
            });

            _mockBackupScheduleService.Setup(x => x.ScheduleNextBackup(
                It.IsAny<BackupDirectoriesAndSchedule>(), It.IsAny<Action>()))
                .Callback<BackupDirectoriesAndSchedule, Action>((b, a) => a());

            _sut.Execute(null);

            Assert.IsTrue(messageHasBeenReceived);
        }

        public void SetIsScheduleValid(bool value)
        {
            _mockSetScheduleModel
                .Setup(x => x.IsScheduleValid())
                .Returns(value);
        }

        private void SetHasDirectories(bool value)
        {
            var directories = value
                ? new ReadOnlyCollection<BackupDirectory>(new[] { new BackupDirectory(Mock.Of<IDirectoryInfoWrap>()) }.ToList())
                : new ReadOnlyCollection<BackupDirectory>(Enumerable.Empty<BackupDirectory>().ToList());

            _mockBackupDirectoryModel
                .Setup(x => x.BackupDirectories)
                .Returns(directories);
        }
    }
}