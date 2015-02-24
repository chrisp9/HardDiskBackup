using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HardDiskBackup;
using Domain;
using Services;
using Services.Disk;
using Moq;
using NUnit.Framework;
using SystemWrapper.IO;

namespace HardDiskBackup.Tests
{
    public class Test_FirstRunViewModel
    {
        private FirstRunViewModel _sut;
        private BackupDirectory _backupDirectory;

        [SetUp]
        public void Setup()
        {
            // Arrange
            _backupDirectory = new BackupDirectory(Mock.Of<IDirectoryInfoWrap>());

            _sut = SetupSut(
                backupDirectoryValidator: SetupValidator(true),
                backupService: SetupService(_backupDirectory));
        }

        [Test]
        public void AddPathCommand_cannot_execute_when_path_is_invalid()
        {
            // Arrange
            var sut = SetupSut(backupDirectoryValidator: SetupValidator(false));

            // Assert
            Assert.IsFalse(sut.AddPathCommand.CanExecute(null));
        }

        [Test]
        public void AddPathCommand_can_execute_when_path_is_valid()
        {
            // Arrange
            var sut = SetupSut(backupDirectoryValidator: SetupValidator(true));

            Assert.IsTrue(sut.AddPathCommand.CanExecute(null));
        }

        [Test]
        public void Invalid_path_error_is_given_for_invalid_paths()
        {
            // Arrange
            var sut = SetupSut(backupDirectoryValidator: SetupValidator(false));

            var errors = sut["DirectoryPath"];

            // Assert
            Assert.AreEqual("This path is not valid", errors);
        }

        [Test]
        public void No_error_is_given_for_valid_paths()
        {
            // Arrange
            var sut = SetupSut(backupDirectoryValidator: SetupValidator(true));

            var errors = sut["DirectoryPath"];

            // Assert
            Assert.IsNull(errors);
        }

        [Test]
        public void A_new_BackupDirectory_is_added_when_AddPathCommand_is_executed()
        {            
            // Act
            _sut.AddPathCommand.Execute(null);

            // Assert
            Assert.Contains(_backupDirectory, _sut.BackupDirectories.ToArray());
        }

        [Test]
        public void Removing_a_BackupDirectory_which_does_not_exist()
        {
            // Act
            _sut.RemovePathCommand.Execute(_backupDirectory);

            // Assert
            Assert.AreEqual(0, _sut.BackupDirectories.Count);
        }

        [Test]
        public void Adding_then_removing_a_BackupDirectory()
        {
            // Act
            _sut.AddPathCommand.Execute(_backupDirectory);
            _sut.RemovePathCommand.Execute(_backupDirectory);

            // Assert
            Assert.AreEqual(0, _sut.BackupDirectories.Count);
        }

        [Test]
        public void Adding_multiple_BackupDirectories_then_removing_one()
        {
            // Act
            _sut.AddPathCommand.Execute(_backupDirectory);
            _sut.AddPathCommand.Execute(new BackupDirectory(_backupDirectory.Directory));
            _sut.RemovePathCommand.Execute(_backupDirectory);

            // Assert
            Assert.AreEqual(1, _sut.BackupDirectories.Count);
        }

        [Test]
        public void BackupDirectories_is_initially_empty()
        {
            // Arrange
            var sut = SetupSut();

            // Assert
            Assert.AreEqual(0, sut.BackupDirectories.Count);
        }

        private IBackupDirectoryValidator SetupValidator(bool isValid)
        {
            var mockBackupDirectoryValidator = new Mock<IBackupDirectoryValidator>();
            
            mockBackupDirectoryValidator
                .Setup(x => x.IsValidDirectory(It.IsAny<string>()))
                .Returns(isValid);

            return mockBackupDirectoryValidator.Object;
        }

        private IBackupDirectoryService SetupService(BackupDirectory backupDirectory)
        {
            var mockBackupDirectoryService = new Mock<IBackupDirectoryService>();

            mockBackupDirectoryService
                .Setup(x => x.GetDirectoryFor(It.IsAny<string>()))
                .Returns(backupDirectory);

            return mockBackupDirectoryService.Object;
        }

        private FirstRunViewModel SetupSut(
            IDateTimeProvider dateTimeProvider = null,
            IPersistedOptions persistedOptions = null,
            IBackupDirectoryService backupService = null,
            IBackupDirectoryValidator backupDirectoryValidator = null
            )
        {
            return new FirstRunViewModel(
                dateTimeProvider ?? Mock.Of<IDateTimeProvider>(),
                persistedOptions ?? Mock.Of<IPersistedOptions>(),
                backupService ?? Mock.Of<IBackupDirectoryService>(),
                backupDirectoryValidator ?? Mock.Of<IBackupDirectoryValidator>()
                );
        }
    }
}
