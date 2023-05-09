using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace TestApp.ViewModels.Base.Commands
{

    /// <summary>
    /// Async Command which implements a basic throttle behavior. 
    /// </summary>
    public class ThrottledCommandAsync<T> : CommandAsync<T>
    {

        public const int DefaultThrottlePeriod = 500;


        private int _throttlePeriod = DefaultThrottlePeriod;



        /// <summary>
        /// Async Command which implements a basic throttle behavior.
        /// 
        /// </summary>
        /// <param name="execute">The Function executed when Execute or ExecuteAsync is called. This does not check canExecute before executing and will execute even if canExecute is false</param>
        /// <param name="canExecute">The Function that verifies whether the command should execute.</param>
        public ThrottledCommandAsync(Func<T, Task> execute, int throttlePeriod, Predicate<object> canExecute = null) : base(execute, canExecute)
        {
            _throttlePeriod = throttlePeriod;
        }

        /// <summary>
        /// Executes the Command as a Task
        /// </summary>
        /// <returns>The executed Task</returns>
        public override async Task ExecuteAsync(T parameter)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            try
            {
                _isRunning = true;
                ChangeCanExecute();

                await _execute(parameter);

                timer.Stop();

                // Wait only if computation time does not already exceed the throttle period
                if (timer.ElapsedMilliseconds < _throttlePeriod)
                    await Task.Delay(_throttlePeriod - (int)timer.ElapsedMilliseconds);
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

    }

    /// <summary>
    /// An implementation of ICommandAsync. Allows Commands to safely be used asynchronously with Task.
    /// </summary
    public class ThrottledCommandAsync : CommandAsync
    {

        public const int DefaultThrottlePeriod = 500;


        private int _throttlePeriod = DefaultThrottlePeriod;


        /// <summary>
        /// Async command
        /// </summary>
        /// <param name="execute">The Function executed when Execute or ExecuteAsync is called. This does not check canExecute before executing and will execute even if canExecute is false</param>
        /// <param name="canExecute">The Function that verifies whether or not ThrottledCommandAsync should execute.</param>
        public ThrottledCommandAsync(Func<Task> execute, int throttlePeriod, Func<object, bool> canExecute = null) : base(execute, canExecute)
        {
            _throttlePeriod = throttlePeriod;
        }

        /// <summary>
        /// Executes the Command as a Task
        /// </summary>
        /// <returns>The executed Task</returns>
        public override async Task ExecuteAsync()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            try
            {
                _isRunning = true;
                ChangeCanExecute();

                await _execute();

                timer.Stop();

                // Wait only if computation time does not already exceed the throttle period
                if (timer.ElapsedMilliseconds < _throttlePeriod)
                    await Task.Delay(_throttlePeriod - (int)timer.ElapsedMilliseconds);
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

    }
}
