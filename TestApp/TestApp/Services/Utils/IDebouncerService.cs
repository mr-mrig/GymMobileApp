using System;
using System.Threading.Tasks;

namespace TestApp.Services.Utils
{
    public interface IDebouncerService
    {

        /// <summary>
        /// Debounce an event by resetting the event timeout every time the it is  fired. 
        /// This way the specified command is fired only after events stop firing for the given timeout period.
        /// <para>Only the last event is processed, all the previous ones are discarded</para>
        /// <param name="delay">Timeout in Milliseconds</param>
        /// <param name="command">The sync action to run when debounced event fires</object></param>
        /// <param name="parameter">Optional parameter</param>
        Task DebounceAsync(uint delay, Func<object, Task> command, object parameter = null);

        /// <summary>
        /// Debounce an event by resetting the event timeout every time the it is  fired. 
        /// This way the specified command is fired only after events stop firing for the given timeout period.
        /// <para>Only the last event is processed, all the previous ones are discarded</para>
        /// <param name="delay">Timeout in Milliseconds</param>
        /// <param name="command">The sync action to run when debounced event fires</object></param>
        /// <param name="parameter">Optional parameter</param>
        /// <typeparam name="T">The return value type of the function to be debounced</typeparam>
        /// <returns>The result of the debounced function as a Task, or default(T)</returns>
        Task<T> DebounceAsync<T>(uint delay, Func<object, Task<T>> command, object parameter = null);

        /// <summary>
        /// This method throttles events by allowing only 1 event to fire for the given timeout period.
        /// <para>An event will be fired every timeout, even if more events are pending</para>
        /// 
        /// <para>NOT IMPLEMENTED YET</para>
        /// </summary>
        /// <param name="period">Timeout period [ms]</param>
        /// <param name="action">Action<object> to fire when debounced event fires</param>
        /// <param name="param">optional parameter</param>
        void ThrottleAsync(int period, Func<object, Task> action, object param = null);
    }
}
