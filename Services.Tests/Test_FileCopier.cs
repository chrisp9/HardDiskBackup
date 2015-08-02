using Moq;
using NUnit.Framework;
using Services.Disk.FileSystem;
using System;
using System.IO;
using SystemWrapper.IO;

namespace Services.Tests
{
    public class Test_FileCopier
    {
        private FileCopier _sut;
        private Mock<IFileWrap> _fileWrap;

        private Mock<IFileInfoWrap> _firstFileToCopy;
        private Mock<IFileInfoWrap> _secondFileToCopy;

        private IFileInfoWrap[] _filesToCopy;

        private string _destination = @"e:\test";

        [SetUp]
        public void Setup()
        {
            _fileWrap = new Mock<IFileWrap>();
            _sut = new FileCopier(_fileWrap.Object);

            _firstFileToCopy = new Mock<IFileInfoWrap>();
            _firstFileToCopy.Setup(x => x.FullName).Returns(@"c:\testFile.txt");
            _firstFileToCopy.Setup(x => x.Name).Returns(@"testFile.txt");
            _firstFileToCopy.Setup(x => x.Attributes).Returns(System.IO.FileAttributes.ReadOnly);

            _secondFileToCopy = new Mock<IFileInfoWrap>();
            _secondFileToCopy.Setup(x => x.FullName).Returns(@"c:\testFile2.txt");
            _secondFileToCopy.Setup(x => x.Name).Returns(@"testFile2.txt");
            _secondFileToCopy.Setup(x => x.Attributes).Returns(System.IO.FileAttributes.ReadOnly);

            _filesToCopy = new[] { _firstFileToCopy.Object, _secondFileToCopy.Object };
        }

        [Test]
        public void Everything_is_successful()
        {
            var result = _sut.CopyFiles(_filesToCopy, _destination, (o) => {});

            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void Only_the_second_copy_is_successful()
        {
            _fileWrap.Setup(x => x.Copy(_firstFileToCopy.Object.FullName, @"e:\test\testFile.txt"))
                .Throws<UnauthorizedAccessException>();

            var result = _sut.CopyFiles(_filesToCopy, _destination, (o) => { });

            Assert.IsTrue(result.IsFail);
        }

        [Test]
        public void Files_are_actually_copied()
        {
            var result = _sut.CopyFiles(_filesToCopy, _destination, o => { });

            _fileWrap.Verify(x => x.Copy(_firstFileToCopy.Object.FullName, @"e:\test\testFile.txt"), Times.Once());
            _fileWrap.Verify(x => x.Copy(_secondFileToCopy.Object.FullName, @"e:\test\testFile2.txt"), Times.Once());
        }

        [Test]
        public void Read_only_flag_is_removed()
        {
            FileAttributes attributes = FileAttributes.ReadOnly;

            _fileWrap.Setup(x => x.SetAttributes(@"e:\test\testFile.txt", It.IsAny<FileAttributes>()))
                .Callback<string, FileAttributes>((_, a) => attributes = a);

            var result = _sut.CopyFiles(_filesToCopy, _destination, o => { });

            Assert.IsFalse(attributes.HasFlag(FileAttributes.ReadOnly));
        }
    }
}
