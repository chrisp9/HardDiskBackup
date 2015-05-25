using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Services
{
    public interface ICurrentApplication
    {
        WindowCollection Windows { get; }
    }

    public class CurrentApplication : ICurrentApplication
    {
        public WindowCollection Windows
        {
            get
            {
                return Application.Current.Windows;
            }
        }
    }
}
