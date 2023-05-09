using System;
using System.ComponentModel;
using Android.Support.Design.Widget;
using TestApp.Droid.Effects;
using TestApp.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("TestApp")]
[assembly: ExportEffect(typeof(TabIndicatorColorEffect), "SelectedTabIndicatorColorEffect")]
namespace TestApp.Droid.Effects
{

    public class TabIndicatorColorEffect : PlatformEffect
    {

        private const int DefaultIndicatorWidth = 4;
        private TabLayout _tabLayout;


        #region Overrides
        protected override void OnAttached()
        {
            try
            {
                _tabLayout = Control as TabLayout;
                DrawTabIndicator();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot set property on attached control. Error: ", ex.Message);
            }
        }

        protected override void OnDetached()
        {
            _tabLayout = null;
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            if (args.PropertyName == ThemedIndicatorEffectWrapper.SelectedIndicatorColorProperty.PropertyName)
                DrawTabIndicator();
        }
        #endregion



        private void DrawTabIndicator()
        {
            int indicatorThickness = ThemedIndicatorEffectWrapper.GetSelectedIndicatorThickness(Element);

            if (indicatorThickness < 0)
                indicatorThickness = DefaultIndicatorWidth;

            _tabLayout.SetSelectedTabIndicatorColor(ThemedIndicatorEffectWrapper.GetSelectedIndicatorColor(Element).ToAndroid());
            _tabLayout.SetSelectedTabIndicatorGravity(indicatorThickness);
        }
    }
}