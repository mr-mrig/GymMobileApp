using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using TestApp.Services.Utils;
using Xamarin.Forms;

namespace TestApp.Controls.Templated
{
    public class ExtendedButton : Button
    {


        //private IDebouncerService _debouncer = new DebouncerService();


        /// <summary>
        /// Content vertical alignment.
        /// </summary>
        public static readonly BindableProperty VerticalContentAlignmentProperty = BindableProperty.Create(
                propertyName: nameof(VerticalContentAlignment),
                returnType: typeof(TextAlignment),
                declaringType: typeof(ExtendedButton),
                defaultValue: TextAlignment.Center);

        /// <summary>
        /// Content horizontal alignment.
        /// </summary>
        public static readonly BindableProperty HorizontalContentAlignmentProperty = BindableProperty.Create(
                propertyName: nameof(VerticalContentAlignment),
                returnType: typeof(TextAlignment),
                declaringType: typeof(ExtendedButton),
                defaultValue: TextAlignment.Center);

        /// <summary>
        /// Button content horizontal alignment.
        /// </summary>
        public static readonly BindableProperty DisabledBackgroundColorProperty = BindableProperty.Create(
                propertyName: nameof(DisabledBackgroundColor),
                returnType: typeof(Color),
                declaringType: typeof(ExtendedButton),
                defaultValue: Color.Default);

        /// <summary>
        /// Button content horizontal alignment.
        /// </summary>
        public static readonly BindableProperty DisabledTextColorProperty = BindableProperty.Create(
                propertyName: nameof(DisabledTextColor),
                returnType: typeof(Color),
                declaringType: typeof(ExtendedButton),
                defaultValue: Color.Default);



        #region Backing Fields
        public TextAlignment VerticalContentAlignment
        {
            get => (TextAlignment)GetValue(VerticalContentAlignmentProperty);
            set => SetValue(VerticalContentAlignmentProperty, value);
        }

        public TextAlignment HorizontalContentAlignment
        {
            get => (TextAlignment)GetValue(HorizontalContentAlignmentProperty);
            set => SetValue(HorizontalContentAlignmentProperty, value);
        }
        public Color DisabledBackgroundColor
        {
            get => (Color)GetValue(DisabledBackgroundColorProperty);
            set => SetValue(DisabledBackgroundColorProperty, value);
        }
        public Color DisabledTextColor
        {
            get => (Color)GetValue(DisabledTextColorProperty);
            set => SetValue(DisabledTextColorProperty, value);
        }
        //public Style DisabledStyle
        //{
        //    get { return (Style)GetValue(DisabledStyleProperty); }
        //    set { SetValue(DisabledStyleProperty, value); }
        //}

        #endregion


        public ExtendedButton()
        {
            //Command = new Command(() => _debouncer.DebounceAsync(1000, () => Command.Execute(CommandParameter), CommandParameter), x => Command.CanExecute(CommandParameter));
        }


        //protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //    if(propertyName == nameof(Style))
        //    {
        //        // Save the style only the first time it is set
        //        if (_enabledStyle == null)
        //            _enabledStyle = Style;
        //    }
        //    if (propertyName == nameof(IsEnabled))
        //    {
        //        if(DisabledStyle != null)
        //            Style = IsEnabled ? _enabledStyle : DisabledStyle;
        //    }
        //    base.OnPropertyChanged(propertyName);
        //}
    }
}
