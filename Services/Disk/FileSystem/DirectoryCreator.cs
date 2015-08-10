using Domain;
using Registrar;
using System;
using SystemWrapper.IO;
using Domain.Exceptions;

namespace Services.Disk.FileSystem
{
    public interface IDirectoryCreator
    {
        Result CreateDirectoryIfNotExist(string directory);
    }

    [Register(LifeTime.Transient)]
    public class DirectoryCreator : IDirectoryCreator
    {
        private readonly IDirectoryWrap _directoryWrap;

        public DirectoryCreator(IDirectoryWrap directoryWrap)
        {
            _directoryWrap = directoryWrap;
        }

        public Result CreateDirectoryIfNotExist(string directory)
        {
            try
            {
                if (!_directoryWrap.Exists(directory))
                    _directoryWrap.CreateDirectory(directory);
            }
            catch (Exception e)
            {
                return Result.Fail(new Error(e, directory));
            }

            return Result.Success();
        }
    }
}