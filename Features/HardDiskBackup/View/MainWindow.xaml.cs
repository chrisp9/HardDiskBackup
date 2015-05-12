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
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}