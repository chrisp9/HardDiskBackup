using Domain;
using Moq;
using NUnit.Framework;
using Services.Disk.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper.IO;
using System.IO;

namespace Services.Tests
{
    public class Test_DirectoryCopier
    {
        private Mock<IFileCopier> _mockFileCopier;
        private Mock<IDirectoryWrap> _mockDirectoryWrap;
        private Mock<IDirectoryCreator> _mockDirectoryCreator;
        private Mock<IFileWrap> _mockFileWrap;
        private DirectoryCopier _sut;

        private Mock<IFileInfoWrap> _rootFile1;
        private Mock<IFileInfoWrap> _rootFile2;

        private Mock<IFileInfoWrap> _subDirectoryFile1;
        private Mock<IFileInfoWrap> _subDirectoryFile2;

        private Exception _exceptionToThrow = new UnauthorizedAccessException("uh..");
        private string _testRootDirectory = @"c:\test";
        private string _testSubDirectory = @"c:\test\subdir1";

        private IFileInfoWrap[] _filesToCopy; 
        private IFileInfoWrap[] _subDirFilesToCopy;

        private string _destinationDirectory = @"e:\test";

        [SetUp]
        public void Setup()
        {
            _mockFileCopier = new Mock<IFileCopier>();
            _mockDirectoryWrap = new Mock<IDirectoryWrap>();
            _mockDirectoryCreator = new Mock<IDirectoryCreator>();
            _mockFileWrap = new Mock<IFileWrap>();

            _rootFile1 = new Mock<IFileInfoWrap>();
            _rootFile2 = new Mock<IFileInfoWrap>();

            _subDirectoryFile1 = new Mock<IFileInfoWrap>();
            _subDirectoryFile2 = new Mock<IFileInfoWrap>();

            _rootFile1.Setup(x => x.FullName).Returns(Path.Combine(_testRootDirectory, "rootFile1"));
            _rootFile2.Setup(x => x.FullName).Returns(Path.Combine(_testRootDirectory, "rootFile2"));

            _subDirectoryFile1.Setup(x => x.FullName).Returns(Path.Combine(_testSubDirectory, "subFile1"));
            _subDirectoryFile2.Setup(x => x.FullName).Returns(Path.Combine(_testRootDirectory, "subFile2"));

            _subDirectoryFile1.Setup(x => x.Name).Returns("subFile1");
            _subDirectoryFile2.Setup(x => x.Name).Returns("subFile2");

            _filesToCopy = new[] { _rootFile1.Object, _rootFile2.Object }.ToArray();
            _subDirFilesToCopy = new[] { _subDirectoryFile1.Object, _subDirectoryFile2.Object }.ToArray();
		  
            _sut = new DirectoryCopier(
                _mockFileCopier.Object,
                _mockFileWrap.Object,
                _mockDirectoryWrap.Object,
                _mockDirectoryCreator.Object);
        }

        [Test]
        public async void Fail_is_returned_when_copying_if_CreateDirectory_fails()
        {
            var directoryInfoWrap = new Mock<IDirectoryInfoWrap>();
            directoryInfoWrap.Setup(x => x.GetFiles())
                .Throws(_exceptionToThrow);

            _mockDirectoryCreator.Setup(x => x.CreateDirectoryIfNotExist(_testRootDirectory))
                .Returns(Result.Fail(_exceptionToThrow));

            var result = await _sut.CopySafe(directoryInfoWrap.Object, _testRootDirectory, (_) => { });
            Assert.AreEqual(true, result.IsFail);
            Assert.Contains(_exceptionToThrow, result.Exceptions.ToArray());
        }

        [Test]
        public async void Fail_is_returned_when_copying_if_GetFiles_fails()
        {
            var directoryInfoWrap = new Mock<IDirectoryInfoWrap>();
            directoryInfoWrap.Setup(x => x.GetFiles())
                .Throws(_exceptionToThrow);

            _mockDirectoryCreator.Setup(x => x.CreateDirectoryIfNotExist(_testRootDirectory))
                .Returns(Result.Success());

            var result = await _sut.CopySafe(directoryInfoWrap.Object, _testRootDirectory, (_) => { });
            Assert.AreEqual(true, result.IsFail);
            Assert.Contains(_exceptionToThrow, result.Exceptions.ToArray());
        }

