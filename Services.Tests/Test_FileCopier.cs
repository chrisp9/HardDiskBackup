﻿using Moq;
using NUnit.Framework;
using Services.Disk.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var result = _sut.CopyFiles(_filesToCopy, @"e:\dest", (o) => {});

            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void Only_the_second_copy_is_successful()
        {
            _fileWrap.Setup(x => x.Copy(_firstFileToCopy.Object.FullName, @"e:\dest"))
                .Throws<UnauthorizedAccessException>();

            var result = _sut.CopyFiles(_filesToCopy, @"e:\dest", (o) => { });

            Assert.IsTrue(result.IsFail);
        }
    }
}
