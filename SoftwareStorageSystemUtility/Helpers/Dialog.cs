using Prism.Interactivity;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SoftwareStorageSystemUtility.Helpers
{
  [TemplatePart(Name = PART_ResizeBox, Type = typeof(UIElement))]
  public class Dialog : Window
  {
    // ReSharper disable once InconsistentNaming
    private const string PART_ResizeBox = "PART_ResizeBox";

    private bool _isWiden;

    public Dialog()
    {
      DefaultStyleKey = typeof(Dialog);
      MouseLeftButtonDown += OnMouseLeftButtonDown;
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
    {
      DragMove();
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      var resizeBox = GetTemplateChild(PART_ResizeBox) as UIElement;
      if (resizeBox != null)
      {
        resizeBox.MouseMove += ResizeBoxOnMouseMove;
        resizeBox.MouseLeftButtonDown += ResizeBoxOnMouseLeftButtonDown;
        resizeBox.MouseLeftButtonUp += ResizeBoxOnMouseLeftButtonUp;
      }
    }

    private void ResizeBoxOnMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
    {
      _isWiden = true;

      args.Handled = true;
    }

    private void ResizeBoxOnMouseLeftButtonUp(object sender, MouseButtonEventArgs args)
    {
      _isWiden = false;

      var element = (UIElement)sender;
      element.ReleaseMouseCapture();

      args.Handled = true;
    }

    private void ResizeBoxOnMouseMove(object sender, MouseEventArgs args)
    {
      if (_isWiden)
      {
        var element = (UIElement)sender;
        element.CaptureMouse();

        var pos = args.GetPosition(this);

        var w = pos.X + 15;
        if (w > 0)
        {
          if (w < MinWidth)
            w = MinWidth;

          Width = w;
        }

        var h = pos.Y + 15;
        if (h > 0)
        {
          if (h < MinHeight)
            h = MinHeight;

          Height = h;
        }
      }
    }

    public static T Show<T>(T confirmation, FrameworkElement content, string style) where T : INotification
    {
      var dialog = new Dialog
      {
        Content = content,
        Style = (Style)Application.Current.Resources[style],
        WindowStartupLocation = WindowStartupLocation.CenterScreen
      };

      var viewModel = (IInteractionRequestAware)content.DataContext;
      viewModel.Notification = confirmation;
      viewModel.FinishInteraction = () => dialog.Close();

      dialog.ShowDialog();

      return confirmation;
    }
  }
}
