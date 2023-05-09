namespace TestApp.Services.Utils
{
    public static class Utilities
    {


        /// <summary>
        /// Check whether a number is divisible by another one - IE, it is a multiple of the second
        /// </summary>
        /// <param name="x">The number to be checked</param>
        /// <param name="divisor">The number which the first one should be divided by</param>
        /// <returns>True if x is divisible by divisor</returns>
        public static bool IsDivisible(int x, int divisor)
        
            => (x % divisor) == 0;


        /// <summary>
        /// Check whether a double has decimal places
        /// </summary>
        /// <param name="input">The number to be checked</param>
        /// <returns>True if number has decimal places, false if it is an integer</returns>
        public static bool IsDecimal(double input)

            => (input % 1) != 0;


    }
}
