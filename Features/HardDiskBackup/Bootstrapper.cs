﻿using System;
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
using Services.BackupSchedule;
using SystemWrapper.IO;
using Services.Factories;

namespace HardDiskBackup
{
    public class Bootstrapper // TODO: Split bootstrapper into modules
    {
        public void RegisterDependencies()
        {
            RegisterTransient<FileWrap, IFileWrap>();
            RegisterTransient<DirectoryWrap, IDirectoryWrap>();
            RegisterTransient<DriveInfoWrap, IDriveInfoWrap>();
            RegisterTransient<EnvironmentWrap, IEnvironmentWrap>();
            RegisterTransient<DefaultScheduler, IScheduler>();
            RegisterTransient<DateTimeProvider, IDateTimeProvider>();
            RegisterTransient<JsonLayer, IJsonLayer>();
            RegisterTransient<PersistedOptions, IPersistedOptions>();
            RegisterTransient<BackupDirectoryFactory, IBackupDirectoryFactory>();
            RegisterTransient<BackupDirectoryService, IBackupDirectoryService>();

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
