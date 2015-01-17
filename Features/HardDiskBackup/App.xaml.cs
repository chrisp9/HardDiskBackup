using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using Domain;
using System.Reactive.Concurrency;
using Services.Disk;
using Queries;

namespace HardDiskBackup
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ContainerBuilder _containerBuilder;
        private IContainer _container;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            _containerBuilder = new ContainerBuilder();

            RegisterTransient<DriveInfoWrap, IDriveInfoWrap>();
            RegisterTransient<DefaultScheduler, IScheduler>();

            RegisterSingle<DriveInfoService, IDriveInfoService>();
            RegisterSingle<DriveNotifier, IDriveNotifier>();
            RegisterSingle<DriveInfoQuery, IDriveInfoQuery>();

            _container = _containerBuilder.Build();
            _container.Resolve<IDriveInfoService>();
        }

        private void RegisterSingle<T, U>()
        {
            _containerBuilder.RegisterType<T>()
                .As<U>()
                .SingleInstance();
        }

        private void RegisterTransient<T, U>()
        {
            _containerBuilder.RegisterType<T>()
                .As<U>()
                .InstancePerDependency();
        }
    }
}
