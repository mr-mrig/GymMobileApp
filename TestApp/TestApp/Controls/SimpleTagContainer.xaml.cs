using System.Collections;
using System.Windows.Input;
using TestApp.ViewModels.Base;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestApp.Controls
{ 
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SimpleTagContainer : ContentView
    {

        private ICommand _removeTagCommand;


        public static readonly BindableProperty ReadOnlyProperty = BindableProperty.Create(
            propertyName: nameof(ReadOnly),
            returnType: typeof(bool),
            declaringType: typeof(SimpleTagContainer),
            defaultValue: true,
            defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            propertyName: nameof(ItemsSource),
            returnType: typeof(IEnumerable),
            declaringType: typeof(SimpleTagContainer),
            defaultValue: null,
            defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty OnRemoveCommandProperty = BindableProperty.Create(
            propertyName: nameof(OnRemoveCommand),
            returnType: typeof(ICommand),
            declaringType: typeof(SimpleTagContainer),
            defaultValue: null,
            defaultBindingMode: BindingMode.OneWay);


        #region Backing Properties

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public bool ReadOnly
        {
            get => (bool)GetValue(ReadOnlyProperty);
            set => SetValue(ReadOnlyProperty, value);
        }

        public ICommand OnRemoveCommand
        {
            get => (ICommand)GetValue(OnRemoveCommandProperty);
            set => SetValue(OnRemoveCommandProperty, value);
        }
        #endregion



        public ICommand RemoveTagCommand => _removeTagCommand ?? (_removeTagCommand =
            new Command(async x =>
            {
                if(OnRemoveCommand != null && OnRemoveCommand.CanExecute(x))
                {
                    if (OnRemoveCommand is ICommandAsync<object> asyncCommand)
                        await asyncCommand.ExecuteAsync(x);
                    else
                        OnRemoveCommand.Execute(x);
                }
            }, x => IsEnabled));

        public SimpleTagContainer()
        {
            InitializeComponent();
        }


   
    }
}