﻿using FirstFloor.ModernUI.Windows.Controls;
using Registrar;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HardDiskBackup.View
{
    public interface IBackupView : IView
    {

    }
    /// <summary>
    /// Interaction logic for BackupView.xaml
    /// </summary>
    [Register(Scope.SingleInstance)]
    public partial class BackupView : ModernWindow, IBackupView
    {
        public BackupView()
        {
            InitializeComponent();
        }
    }
}