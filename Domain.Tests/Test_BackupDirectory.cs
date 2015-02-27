using Domain;
using Moq;
using NUnit.Framework;
using System;
using SystemWrapper.IO;

namespace Test_Domain
{
    public class Test_BackupDirectory
    {
        public void Does_not_throw_if_directory_exists()
        {
            Assert.DoesNotThrow(() => SetupSut(exists: true));
        }

        public void Throws_if_directory__does_not_exist()
        {
            Assert.Throws<ArgumentException>(() => SetupSut(exists: false));
        }

        [TestCase(@"c:/program files", @"c:\program files")]
        [TestCase(@"c:///program files", @"c:\\\program files")]
        [TestCase(@"c:/Program Files", @"c:\program files")]
        public void String_representation_is_correct(string input, string expected)
        {
            var sut = SetupSut(exists: true, path: input);

            Assert.AreEqual(expected, sut.ToString());
        }

        public BackupDirectory SetupSut(bool exists, string path = null)
        {
            var directoryInfoWrap = new Mock<IDirectoryInfoWrap>();
            directoryInfoWrap.Setup(x => x.Exists).Returns(exists);

            if (path != null)
                directoryInfoWrap.Setup(x => x.FullName).Returns(path);

            return new BackupDirectory(directoryInfoWrap.Object);
        }

    }
}
