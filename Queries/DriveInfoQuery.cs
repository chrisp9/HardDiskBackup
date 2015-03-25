using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using System.IO;
using Registrar;

namespace Queries
{
    public interface IDriveInfoQuery
    {
        IEnumerable<IDriveInfoWrap> GetDrives();
    }

    [Register(Scope.SingleInstance)]
    public class DriveInfoQuery : IDriveInfoQuery
    {
        public IEnumerable<IDriveInfoWrap> GetDrives()
        {
            return DriveInfo.GetDrives().Select(x => new DriveInfoWrap(x));
        }
    }
}
