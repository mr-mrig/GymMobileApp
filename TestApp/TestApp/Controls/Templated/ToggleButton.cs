using System;
using System.Threading.Tasks;
using System.Windows.Input;
using TestApp.ViewModels.Base;
using Xamarin.Forms;

namespace TestApp.Controls.Templated
{
    public class ToggleButton : ContentView
    {


        private ICommand _toggleCommand;
        private Image _toggleImage;



        public static readonly BindableProperty CommandProperty = BindableProperty.Create(
            propertyName: nameof(Command), 
            returnType: typeof(ICommand),
            declaringType: typeof(ToggleButton),
            defaultValue: null);

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(
            propertyName: nameof(CommandProperty), 
            returnType: typeof(object),
            declaringType: typeof(ToggleButton),
            defaultValue: null);

        public static readonly BindableProperty CheckedProperty = BindableProperty.Create(
            propertyName: nameof(Checked),
            returnType: typeof(bool),
            declaringType: typeof(ToggleButton),
            defaultValue: false,
            propertyChanged: OnCheckedChanged);

        public static readonly BindableProperty AnimateProperty = BindableProperty.Create(
            propertyName: nameof(Animate),
            returnType: typeof(bool),
            declaringType: typeof(ToggleButton),
            defaultValue: false);

        public static readonly BindableProperty CheckedImageProperty = BindableProperty.Create(
            propertyName: nameof(CheckedImage),
            returnType: typeof(ImageSource),
            declaringType: typeof(ToggleButton),
            defaultValue: null,
            propertyChanged: CheckedImagePropertyChanged);


        public static readonly BindableProperty UnCheckedImageProperty = BindableProperty.Create(
            propertyName: nameof(UnCheckedImage), 
            returnType: typeof(ImageSource),
            declaringType: typeof(ToggleButton),
            defaultValue: null,
            propertyChanged: UnCheckedImagePropertyChanged);


        #region Backing Properties
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public bool Checked
        {
            get { return (bool)GetValue(CheckedProperty); }
            set { SetValue(CheckedProperty, value); }
        }

        public bool Animate
        {
            get { return (bool)GetValue(AnimateProperty); }
            set { SetValue(CheckedProperty, value); }
        }

        public ImageSource CheckedImage
        {
            get { return (ImageSource)GetValue(CheckedImageProperty); }
            set { SetValue(CheckedImageProperty, value); }
        }

        public ImageSource UnCheckedImage
        {
            get { return (ImageSource)GetValue(UnCheckedImageProperty); }
            set { SetValue(UnCheckedImageProperty, value); }
        }

        #endregion


        #region Bindable Properties Methods
        private static async void OnCheckedChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var toggleButton = (ToggleButton)bindable;

            if (Equals(newValue, null) && !Equals(oldValue, null))
                return;

            toggleButton._toggleImage.Source = toggleButton.Checked ?
                toggleButton.CheckedImage :
                toggleButton.UnCheckedImage;

            toggleButton.Content = toggleButton._toggleImage;

            if (toggleButton.Animate)
            {
                await toggleButton.ScaleTo(0.9, 50, Easing.Linear);
                await Task.Delay(100);
                await toggleButton.ScaleTo(1, 50, Easing.Linear);
            }
        }

        private static void UnCheckedImagePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ToggleButton button)
            {
                // To be done here as the constructor won't have the properties set
                if (!button.Checked)
                    button.InitImage(newValue as ImageSource);
            }
        }

        private static void CheckedImagePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if(bindable is ToggleButton button)
            {
                // To be done here as the constructor won't have the properties set
                if(button.Checked)
                    button.InitImage(newValue as ImageSource);
            }
        }
        #endregion


        public ICommand ToggleCommand
        {
            get => _toggleCommand ?? (_toggleCommand = new Command(async () =>
            {
                Checked = !Checked;

                if(Command != null && Command.CanExecute(CommandParameter))
                {
                    if (Command is ICommandAsync<object> asyncCommand)
                        await asyncCommand.ExecuteAsync(CommandParameter);
                    else
                        Command?.Execute(CommandParameter);
                }
            }));
        }



        public ToggleButton()
        {
            Initialize();
        }



        private void Initialize()
        {
            _toggleImage = new Image();

            GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = ToggleCommand
            });
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();
        }

        protected void InitImage(ImageSource src)
        {
            _toggleImage.Source = src;
            Content = _toggleImage;
        }

    }
}