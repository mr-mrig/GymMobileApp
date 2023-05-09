using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestApp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FloatingLabelEntry : ContentView
    {

        public const int PlaceholderFontSize = 18;
        public const int TitleFontSize = 14;
        public const int TopMargin = -32;

        public event EventHandler Completed;


        /// <summary>
        /// The entry text
        /// </summary>
        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            propertyName: nameof(Text),
            returnType: typeof(string),
            declaringType: typeof(string),
            defaultValue: string.Empty,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: TextPropertyChanged);

        /// <summary>
        /// The title which appears on the floating label
        /// </summary>
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
            propertyName: nameof(Title),
            returnType: typeof(string),
            declaringType: typeof(string),
            defaultValue: string.Empty,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: null);


        /// <summary>
        /// The return type, IE: the type of the send key on the soft keyboard
        /// </summary>
        public static readonly BindableProperty ReturnTypeProperty = BindableProperty.Create(
            propertyName: nameof(ReturnType),
            returnType: typeof(ReturnType),
            declaringType: typeof(FloatingLabelEntry),
            defaultValue: ReturnType.Default);

        /// <summary>
        /// Tells wether the entry is for password input
        /// </summary>
        public static readonly BindableProperty IsPasswordProperty = BindableProperty.Create(
            propertyName: nameof(IsPassword),
            returnType: typeof(bool),
            declaringType: typeof(FloatingLabelEntry),
            defaultValue: false);

        /// <summary>
        /// The soft keyboard type
        /// </summary>
        public static readonly BindableProperty KeyboardProperty = BindableProperty.Create(
            propertyName: nameof(Keyboard),
            returnType: typeof(Keyboard),
            declaringType: typeof(FloatingLabelEntry),
            defaultValue: Keyboard.Default,
            coerceValue: (o, v) => (Keyboard)v ?? Keyboard.Default);


        /// <summary>
        /// The text entry color
        /// </summary>
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
            propertyName: nameof(TextColor),
            returnType: typeof(Color),
            declaringType: typeof(FloatingLabelEntry),
            defaultValue: Color.Default,
            defaultBindingMode: BindingMode.OneWay);


        /// <summary>
        /// The floating label color, which is the same as the placeholder color
        /// </summary>
        public static readonly BindableProperty FloatingLabelColorProperty = BindableProperty.Create(
            propertyName: nameof(FloatingLabelColor),
            returnType: typeof(Color),
            declaringType: typeof(FloatingLabelEntry),
            defaultValue: Color.Default,
            defaultBindingMode: BindingMode.OneWay);


        static async void TextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is FloatingLabelEntry control)
            {
                if (!control.InputEntry.IsFocused)
                {
                    if (!string.IsNullOrEmpty((string)newValue))
                        await control.TransitionToTitle(false);
                    else
                        await control.TransitionToPlaceholder(false);
                }
            }
        }

        #region Backing Properties
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public Color FloatingLabelColor
        {
            get => (Color)GetValue(FloatingLabelColorProperty);
            set => SetValue(FloatingLabelColorProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public ReturnType ReturnType
        {
            get => (ReturnType)GetValue(ReturnTypeProperty);
            set => SetValue(ReturnTypeProperty, value);
        }

        public bool IsPassword
        {
            get { return (bool)GetValue(IsPasswordProperty); }
            set { SetValue(IsPasswordProperty, value); }
        }

        public Keyboard Keyboard
        {
            get { return (Keyboard)GetValue(KeyboardProperty); }
            set { SetValue(KeyboardProperty, value); }
        }

        #endregion


        public FloatingLabelEntry()
        {
            InitializeComponent();
            TitleLabel.TranslationX = 10;
            TitleLabel.FontSize = PlaceholderFontSize;
        }

        public new void Focus()
        {
            if (IsEnabled)
                InputEntry.Focus();
        }


        #region Animations
        async Task TransitionToTitle(bool animated)
        {
            if (animated)
            {
                Task<bool> t1 = TitleLabel.TranslateTo(0, TopMargin, 100);
                Task t2 = ScaleFloatingLabelTo(TitleFontSize);
                await Task.WhenAll(t1, t2);
            }
            else
            {
                TitleLabel.TranslationX = 0;
                TitleLabel.TranslationY = -30;
                TitleLabel.FontSize = TitleFontSize;
            }
        }

        async Task TransitionToPlaceholder(bool animated)
        {
            if (animated)
            {
                Task<bool> t1 = TitleLabel.TranslateTo(10, 0, 100);
                Task t2 = ScaleFloatingLabelTo(PlaceholderFontSize);
                await Task.WhenAll(t1, t2);
            }
            else
            {
                TitleLabel.TranslationX = 10;
                TitleLabel.TranslationY = 0;
                TitleLabel.FontSize = PlaceholderFontSize;
            }
        }

        /// <summary>
        /// Scale the Title floating label to the specified font size
        /// </summary>
        /// <param name="fontSize">The target font size</param>
        /// <returns>The animation Task</returns>
        protected virtual Task ScaleFloatingLabelTo(int fontSize)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            TitleLabel.Animate(
                name: "invis",
                callback: input => { TitleLabel.FontSize = input; }, 
                start: TitleLabel.FontSize, 
                end: fontSize, 
                rate: 5, 
                length: 100, 
                easing: Easing.CubicInOut,
                finished: (v, c) => taskCompletionSource.SetResult(c));

            return taskCompletionSource.Task;
        }
        #endregion
       


        #region Event Handlers
        void OnEntryCompleted(object sender, EventArgs e)
        {
            Completed?.Invoke(this, e);
        }

        void OnControlTapped(object sender, EventArgs e)
        {
            if (IsEnabled)
                InputEntry.Focus();
        }

        async void OnEntryFocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
                await TransitionToTitle(true);
        }

        async void OnEntryUnfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
                await TransitionToPlaceholder(true);
        }
        #endregion


        protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(IsEnabled))
                InputEntry.IsEnabled = IsEnabled;
        }
    }
}