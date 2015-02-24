using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Services.Disk;
using SystemWrapper.IO;
using Moq;


namespace Services.Tests
{    
    public class Test_BackupDirectoryValidator
    {
        [TestCase(@"c:\", true, true)]
        [TestCase(@"c:/", true, true)]
        [TestCase(@"c:\stuff/stuff", true, true)]
        [TestCase("c:", true, false)] // Edge case - should be considered invalid even though it exists
        public void Path_is_only_valid_if_it_exists(string path, bool exists, bool shouldReturn)
        {
            // Arrange
            var sut = SetupSut(path, exists);

            // Act
            var result = sut.IsValidDirectory(path);

            // Assert
            Assert.AreEqual(shouldReturn, result);
        }

        private IBackupDirectoryValidator SetupSut(string path, bool exists)
        {
            var mockDirectoryWrap = new Mock<IDirectoryWrap>();
            mockDirectoryWrap.Setup(x => x.Exists(path)).Returns(exists);

            return new BackupDirectoryValidator(mockDirectoryWrap.Object);
        }
    }
}
