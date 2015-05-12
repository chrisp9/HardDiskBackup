using System;
using System.Collections.Generic;
using System.Text;
using SystemWrapper.IO;

namespace Services.Disk
{
    public interface IFailedBackupReportingService
    {
        void LogFailure(IFileInfoWrap backupDirectory, string reason);
    }

    public class FailedBackupReportingService
    {
        private IDictionary<IFileInfoWrap, string> _reasons;

        public FailedBackupReportingService()
        {
            _reasons = new Dictionary<IFileInfoWrap, string>();
        }

        public void LogFailure(FileInfoWrap backupDirectory, Exception reason)
        {
            _reasons.Add(backupDirectory, reason + ": " + reason.Message);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var reason in _reasons)
                sb.Append(string.Format("Copy failed for: {0}, with reason: {1} ", reason.Key.FullName, reason.Value));

            return sb.ToString();
        }
    }
}