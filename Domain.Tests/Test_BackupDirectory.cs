using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Services.Disk;
using Domain;
using TestHelpers;
using System.IO;
using SystemWrapper;
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

        public BackupDirectory SetupSut(bool exists)
        {
            var directoryInfoWrap = new Mock<IDirectoryInfoWrap>();
            directoryInfoWrap.Setup(x => x.Exists).Returns(exists);

            return new BackupDirectory(directoryInfoWrap.Object);
        }

    }
}
