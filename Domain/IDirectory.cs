using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper.IO;

namespace Domain
{
    public interface IDirectory
    {
        IDirectoryInfoWrap Directory { get; }
    }
}
