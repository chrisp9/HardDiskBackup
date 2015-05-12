using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SystemWrapper.IO;

namespace TestHelpers
{
    public static class FakeDirectoryInfoBuilder
    {
        public static Mock<IDirectoryInfoWrap> Create(string path)
        {
            return CreateMock(path);
        }

        private static Mock<IDirectoryInfoWrap> CreateMock(string path)
        {
            var mock = new Mock<IDirectoryInfoWrap>();
            mock.Setup(x => x.FullName).Returns(path);

            return mock;
        }

        public static Mock<IDirectoryInfoWrap> WithSubDirectories(this Mock<IDirectoryInfoWrap> parent, params Mock<IDirectoryInfoWrap>[] children)
        {
            parent.Setup(x => x.GetDirectories()).Returns(children.Select(x => x.Object).ToArray());
            return parent;
        }

        public static Mock<IDirectoryInfoWrap> WithFiles(this Mock<IDirectoryInfoWrap> parent, params Tuple<string, long>[] children)
        {
            var files = parent.Object.GetFiles().ToList() ?? new List<IFileInfoWrap>();
            foreach (var child in children)
            {
                var mockFileInfoWrap = new Mock<IFileInfoWrap>();
                mockFileInfoWrap.Setup(x => x.FullName).Returns(Path.Combine(parent.Object.FullName, child.Item1).ToString());
                mockFileInfoWrap.Setup(x => x.Name).Returns(child.Item1);
                mockFileInfoWrap.Setup(x => x.Length).Returns(child.Item2);
                files.Add(mockFileInfoWrap.Object);
            }

            parent.Setup(x => x.GetFiles()).Returns(files.ToArray());
            return parent;
        }
    }
}