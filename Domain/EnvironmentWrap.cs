using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public interface IEnvironmentWrap
    {
        public string AppDataPath { get; }
    }

    public class EnvironmentWrap
    {
        public string AppDataPath
        {
            get
            {
                return Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData);
            }
        }

    }
}
