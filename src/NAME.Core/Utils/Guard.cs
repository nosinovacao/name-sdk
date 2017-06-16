using System;

namespace NAME.Core.Utils
{
    /// <summary>
    /// Provides methods to guard values.
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Checks the value for nullity.
        /// </summary>
        /// <typeparam name="TValue">The type of the value</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="name">The name.</param>
        /// <returns>The value.</returns>
        /// <exception cref="System.ArgumentNullException">Throws the exception when the value is null.</exception>
        public static TValue NotNull<TValue>(TValue value, string name)
        {
            if (value == null)
                throw new ArgumentNullException(name);
            return value;
        }
    }
}
