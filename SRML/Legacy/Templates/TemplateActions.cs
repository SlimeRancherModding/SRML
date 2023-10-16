using System;
using System.Collections.Generic;
using UnityEngine;

namespace SRML.SR.Templates
{
    public static class TemplateActions
    {
        internal readonly static Dictionary<string, Action<GameObject>> actions = new Dictionary<string, Action<GameObject>>();

        public static void RegisterAction(string ID, Action<GameObject> action)
        {
            if (actions.ContainsKey(ID))
            {
                UnityEngine.Debug.LogError($"The template action with id '<color=white>{ID}</color>' is already registered");
                return;
            }

            actions.Add(ID, action);
        }

        public static void OverrideAction(string ID, Action<GameObject> action)
        {
            if (actions.ContainsKey(ID))
                actions[ID] = action;
            else
                actions.Add(ID, action);
        }

        public static void RunAction(string ID, GameObject obj)
        {
            if (actions.ContainsKey(ID))
                actions[ID]?.Invoke(obj);
        }
    }
}
