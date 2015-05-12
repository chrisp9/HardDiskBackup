using Domain;
using Registrar;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Queries
{
    public interface IDriveInfoQuery
    {
        IEnumerable<IDriveInfoWrap> GetDrives();
    }

    [Register(LifeTime.SingleInstance)]
    public class DriveInfoQuery : IDriveInfoQuery
    {
        public IEnumerable<IDriveInfoWrap> GetDrives()
        {
            return DriveInfo.GetDrives().Select(x => new DriveInfoWrap(x));
        }
    }
}