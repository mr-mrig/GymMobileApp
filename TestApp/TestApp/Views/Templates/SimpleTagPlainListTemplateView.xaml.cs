using System.Collections.Generic;
using System.Linq;
using TestApp.Models.Common;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestApp.Views.Templates
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SimpleTagPlainListTemplateView : ContentView
    {


        private const string TagsSeparator = ", ";



        /// <summary>
        /// The Tags which must be displayed
        /// </summary>
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            propertyName: nameof(ItemsSource),
            returnType: typeof(IEnumerable<ISimpleTagElement>),
            declaringType: typeof(SimpleTagPlainListTemplateView),
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: ItemSourceChanged);    // Shared with AdditionlItemsSource


        /// <summary>
        /// The Tags which must be displayed
        /// </summary>
        public IEnumerable<ISimpleTagElement> ItemsSource
        {
            get => (IEnumerable<ISimpleTagElement>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        /// <summary>
        /// The Tags which must be displayed in addition to the first ones.
        /// To be used in case more tag sources are meant to be displayed together
        /// </summary>
        public static readonly BindableProperty AdditionalItemsSourceProperty = BindableProperty.Create(
            propertyName: nameof(AdditionalItemsSource),
            returnType: typeof(IEnumerable<ISimpleTagElement>),
            declaringType: typeof(SimpleTagPlainListTemplateView),
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: ItemSourceChanged);    // Shared with ItemsSource


        /// <summary>
        /// The Tags which must be displayed in addition to the first ones.
        /// To be used in case more tag sources are meant to be displayed together
        /// </summary>
        public IEnumerable<ISimpleTagElement> AdditionalItemsSource
        {
            get => (IEnumerable<ISimpleTagElement>)GetValue(AdditionalItemsSourceProperty);
            set => SetValue(AdditionalItemsSourceProperty, value);
        }



        public SimpleTagPlainListTemplateView()
        {
            InitializeComponent();
        }


        private static void ItemSourceChanged(BindableObject sender, object oldValue, object newValue)
        {
            IEnumerable<ISimpleTagElement> items = newValue as IEnumerable<ISimpleTagElement> ?? new List<ISimpleTagElement>();

            if (sender is SimpleTagPlainListTemplateView thisView)
            {
                if(string.IsNullOrEmpty(thisView.plainList.Text))
                    thisView.plainList.Text = string.Join(TagsSeparator, items.Select(x => x.Body)).TrimEnd(',');
                else
                    thisView.plainList.Text += TagsSeparator + string.Join(TagsSeparator, items.Select(x => x.Body)).TrimEnd(',');
            }
        }

    }
}