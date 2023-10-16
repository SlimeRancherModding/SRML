using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace SRML.Utils
{
    public static class LogUtils
    {
        private static StringBuilder currentBuilder;

        public static void OpenLogSession()
        {
            currentBuilder = new StringBuilder();
        }

        public static void Log(object b)
        {
            if (currentBuilder != null)
            {
                currentBuilder.AppendLine(b.ToString());
            }
            else
            {
                Debug.Log(b);
            }
        }

        public static void CloseLogSession()
        {
            if (currentBuilder != null)
            {
                Debug.Log(currentBuilder.ToString());
                currentBuilder = null;
            }
        }

    }
}
