using System.Windows.Input;
using TestApp.Models.Common;
using TestApp.ViewModels.Base;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestApp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SimpleTag : ContentView
    {

        private ICommand _onRemoveCommand;

        public static readonly BindableProperty ReadOnlyProperty = BindableProperty.Create(
            propertyName: nameof(ReadOnly),
            returnType: typeof(bool),
            declaringType: typeof(SimpleTag),
            defaultValue: true,
            defaultBindingMode: BindingMode.OneWay);


        public static readonly BindableProperty RemoveCommandProperty = BindableProperty.Create(
            propertyName: nameof(RemoveCommand),
            returnType: typeof(ICommand),
            declaringType: typeof(SimpleTag),
            defaultValue: null,
            defaultBindingMode: BindingMode.OneWay);


        public static readonly BindableProperty ValueProperty = BindableProperty.Create(
            propertyName: nameof(Value),
            returnType: typeof(ISimpleTagElement),
            declaringType: typeof(SimpleTag),
            defaultValue: null,
            defaultBindingMode: BindingMode.OneWay);



        #region Backing Fields

        public bool ReadOnly
        {
            get => (bool)GetValue(ReadOnlyProperty);
            set => SetValue(ReadOnlyProperty, value);
        }

        public ICommand RemoveCommand
        {
            get => (ICommand)GetValue(RemoveCommandProperty);
            set => SetValue(RemoveCommandProperty, value);
        }

        public ISimpleTagElement Value
        {
            get => (ISimpleTagElement)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        #endregion


        public ICommand OnRemoveCommand => _onRemoveCommand ?? (_onRemoveCommand = 
            new Command(async x =>
            {
                if(RemoveCommand != null && RemoveCommand.CanExecute(x))
                {
                    if (RemoveCommand is ICommandAsync<ISimpleTagElement> asyncCommand)
                        await asyncCommand.ExecuteAsync(x as ISimpleTagElement);
                    else
                        RemoveCommand.Execute(x);
                }
            }, x => IsEnabled));


        public SimpleTag()
        {
            InitializeComponent();
        }
    }
}