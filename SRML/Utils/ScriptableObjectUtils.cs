using System;
using UnityEngine;

namespace SRML.Utils
{
    public static class ScriptableObjectUtils
    {
        public static T CreateScriptable<T>(Action<T> constructor = null) where T : ScriptableObject
        {
            var s = ScriptableObject.CreateInstance<T>();
            constructor?.Invoke(s);
            return s;
        }
    }
}
