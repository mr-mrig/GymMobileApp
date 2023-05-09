
using Xamarin.Forms;

namespace TestApp.Services.AppSession
{
    public static class AppSession
    {

        public static uint? CurrentUserId
        { 
            get => Application.Current.Properties["CurrentUserId"] as uint?;
            set
            {
                Application.Current.Properties["CurrentUserId"] = value;
                Application.Current.SavePropertiesAsync();
            }
        }

    }
}
