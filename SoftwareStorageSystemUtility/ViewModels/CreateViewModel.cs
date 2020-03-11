using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SoftwareStorageSystemUtility.ObjectModel;

namespace SoftwareStorageSystemUtility.ViewModels
{
 public class CreateViewModel : INotifyPropertyChanged
  {
    public CreateIdViewModel CreateIdViewModel { get; }
    public CreateAuditViewModel CreateAuditViewModel { get; }

    public CreateViewModel()
    {
      CreateIdViewModel = new CreateIdViewModel();
      CreateAuditViewModel = new CreateAuditViewModel();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
   
  }
}
