using Registrar;
using System;

namespace Services
{
    public interface IDispatcher
    {
        void InvokeAsync(Action action);
    }

    [Register(LifeTime.SingleInstance)]
    public class Dispatcher : IDispatcher
    {
        private System.Windows.Threading.Dispatcher _dispatcher;

        public Dispatcher() 
        {
            _dispatcher = System.Windows.Application.Current.Dispatcher;
        }

        public void InvokeAsync(Action action)
        {
            _dispatcher.InvokeAsync(action);
        }
    }
}
