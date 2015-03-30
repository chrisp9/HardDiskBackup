using Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Queries;
using Registrar;

namespace Services.Disk
{
    public interface IDriveInfoService
    {
        IEnumerable<IDriveInfoWrap> GetDrives();
    }

    [Register(LifeTime.SingleInstance)]
    public class DriveInfoService : IDriveInfoService
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
