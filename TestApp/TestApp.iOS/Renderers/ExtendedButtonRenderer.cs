using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Foundation;
using TestApp.Controls.Templated;
using TestApp.iOS.Extensions;
using TestApp.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ExtendedButton), typeof(ExtendedButtonRenderer))]
namespace TestApp.iOS.Renderers
{
    public class ExtendedButtonRenderer : ButtonRenderer
    {

        public new ExtendedButton Element => base.Element as ExtendedButton;



        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (Element != null && Control != null)
            {
                Control.VerticalAlignment = Element.VerticalContentAlignment.ToContentVerticalAlignment();
                Control.HorizontalAlignment = Element.HorizontalContentAlignment.ToContentHorizontalAlignment();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Element.VerticalContentAlignment):
                    Control.VerticalAlignment = Element.VerticalContentAlignment.ToContentVerticalAlignment();
                    break;
                case nameof(Element.HorizontalContentAlignment):
                    Control.HorizontalAlignment = Element.HorizontalContentAlignment.ToContentHorizontalAlignment();
                    break;
                default:
                    break;
            }
            base.OnElementPropertyChanged(sender, e);
        }

    }
}