using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SRML.Editor;
using UnityEngine;

namespace SRML.Utils
{
    public static class PrefabUtils
    {
        public static void FixPrefabFields(GameObject prefab)
        {
            var components = prefab.GetComponentsInChildren<Component>(true);

            foreach (var c in components)
            {
                if (!c) continue;
                foreach (var field in c.GetType().GetFields())
                {
                    if (field.FieldType == typeof(GameObject))
                    {
                        GameObject obj = field.GetValue(c) as GameObject;
                        if (!obj) continue;
                        if (obj.GetComponent<GameObjectReplacer>())
                        {
                            var g = obj.GetComponent<GameObjectReplacer>().GetReplacement();
                            field.SetValue(c,g);
                            GameObject.Destroy(obj);
                        }
                    }
                }
            }
        }
    }
}
