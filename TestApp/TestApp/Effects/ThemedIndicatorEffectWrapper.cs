using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace TestApp.Effects
{


    public static class ThemedIndicatorEffectWrapper
    {


        /// <summary>
        /// Add the attached property to the themed effect.
        /// </summary>
        /// <typeparam name="TEffect">The effect type</typeparam>
        /// <typeparam name="TProp">The attached property type</typeparam>
        /// <param name="bindable">The View which the effect is attached to</param>
        /// <param name="oldValue">The previous value</param>
        /// <param name="newValue">The new value</param>
        private static void OnThemedPropertyChanged<TEffect, TProp>(BindableObject bindable, object oldValue, object newValue) where TEffect : Effect, new()
        {
            if (bindable is View view)
            {
                if (EqualityComparer<TProp>.Equals(newValue, default(TProp)))
                {
                    var toRemove = view.Effects.FirstOrDefault(e => e is TEffect);
                    if (toRemove != null)
                        view.Effects.Remove(toRemove);
                }
                else
                    view.Effects.Add(new TEffect());
            }
        }



        #region TabIndicatorColorEffect

        /// <summary>
        /// Color of the indicator when selecting a tab in a TabPage
        /// </summary>
        public static readonly BindableProperty SelectedIndicatorColorProperty = BindableProperty.CreateAttached(
            propertyName: "SelectedIndicatorColor",
            returnType: typeof(Color),
            declaringType: typeof(ThemedIndicatorEffectWrapper), 
            defaultValue: null, 
            propertyChanged: OnThemedPropertyChanged<SelectedTabIndicatorColorEffect, Color>);

        public static Color GetSelectedIndicatorColor(BindableObject view) => (Color)view.GetValue(SelectedIndicatorColorProperty);

        public static void SetSelectedIndicatorColor(BindableObject view, Color indicatorColor) => view.SetValue(SelectedIndicatorColorProperty, indicatorColor);

        /// <summary>
        /// Thickness of the indicator when selecting a tab in a TabPage
        /// </summary>
        public static readonly BindableProperty SelectedIndicatorThicknessProperty = BindableProperty.CreateAttached(
            propertyName: "SelectedIndicatorThickness",
            returnType: typeof(int),
            declaringType: typeof(ThemedIndicatorEffectWrapper), 
            defaultValue: -1, 
            propertyChanged: OnThemedPropertyChanged<SelectedTabIndicatorColorEffect, int>);

        public static int GetSelectedIndicatorThickness(BindableObject view) => (int)view.GetValue(SelectedIndicatorThicknessProperty);

        public static void SetSelectedIndicatorThickness(BindableObject view, int indicatorColor) => view.SetValue(SelectedIndicatorThicknessProperty, indicatorColor);


        /// <summary>
        /// Change the color of the selected tab indicator.
        /// Developer Note: UWP apps might not need it as it seems that this is kust a styling task, please have a look at <see cref="TestApp.UWP.App"/> for some insights.
        /// </summary>
        private class SelectedTabIndicatorColorEffect : RoutingEffect
        {

            /// <summary>
            /// Change the color of the selected tab indicator.
            /// Developer Note: UWP apps might not need it as it seems that this is kust a styling task, please have a look at <see cref="TestApp.UWP.App"/> for some insights.
            /// 
            /// </summary>
            public SelectedTabIndicatorColorEffect() : base("TestApp.SelectedTabIndicatorColorEffect") { }
        }
        #endregion

    }
}
