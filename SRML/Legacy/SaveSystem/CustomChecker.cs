using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.SaveSystem
{
    internal static class CustomChecker
    {
        public delegate CustomLevel CustomCheckerGenericDelegate<T>(T obj);

        public delegate CustomLevel CustomCheckerDelegate(object obj);

        public static Dictionary<Type,CustomCheckerDelegate> customCheckers = new Dictionary<Type, CustomCheckerDelegate>();

        public static void RegisterCustomChecker(Type type, CustomCheckerDelegate del)
        {
            customCheckers[type] = del;
        }

        public static void RegisterCustomChecker<T>(CustomCheckerGenericDelegate<T> del)
        {
            RegisterCustomChecker(typeof(T),(x)=>del((T)x));
        }

        public static CustomLevel GetCustomLevel(object b)
        {
            var currentLevel = CustomLevel.NONE;
            foreach (var v in customCheckers.Where((x) => x.Key.IsAssignableFrom(b.GetType())))
            {
                var newLevel = v.Value(b);
                if (newLevel > currentLevel) currentLevel = newLevel;
            }

            return currentLevel;
        }

        public enum CustomLevel
        {
            NONE,
            VANILLA,
            PARTIAL,
            FULL
        }
    }

    
}
