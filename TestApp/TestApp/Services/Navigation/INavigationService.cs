using System.Threading.Tasks;
using TestApp.ViewModels.Base;

namespace TestApp.Services.Navigation
{
    public interface INavigationService
    {

        /// <summary>
        /// The View Model the current one has been accessed from - Backward navigation support
        /// </summary>
        BaseViewModel PreviousViewModel { get; }

        /// <summary>
        /// To be called before starting the navigation - Should point to the first page
        /// </summary>
        /// <returns>The return Task</returns>
        Task InitializeAsync();

        /// <summary>
        /// Navigate to the specified View Model
        /// </summary>
        /// <typeparam name="TViewModel">The View Model type to navigate to</typeparam>
        /// <param name="dispose">Tells whether the View Model we are navigating from should be disposed</param>
        /// <returns></returns>
        Task NavigateToAsync<TViewModel>(bool dispose = false) where TViewModel : BaseViewModel;

        /// <summary>
        /// Navigate to the specified View Model passing the specified parameter.
        /// </summary>
        /// <typeparam name="TViewModel">The View Model type to navigate to</typeparam>
        /// <param name="parameter">The parameter to be passed, wrapped into an object</param>
        /// <param name="dispose">Tells whether the View Model we are navigating from should be disposed</param>
        /// <returns></returns>
        Task NavigateToAsync<TViewModel>(object parameter, bool dispose = false) where TViewModel : BaseViewModel;

        /// <summary>
        /// Open the specified popup view model
        /// </summary>
        /// <typeparam name="TViewModel">The View Model type to navigate to</typeparam>
        /// <param name="parameter">The parameter to be passed, wrapped into an object</param>
        /// <returns></returns>
        Task OpenPopup<TViewModel>(object parameter) where TViewModel : BaseViewModel;

        /// <summary>
        /// Close the current popup, provided there's one
        /// </summary>
        /// <typeparam name="TViewModel">The View Model type to navigate to</typeparam>
        /// <param name="parameter">The parameter to be passed, wrapped into an object</param>
        /// <returns></returns>
        Task ClosePopup();

        /// <summary>
        /// Navigate back to the previous page, provided that the Navigation Stack is not empty
        /// </summary>
        /// <param name="dispose">Tells whether the View Model we are closing should be disposed - If not the developer is supposed to manually dispose when it's time</param>
        /// <returns></returns>
        Task GoBackAsync(bool dispose = true);

        /// <summary>
        /// Remove the previous page from the Navigation Stack - The View Model is disposed as well
        /// </summary>
        /// <returns></returns>
        Task RemoveLastFromBackStackAsync();

        /// <summary>
        /// Clears the Navigation Stack
        /// </summary>
        /// <returns></returns>
        Task RemoveBackStackAsync();

    }
}
