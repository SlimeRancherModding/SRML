using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace SRML.Utils.Enum
{
    internal static class EnumHolderResolver
    {
        public static void RegisterAllEnums(Module module)
        {
            SRMod.ForceModContext(SRModLoader.GetModForAssembly(module.Assembly));
            foreach (var type in module.GetTypes())
            {
                if (type.GetCustomAttributes(true).Any((x) => x is EnumHolderAttribute))
                {
                    foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public |
                                                         BindingFlags.NonPublic))
                    {
                        if (!field.FieldType.IsEnum) continue;
                        
                        EnumPatcher.AddEnumValueWithAlternatives(field.FieldType,field.GetValue(null),field.Name);
                        
                    }
                }
            }
            SRMod.ClearModContext();
        }
    }
}
