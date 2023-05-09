using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace TestApp.Controls.Templated
{

    public enum FloatingTitlePosition : byte
    {
        /// <summary>
        /// Aligned with the Entry border
        /// </summary>
        Start,
        /// <summary>
        /// In the middle of the Entry - Not working, needs further implementation
        /// </summary>
        Center,
        /// <summary>
        /// At the end of the Entry - Not working, needs further implementation
        /// </summary>
        End,
        /// <summary>
        /// Aligned with the placeholder position
        /// </summary>
        PlaceholderAligned,
    }


    public class CustomFloatingLabelEntry : Grid, IDisposable
    {

        public const int DefaultTitleFontSize = 14;
        public const int PlaceholderLeftMargin = 10;

        protected InputView _inputEntry;
        protected Label _floatingLabel;

        public event EventHandler Completed;
        public new event EventHandler Unfocused;
        public new event EventHandler Focused;


        #region Attached Properties
        /// <summary>
        /// The template for the input element.
        /// <para>This property is mandatory, if no template is supposed to be provided then it's better off to use the not templeted version <see cref="FloatingLabelEntry"/></para> 
        /// </summary>
        public static readonly BindableProperty EntryTemplateProperty = BindableProperty.Create(
            propertyName: nameof(EntryTemplate),
            returnType: typeof(DataTemplate),
            declaringType: typeof(InputView),
            defaultValue: null,
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: EntryTemplateChanged);

        /// <summary>
        /// <para>The entry text</para>
        /// <para>IMPORTANT: does not work with something different from <see cref="BindingMode.TwoWay"/>!</para>
        /// </summary>
        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            propertyName: nameof(Text),
            returnType: typeof(string),
            declaringType: typeof(string),
            defaultValue: string.Empty,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: TextPropertyChanged);

        /// <summary>
        /// The title which appears on the floating label when it is positioned on the top
        /// </summary>
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
            propertyName: nameof(Title),
            returnType: typeof(string),
            declaringType: typeof(string),
            defaultValue: string.Empty,
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: null);

        /// <summary>
        /// The placeholder text. If not explicitly set the placeholder will be the same as the Title
        /// </summary>
        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
            propertyName: nameof(Placeholder),
            returnType: typeof(string),
            declaringType: typeof(string),
            defaultValue: string.Empty,
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: null);


        /// <summary>
        /// The floating label color, which is the same as the placeholder color
        /// </summary>
        public static readonly BindableProperty FloatingTextColorProperty = BindableProperty.Create(
            propertyName: nameof(FloatingTextColor),
            returnType: typeof(Color),
            declaringType: typeof(CustomFloatingLabelEntry),
            defaultValue: Color.Default,
            defaultBindingMode: BindingMode.OneWay);


        /// <summary>
        /// The title color when the entry is focusing, so that it looks highlighted.
        /// If not set, it is initialized with the <see cref="FloatingTextColorProperty"/>
        /// </summary>
        public static readonly BindableProperty FocusedTitleColorProperty = BindableProperty.Create(
            propertyName: nameof(FocusedTitleColor),
            returnType: typeof(Color),
            declaringType: typeof(CustomFloatingLabelEntry),
            defaultBindingMode: BindingMode.OneWay,
            defaultValueCreator: FocusedTitleColorDefaultCreator);


        /// <summary>
        /// The margin
        /// </summary>
        public static readonly BindableProperty TitlePositionProperty = BindableProperty.Create(
            propertyName: nameof(TitlePosition),
            returnType: typeof(FloatingTitlePosition),
            declaringType: typeof(CustomFloatingLabelEntry),
            defaultValue: FloatingTitlePosition.Start,
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: null);

        #endregion


        #region Bindable properties methods

        private static object FocusedTitleColorDefaultCreator(BindableObject bindable)
        {
            CustomFloatingLabelEntry control = bindable as CustomFloatingLabelEntry;
            return control.FloatingTextColor;
        }


        private static async void TextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            CustomFloatingLabelEntry control = bindable as CustomFloatingLabelEntry;

            if (!control._inputEntry.IsFocused)
            {
                if (!string.IsNullOrEmpty((string)newValue))
                {
                    await control.TransitionToTitle(false);
                    control._floatingLabel.IsVisible = true;
                }
                else
                    await control.TransitionToPlaceholder(false);
            }
        }

        private static void EntryTemplateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            // The Template might be null, which is not handled!
            if (newValue == null)
                DestroyControl(bindable as CustomFloatingLabelEntry);
            else
                InitControl(bindable as CustomFloatingLabelEntry);
        }

        /// <summary>
        /// Forces the placeholder to have the same text of the title, when the first is not set
        /// </summary>
        private static void CoerceEmptyPlaceholderToTitleValue(BindableObject bindable, object oldValue, object newValue)
        {
            CustomFloatingLabelEntry control = bindable as CustomFloatingLabelEntry;
            if (string.IsNullOrEmpty(control.Placeholder))
                control.Placeholder = newValue as string;
        }
        #endregion



        #region Backing Properties

        /// <summary>
        /// The template for the entry
        /// </summary>
        public DataTemplate EntryTemplate
        {
            get => (DataTemplate)GetValue(EntryTemplateProperty);
            set => SetValue(EntryTemplateProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public Color FloatingTextColor
        {
            get => (Color)GetValue(FloatingTextColorProperty);
            set => SetValue(FloatingTextColorProperty, value);
        }

        public Color FocusedTitleColor
        {
            get => (Color)GetValue(FocusedTitleColorProperty);
            set => SetValue(FocusedTitleColorProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public FloatingTitlePosition TitlePosition
        {
            get => (FloatingTitlePosition)GetValue(TitlePositionProperty);
            set => SetValue(TitlePositionProperty, value);
        }
        #endregion




        public CustomFloatingLabelEntry() { }

        ~CustomFloatingLabelEntry() 
        { 
            Dispose(); 
        }


        public new void Focus()
        {
            if (IsEnabled)
                _inputEntry.Focus();
        }



        #region Animations
        protected virtual async Task TransitionToTitle(bool animated)
        {
            int topMargin = -(DefaultTitleFontSize + (int)
                (_inputEntry.Height > 0 ? _inputEntry.Height : _inputEntry.HeightRequest) / 2) + 4;     // RIGM: watch out - is _inputEntry.HeightRequest correct?
            int leftMargin = GetTitleTranslationX();

            _floatingLabel.Text = Title;

            if (animated)
            {
                Task<bool> t1 = _floatingLabel.TranslateTo(leftMargin, topMargin, 100);
                Task t2 = ScaleFloatingLabelTo(DefaultTitleFontSize);
                await Task.WhenAll(t1, t2);
            }
            else
            {
                _floatingLabel.TranslationX = leftMargin;
                _floatingLabel.TranslationY = topMargin;
                _floatingLabel.FontSize = DefaultTitleFontSize;
            }
        }

        protected virtual async Task TransitionToPlaceholder(bool animated)
        {
            _floatingLabel.Text = Placeholder;
            SetPlaceholderStyle();

            if (animated)
            {
                Task<bool> t1 = _floatingLabel.TranslateTo(PlaceholderLeftMargin, 0, 100);
                Task t2 = ScaleFloatingLabelTo((int)GetFontSize());
                await Task.WhenAll(t1, t2);
            }
            else
            {
                _floatingLabel.TranslationX = PlaceholderLeftMargin;
                _floatingLabel.TranslationY = 0;
                _floatingLabel.FontSize = GetFontSize();
            }
        }

        /// <summary>
        /// Scale the Title floating label to the specified font size
        /// </summary>
        /// <param name="fontSize">The target font size</param>
        /// <returns>The animation Task</returns>
        protected virtual Task ScaleFloatingLabelTo(float fontSize)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            _floatingLabel.Animate(
                name: $"floatingLabelScaleTo_{this.Id}",
                callback: input => { _floatingLabel.FontSize = input; },
                start: _floatingLabel.FontSize,
                end: fontSize,
                rate: 5,
                length: 100,
                easing: Easing.CubicInOut,
                finished: (v, c) => taskCompletionSource.SetResult(c));

            return taskCompletionSource.Task;
        }
        #endregion

        


        #region Event Handlers
        private void OnEntryCompleted(object sender, EventArgs e)
        {
            // Propagate event
            Completed?.Invoke(this, e);     //RIGM: Entry does not raise this event, at least onib UWP
        }

        private async void OnEntryFocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
            {
                _floatingLabel.IsVisible = true;
                _inputEntry.Placeholder = string.Empty;

                await TransitionToTitle(true);
            }
            SetTitleStyle();

            // Propagate event
            Focused?.Invoke(this, e);
        }

        private async void OnEntryUnfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
            {
                await TransitionToPlaceholder(true);

                _floatingLabel.IsVisible = false;
                _inputEntry.Placeholder = Placeholder;
            }
            SetPlaceholderStyle();

            // Propagate event
            Unfocused?.Invoke(this, e);
        }
        #endregion


        protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(IsEnabled))
                _inputEntry.IsEnabled = IsEnabled;
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            base.LayoutChildren(x, y, width, height);

            // When the editor is redrawn because of a AutoSize="TextChanged" then the title must be adapted to the new control position to be placed still on top
            if (_inputEntry is Editor)
                RedrawTitle();
        }


        private static void InitControl(CustomFloatingLabelEntry control)
        {
            // Add enough space to hold the floating label on the top
            control.Padding = new Thickness(control.Padding.Left, control.Padding.Top + DefaultTitleFontSize * 2, control.Padding.Right, control.Padding.Bottom);

            control._inputEntry = control.CreateEntryFromTemplate();
            control._floatingLabel = control.InitFloatingLabel();

            control.Children.Clear();
            control.Children.Add(control._inputEntry, 0, 0);
            control.Children.Add(control._floatingLabel, 0, 0);     // Always the last to allow higher Z-index
        }

        private static void DestroyControl(CustomFloatingLabelEntry control)
        {
            control.Dispose();
        }


        private InputView CreateEntryFromTemplate()
        {
            InputView entry = (InputView)EntryTemplate.CreateContent();
            entry.Placeholder = Placeholder;
            entry.PlaceholderColor = FloatingTextColor;
            // Bindings
            entry.BindingContext = this;
            entry.SetBinding(InputView.TextProperty, nameof(Text), BindingMode.TwoWay);     //RIGM: this prevent the Text to be used as OneWay. We should get the TextProperty BindingMode, but no simple workoround has been found
            //Events
            AttachCompletedEventHandler(entry);
            entry.Focused += OnEntryFocused;
            entry.Unfocused += OnEntryUnfocused;
            return entry;
        }

        private void AttachCompletedEventHandler(InputView entryElement)
        {
            if (entryElement is Entry entry)
                entry.Completed += OnEntryCompleted;

            else if (entryElement is Editor editor)
                editor.Completed += OnEntryCompleted;
        }

        private Label InitFloatingLabel()
        {
            // Coerce the Placeholder text, whenver not explicitly set
            if (string.IsNullOrEmpty(Placeholder))
                Placeholder = Title;

            return new Label()
            {
                FontSize = GetFontSize(),
                VerticalOptions = new LayoutOptions(LayoutAlignment.Center, false),
                TranslationX = PlaceholderLeftMargin,
                LineBreakMode = LineBreakMode.NoWrap,
                IsVisible = string.IsNullOrEmpty(Text) ? false : true,
                // Bindings
                Text = Placeholder,
                TextColor = FloatingTextColor,
            };
        }

        /// <summary>
        /// Refresh the title top position in response to a layout change.
        /// </summary>
        protected void RedrawTitle()
        {
            int topMargin = -(DefaultTitleFontSize + (int)_inputEntry.Height / 2) + 4;
            _floatingLabel.TranslationY = topMargin;
        }

        /// <summary>
        /// Modify the floating text appearance to match the placeholder style.
        /// To be called when minimizing to placeholder
        /// </summary>
        private void SetPlaceholderStyle()
        {
            _floatingLabel.TextColor = FloatingTextColor;
            _floatingLabel.FontAttributes = FontAttributes.None;
        }

        /// <summary>
        /// Modify the floating text appearance to match the title style.
        /// To be called when maximizing to title
        /// </summary>
        private void SetTitleStyle()
        {
            _floatingLabel.TextColor = FocusedTitleColor;
            _floatingLabel.FontAttributes = FontAttributes.Bold;
        }

        private int GetTitleTranslationX()
        {
            switch (TitlePosition)
            {
                case FloatingTitlePosition.PlaceholderAligned:
                    return PlaceholderLeftMargin;

                case FloatingTitlePosition.Start:
                    return 0;

                case FloatingTitlePosition.End:
                    return (int)(_inputEntry.Width - _floatingLabel.Width);             // Not working since the _floatingLabel is expanded to matche the _inputEntry

                case FloatingTitlePosition.Center:
                    return (int)((_inputEntry.Width / 2) - _floatingLabel.Width / 2);   // Not working since the _floatingLabel is expanded to matche the _inputEntry

                default:
                    return -1;
            }
        }

        private float GetFontSize()

            => _inputEntry is IFontElement fontElement
                ? (float)fontElement.FontSize
                : DefaultTitleFontSize * 1.5f;

        public void Dispose()
        {
            // Release handlers
            if (_inputEntry != null)
            {
                _inputEntry.Focused -= OnEntryFocused;
                _inputEntry.Unfocused -= OnEntryUnfocused;

                var gestureRec = GestureRecognizers.FirstOrDefault() as TapGestureRecognizer;

                if (_inputEntry is Editor editor)
                    editor.Completed -= OnEntryCompleted;
                if (_inputEntry is Entry entry)
                    entry.Completed -= OnEntryCompleted;
            }
        }
    }
}
