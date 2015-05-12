using Autofac;
using HardDiskBackup.View;
using HardDiskBackup.ViewModel;
using System.Windows;

namespace HardDiskBackup
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            var bootstrapper = new Bootstrapper(new ContainerBuilder());
            var builder = bootstrapper.Bootstrap();

            var container = builder.Build();
            var main = container.Resolve<IWindowPresenter<MainWindowViewModel, IMainWindowView>>();

            var window = main.Present();
            window.Show();
        }
    }
}