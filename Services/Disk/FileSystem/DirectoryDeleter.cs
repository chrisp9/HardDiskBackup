﻿using Domain;
using Registrar;
using System;
using System.Threading.Tasks;
using SystemWrapper.IO;
using Domain.Exceptions;

namespace Services.Disk.FileSystem
{
    public interface IDirectoryDeleter
    {
        Task<Result> Delete(IDirectoryInfoWrap existingBackup, Action onDeleteComplete);
    }
    
    [Register(LifeTime.Transient)]
    public class DirectoryDeleter : IDirectoryDeleter
    {
        public async Task<Result> Delete(
            IDirectoryInfoWrap existingBackup, 
            Action onDeleteComplete)
        {
            var toDelete = existingBackup;
            var result = await Task.Run<Result>(() =>
            {
                try
                {
                    toDelete.Delete(true);
                }
                catch (Exception e)
                {
                    return Result.Fail(new Error(e, toDelete.FullName));
                }

                return Result.Success();
            });

            onDeleteComplete();
            return result;
        }
    }
}
