using System;
using Xamarin.Forms;

namespace TestApp.Controls.Templated
{


    /// <summary>
    /// Class from which to inherit when the view is required to recognize prolonged pressures
    /// </summary>
    public class LongPressRecognizingView : ContentView
    {
        public event EventHandler<EventArgs> LongPressEvent;

        public void RaiseLongPressEvent()
        {
            if (IsEnabled)
                LongPressEvent?.Invoke(this, EventArgs.Empty);
        }



        /// <summary>
        /// Class from which to inherit when the view is required to recognize prolonged pressures
        /// </summary>
        public LongPressRecognizingView() : base() { }
    }
}
