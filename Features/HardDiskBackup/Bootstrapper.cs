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
using Services;
using SystemWrapper.IO;
using Services.Factories;
using Services.Persistence;
using Services.Scheduling;
using HardDiskBackup.Commands;
using HardDiskBackup.View;
using HardDiskBackup.ViewModel;

namespace HardDiskBackup
{
    public class Bootstrapper // TODO: Split bootstrapper into modules
    {
        public void RegisterDependencies()
        {
            //TODO: THIS IS A MESS - use autowiring.
            RegisterTransient<FileWrap, IFileWrap>();
            RegisterTransient<DirectoryWrap, IDirectoryWrap>();
            RegisterTransient<DriveInfoWrap, IDriveInfoWrap>();
            RegisterTransient<EnvironmentWrap, IEnvironmentWrap>();
            RegisterTransient<DefaultScheduler, IScheduler>();
            RegisterTransient<DateTimeProvider, IDateTimeProvider>();
            RegisterTransient<JsonSerializer, IJsonSerializer>();
            RegisterTransient<BackupSettings, IBackupSettings>();
            RegisterTransient<BackupFactory, IDirectoryFactory>();
            RegisterTransient<BackupDirectoryValidator, IBackupDirectoryValidator>();
            RegisterTransient<BackupScheduleFactory, IBackupScheduleFactory>();
            RegisterTransient<BackupScheduleService, IBackupScheduleService>();
            RegisterTransient<NextBackupDateTimeFactory, INextBackupDateTimeFactory>();

            RegisterSingle<SetScheduleModel, ISetScheduleModel>();
            RegisterSingle<DriveInfoService, IDriveInfoService>();
            RegisterSingle<DriveNotifier, IDriveNotifier>();
            RegisterSingle<DriveInfoQuery, IDriveInfoQuery>();
            RegisterSingle<BackupDirectoryModel, IBackupDirectoryModel>();
            RegisterSingle<ScheduleBackupCommand, IScheduleBackupCommand>();
            RegisterSingle<BackupView, IBackupView>();
            RegisterSingle<MainWindow, IMainWindowView>();
            Ioc.ContainerBuilder.RegisterType<MainWindowViewModel>();
            Ioc.ContainerBuilder.RegisterType<SetScheduleViewModel>();
            Ioc.ContainerBuilder.RegisterType<BackupViewModel>();


            Ioc.ContainerBuilder.RegisterType<FirstRunViewModel>();
            Ioc.ContainerBuilder.RegisterType<Dispatcher>().As<IDispatcher>().SingleInstance();

            Ioc.ContainerBuilder.RegisterType<WindowPresenter<MainWindowViewModel, IMainWindowView>>()
                .As<IWindowPresenter<MainWindowViewModel, IMainWindowView>>();

            Ioc.ContainerBuilder.RegisterType <WindowPresenter<BackupViewModel, IBackupView>>()
                .As <IWindowPresenter<BackupViewModel, IBackupView>>();

            Ioc.ContainerBuilder.RegisterInstance<DefaultScheduler>(DefaultScheduler.Instance).As<IScheduler>();
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
