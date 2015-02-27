using Domain;
using Moq;
using NUnit.Framework;
using Services.Disk;
using SystemWrapper.IO;

namespace Services.Tests
{    
    public class Test_BackupDirectoryValidator
    {
        [TestCase(@"c:\", true, false, ValidationResult.Success)]
        [TestCase(@"c:\stuff\stuff", true, false, ValidationResult.Success)]

        [TestCase(@"c:\", true, true, ValidationResult.PathAlreadyExists)]
        [TestCase(@"c:\stuff\stuff", true, true, ValidationResult.PathAlreadyExists)]

        [TestCase(@"c:\stuff\..\", true, false, ValidationResult.InvalidPath)]
        [TestCase(@"c:\stuff\.\.\.\.\", true, false, ValidationResult.InvalidPath)]
        [TestCase(@"c:\.", true, false, ValidationResult.InvalidPath)]
        [TestCase("c:", true, false, ValidationResult.InvalidPath)] // Edge case - should be considered invalid even though it exists
        public void Path_is_only_valid_if_it_exists(string path, bool exists, bool isSubDirectoryOfExisting, ValidationResult shouldReturn)
        {
            // Arrange
            var sut = SetupSut(path, exists, isSubDirectoryOfExisting);

            // Act
            var result = sut.CanAdd(path);

            // Assert
            Assert.AreEqual(shouldReturn, result);
        }

        private IBackupDirectoryValidator SetupSut(string path, bool pathExists, bool isSubirectoryOfExisting)
        {
            var mockDirectoryWrap = new Mock<IDirectoryWrap>();
            mockDirectoryWrap.Setup(x => x.Exists(path)).Returns(pathExists);

            var mockBackupDirectoryModel = new Mock<IBackupDirectoryModel>();
            mockBackupDirectoryModel.Setup(x => x.IsSubdirectoryOfExisting(path)).Returns(isSubirectoryOfExisting);

            return new BackupDirectoryValidator(mockDirectoryWrap.Object, mockBackupDirectoryModel.Object);
        }
    }
}
