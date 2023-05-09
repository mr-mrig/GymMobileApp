using System;
using Android.Widget;
using TestApp.Droid.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportEffect(typeof(BorderlessEntryEffectDroid), "BorderlessEntryEffect")]
namespace TestApp.Droid.Effects
{
    public class BorderlessEntryEffectDroid : PlatformEffect
    {

        private EditText _control;

        protected override void OnAttached()
        {
            try
            {
                _control = Control as EditText;
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
            try
            {
                if (_control != null)
                    _control.Background = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}