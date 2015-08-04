using Registrar;
using Services;

namespace HardDiskBackup.View
{
    public interface IBackupView : IView
    {
    }

    /// <summary>
    /// Interaction logic for BackupView.xaml
    /// </summary>
    [Register(LifeTime.SingleInstance)]
    public partial class BackupView
    {
        public BackupView()
        {
            InitializeComponent();
        }
    }
}