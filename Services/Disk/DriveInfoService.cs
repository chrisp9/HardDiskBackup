using Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Queries;

namespace Services.Disk
{
    public interface IDiskService
    {
        IEnumerable<IDriveInfoWrap> GetDrives();
    }

    public class DriveInfoService : IDiskService
    {
        private IDriveInfoQuery _driveInfoQuery;

        public DriveInfoService(IDriveInfoQuery driveInfoQuery)
        {
            _driveInfoQuery = driveInfoQuery;
        }

        public IEnumerable<IDriveInfoWrap> GetDrives()
        {
            return _driveInfoQuery
               .GetDrives()
               .Where(drive => drive.IsReady
                  && drive.DriveType == DriveType.Removable);
        }
    }
}
