using Domain;
using Registrar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper.IO;

namespace Services.Factories
{
    public interface ITimestampedBackupRootProvider
    {
        TimestampedBackupRoot CreateTimestampedBackup(BackupRootDirectory backupRoot);
    }

    [Register(LifeTime.SingleInstance)]
    public class TimestampedBackupRootProvider : ITimestampedBackupRootProvider
    {
        private IDateTimeProvider _dateTimeProvider;

        public TimestampedBackupRootProvider(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        /// <summary>
        /// Creates a TimestampedBackupRoot under the provided BackupRootDirectory
        /// </summary>
        /// <param name="backupRoot">The BackupRootDirectory</param>
        /// <returns>The timestamped backup directory which was just created</returns>
        public TimestampedBackupRoot CreateTimestampedBackup(BackupRootDirectory backupRoot)
        {
            var now = _dateTimeProvider.Now.ToString("yyyy-MM-dd_HH.mm.ss");
            backupRoot.Directory.CreateSubdirectory(now);

            return new TimestampedBackupRoot(new DirectoryInfoWrap(
                Path.Combine(backupRoot.Directory.FullName, now)));
        }
    }
}