using Xamarin.Forms;

namespace TestApp.Effects
{

    /// <summary>
    /// Create a borderless entry
    /// </summary>
    public class BorderlessEntryEffect : RoutingEffect
    {

        /// <summary>
        /// Create a borderless entry
        /// </summary>
        public BorderlessEntryEffect() : base("TestApp.BorderlessEntryEffect") { }


        /// <summary>
        /// Attach the effect to an Entry. This is used to allow effects XAML styling.
        /// </summary>
        public static readonly BindableProperty IsBorderlessProperty = BindableProperty.CreateAttached(
            propertyName: "IsBorderless",
            returnType: typeof(bool),
            declaringType: typeof(InputView),
            defaultValue: false,
            propertyChanged: OnBorderlessChanged);

        public static bool? GetIsBorderless(BindableObject view)
        {
            return (bool?)view.GetValue(IsBorderlessProperty);
        }

        public static void OnBorderlessChanged(BindableObject view, object oldValue, object newValue)
        {
            view.SetValue(IsBorderlessProperty, newValue);

            if((bool)view.GetValue(IsBorderlessProperty))
                ((InputView)view).Effects.Add(new BorderlessEntryEffect());
        }

    }
}
