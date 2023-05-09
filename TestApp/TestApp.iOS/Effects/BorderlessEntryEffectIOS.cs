using System;
using TestApp.iOS.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(BorderlessEntryEffectIOS), "BorderlessEntryEffect")]
namespace TestApp.iOS.Effects
{
    public class BorderlessEntryEffectIOS : PlatformEffect
    {

        private UITextField _control;

        protected override void OnAttached()
        {
            try
            {
                _control = Control as UITextField;
                RemoveBorders();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot set property on attached control. Error: ", ex.Message);
            }
        }

        protected override void OnDetached()
        {
            _control = null;
        }


        private void RemoveBorders()
        {
            _control.BorderStyle = UITextBorderStyle.None;
            _control.Layer.BorderWidth = 0f;
        }

    }
}