using Domain;
using HardDiskBackup.Commands;
using Moq;
using NUnit.Framework;
using Services.Disk;
using Services.Factories;
using Services.Persistence;
using Services.Scheduling;
using SystemWrapper.IO;

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
                directoryFactory: SetupFactory(_backupDirectory),
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

        private IDirectoryFactory SetupFactory(BackupDirectory backupDirectory)
        {
            var mockBackupDirectoryFactory = new Mock<IDirectoryFactory>();

            mockBackupDirectoryFactory
                .Setup(x => x.CreateBackupDirectory(It.IsAny<string>()))
                .Returns(backupDirectory);

            return mockBackupDirectoryFactory.Object;
        }

        private FirstRunViewModel SetupSut(
            IDateTimeProvider dateTimeProvider = null,
            IJsonSerializer jsonSerializer = null,
            IBackupDirectoryValidator backupDirectoryValidator = null,
            IDirectoryFactory directoryFactory = null,
            IBackupDirectoryModel backupDirectoryModel = null,
            ISetScheduleModel setScheduleModel = null,
            IScheduleBackupCommand scheduleBackupCommand = null
            )
        {
            return new FirstRunViewModel(
                dateTimeProvider ?? Mock.Of<IDateTimeProvider>(),
                jsonSerializer ?? Mock.Of<IJsonSerializer>(),
                backupDirectoryValidator ?? Mock.Of<IBackupDirectoryValidator>(),
                directoryFactory ?? Mock.Of<IDirectoryFactory>(),
                backupDirectoryModel ?? Mock.Of<IBackupDirectoryModel>(),
                setScheduleModel ?? Mock.Of<ISetScheduleModel>(),
                scheduleBackupCommand ?? Mock.Of<IScheduleBackupCommand>()
                );
        }
    }
}
