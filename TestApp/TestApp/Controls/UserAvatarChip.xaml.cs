using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestApp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserAvatarChip : ContentView
    {



        /// <summary>
        /// The profile image URL
        /// </summary>
        public static readonly BindableProperty ProfileImageUrlProperty = BindableProperty.Create(
            propertyName: nameof(ProfileImageUrl),
            returnType: typeof(string),
            declaringType: typeof(UserAvatarChip),
            defaultValue: null,
            defaultBindingMode: BindingMode.OneTime);

        /// <summary>
        /// The profile image URL
        /// </summary>
        public string ProfileImageUrl
        {
            get => (string)GetValue(ProfileImageUrlProperty);
            set => SetValue(ProfileImageUrlProperty, value);
        }


        /// <summary>
        /// The user name
        /// </summary>
        public static readonly BindableProperty UserNameProperty = BindableProperty.Create(
            propertyName: nameof(UserName),
            returnType: typeof(string),
            declaringType: typeof(UserAvatarChip),
            defaultValue: null,
            defaultBindingMode: BindingMode.OneTime);

        /// <summary>
        /// The user name
        /// </summary>
        public string UserName
        {
            get => (string)GetValue(UserNameProperty);
            set => SetValue(UserNameProperty, value);
        }



        public UserAvatarChip()
        {
            InitializeComponent();
        }
    }
}