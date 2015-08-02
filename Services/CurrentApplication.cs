using Registrar;
using System.Windows;

namespace Services
{
    public interface ICurrentApplication
    {
        WindowCollection Windows { get; }
    }

    [Register(LifeTime.SingleInstance)]
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
