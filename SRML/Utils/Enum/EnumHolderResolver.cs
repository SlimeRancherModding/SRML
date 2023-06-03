using SRML.SR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using SRML.SR.Utils;
using SRML.Core.ModLoader;

namespace SRML.Utils.Enum
{
    internal static class EnumHolderResolver
    {
        public static void RegisterAllEnums(IMod mod) => RegisterAllEnums(mod, CoreLoader.Main.assembliesForMod[mod]);

        public static void RegisterAllEnums(IMod mod, Assembly assembly)
        {
            CoreLoader loader = CoreLoader.Main;
            loader.ForceModContext(mod);

            // TODO: Update this so that auto-categorization can easier be defined by a mod.
            try
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.GetCustomAttributes(true).Any((x) => x is EnumHolderAttribute))
                    {
                        EnumHolderAttribute enumHolder = type.GetCustomAttribute<EnumHolderAttribute>();

                        foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public |
                                                             BindingFlags.NonPublic))
                        {
                            if (!field.FieldType.IsEnum) continue;

                            if ((int)field.GetValue(null) == 0)
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
            }
            finally
            {
                loader.ClearModContext();
            }
        }
    }
}
