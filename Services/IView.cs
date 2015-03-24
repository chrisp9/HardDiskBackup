using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IView
    {
        object DataContext { get; set; }
        void Show();
    }
}
