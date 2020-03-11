using Prism.Interactivity;
using System.Windows;

namespace SoftwareStorageSystemUtility.Helpers
{
  class DialogAction : PopupWindowAction
  {
    protected override Window CreateWindow()
    {
      return new Dialog();
    }
  }
}
