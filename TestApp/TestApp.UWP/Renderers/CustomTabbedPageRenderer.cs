//using System;
//using Xamarin.Forms;
//using Xamarin.Forms.Platform.UWP;

//using Windows.UI.Xaml.Controls;
//using TestApp.Controls.Custom;
//using TestApp.UWP.Renderers;

//[assembly: ExportRenderer(typeof(TabbedPage), typeof(CustomTabbedPageRenderer))]
//namespace TestApp.UWP.Renderers
//{
//    public class CustomTabbedPageRenderer : TabbedPageRenderer
//    {

//        protected const int LineBottomOffset = 8;
//        protected const int DefaultLineWidth = 4;

//        private Windows.UI.Xaml.
//        private TabLayout _tabLayout;
//        private ViewPager _viewPager;
//        private CustomIndicatorTabbedPage _tabbedPage;
//        private bool _firstTime = true;

//        public CustomTabbedPageRenderer(Context context) : base(context)
//        {
//        }


//        private void DrawTabIndicator(TabLayout.Tab item)
//        {
//            try
//            {
//                int itemHeight = _tabLayout.Height - LineBottomOffset;
//                int itemWidth = _tabLayout.Width / Element.Children.Count;
//                int leftOffset = item.Position * itemWidth;
//                int rightOffset = itemWidth * (Element.Children.Count - (item.Position + 1));

//                GradientDrawable bottomLine = new GradientDrawable();
//                bottomLine.SetShape(ShapeType.Line);
//                bottomLine.SetStroke(_tabbedPage.SelectedTabIndicatorWidth, _tabbedPage.SelectedTabIndicatorColor.ToAndroid());

//                LayerDrawable layerDrawable = new LayerDrawable(new Drawable[] { bottomLine });
//                layerDrawable.SetLayerInset(0, leftOffset, itemHeight, rightOffset, 0);

//                _tabLayout.SetBackground(layerDrawable);
//            }
//            catch (Exception ex)
//            {
//                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
//            }
//        }


//        #region Overrides
//        protected override void OnElementChanged(ElementChangedEventArgs<TabbedPage> args)
//        {
//            base.OnElementChanged(args);

//            _tabLayout = ViewGroup.FindChildOfType<TabLayout>();
//            _tabbedPage = args.NewElement as CustomIndicatorTabbedPage;

//            if (_tabLayout == null || _tabbedPage == null)
//                return;

//            _viewPager = (ViewPager)GetChildAt(0);

//            _tabLayout.TabSelected += (s, e) =>
//            {
//                Page page = _tabbedPage.Children[e.Tab.Position];
//                DrawTabIndicator(e.Tab);
//                _viewPager.SetCurrentItem(e.Tab.Position, false);
//            };
//        }


//        protected override void OnLayout(bool changed, int l, int t, int r, int b)
//        {
//            base.OnLayout(changed, l, t, r, b);

//            if (!_firstTime)
//                return;

//            DrawTabIndicator(_tabLayout.GetTabAt(_tabLayout.SelectedTabPosition));
//            _firstTime = false;
//        }

//        //protected override void DispatchDraw(Canvas canvas)
//        //{
//        //    base.DispatchDraw(canvas);

//        //    if (!_firstTime)
//        //        return;

//        //    for (int i = 0; i < _tabLayout.TabCount; i++)
//        //    {
//        //        var tab = _tabLayout.GetTabAt(i);

//        //        DrawTabItem(tab);

//        //        if (!string.IsNullOrEmpty(_tabbedPage.Title))
//        //            tab.SetText(string.Empty);
//        //    }

//        //    _firstTime = false;
//        //}
//        #endregion
//    }
//}