using System.ComponentModel;

using Android.Content;
using TestApp.Controls.Templated;
using TestApp.Droid.Extensions;
using TestApp.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ExtendedButton), typeof(ExtendedButtonRenderer))]
namespace TestApp.Droid.Renderers
{
    public class ExtendedButtonRenderer : ButtonRenderer
    {

        public ExtendedButtonRenderer(Context context) : base(context)
        {

        }


        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            UpdateAlignment();
            UpdateFont();
        }


        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ExtendedButton.VerticalContentAlignmentProperty.PropertyName ||
                e.PropertyName == ExtendedButton.HorizontalContentAlignmentProperty.PropertyName)
                UpdateAlignment();

            else if (e.PropertyName == Xamarin.Forms.Button.FontProperty.PropertyName)
                UpdateFont();

            base.OnElementPropertyChanged(sender, e);
        }

        private void UpdateFont()
        {
            Control.Typeface = Element.Font.ToExtendedTypeface(Context);
        }

        private void UpdateAlignment()
        {
            if (Element is ExtendedButton element && Control != null)
            {
                Control.Gravity = element.VerticalContentAlignment.ToDroidVerticalGravityFlags() |
                    element.HorizontalContentAlignment.ToDroidHorizontalGravityFlags();
            }
        }
    }
}