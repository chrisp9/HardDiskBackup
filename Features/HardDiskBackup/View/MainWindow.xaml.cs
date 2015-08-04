using Domain;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;
using Registrar;
using Services;

namespace HardDiskBackup.View
{
    public interface IMainWindowView : IView { }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary
    [Register(LifeTime.SingleInstance)]
    public partial class MainWindow : MetroWindow, IMainWindowView
    {
        private IMessenger _messenger;
        private IDispatcher _dispatcher;

        public MainWindow(IMessenger messenger, IDispatcher dispatcher)
        {
            InitializeComponent();
            _messenger = messenger;
            _dispatcher = dispatcher;

            _messenger.Register<Messages>(this, m =>
            {
                if (m == Messages.PerformBackup)
                {
                    _dispatcher.InvokeAsync(() =>
                    {
                        Focus();
                        BringIntoView();
                    }); 
                }
            });
        }
    }
}