        [Test]
        public async void Success_is_returned_if_everything_goes_well()
        {
            var filesToCopy = new[] { _rootFile1.Object, _rootFile2.Object }.ToArray();

            var directoryInfoWrap = new Mock<IDirectoryInfoWrap>();
            directoryInfoWrap.Setup(x => x.GetFiles())
                .Returns(filesToCopy);

            _mockDirectoryCreator.Setup(x => x.CreateDirectoryIfNotExist(It.IsAny<string>()))
                .Returns(Result.Success());

            _mockFileCopier.Setup(x => x.CopyFiles(
                It.IsAny<IFileInfoWrap[]>(),
                It.IsAny<string>(),
                It.IsAny<Action<IFileInfoWrap>>()))
                .Returns(Result.Success());

            directoryInfoWrap.Setup(x => x.GetDirectories())
                .Returns(Enumerable.Empty<IDirectoryInfoWrap>().ToArray());

            var result = await _sut.CopySafe(directoryInfoWrap.Object, @"e:\test", (_) => { });

            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public async void Files_within_directory_are_copied()
        {
            var filesToCopy = new[] { _rootFile1.Object, _rootFile2.Object}.ToArray();

            var directoryInfoWrap = new Mock<IDirectoryInfoWrap>();
            directoryInfoWrap.Setup(x => x.GetFiles())
                .Returns(filesToCopy);

            _mockDirectoryCreator.Setup(x => x.CreateDirectoryIfNotExist(It.IsAny<string>()))
                .Returns(Result.Success());

            CopyFilesReturnsSuccess();

            directoryInfoWrap.Setup(x => x.GetDirectories())
                .Returns(Enumerable.Empty<IDirectoryInfoWrap>().ToArray());

            var result = await _sut.CopySafe(directoryInfoWrap.Object, @"e:\test", (_) => {});

            _mockFileCopier.Verify(x => x.CopyFiles(
                filesToCopy,  @"e:\test", It.IsAny<Action<IFileInfoWrap>>()), Times.Once());
        }

        [Test]
        public async void Files_in_subdirectories_are_also_copied()
        {
            var directoryInfoWrap = new Mock<IDirectoryInfoWrap>();
            directoryInfoWrap.Setup(x => x.GetFiles())
                .Returns(_filesToCopy);

            var subDirectoryInfoWrap = new Mock<IDirectoryInfoWrap>();
            subDirectoryInfoWrap.Setup(x => x.GetFiles())
                .Returns(_subDirFilesToCopy);

            subDirectoryInfoWrap.Setup(x => x.Name).Returns("subdir1");
            subDirectoryInfoWrap.Setup(x => x.FullName).Returns(_testSubDirectory);

            directoryInfoWrap.Setup(x => x.GetDirectories()).Returns(
                new[] { subDirectoryInfoWrap.Object }.ToArray());

            _mockDirectoryCreator.Setup(x => x.CreateDirectoryIfNotExist(It.IsAny<string>()))
                .Returns(Result.Success());

            CopyFilesReturnsSuccess();

            directoryInfoWrap.Setup(x => x.GetDirectories())
                .Returns(new[] {subDirectoryInfoWrap.Object}.ToArray());

            var result = await _sut.CopySafe(directoryInfoWrap.Object, _destinationDirectory, (_) => { });

            _mockFileCopier.Verify(x => x.CopyFiles(_subDirFilesToCopy, @"e:\test\subdir1", It.IsAny<Action<IFileInfoWrap>>()), Times.Once());
        }

        private void CopyFilesReturnsSuccess()
        {
            _mockFileCopier.Setup(x => x.CopyFiles(
                It.IsAny<IFileInfoWrap[]>(),
                It.IsAny<string>(),
                It.IsAny<Action<IFileInfoWrap>>()))
                .Returns(Result.Success());
        }
    }
}