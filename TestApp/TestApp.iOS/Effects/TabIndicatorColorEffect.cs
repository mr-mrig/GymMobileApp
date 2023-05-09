using System;
using System.ComponentModel;
using CoreGraphics;
using TestApp.Effects;
using TestApp.iOS.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("TestApp")]
[assembly: ExportEffect(typeof(TabIndicatorColorEffect), "SelectedTabIndicatorColorEffect")]
namespace TestApp.iOS.Effects
{
    public class TabIndicatorColorEffect : PlatformEffect
    {


        private const int DefaultIndicatorWidth = 4;
        private UITabBar _tabBar;


        #region Overrides
        protected override void OnAttached()
        {
            try
            {
                _tabBar = Control as UITabBar;
                DrawTabIndicator();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot set property on attached control. Error: ", ex.Message);
            }
        }

        protected override void OnDetached()
        {
            _tabBar = null;
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

            UITabBar.Appearance.SelectionIndicatorImage =  SetIndicatorColor(
                ThemedIndicatorEffectWrapper.GetSelectedIndicatorColor(Element).ToUIColor(),
                new CGSize(UIScreen.MainScreen.Bounds.Width / _tabBar.Items.Length, _tabBar.Bounds.Size.Height + indicatorThickness), 
                new CGSize(UIScreen.MainScreen.Bounds.Width / _tabBar.Items.Length, indicatorThickness));
        }

        private UIImage SetIndicatorColor(UIColor color, CGSize size, CGSize lineSize)
        {
            CGRect rect = new CGRect(0, 0, size.Width, size.Height);
            CGRect rectLine = new CGRect(0, size.Height - lineSize.Height, lineSize.Width, lineSize.Height);

            UIGraphics.BeginImageContextWithOptions(size, false, 0);

            UIColor.Clear.SetFill();
            UIGraphics.RectFill(rect);
            color.SetFill();
            UIGraphics.RectFill(rectLine);

            UIImage img = UIGraphics.GetImageFromCurrentImageContext();

            UIGraphics.EndImageContext();
            return img;
        }
    }
}