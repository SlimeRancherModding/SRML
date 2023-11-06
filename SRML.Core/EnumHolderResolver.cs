using SRML.SR;
using System.Linq;
using System.Reflection;
using SRML.Core.ModLoader;
using SRML.Core.API.BuiltIn;
using System;
using SRML.Core.SRML.Attributes;

namespace SRML.Utils.Enum
{
    internal static class EnumHolderResolver
    {
        public static void RegisterAllEnums(Type modType, IModInfo info)
        {
            foreach (var type in modType.Assembly.ManifestModule.GetTypes())
            {
                try
                {
                    if (type.GetCustomAttributes(true).Any((x) => x is EnumHolderAttribute))
                    {
                        EnumHolderAttribute enumHolder = type.GetCustomAttribute<EnumHolderAttribute>();

                        foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public |
                                                             BindingFlags.NonPublic))
                        {
                            if (!field.FieldType.IsEnum) continue;

                            foreach (EnumForwardedToAttribute att in field.FieldType.GetCustomAttributes<EnumForwardedToAttribute>())
                                EnumPatcher.RegisterForward(field.FieldType, att.ForwardedTo);
                            foreach (EnumForwardedFromAttribute att in field.FieldType.GetCustomAttributes<EnumForwardedFromAttribute>())
                                EnumPatcher.RegisterForward(att.ForwardedFrom, field.FieldType);

                            if ((int)field.GetValue(null) == 0)
                            {
                                var newVal = EnumPatcher.GetFirstFreeValue(field.FieldType);
                                EnumPatcher.AddEnumValue(field.FieldType, newVal, field.Name);
                                field.SetValue(null, newVal);
                            }
                            else
                                EnumPatcher.AddEnumValue(field.FieldType, field.GetValue(null), field.Name);

                            if (enumHolder.shouldCategorize)
                            {
                                System.Enum generated = (System.Enum)field.GetValue(null);

                                foreach (var att in field.GetCustomAttributes())
                                {
                                    // TODO: categorize reg
                                    /*if (EnumPatcher.categorizableRegistries.TryGetValue(x => x.AttributeType == att.GetType(), out var reg))
                                    {
                                        if (reg is ICategorizableEnum catEnum && reg.TakesPresidenceOverCategorizable)
                                            catEnum.Decategorize(generated);
                                        reg.Categorize(generated, att);
                                    }*/
                                }
                            }
                        }
                    }
                }
                catch (ReflectionTypeLoadException)
                {
                }
            }
        }
    }
}
