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
            obj.SetActive(false);
            GameObject.DontDestroyOnLoad(obj);
        }
    }
}
