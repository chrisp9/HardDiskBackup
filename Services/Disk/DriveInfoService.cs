using Domain;
using Queries;
using Registrar;
using System.Collections.Generic;
using System.Linq;

namespace Services.Disk
{
    public interface IDriveInfoService
    {
        IEnumerable<IDriveInfoWrap> GetDrives();
    }

    [Register(LifeTime.SingleInstance)]
    public class DriveInfoService : IDriveInfoService
    {
        private readonly IDriveInfoQuery _driveInfoQuery;

        public DriveInfoService(IDriveInfoQuery driveInfoQuery)
        {
            _driveInfoQuery = driveInfoQuery;
        }

        public IEnumerable<IDriveInfoWrap> GetDrives()
        {
            return _driveInfoQuery
               .GetDrives()
               .Where(drive => drive.IsReady);
        }
    }
}