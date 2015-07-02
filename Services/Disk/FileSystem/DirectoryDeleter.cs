using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper.IO;

namespace Services.Disk.FileSystem
{
    public interface IDirectoryDeleter
    {
        Task<Result> Delete(IDirectoryInfoWrap existingBackup, Action onDeleteComplete);
    }

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
                    return Result.Fail(e);
                }

                return Result.Success();
            });

            onDeleteComplete();
            return result;
        }
    }
}
