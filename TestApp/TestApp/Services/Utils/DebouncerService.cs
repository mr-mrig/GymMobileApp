using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestApp.Services.Utils
{


    /// <summary>
    /// Class that provide Debouncing/Throttling utilities
    /// </summary>
    public class DebouncerService : IDebouncerService
    {



        private CancellationTokenSource _debounceCancellationSource;




        /// <summary>
        /// Class that provide Debouncing/Throttling utilities
        /// </summary>
        public DebouncerService() 
        {
            _debounceCancellationSource = new CancellationTokenSource();
        }




        /// <summary>
        /// Debounce an event by resetting the event timeout every time the it is  fired. 
        /// This way the specified command is fired only after events stop firing for the given timeout period.
        /// <para>Only the last event is processed, all the previous ones are discarded</para>
        /// <param name="delay">Timeout in Milliseconds</param>
        /// <param name="command">The sync action to run when debounced event fires</object></param>
        /// <param name="parameter">Optional parameter</param>
        public async Task DebounceAsync(uint delay, Func<object, Task> command, object parameter = null)
        {
            if (command == null)
                return;

            Interlocked.Exchange(ref _debounceCancellationSource, new CancellationTokenSource()).Cancel();      // Atomically cancel the previous operation and reset the CTS

            try
            {
                // Wait for the delay to expire then call the command - Once it is called it cannot be interrupted
                await Task.Delay((int)delay, _debounceCancellationSource.Token).ContinueWith(async task =>
                {
                    if (!_debounceCancellationSource.IsCancellationRequested)
                        await command.Invoke(parameter);

                },
                CancellationToken.None,
                TaskContinuationOptions.OnlyOnRanToCompletion,
                TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch(TaskCanceledException)
            {
                // Task has been aborted because of multiple calls
            }
        }

        /// <summary>
        /// Debounce an event by resetting the event timeout every time the it is  fired. 
        /// This way the specified command is fired only after events stop firing for the given timeout period.
        /// <para>Only the last event is processed, all the previous ones are discarded</para>
        /// <param name="delay">Timeout in Milliseconds</param>
        /// <param name="command">The sync action to run when debounced event fires</object></param>
        /// <param name="parameter">Optional parameter</param>
        /// <typeparam name="T">The return value type of the function to be debounced</typeparam>
        /// <returns>The result of the debounced function as a Task, or default(T)</returns>
        public async Task<T> DebounceAsync<T>(uint delay, Func<object, Task<T>> command, object parameter = null)
        {
            T result = default(T);

            if (command == null)
                return default(T);

            Interlocked.Exchange(ref _debounceCancellationSource, new CancellationTokenSource()).Cancel();      // Atomically cancel the previous operation and reset the CTS

            try
            {
                // Wait for the delay to expire then call the command - Once it is called it cannot be interrupted
                await Task.Delay((int)delay, _debounceCancellationSource.Token).ContinueWith(async task =>
                {
                    if (!_debounceCancellationSource.IsCancellationRequested)
                        result = await command.Invoke(parameter);

                },
                CancellationToken.None,
                TaskContinuationOptions.OnlyOnRanToCompletion,
                TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch(TaskCanceledException)
            {
                // Task has been aborted because of multiple calls
            }
            return result;
        }

        /// <summary>
        /// This method throttles events by allowing only 1 event to fire for the given timeout period.
        /// <para>An event will be fired every timeout, even if more events are pending</para>
        /// 
        /// <para>NOT IMPLEMENTED YET</para>
        /// </summary>
        /// <param name="period">Timeout period [ms]</param>
        /// <param name="action">Action<object> to fire when debounced event fires</param>
        /// <param name="param">optional parameter</param>
        public void ThrottleAsync(int period, Func<object, Task> action, object param = null)
        {
            throw new NotImplementedException("Throttle not implemented yet");
        }
    }
}
