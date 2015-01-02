using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.IO;

namespace DiskDetector.Model
{
    public class DiskPoller
    {
        private IEnumerable<DriveInfo> _last;

        public DiskPoller()
        {
            _last = GetDrives();
        }

        private IEnumerable<DriveInfo> GetDrives()
        {
           return DriveInfo
              .GetDrives()
              .Where(drive => drive.DriveType == DriveType.Removable
                 && drive.DriveType == DriveType.Removable);
        }
    }
}
