using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SoftwareStorageSystemUtility.Converters
{
  public class IntStateToStringConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var res = String.Empty;
      if ((int)value == 1)
        res = "Тестовая ревизия";
      if ((int)value == 2)
        res = "Релиз";
      return res;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
