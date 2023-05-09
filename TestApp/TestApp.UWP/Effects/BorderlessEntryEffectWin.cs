using System;
using System.Diagnostics;
using Xaml = Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using TestApp.UWP.Effects;

[assembly: ResolutionGroupName("TestApp")]
[assembly: ExportEffect(typeof(BorderlessEntryEffectWin), "BorderlessEntryEffect")]
namespace TestApp.UWP.Effects
{
    public class BorderlessEntryEffectWin : PlatformEffect
    {
        private TextBox _control;

        protected override void OnAttached()
        {
            try
            {
                _control = Control as TextBox;
                RemoveBorders();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Cannot set property on attached control. Error: ", ex.Message);
            }
        }

        protected override void OnDetached()
        {
            _control = null;
        }


        private void RemoveBorders()
        {
            if (_control != null)
            {
                _control.BorderThickness = new Xaml::Thickness(0, 0, 0, 0);
                //var lineColor = XamarinFormColorToWindowsColor(LineColorBehavior.GetLineColor(Element));
                //control.BorderBrush = new Media.SolidColorBrush(lineColor);

                //var style = Xaml::Application.Current.Resources["FormTextBoxStyle"] as Xaml::Style;
                //control.Style = style;
            }
        }

    }
}
