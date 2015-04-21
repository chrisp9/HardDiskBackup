using Domain;
using Registrar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Disk.FileSystem
{
    public interface IExistingBackupsModel
    {
        ObservableCollection<FormattedExistingBackup> ExistingBackups { get; }
        void Add(FormattedExistingBackup existingBackup);
        void Remove(FormattedExistingBackup existingBackup);
        void Clear();
    }

    [Register(LifeTime.SingleInstance)]
    public class ExistingBackupsModel : IExistingBackupsModel
    {
        public ObservableCollection<FormattedExistingBackup> ExistingBackups { get;  private set; }

        public ExistingBackupsModel()
        {
            ExistingBackups = new ObservableCollection<FormattedExistingBackup>();
        }

        public void Add(FormattedExistingBackup formattedExistingBackup)
        {
            ExistingBackups.Add(formattedExistingBackup);
        }

        public void Remove(FormattedExistingBackup formattedExistingBackup)
        {
            ExistingBackups.Remove(formattedExistingBackup);
        }

        public void Clear()
        {
            ExistingBackups.Clear();
        }
    }
}
