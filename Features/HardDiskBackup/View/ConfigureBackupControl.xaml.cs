using System.Windows.Controls;
using System.Windows.Data;

namespace HardDiskBackup.View
{
    /// <summary>
    /// Interaction logic for ConfigureBackupControl.xaml
    /// </summary>
    public partial class ConfigureBackupControl : UserControl
    {
        public ConfigureBackupControl()
        {
            InitializeComponent();
        }

        private void Binding_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            var be = BackupDirectoryInput.GetBindingExpression(TextBox.TextProperty);
            be.UpdateSource();
        }
    }
}