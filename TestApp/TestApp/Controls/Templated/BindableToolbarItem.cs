using System.Linq;
using Xamarin.Forms;

namespace TestApp.Controls.Templated
{

    /// <summary>
    /// Class for generating a ToolbarItem that can be dynamically displayed/hidden
    /// </summary>
    public class BindableToolbarItem : ToolbarItem
    {



        public static readonly BindableProperty IsVisibleProperty = BindableProperty.Create(
            propertyName: nameof(IsVisible),
            returnType: typeof(bool),
            declaringType: typeof(BindableToolbarItem),
            defaultValue: true, 
            propertyChanged: OnIsVisibleChanged);


        private static void OnIsVisibleChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            Device.BeginInvokeOnMainThread(() 
                => (bindable as BindableToolbarItem).SetVisibility(oldvalue, newvalue));
        }



        #region Backing Properties

        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }
        #endregion



        public BindableToolbarItem()
        {
            // Do not init visibility here as the parent is not set yet
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();
            SetVisibility(false, IsVisible);
        }


        private void SetVisibility(object oldValue, object newValue)
        {
            if (Parent == null)
                return;

            var items = (Parent as Page)?.ToolbarItems;

            if (items != null)
            {
                //if ((bool)newValue && !items.Contains(this))
                //    Device.BeginInvokeOnMainThread(() => items.Add(this));

                if ((bool)newValue && !items.Contains(this))
                {
                    // Find the insertion point according to the priority. This to avoid unordered items beacuse of delay among threads
                    ToolbarItem nextItem = items.FirstOrDefault(i => i.Priority > Priority);

                    int index = (nextItem != null) ? 
                        items.IndexOf(nextItem) : 
                        items.Count - 1;

                    Device.BeginInvokeOnMainThread(() => items.Insert(index, this));
                }
                else if (!(bool)newValue && items.Contains(this))
                    Device.BeginInvokeOnMainThread(() => items.Remove(this));
            }
        }
    }
}
