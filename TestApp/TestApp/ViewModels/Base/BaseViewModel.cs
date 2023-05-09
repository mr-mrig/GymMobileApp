using System;
using System.Threading.Tasks;

namespace TestApp.ViewModels.Base
{
    public class BaseViewModel : ExtendedBindableObject, IDisposable
    {

        //protected readonly INavigationService NavigationService;

        /// <summary>
        /// <para>Tell whether the VM should be automatically disposed when the Back Button is pressed - The default value is True.</para>
        /// <para>The automatic dispose happens only when the linked View is <seealso cref="ContentView"/></para>
        /// </summary>
        public virtual bool ShouldDisposeOnBack { get; set; } = true;


        /// <summary>
        /// Do not use this for content initialization, but for DI-related stuff only
        /// </summary>
        public BaseViewModel()
        {
            //NavigationService = ViewModelLocator.Resolve<INavigationService>();
        }


        /// <summary>
        /// Initialize the View Model content.
        /// This should be used instead of the Ctor to initialize the View Model, since the Navigation Service uses this one.
        /// The Ctor should be used for DI-related stuff.
        /// </summary>
        /// <param name="navigationData">Parameters needed to initialize the View Model, wrapped into an object</param>
        /// <returns></returns>
        public virtual Task InitializeAsync(object navigationData) => Task.FromResult(false);

        /// <summary>
        /// Release unmanaged resources. This should be called on page disapperaing.
        /// </summary>
        public virtual void Dispose() { }
    }
}
