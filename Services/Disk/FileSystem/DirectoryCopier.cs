﻿using Domain;
using Registrar;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SystemWrapper.IO;

namespace Services.Disk.FileSystem
{
    public interface IDirectoryCopier
    {
        Task<Result<long>> CalculateTotalSizeSafe(IDirectoryInfoWrap directory);

        Task<Result> CopySafe(
            IDirectoryInfoWrap source,
            string destination,
            Action<IFileInfoWrap> onFileCopied);
    }

    [Register(LifeTime.Transient)]
    public class DirectoryCopier : IDirectoryCopier
    {
        private readonly IFileWrap _fileWrap;
        private readonly IDirectoryWrap _directoryWrap;
        private readonly IDirectoryCreator _directoryCreator;
        private readonly IFileCopier _fileCopier;

        public DirectoryCopier(
            IFileCopier fileCopier,
            IFileWrap fileWrap, 
            IDirectoryWrap directoryWrap,
            IDirectoryCreator directoryCreator)
        {
            _fileWrap = fileWrap;
            _directoryWrap = directoryWrap;
            _directoryCreator = directoryCreator;
            _fileCopier = fileCopier;
        }

        public async Task<Result<long>> CalculateTotalSizeSafe(
            IDirectoryInfoWrap directory)
        {
            var filesResult = directory.GetFilesSafe();
            if (filesResult.IsFail)
                return Result<long>.Fail(filesResult.Errors.ToArray());

            var directoriesResult = directory.GetDirectoriesSafe();
            if (directoriesResult.IsFail)
                return Result<long>.Fail(directoriesResult.Errors.ToArray());

            long totalSize = filesResult.Value.Sum(file => file.Length);

            foreach (var dir in directoriesResult.Value)
            {
                var subDirResult = await CalculateTotalSizeSafe(dir);
                if (subDirResult.IsSuccess)
                    totalSize += subDirResult.Value;
            }

            return Result<long>.Success(totalSize);
        }

        public Task<Result> CopySafe(
            IDirectoryInfoWrap source, 
            string destination,
            Action<IFileInfoWrap> onFileCopied)
        {
            return CopySafeRecursive(
                source, 
                Path.Combine(destination, source.FullName.First().ToString(), source.Name),
                onFileCopied);
        }

        /// <summary>
        /// Safely copies files from source to destination
        /// </summary>
        /// <returns>
        /// A task indicating any errors during copy
        /// </returns>
        public async Task<Result> CopySafeRecursive(
            IDirectoryInfoWrap source, 
            string destination, 
            Action<IFileInfoWrap> onFileCopied)
        {
            var result = _directoryCreator.CreateDirectoryIfNotExist(destination);
            if (result.IsFail) 
                return result;

            var filesResult = source.GetFilesSafe();
            if (filesResult.IsFail)
                return filesResult.ToUnit();

            var fileCopyResult = await Task.Run(() =>           
                _fileCopier.CopyFiles(
                    filesResult.Value, destination, onFileCopied));

            var subDirectoriesResult = source.GetDirectoriesSafe();
            if (subDirectoriesResult.IsFail)
                return subDirectoriesResult.ToUnit();

            var directoryCopyResult = Result.Success();
            foreach (var directory in subDirectoriesResult.Value) 
            {
                var recursiveResult = await CopySafeRecursive(
                    directory,
                    Path.Combine(destination, directory.Name),
                    onFileCopied);

                directoryCopyResult = Result.Combine(
                    directoryCopyResult, recursiveResult);
            }

            return Result.Combine(fileCopyResult, directoryCopyResult); 
        }
    }
}