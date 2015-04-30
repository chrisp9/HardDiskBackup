using Domain;
using HardDiskBackup.ViewModel;
using Moq;
using NUnit.Framework;
using Services.Disk;
using Services.Factories;
using System;
using System.Collections;
using System.Collections.Generic;
using SystemWrapper.IO;
using System.Linq;
using System.Threading.Tasks;
using Services.Disk.FileSystem;
using HardDiskBackup.Commands;

namespace HardDiskBackup.Tests
{
    public class Test_ManageBackupsViewModel
    {
        [Test]
        public void ViewModel_subscribes_to_poller_on_construction()
        {
            _existingBackupsPoller.Verify(x => x.Subscribe(
                It.IsAny<Action<BackupRootDirectory>>(), It.IsAny<Action<BackupRootDirectory>>()), Times.Once());
        }

        [Test]
        public void DeviceWithBackupsExists_is_initially_false()
        {
            Assert.IsFalse(_sut.DeviceWithBackupsExists);
        }

        [Test]
        public void DeviceWithBackupsExists_is_false_after_adding_then_removing_a_device()
        {
            _onAddCallback(_backupRootDirectory);
            _onRemoveCallback(_backupRootDirectory);

            Assert.IsFalse(_sut.DeviceWithBackupsExists);
        }

        [Test]
        public void DeviceWithBackupsExists_is_set_to_true_when_a_BackupRootDirectory_turns_up()
        {
            _onAddCallback(_backupRootDirectory);

            Assert.IsTrue(_sut.DeviceWithBackupsExists);
        }

        [SetUp]
        public void Setup()
        {
            _existingBackupsPoller = new Mock<IExistingBackupsPoller>();
            
            _existingBackupsPoller.Setup(x => x.Subscribe(It.IsAny<Action<BackupRootDirectory>>(), It.IsAny<Action<BackupRootDirectory>>()))
                .Callback<Action<BackupRootDirectory>, Action<BackupRootDirectory>>((x, y) => { _onAddCallback = x; _onRemoveCallback = y; });

            _mockDirectory = new Mock<IDirectoryInfoWrap>();
            _mockDirectory.Setup(x => x.FullName).Returns(@"e:\test");

            _existingBackupsFactory = new Mock<IExistingBackupsFactory>();
            _backupRootDirectory = new BackupRootDirectory(_mockDirectory.Object);
            _mockExistingBackupsModel = new Mock<IExistingBackupsModel>();
            _mockDeleteBackupCommand = new Mock<IDeleteBackupCommand>();

            _existingBackups = CreateExistingBackups().ToArray();

            _existingBackupsFactory.Setup(x => x.Create(_backupRootDirectory))
                .Returns(Task.FromResult(_existingBackups));

            _sut = new ManageBackupsViewModel(
                _existingBackupsPoller.Object, 
                _existingBackupsFactory.Object, 
                _mockDeleteBackupCommand.Object, 
                _mockExistingBackupsModel.Object);
        }

        private IEnumerable<ExistingBackup> CreateExistingBackups()
        {
            var directory = new Mock<IDirectoryInfoWrap>();
            directory.Setup(x => x.FullName).Returns(@"c:\test");

            var directory2 = new Mock<IDirectoryInfoWrap>();
            directory2.Setup(x => x.FullName).Returns(@"c:\test2");

            var first = new ExistingBackup(
                new BackupDate(2015, 05, 01),
                new BackupTime(20, 00, 00),
                new TimestampedBackupRoot(directory.Object),
                200L);

            var second = new ExistingBackup(
                new BackupDate(2014, 01, 04),
                new BackupTime(00, 00, 01),
                new TimestampedBackupRoot(directory2.Object),
                300L
                );

            return new[] { first, second };
        }

        private ExistingBackup[] _existingBackups;

        private ManageBackupsViewModel _sut;
        private Mock<IExistingBackupsPoller> _existingBackupsPoller;
        private Mock<IExistingBackupsFactory> _existingBackupsFactory;
        private Mock<IExistingBackupsModel> _mockExistingBackupsModel;
        private Mock<IDeleteBackupCommand> _mockDeleteBackupCommand;

        private Action<BackupRootDirectory> _onAddCallback;
        private Action<BackupRootDirectory> _onRemoveCallback;

        private Mock<IDirectoryInfoWrap> _mockDirectory;
        private BackupRootDirectory _backupRootDirectory;
    }
}