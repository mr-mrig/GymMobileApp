using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TestApp.ViewModels.Base.Commands
{

    /// <summary>
    /// An implementation of ICommandAsync. Allows Commands to safely be used asynchronously with Task.
    /// </summary>
    public class CommandAsync<T> : ICommandAsync<T>
    {
        protected readonly Func<T, Task> _execute;
        protected readonly Predicate<object> _canExecute;
        protected bool _isRunning = false;

        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Async Command
        /// </summary>
        /// <param name="execute">The Function executed when Execute or ExecuteAsync is called. This does not check canExecute before executing and will execute even if canExecute is false</param>
        /// <param name="canExecute">The Function that verifies whether the command should execute.</param>
        public CommandAsync(Func<T, Task> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute), $"{nameof(execute)} cannot be null");
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
        public virtual async Task ExecuteAsync(T parameter)
        {
            try
            {
                _isRunning = true;
                ChangeCanExecute();

                await _execute(parameter);
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
            await ExecuteAsync((T)parameter);
        }

    }

    /// <summary>
    /// An implementation of ICommandAsync. Allows Commands to safely be used asynchronously with Task.
    /// </summary
    public class CommandAsync : ICommandAsync
    {
        protected readonly Func<Task> _execute;
        protected readonly Func<object, bool> _canExecute;
        protected bool _isRunning = false;

        public event EventHandler CanExecuteChanged;


        /// <summary>
        /// Async command
        /// </summary>
        /// <param name="execute">The Function executed when Execute or ExecuteAsync is called. This does not check canExecute before executing and will execute even if canExecute is false</param>
        /// <param name="canExecute">The Function that verifies whether or not CommandAsync should execute.</param>
        public CommandAsync(Func<Task> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute), $"{nameof(execute)} cannot be null");
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
        public virtual async Task ExecuteAsync()
        {
            try
            {
                _isRunning = true;
                ChangeCanExecute();

                await _execute();
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
