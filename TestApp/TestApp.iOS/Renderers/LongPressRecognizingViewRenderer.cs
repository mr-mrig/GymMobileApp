using TestApp.Controls.Templated;
using TestApp.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(LongPressRecognizingView), typeof(LongPressRecognizingViewRenderer))]
namespace TestApp.iOS.Renderers
{

    public class LongPressRecognizingViewRenderer : ViewRenderer<LongPressRecognizingView, UIView>
    {
        UILongPressGestureRecognizer longPressGestureRecognizer;


        protected override void OnElementChanged(ElementChangedEventArgs<LongPressRecognizingView> e)
        {
            longPressGestureRecognizer = longPressGestureRecognizer ??
                new UILongPressGestureRecognizer(() =>
                {
                    Element.RaiseLongPressEvent();
                });

            if (longPressGestureRecognizer != null)
            {
                if (e.NewElement == null)
                    RemoveGestureRecognizer(longPressGestureRecognizer);

                else if (e.OldElement == null)
                    AddGestureRecognizer(longPressGestureRecognizer);
            }
        }
    }
}