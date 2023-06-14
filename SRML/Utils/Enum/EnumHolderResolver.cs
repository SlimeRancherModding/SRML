using SRML.SR;
using System.Linq;
using System.Reflection;
using SRML.Core.ModLoader;
using SRML.Core.API.BuiltIn;

namespace SRML.Utils.Enum
{
    internal static class EnumHolderResolver
    {
        public static void RegisterAllEnums(IMod mod) => RegisterAllEnums(mod, CoreLoader.Instance.assembliesForMod[mod]);

        public static void RegisterAllEnums(IMod mod, Assembly assembly)
        {
            CoreLoader loader = CoreLoader.Instance;
            loader.ForceModContext(mod);

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
                                    if (EnumPatcher.categorizableRegistries.TryGetValue(x => x.AttributeType == att.GetType(), out var reg))
                                    {
                                        if (reg is ICategorizableEnum catEnum && reg.TakesPresidenceOverCategorizable)
                                            catEnum.Decategorize(generated);
                                        reg.Categorize(generated, att);
                                    }
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
