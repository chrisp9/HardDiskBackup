using Domain;
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

        [SetUp]
        public void Setup()
        {
            _mockSetScheduleModel = new Mock<ISetScheduleModel>();
            _mockBackupScheduleService = new Mock<IBackupScheduleService>();
            _mockBackupDirectoryModel = new Mock<IBackupDirectoryModel>();
            _mockDispatcher = new Mock<IDispatcher>();
            _mockPresenter = new Mock<IWindowPresenter<BackupViewModel, IBackupView>>();
            _mockJsonSerializer = new Mock<IJsonSerializer>();

            _sut = new ScheduleBackupCommand(
                _mockSetScheduleModel.Object,
                _mockBackupScheduleService.Object,
                _mockBackupDirectoryModel.Object,
                _mockPresenter.Object,
                _mockDispatcher.Object,
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
        public void Presenter_present_is_scheduled_and_ran_on_the_dispatcher_when_execute_is_called()
        {
            _mockBackupScheduleService.Setup(x => x.ScheduleNextBackup(
                It.IsAny<BackupDirectoriesAndSchedule>(), It.IsAny<Action>()))
                .Callback<BackupDirectoriesAndSchedule, Action>((b, a) => a());

            _mockDispatcher.Setup(x => x.InvokeAsync(It.IsAny<Action>()))
                .Callback<Action>(a => a());

            _sut.Execute(null);

            _mockPresenter.Verify(x => x.Present(), Times.Once());
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