using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
namespace SRML.Utils
{
    public static class GameObjectUtils
    {
        public static object GameContext { get; internal set; }

        public static String PrintObjectTree(GameObject obj)
        {
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
            }

            string currentIndent = "";
            StringBuilder builder = new StringBuilder();
            PrintObjectTreeInternal(obj, currentIndent, builder);
            return builder.ToString();
        }

        static void PrintObjectTreeInternal(GameObject obj,string indent, StringBuilder builder)
        {
            builder.AppendLine(indent+"GameObject: " + obj.name);
            indent = indent + "    ";
            builder.AppendLine(indent + "components:");
            string indent2 = indent + "    ";
            var h = obj.GetComponents<Component>().ToList();
            var c = StringComparer.Create(CultureInfo.CurrentCulture, true);
            h.Sort((component, component1) => c.Compare(component.GetType().Name, component1.GetType().Name));
            foreach (var v in h)
            {
                MeshRenderer rend = v as MeshRenderer;
                SkinnedMeshRenderer rend2 = v as SkinnedMeshRenderer;

                Transform form = v as Transform;
                if (form)
                {
                    builder.AppendLine(indent2 + v.GetType().Name + " " + form.localPosition+", "+form.localRotation+" "+form.localScale);
                    continue;
                }
                builder.AppendLine(indent2 + v.GetType().Name);
                if (rend)
                {
                    var indent3 = indent2 + "    ";
                    builder.AppendLine(indent3 + "Material: " + rend.material.name);
                }
                if (rend2)
                {
                    var indent3 = indent2 + "    ";
                    builder.AppendLine(indent3 + "Material: " + rend2.material.name);
                }
            }

            builder.AppendLine(indent + "children: ");
           
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                PrintObjectTreeInternal(obj.transform.GetChild(i).gameObject, indent2, builder);
            }
        }

        public static void Prefabitize(GameObject obj)
        {
            obj.transform.SetParent(SRML.Core.Main.prefabParent, false);
        }

        public static GameObject InstantiateInactive(GameObject original)
        {
            GameObject newObj = GameObject.Instantiate(original, SRML.Core.Main.prefabParent, true);
            newObj.SetActive(false);
            newObj.transform.SetParent(null, false);
            return newObj;
        }

        public static GameObject InstantiateInactive(GameObject original, bool keepOriginalName = false)
        {
            GameObject newObj = InstantiateInactive(original);
            if (keepOriginalName) newObj.name = original.name;
            return newObj;
        }

        public static GameObject InstantiateInactive(GameObject original, Transform parent, bool keepOriginalName = false)
        {
            GameObject newObj = InstantiateInactive(original, keepOriginalName);
            newObj.transform.SetParent(parent);
            return newObj;
        }

        public static GameObject InstantiateInactive(GameObject original, Transform parent, bool worldPositionStays, bool keepOriginalName = false)
        {
            GameObject newObj = InstantiateInactive(original, keepOriginalName);
            newObj.transform.SetParent(parent, worldPositionStays);
            return newObj;
        }

        public static GameObject InstantiateInactive(GameObject original, Vector3 position, Quaternion rotation, bool keepOriginalName = false)
        {
            GameObject newObj = InstantiateInactive(original, keepOriginalName);
            newObj.transform.SetPositionAndRotation(position, rotation);
            return newObj;
        }

        public static GameObject InstantiateInactive(GameObject original, Vector3 position, Quaternion rotation, Transform parent, bool keepOriginalName = false)
        {
            GameObject newObj = InstantiateInactive(original, parent, keepOriginalName);
            newObj.transform.SetPositionAndRotation(position, rotation);
            return newObj;
        }

        public static GameObject InstantiateInactive(GameObject original, Vector3 position, Quaternion rotation, Transform parent, bool worldPositionStays, bool keepOriginalName = false)
        {
            GameObject newObj = InstantiateInactive(original, parent, worldPositionStays, keepOriginalName);
            newObj.transform.SetPositionAndRotation(position, rotation);
            return newObj;
        }
    }
}
