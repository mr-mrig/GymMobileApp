using Xamarin.Forms;

namespace TestApp.Controls.Templated
{

    /// <summary>
    /// Custom Tabbed Page which adds the possibility to set the tab indicator color and Width
    /// </summary>
    public class CustomIndicatorTabbedPage : TabbedPage
    {


        /// <summary>
        /// The selected tab indicator color
        /// </summary>
        public static readonly BindableProperty SelectedTabIndicatorColorProperty = BindableProperty.CreateAttached(
            propertyName: nameof(SelectedTabIndicatorColor),
            returnType: typeof(Color),
            declaringType: typeof(CustomIndicatorTabbedPage),
            defaultValue: Color.Default,
            defaultBindingMode: BindingMode.OneWay);


        /// <summary>
        /// The selected tab indicator color
        /// </summary>
        public Color SelectedTabIndicatorColor
        {
            get => (Color)GetValue(SelectedTabIndicatorColorProperty);
            set => SetValue(SelectedTabIndicatorColorProperty, value);
        }


        /// <summary>
        /// The selected tab indicator width
        /// </summary>
        public static readonly BindableProperty SelectedTabIndicatorWidthProperty = BindableProperty.CreateAttached(
            propertyName: nameof(SelectedTabIndicatorWidth),
            returnType: typeof(int),
            declaringType: typeof(CustomIndicatorTabbedPage),
            defaultValue: -1,
            defaultBindingMode: BindingMode.OneWay);


        /// <summary>
        /// The selected tab indicator width
        /// </summary>
        public int SelectedTabIndicatorWidth
        {
            get => (int)GetValue(SelectedTabIndicatorWidthProperty);
            set => SetValue(SelectedTabIndicatorWidthProperty, value);
        }


    }
}
