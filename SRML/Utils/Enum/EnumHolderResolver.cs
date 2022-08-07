using SRML.SR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using SRML.SR.Utils;

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
                    EnumHolderAttribute enumHolder = type.GetCustomAttribute<EnumHolderAttribute>();

                    foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public |
                                                         BindingFlags.NonPublic))
                    {
                        if (!field.FieldType.IsEnum) continue;

                        if ((int) field.GetValue(null) == 0)
                        {
                            var newVal = EnumPatcher.GetFirstFreeValue(field.FieldType);
                            EnumPatcher.AddEnumValueWithAlternatives(field.FieldType, newVal, field.Name);
                            field.SetValue(null, newVal);
                        }
                        else
                        EnumPatcher.AddEnumValueWithAlternatives(field.FieldType, field.GetValue(null), field.Name);

                        if (field.FieldType == typeof(Identifiable.Id))
                        {
                            if (enumHolder.shouldCategorize)
                            {
                                foreach (var att in field.GetCustomAttributes())
                                    if (att is IdentifiableCategorization)
                                        ((Identifiable.Id)field.GetValue(null)).Categorize(((IdentifiableCategorization)att).rules);
                            }
                            else
                            {
                                IdentifiableCategorization.doNotAutoCategorize.Add((Identifiable.Id)field.GetValue(null));
                            }
                        }

                        if (field.FieldType == typeof(Gadget.Id))
                        {
                            if (enumHolder.shouldCategorize)
                            {
                                foreach (var att in field.GetCustomAttributes())
                                    if (att is GadgetCategorization)
                                        ((Gadget.Id)field.GetValue(null)).Categorize(((GadgetCategorization)att).rules);
                            }
                            else
                            {
                                IdentifiableCategorization.doNotAutoCategorize.Add((Identifiable.Id)field.GetValue(null));
                            }
                        }
                    }
                }
            }
            SRMod.ClearModContext();    
        }
    }
}
