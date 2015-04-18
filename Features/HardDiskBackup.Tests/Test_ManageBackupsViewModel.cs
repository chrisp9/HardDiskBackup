using Domain;
using HardDiskBackup.ViewModel;
using Moq;
using NUnit.Framework;
using Services.Disk;
using Services.Factories;
using System;
using SystemWrapper.IO;

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
            _existingBackupsFactory = new Mock<IExistingBackupsFactory>();
            _backupRootDirectory = new BackupRootDirectory(_mockDirectory.Object);
            

            _sut = new ManageBackupsViewModel(_existingBackupsPoller.Object, _existingBackupsFactory.Object);
        }

        private ManageBackupsViewModel _sut;
        private Mock<IExistingBackupsPoller> _existingBackupsPoller;
        private Mock<IExistingBackupsFactory> _existingBackupsFactory;

        private Action<BackupRootDirectory> _onAddCallback;
        private Action<BackupRootDirectory> _onRemoveCallback;

        private Mock<IDirectoryInfoWrap> _mockDirectory;
        private BackupRootDirectory _backupRootDirectory;
    }
}