using System;
using System.Threading.Tasks;
using System.Windows.Input;
using TestApp.Services.Navigation;
using Xamarin.Forms;

namespace TestApp.ViewModels.Base.Commands
{

    /// <summary>
    /// An implementation of ICommandAsync. 
    /// Like <seealso cref="CommandAsync"/> but it also notifies the user with a <seealso cref="LoadingPopup"/>
    /// </summary>
    public class BlockingCommandAsync<T> : ICommandAsync<T>
    {
        private readonly Func<T, Task> _execute;
        private readonly Predicate<object> _canExecute;
        private bool _isRunning = false;
        private INavigationService _navigationService;

        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// An async command which blocks the UI until its completion showing an activity indicator <seealso cref="LoadingPopup"/>
        /// </summary>
        /// <param name="execute">The Function executed when Execute or ExecuteAsync is called. This does not check canExecute before executing and will execute even if canExecute is false</param>
        /// <param name="canExecute">The Function that verifies whether the command should execute.</param>
        public BlockingCommandAsync(Func<T, Task> execute, INavigationService navigationService, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute), $"{nameof(execute)} cannot be null");
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService), $"{nameof(navigationService)} cannot be null");
            _canExecute = canExecute ?? (_ => true);
        }


        /// <summary>
        /// Determines whether the command can execute in its current state
        /// </summary>
        /// <returns><c>true</c>, if this command can be executed; otherwise, <c>false</c>.</returns>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        public bool CanExecute(object parameter) => !_isRunning && _canExecute(parameter);


        /// <summary>
        /// Raises the CanExecuteChanged event.
        /// </summary>
        public void ChangeCanExecute() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Executes the Command as a Task
        /// </summary>
        /// <returns>The executed Task</returns>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        public async Task ExecuteAsync(T parameter)
        {
            try
            {
                _isRunning = true;
                ChangeCanExecute();

                MessagingCenter.Send((object)this, MessageKeys.OpenActivityIndicatorPopup);
                await _execute(parameter);
                MessagingCenter.Send((object)this, MessageKeys.CloseActivityIndicatorPopup);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            finally
            {
                _isRunning = false;
                ChangeCanExecute();
            }
        }


        async void ICommand.Execute(object parameter)
        {
            await ExecuteAsync(default);
        }

    }

    /// <summary>
    /// An implementation of ICommandAsync. Allows Commands to safely be used asynchronously with Task.
    /// </summary
    public class BlockingCommandAsync : ICommandAsync
    {
        private readonly Func<Task> _execute;
        private readonly Func<object, bool> _canExecute;
        private bool _isRunning = false;

        public event EventHandler CanExecuteChanged;


        /// <summary>
        /// An async command which blocks the UI until its completion showing an activity indicator <seealso cref="LoadingPopup"/>
        /// </summary>
        /// <param name="execute">The Function executed when Execute or ExecuteAsync is called. This does not check canExecute before executing and will execute even if canExecute is false</param>
        /// <param name="canExecute">The Function that verifies whether or not BlockingCommandAsync should execute.</param>
        public BlockingCommandAsync(Func<Task> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute), $"{nameof(execute)} cannot be null");
            _canExecute = canExecute ?? (_ => true);
        }


        /// <summary>
        /// Determines whether the command can execute in its current state
        /// </summary>
        /// <returns><c>true</c>, if this command can be executed; otherwise, <c>false</c>.</returns>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        public bool CanExecute(object parameter) => _canExecute(parameter);

        /// <summary>
        /// Raises the CanExecuteChanged event.
        /// </summary>
        public void ChangeCanExecute() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Executes the Command as a Task
        /// </summary>
        /// <returns>The executed Task</returns>
        public async Task ExecuteAsync()
        {
            try
            {
                _isRunning = true;
                ChangeCanExecute();

                MessagingCenter.Send((object)this, MessageKeys.OpenActivityIndicatorPopup);
                await _execute();
                MessagingCenter.Send((object)this, MessageKeys.CloseActivityIndicatorPopup);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            finally
            {
                _isRunning = false;
                ChangeCanExecute();
            }
        }


        async void ICommand.Execute(object parameter)
        {
            await ExecuteAsync();
        }
    }
}
