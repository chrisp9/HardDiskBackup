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
using HardDiskBackup.ViewModel;
using HardDiskBackup.View;

namespace HardDiskBackup
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Bootstrapper _bootstrapper;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var bootstrapper2 = new Bootstrapper2(Ioc.ContainerBuilder);
            bootstrapper2.Bootstrap();
            
            Ioc.Build();
            var main = Ioc.Container.Resolve<IWindowPresenter<MainWindowViewModel, IMainWindowView>>();

            var window = main.Present();
            window.Show();
        }
    }
}
