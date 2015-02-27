using System.Windows.Controls;
using System.Windows.Data;

namespace HardDiskBackup.View
{
    /// <summary>
    /// Interaction logic for FirstRunControl.xaml
    /// </summary>
    public partial class FirstRunControl : UserControl
    {
        public FirstRunControl()
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