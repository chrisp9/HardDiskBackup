using Domain;
using HardDiskBackup.Commands;
using HardDiskBackup.View;
using HardDiskBackup.ViewModel;
using Moq;
using NUnit.Framework;
using Services;
using Services.Scheduling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper.IO;

namespace HardDiskBackup.Tests
{
    public class Test_ScheduleBackupCommand
    {
        private ScheduleBackupCommand _sut;
        private Mock<ISetScheduleModel> _mockSetScheduleModel;
        private Mock<IBackupScheduleService> _mockBackupScheduleService;
        private Mock<IBackupDirectoryModel> _mockBackupDirectoryModel;

        [SetUp]
        public void Setup()
        {
            _mockSetScheduleModel = new Mock<ISetScheduleModel>();
            _mockBackupScheduleService = new Mock<IBackupScheduleService>();
            _mockBackupDirectoryModel = new Mock<IBackupDirectoryModel>();

            _sut = new ScheduleBackupCommand(
                _mockSetScheduleModel.Object,
                _mockBackupScheduleService.Object,
                _mockBackupDirectoryModel.Object,
                Mock.Of<IWindowPresenter<BackupViewModel, IBackupView>>(),
                Mock.Of<IDispatcher>());
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


        public void SetIsScheduleValid(bool value)
        {
            _mockSetScheduleModel
                .Setup(x => x.IsScheduleValid())
                .Returns(value);
        }

        private void SetHasDirectories(bool value)
        {
            var directories = value 
                ? new ReadOnlyCollection<BackupDirectory>(new[] { new BackupDirectory(Mock.Of<IDirectoryInfoWrap>())}.ToList())
                : new ReadOnlyCollection<BackupDirectory>(Enumerable.Empty<BackupDirectory>().ToList());

            _mockBackupDirectoryModel
                .Setup(x => x.BackupDirectories)
                .Returns(directories);
        }
    }
}
