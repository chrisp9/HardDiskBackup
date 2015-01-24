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
using Autofac.Core;

namespace HardDiskBackup
{
    public class Bootstrapper
    {
        public void RegisterDependencies()
        {
            RegisterTransient<DriveInfoWrap, IDriveInfoWrap>();
            RegisterTransient<DefaultScheduler, IScheduler>();

            RegisterSingle<DriveInfoService, IDriveInfoService>();
            RegisterSingle<DriveNotifier, IDriveNotifier>();
            RegisterSingle<DriveInfoQuery, IDriveInfoQuery>();
        }

        private void RegisterSingle<T, U>()
        {
            Ioc.ContainerBuilder.RegisterType<T>()
                .As<U>()
                .SingleInstance();
        }

        private void RegisterTransient<T, U>()
        {
            Ioc.ContainerBuilder.RegisterType<T>()
                .As<U>()
                .InstancePerDependency();
        }

        private void RegisterTransient<T>()
        {
            Ioc.ContainerBuilder.RegisterType<T>()
                .InstancePerDependency();
        }

    }

}
