﻿using System;
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
using Services.Factories;

namespace HardDiskBackup.Tests
{
    public class Test_FirstRunViewModel
    {
        private FirstRunViewModel _sut;
        private BackupDirectory _backupDirectory;
        private Mock<IBackupDirectoryModel> _mockBackupDirectoryModel;

        [SetUp]
        public void Setup()
        {
            // Arrange
            _backupDirectory = new BackupDirectory(Mock.Of<IDirectoryInfoWrap>());
            _mockBackupDirectoryModel = new Mock<IBackupDirectoryModel>();

            _sut = SetupSut(
                backupDirectoryValidator: SetupValidator(ValidationResult.Success),
                backupDirectoryFactory: SetupFactory(_backupDirectory),
                backupDirectoryModel: _mockBackupDirectoryModel.Object);
        }

        [TestCase(ValidationResult.InvalidPath)]
        [TestCase(ValidationResult.PathAlreadyExists)]
        public void AddPathCommand_cannot_execute_when_path_is_invalid(ValidationResult validationResult)
        {
            // Arrange
            var sut = SetupSut(backupDirectoryValidator: SetupValidator(validationResult));

            // Assert
            Assert.IsFalse(sut.AddPathCommand.CanExecute(null));
        }

        [Test]
        public void AddPathCommand_can_execute_when_path_is_valid()
        {
            // Arrange
            var sut = SetupSut(backupDirectoryValidator: SetupValidator(ValidationResult.Success));

            Assert.IsTrue(sut.AddPathCommand.CanExecute(null));
        }

        [Test]
        public void Invalid_path_error_is_given_for_invalid_paths()
        {
            // Arrange
            var sut = SetupSut(backupDirectoryValidator: SetupValidator(ValidationResult.InvalidPath));

            var errors = sut["DirectoryPath"];

            // Assert
            Assert.AreEqual("This path is not valid", errors);
        }

        [Test]
        public void No_error_is_given_for_valid_paths()
        {
            // Arrange
            var sut = SetupSut(backupDirectoryValidator: SetupValidator(ValidationResult.Success));

            var errors = sut["DirectoryPath"];

            // Assert
            Assert.IsNull(errors);
        }

        [Test]
        public void A_new_BackupDirectory_is_added_when_AddPathCommand_is_executed()
        {            
            // Act
            _sut.AddPathCommand.Execute(_backupDirectory);

            // Assert
            _mockBackupDirectoryModel.Verify(x => x.Add(_backupDirectory), Times.Once());
        }

        [Test]
        public void Removing_a_BackupDirectory()
        {
            // Act
            _sut.RemovePathCommand.Execute(_backupDirectory);

            // Assert
            _mockBackupDirectoryModel.Verify(x => x.Remove(_backupDirectory), Times.Once());
        }

        private IBackupDirectoryValidator SetupValidator(ValidationResult validationResult)
        {
            var mockBackupDirectoryValidator = new Mock<IBackupDirectoryValidator>();
            
            mockBackupDirectoryValidator
                .Setup(x => x.CanAdd(It.IsAny<string>()))
                .Returns(validationResult);

            return mockBackupDirectoryValidator.Object;
        }

        private IBackupDirectoryFactory SetupFactory(BackupDirectory backupDirectory)
        {
            var mockBackupDirectoryFactory = new Mock<IBackupDirectoryFactory>();

            mockBackupDirectoryFactory
                .Setup(x => x.Create(It.IsAny<string>()))
                .Returns(backupDirectory);

            return mockBackupDirectoryFactory.Object;
        }

        private FirstRunViewModel SetupSut(
            IDateTimeProvider dateTimeProvider = null,
            IPersistedOptions persistedOptions = null,
            IBackupDirectoryValidator backupDirectoryValidator = null,
            IBackupDirectoryFactory backupDirectoryFactory = null,
            IBackupDirectoryModel backupDirectoryModel = null
            )
        {
            return new FirstRunViewModel(
                dateTimeProvider ?? Mock.Of<IDateTimeProvider>(),
                persistedOptions ?? Mock.Of<IPersistedOptions>(),
                backupDirectoryValidator ?? Mock.Of<IBackupDirectoryValidator>(),
                backupDirectoryFactory ?? Mock.Of<IBackupDirectoryFactory>(),
                backupDirectoryModel ?? Mock.Of<IBackupDirectoryModel>()
                );
        }
    }
}
