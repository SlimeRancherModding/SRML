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

        public static GameObject CopyPrefab(GameObject prefab)
        {
            return GameObject.Instantiate(prefab, Main.prefabParent, false);
        }

        public static UnityEngine.Object DeepCopyObject(UnityEngine.Object ob)
        {
            Dictionary<UnityEngine.Object, UnityEngine.Object> refToRef = new Dictionary<UnityEngine.Object, UnityEngine.Object>();
            UnityEngine.Object DeepCopyObject_Internal(UnityEngine.Object obj)
            {
                if (!obj) return obj;
                if (refToRef.TryGetValue(obj, out var old)) return old;
                if (refToRef.Values.Contains(obj)) return obj;
                var newObj = UnityEngine.Object.Instantiate(obj);
                if (!typeof(ScriptableObject).IsAssignableFrom(newObj.GetType())) return newObj;
                refToRef[obj] = newObj;
                System.Object ProcessObject(System.Object theObj)
                {
                    if (theObj == null) return null;
                    
                    if (typeof(Component).IsAssignableFrom(theObj.GetType()) || typeof(GameObject).IsAssignableFrom(theObj.GetType()))
                    {
                        return theObj;
                    }
                    else
                    if (typeof(UnityEngine.Object).IsAssignableFrom(theObj.GetType()))
                    {
                        return DeepCopyObject_Internal((UnityEngine.Object)theObj);
                    }
                    else if (theObj.GetType().IsArray)
                    {
                        var g = theObj as Array;
                        for (int i = 0; i < g.Length; i++)
                        {
                            g.SetValue(ProcessObject(g.GetValue(i)), i);
                        }
                        
                    }
                    else if (!theObj.GetType().IsPrimitive&&!typeof(String).IsAssignableFrom(theObj.GetType())&&!theObj.GetType().IsEnum)
                    {
                        foreach(var v in theObj.GetType().GetFields(System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.Public))
                        {
                            v.SetValue(theObj, ProcessObject(v.GetValue(theObj)));
                        }
                    }
                    
                    return theObj;
                }

                foreach (var v in newObj.GetType().GetFields())
                {
                    v.SetValue(newObj,ProcessObject(v.GetValue(newObj)));
                }
                return newObj;
            }
            return DeepCopyObject_Internal(ob);
        }
    }
}
