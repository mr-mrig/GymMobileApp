using System.Threading.Tasks;
using System.Windows.Input;

namespace TestApp.ViewModels.Base
{

    /// <summary>
    /// An Async implementation of ICommand for Task
    /// </summary>
    public interface ICommandAsync<T> : ICommand
    {
        /// <summary>
        /// Executes the Command as a Task
        /// </summary>
        /// <returns>The Task</returns>
        /// <param name="parameter">Command parameters. If the command does not require data to be passed, this object can be set to null.</param>
        Task ExecuteAsync(T parameter);

        /// <summary>
        /// Raises the CanExecuteChanged event.
        /// </summary>
        void ChangeCanExecute();
    }



    /// <summary>
    /// An Async implementation of ICommand for Task
    /// </summary>
    public interface ICommandAsync : ICommand
    {
        /// <summary>
        /// Executes the Command as a Task
        /// </summary>
        /// <returns>The Task</returns>
        System.Threading.Tasks.Task ExecuteAsync();

        /// <summary>
        /// Raises the CanExecuteChanged event.
        /// </summary>
        void ChangeCanExecute();
    }
}
