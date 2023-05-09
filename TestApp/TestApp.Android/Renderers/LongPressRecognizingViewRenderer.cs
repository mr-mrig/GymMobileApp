using Android.Content;
using Android.Views;
using TestApp.Controls.Templated;
using TestApp.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(LongPressRecognizingView), typeof(LongPressRecognizingViewRenderer))]
namespace TestApp.Droid.Renderers
{

    public class LongPressRecognizingViewRenderer : ViewRenderer<LongPressRecognizingView, Android.Views.View>
    {
     
        public LongPressRecognizingViewListener Listener { get; private set; }
        public GestureDetector Detector { get; private set; }



        public LongPressRecognizingViewRenderer(Context context) : base(context) { }


        protected override void OnElementChanged(ElementChangedEventArgs<LongPressRecognizingView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                GenericMotion += HandleGenericMotion;
                Touch += HandleTouch;

                Listener = new LongPressRecognizingViewListener(Element);
                Detector = new GestureDetector(Context, Listener);
            }
        }

        protected override void Dispose(bool disposing)
        {
            GenericMotion -= HandleGenericMotion;
            Touch -= HandleTouch;

            Listener = null;
            Detector?.Dispose();
            Detector = null;

            base.Dispose(disposing);
        }

        internal void HandleTouch(object sender, TouchEventArgs e)
        {
            Detector.OnTouchEvent(e.Event);
        }

        internal void HandleGenericMotion(object sender, GenericMotionEventArgs e)
        {
            Detector.OnTouchEvent(e.Event);
        }
    }



    public class LongPressRecognizingViewListener : GestureDetector.SimpleOnGestureListener
    {
        readonly LongPressRecognizingView _target;

        public LongPressRecognizingViewListener(LongPressRecognizingView s)
        {
            _target = s;
        }

        public override void OnLongPress(MotionEvent e)
        {
            _target.RaiseLongPressEvent();

            base.OnLongPress(e);
        }
    }
}