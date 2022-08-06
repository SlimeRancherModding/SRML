using System;
using System.Collections.Generic;
using System.Linq;

namespace SRML.Utils
{
    /// <summary>
    /// An utility class to help with Enums
    /// </summary>
    public static class EnumUtils
    {
        /// <summary>
        /// Parses an enum in a easier way
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Value to parse</param>
        /// <returns>The parsed enum on success, null on failure.</returns>
        public static object Parse(Type enumType, string value)
        {
            if (!enumType.IsEnum)
                throw new Exception($"The given type isn't an enum ({enumType.FullName} isn't an Enum)");

            try
            {
                return System.Enum.Parse(enumType, value);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Parses an enum in a easier way
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Value to parse</param>
        /// <param name="ignoreCase">true to ignore case; false to regard case.</param>
        /// <returns>The parsed enum on success, null on failure.</returns>
        public static object Parse(Type enumType, string value, bool ignoreCase)
        {
            if (!enumType.IsEnum)
                throw new Exception($"The given type isn't an enum ({enumType.FullName} isn't an Enum)");

            try
            {
                return System.Enum.Parse(enumType, value, ignoreCase);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Converts int to enum
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Int to convert to enum</param>
        /// <returns>The enum equal to the int</returns>
        public static object FromInt(Type enumType, int value)
        {
            if (!enumType.IsEnum)
                throw new Exception($"The given type isn't an enum ({enumType.FullName} isn't an Enum)");

            return System.Enum.ToObject(enumType, value);
        }

        /// <summary>
        /// Gets all names in an enum
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <returns>The list of names in the enum</returns>
        public static string[] GetAllNames(Type enumType)
        {
            if (!enumType.IsEnum)
                throw new Exception($"The given type isn't an enum ({enumType.FullName} isn't an Enum)");

            return System.Enum.GetNames(enumType);
        }

        /// <summary>
        /// Gets all enum values in an enum
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <returns>The list of all values in the enum</returns>
        public static object[] GetAll(Type enumType)
        {
            List<object> enums = new List<object>();

            foreach (string name in GetAllNames(enumType))
            {
                object value = Parse(enumType, name);
                if (value != null)
                    enums.Add(value);
            }

            return enums.ToArray();
        }

        /// <summary>
        /// Checks if an enum is defined.
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Value to check</param>
        /// <returns>true if defined, false if not.</returns>
        public static bool IsDefined(Type enumType, string value)
        {
            try
            {
                return System.Enum.IsDefined(enumType, value);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks all names in an enum to see if what you need exists.
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Value to find</param>
        /// <returns>true if found, false if not.</returns>
        public static bool HasEnumValue(Type enumType, string value)
        {
            foreach (string name in GetAllNames(enumType))
            {
                if (name.Equals(value))
                {
                    return true;
                }
            }

            return false;
        }

        public static object GetMinValue(Type enumType)
        {
            return GetAll(enumType).Cast<int>().Min();
        }

        public static object GetMaxValue(Type enumType)
        {
            return GetAll(enumType).Cast<int>().Max();
        }

        /// <summary>
        /// Parses an enum in a easier way
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value to parse</param>
        /// <param name="errorReturn">What to return if the parse fails.</param>
        /// <returns>The parsed enum on success, <paramref name="errorReturn"/> on failure.</returns>
        public static T Parse<T>(string value, T errorReturn = default) where T : System.Enum
        {
            try
            {
                return (T)System.Enum.Parse(typeof(T), value);
            }
            catch
            {
                return errorReturn;
            }
        }

        /// <summary>
        /// Parses an enum in a easier way
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value to parse</param>
        /// <param name="ignoreCase">true to ignore case; false to regard case.</param>
        /// <param name="errorReturn">What to return if the parse fails.</param>
        /// <returns>The parsed enum on success, <paramref name="errorReturn"/> on failure.</returns>
        public static T Parse<T>(string value, bool ignoreCase, T errorReturn = default) where T : System.Enum
        {
            try
            {
                return (T)System.Enum.Parse(typeof(T), value, ignoreCase);
            }
            catch
            {
                return errorReturn;
            }
        }

        /// <summary>
        /// Converts int to enum
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Int to convert to enum</param>
        /// <returns>The enum equal to the int</returns>
        public static T FromInt<T>(int value) where T : System.Enum
        {
            return (T)System.Enum.ToObject(typeof(T), value);
        }

        /// <summary>
        /// Gets all names in an enum
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <returns>The list of names in the enum</returns>
        public static string[] GetAllNames<T>() where T : System.Enum
        {
            return System.Enum.GetNames(typeof(T));
        }

        /// <summary>
        /// Gets all enum values in an enum
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="errorReturn">What to return if the parse fails.</param>
        /// <returns>The list of all values in the enum</returns>
        public static T[] GetAll<T>(T errorReturn = default) where T : System.Enum
        {
            List<T> enums = new List<T>();

            foreach (string name in GetAllNames<T>())
                enums.Add(Parse<T>(name, errorReturn));

            return enums.ToArray();
        }

        /// <summary>
        /// Checks if an enum is defined.
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value to check</param>
        /// <returns>true if defined, false if not.</returns>
        public static bool IsDefined<T>(string value) where T : System.Enum
        {
            try
            {
                return System.Enum.IsDefined(typeof(T), value);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks all names in an enum to see if what you need exists.
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value to find</param>
        /// <returns>true if found, false if not.</returns>
        public static bool HasEnumValue<T>(string value) where T : System.Enum
        {
            foreach (string name in GetAllNames<T>())
            {
                if (name.Equals(value))
                {
                    return true;
                }
            }

            return false;
        }

        public static T GetMinValue<T>() where T : System.Enum
        {
            return GetAll<T>().Min();
        }

        public static T GetMaxValue<T>() where T : System.Enum
        {
            return GetAll<T>().Max();
        }
    }
}