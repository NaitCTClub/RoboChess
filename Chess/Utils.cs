using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    /// <summary>
    /// General utility functions.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Clamps a value between a minimum and maximum value..
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val">The input value</param>
        /// <param name="min">The minimum allowable output.</param>
        /// <param name="max">The maximum allowable output.</param>
        /// <returns></returns>
        public static T Clamp<T>(T val, T min, T max) where T : IComparable
        {
            val = val.CompareTo(min) < 0 ? min : val;
            return val.CompareTo(max) > 0 ? max : val;
        }

        /// <summary>
        /// Determines if a value falls between two limits.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val">The input value</param>
        /// <param name="min">The minimum allowable output.</param>
        /// <param name="max">The maximum allowable output.</param>
        /// <returns>True if within range.</returns>
        public static bool InRange<T>(T val, T min, T max) where T : IComparable
        {
            if (val.CompareTo(min) < 0 || val.CompareTo(max) > 0) return false;

            return true;
        }
    }
}
