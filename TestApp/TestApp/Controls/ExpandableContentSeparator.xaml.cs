using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestApp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExpandableContentSeparator : ContentView
    {

        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
            propertyName: nameof(Title),
            returnType: typeof(string),
            declaringType: typeof(ExpandableContentSeparator),
            defaultValue: null,
            defaultBindingMode: BindingMode.OneTime);

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }


        public static readonly BindableProperty IsExpandedProperty = BindableProperty.Create(
            propertyName: nameof(IsExpanded),
            returnType: typeof(bool),
            declaringType: typeof(ExpandableContentSeparator),
            defaultValue: false,
            defaultBindingMode: BindingMode.OneWay);

        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }



        public ExpandableContentSeparator()
        {
            InitializeComponent();
        }


        private void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            IsExpanded = !IsExpanded;
        }

    }
}