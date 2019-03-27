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
        static List<KeyValuePair<GameObject, IFieldReplacer>> replacers = new List<KeyValuePair<GameObject, IFieldReplacer>>();
        public static void FixPrefabFields(GameObject prefab, IFieldReplacer replacer)
        {
            replacers.Add(new KeyValuePair<GameObject, IFieldReplacer>(prefab,replacer));
        }

        internal static void ProcessReplacements()
        {
            replacers.ForEach((x)=>FixPrefabFieldsInternal(x.Key,x.Value));
        }
        static void FixPrefabFieldsInternal(GameObject prefab, IFieldReplacer replacementInfo)
        {

            var replacer = ReplacerCache.GetReplacer(replacementInfo);

            var components = replacementInfo.ReplaceInChildren? prefab.GetComponentsInChildren<Component>(true):prefab.GetComponents<Component>();

            foreach (var comp in components)
            {
                if (!comp) continue;
                foreach (var field in replacer.FieldToField.Where((x)=>x.Value.DeclaringType==comp.GetType()))
                {
                    Debug.Log(field.Value.Name+" "+field.Key.Name+" "+(replacer.InstanceInfo.Instance as GameObject).GetComponent<SlimeEyeComponents>());
                    field.Value.SetValue(comp,field.Key.GetValue((replacer.InstanceInfo.Instance as GameObject).GetComponentInChildren(comp.GetType())));
                }
            }
        }

        public static void FixPrefabFields(GameObject prefab)
        {
            foreach (var replacement in prefab.GetComponentsInChildren<FieldReplacerContainer>())
            {
                FixPrefabFields(prefab.gameObject, replacement.Replacer);
                MonoBehaviour.Destroy(replacement);
            }
        }



        public static void ReplaceFieldsWith<T>(GameObject prefab, T original, T newValue)
        {
            var components = prefab.GetComponentsInChildren<Component>(true);

            foreach (var comp in components)
            {
                if (!comp) continue;
                foreach (var field in comp.GetType().GetFields())
                {
                    if (field.FieldType == typeof(T))
                    {
                        T t = (T)field.GetValue(comp);
                        if (!t.Equals(original)) continue;
                        field.SetValue(comp,newValue);
                    }
                }

            }
        }
    }
}
