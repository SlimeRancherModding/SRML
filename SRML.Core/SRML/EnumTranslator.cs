using MonomiPark.SlimeRancher.DataModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML
{
    /// <summary>
    /// Serializable table that maps enum values from placeholder numerical values to their string counterparts
    /// Used to assure that enum values map properly between game loads, even if registration order changes
    /// Mapped values are always negative
    /// </summary>
    public partial class EnumTranslator
    {
        public Dictionary<Type, Dictionary<int,string>> MappedValues = new Dictionary<Type, Dictionary<int, string>>();
        internal static Dictionary<Type, int> EnumMinCache = new Dictionary<Type, int>();

        /// <summary>
        /// Get the next free value for the given enumType
        /// </summary>
        /// <param name="enumType">The enum type</param>
        /// <returns>The next free value</returns>
        public int GetFreeValue(Type enumType)
        {
            if (!MappedValues.ContainsKey(enumType)) return -1;
            return MappedValues[enumType].Keys.LastOrDefault()-1;
        }   

        /// <summary>
        /// Generate a translation table for  a list of enumValues
        /// </summary>
        /// <param name="enumValues">List of enum values to translate</param>
        public void GenerateTranslationTable(IList enumValues)
        {
            if (enumValues.Count == 0) return;
            var type = enumValues[0].GetType();
            var newDict = MappedValues.ContainsKey(type) ? MappedValues[type] : new Dictionary<int, string>();
            MappedValues[type] = newDict;
            int startValue = GetFreeValue(type);
            for (int i = startValue; i > startValue - enumValues.Count; i--)
            {
                newDict[i] = Enum.GetName(type, enumValues[-i+startValue]);
            }
        }


        /// <summary>
        /// Replace missing enum values with default values, or remove them all together
        /// </summary>
        public void FixMissingEnumValues()
        {
            var toChangeList = new Dictionary<int,string>();
            foreach (var pair in MappedValues)
            {
                toChangeList.Clear();
                foreach (var v in pair.Value)
                {
                    var curString = v.Value;
                    if (!Enum.IsDefined(pair.Key, curString))
                    {
                        toChangeList.Add(v.Key,OnTranslationFallback(pair.Key,ref curString)?curString:null);
                    }
                }

                foreach (var toChange in toChangeList)
                {
                    if (toChange.Value == null) pair.Value.Remove(toChange.Key);
                    else
                    pair.Value[toChange.Key] = toChange.Value;
                }
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine("EnumTranslator");
            foreach (var v in MappedValues)
            {
                builder.AppendLine("    " + v.Key.FullName);
                foreach (var pair in v.Value)
                {
                    builder.AppendLine("        " + pair.Key + " = " + pair.Value);
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Translate an enum value
        /// </summary>
        /// <param name="val">Original enum value</param>
        /// <returns>Translated integer</returns>
        public int TranslateTo(object val)
        {
            var type = val.GetType();
            if (!MappedValues.ContainsKey(type)) return ((int) val);
            if (!EnumMinCache.ContainsKey(type)) EnumMinCache[type] = ((int[])Enum.GetValues(type)).Min();

            var potential = MappedValues[type].FirstOrDefault((x) => Enum.GetName(type, val) == x.Value);
            return potential.Key < EnumMinCache[type] ? potential.Key : ((int)val);
        }

        public T TranslateFrom<T>(int val)
        {
            return (T)TranslateFrom(typeof(T), val);
        }

        public object TranslateFrom(Type enumType, int val)
        {
            if (!EnumMinCache.ContainsKey(enumType)) EnumMinCache[enumType] = ((int[])Enum.GetValues(enumType)).Min();
            if (val < EnumMinCache[enumType])
            {
                if(!MappedValues.ContainsKey(enumType) || !MappedValues[enumType].ContainsKey(val)||!Enum.IsDefined(enumType,MappedValues[enumType][val])) throw new MissingTranslationException(val,TranslationMode.FROMTRANSLATED);
                return Enum.ToObject(enumType, Enum.Parse(enumType, MappedValues[enumType][val]));
            }
            else
            {
                return Enum.ToObject(enumType, val);
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(MappedValues.Count);
            foreach (var v in MappedValues)
            {
                writer.Write(v.Key.AssemblyQualifiedName);
                writer.Write(v.Value.Count);
                foreach (var pair in v.Value)
                {
                    writer.Write(pair.Key);
                    writer.Write(pair.Value);
                }
            }
        }

        public void Read(BinaryReader reader)
        {
            MappedValues.Clear();
            int enumCount = reader.ReadInt32();
            for (int i = 0; i < enumCount; i++)
            {
                var currentType = Type.GetType(reader.ReadString());
                int valueCount = reader.ReadInt32();
                var newDict = new Dictionary<int, string>();
                MappedValues[currentType] = newDict;
                for (int j = 0; j < valueCount; j++)
                {
                    newDict.Add(reader.ReadInt32(),reader.ReadString());
                }
            }
        }

        public T TranslateEnum<T>(TranslationMode mode, T id)
        {
            return (T)TranslateEnum(typeof(T), mode, id);
        }

        public object TranslateEnum(Type enumType, TranslationMode mode, object id)
        {
            return TranslateEnum(enumType, this, mode, id);
        }

        public void FixEnumValues(TranslationMode mode, object toFix)
        {
            FixEnumValues(this,mode,toFix);
        }
    }

    public class MissingTranslationException : Exception
    {
        public object value;
        public EnumTranslator.TranslationMode mode;

        public MissingTranslationException(object value, EnumTranslator.TranslationMode mode)
        {
            this.value = value;
            this.mode = mode;
        }
    }

    public partial class EnumTranslator
    {
        public delegate void EnumFixerGenericDelegate<T>(EnumTranslator translator,TranslationMode mode, T toFix);
        public delegate void EnumFixerDelegate(EnumTranslator translator, TranslationMode mode, object toFix);
        static Dictionary<Type,EnumFixerDelegate> enumFixers = new Dictionary<Type, EnumFixerDelegate>();

        public delegate bool MissingTranslationDelegate(Type enumType, ref string value);

        public delegate bool MissingTranslationGenericDelegate<T>(ref string value);
        static List<MissingTranslationDelegate> missingDelegates = new List<MissingTranslationDelegate>();

        public static bool DoDefaultTranslationFallbacks = true;

        private static List<MissingTranslationDelegate> defaultFallbacks = new List<MissingTranslationDelegate>()
        {
            ConvertGenericFallback<Identifiable.Id>((ref string x) =>
            {
                x = Identifiable.Id.NONE.ToString();
                return true;
            }),
            ConvertGenericFallback<GordoIdentifiable.Id>((ref string x) =>
            {
                x = GordoIdentifiable.Id.NONE.ToString();
                return true;
            }),
            ConvertGenericFallback<Gadget.Id>((ref string x) =>
            {
                x = Gadget.Id.NONE.ToString();
                return true;
            }),
            ConvertGenericFallback<LandPlot.Id>((ref string x) =>
            {
                x = LandPlot.Id.NONE.ToString();
                return true;
            }),
            ConvertGenericFallback<LandPlot.Upgrade>((ref string x) =>
            {
                x = LandPlot.Upgrade.NONE.ToString();
                return true;
            }),
            ConvertGenericFallback<SpawnResource.Id>((ref string x) =>
            {
                x = SpawnResource.Id.NONE.ToString();
                return true;
            }),
            ConvertGenericFallback<ProgressDirector.ProgressType>((ref string x) =>
            {
                x = ProgressDirector.ProgressType.NONE.ToString();
                return true;
            }),
            ConvertGenericFallback<Fashion.Slot>((ref string x) =>
            {
                x = Fashion.Slot.FRONT.ToString();
                return true;
            }),
            ConvertGenericFallback<MusicDirector.Priority>((ref string x) =>
            {
                x = MusicDirector.Priority.DEFAULT.ToString();
                return true;
            }),
            ConvertGenericFallback<MailDirector.Type>((ref string x) =>
            {
                x = MailDirector.Type.PERSONAL.ToString();
                return true;
            }),
            ConvertGenericFallback<SlimeAppearance.AppearanceSaveSet>((ref string x) =>
            {
                x = SlimeAppearance.AppearanceSaveSet.NONE.ToString();
                return true;
            }),
            ConvertGenericFallback<SlimeFace.SlimeExpression>((ref string x) =>
            {
                x = SlimeFace.SlimeExpression.None.ToString();
                return true;
            }),
            ConvertGenericFallback<AmbianceDirector.Weather>((ref string x) =>
            {
                x = AmbianceDirector.Weather.NONE.ToString();
                return true;
            }),
            ConvertGenericFallback<AmbianceDirector.Zone>((ref string x) =>
            {
                x = AmbianceDirector.Zone.DEFAULT.ToString();
                return true;
            }),
            ConvertGenericFallback<ZoneDirector.Zone>((ref string x) =>
            {
                x = ZoneDirector.Zone.NONE.ToString();
                return true;
            }),
            ConvertGenericFallback<MonomiPark.SlimeRancher.Regions.RegionRegistry.RegionSetId>((ref string x) =>
            {
                x = MonomiPark.SlimeRancher.Regions.RegionRegistry.RegionSetId.UNSET.ToString();
                return true;
            }),
            ConvertGenericFallback<RanchDirector.Palette>((ref string x) =>
            {
                x = RanchDirector.Palette.DEFAULT.ToString();
                return true;
            }),
            ConvertGenericFallback<RanchDirector.PaletteType>((ref string x) =>
            {
                x = RanchDirector.Palette.DEFAULT.ToString();
                return true;
            }),
            ConvertGenericFallback<RancherChatMetadata.Entry.RancherName>((ref string x) =>
            {
                x = RancherChatMetadata.Entry.RancherName.UNKNOWN.ToString();
                return true;
            }),
            ConvertGenericFallback<SiloCatcher.Type>((ref string x) =>
            {
                x = SiloCatcher.Type.SILO_DEFAULT.ToString();
                return true;
            }),
            ConvertGenericFallback<InstrumentModel.Instrument>((ref string x) =>
            {
                x = InstrumentModel.Instrument.NONE.ToString();
                return true;
            })
        };

        /// <summary>
        /// Register an EnumFixer that allows for objects of type <typeparamref name="T"/> to have their enum values processed by an enumtranslator 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="del"></param>
        public static void RegisterEnumFixer<T>(EnumFixerGenericDelegate<T> del)
        {
            RegisterEnumFixer(typeof(T),((EnumTranslator translator,TranslationMode mode, object fix) => del(translator,mode,(T)fix)));
        }

        public static void RegisterFallbackHandler(MissingTranslationDelegate del)
        {
            missingDelegates.Add(del);
        }

        public static void RegisterFallbackHandler<T>(MissingTranslationGenericDelegate<T> del)
        {
            RegisterFallbackHandler(ConvertGenericFallback(del));
        }

        public static bool OnTranslationFallback(Type enumType, ref string value)
        {
            bool success = false;
            foreach (var v in missingDelegates)
            {
                success |= v(enumType, ref value);
            }


            if (DoDefaultTranslationFallbacks)
            {
                if (!success) foreach (var v in defaultFallbacks)
                    {
                        success |= v(enumType, ref value);
                    }
            }
            return success;
        }

        public static void RegisterEnumFixer(Type type, EnumFixerDelegate del)
        {
            enumFixers.Add(type,del);
        }

        internal static MissingTranslationDelegate ConvertGenericFallback<T>(MissingTranslationGenericDelegate<T> del)
        {
            return (Type t, ref string x) =>
            {
                if (typeof(T) != t) return false;
                return del(ref x);
            };
        }

        static void FixEnumValues(EnumTranslator translator, TranslationMode mode, ref object toFix)
        {
            if (toFix == null) return;
            var type = toFix.GetType();
            if (type.IsEnum)
            {
                toFix = TranslateEnum(type, translator, mode, toFix);
            }
            else
            foreach (var v in enumFixers.Where((x) => x.Key.IsAssignableFrom(type)))
            {
                v.Value(translator, mode, toFix);
            }
            DoDefaultTranslationFallbacks = true;
        }

        /// <summary>
        /// Fix enum values in an object using the registered EnumFixer
        /// </summary>
        /// <param name="translator">the <see cref="EnumTranslator"/> to use for translating</param>
        /// <param name="mode">Whether to translate TO or FROM the translated form</param>
        /// <param name="toFix">Object to fix</param>
        public static void FixEnumValues(EnumTranslator translator, TranslationMode mode, object toFix)
        {
            if (toFix == null) return;
            var type = toFix.GetType();
            if (type.IsEnum)
            {
                throw new Exception("Use TranslateEnum for enumvalues");
            }
            else
                foreach (var v in enumFixers.Where((x) => x.Key.IsAssignableFrom(type))) v.Value(translator, mode, toFix);
        }

        public static T TranslateEnum<T>(EnumTranslator translator,TranslationMode mode, T id)
        {
            return (T)TranslateEnum(typeof(T),translator,mode,id);
        }

        public static object TranslateEnum(Type enumType, EnumTranslator translator, TranslationMode mode, object id)
        {
            return (mode == TranslationMode.TOTRANSLATED
                ? Enum.ToObject(enumType,translator.TranslateTo(id))
                : translator.TranslateFrom(enumType,(int)id));
        }

        public enum TranslationMode
        {
            TOTRANSLATED,
            FROMTRANSLATED
        }

        static EnumTranslator()
        {
            // basic enum fixer for lists
            RegisterEnumFixer((EnumTranslator translator, TranslationMode mode, IList list) =>
            {
                DoDefaultTranslationFallbacks = false;
                for (int i = list.Count-1;i>=0;i--)
                {
                    var temp = list[i];
                    try
                    {
                        FixEnumValues(translator, mode, ref temp);
                        list[i] = temp;
                    }
                    catch (MissingTranslationException)
                    {
                        list.RemoveAt(i);
                    }
                }
                DoDefaultTranslationFallbacks = true;
            });

            // basic enum fixer for dictionaries
            RegisterEnumFixer((EnumTranslator translator, TranslationMode mode, IDictionary dict) =>
            {
                DoDefaultTranslationFallbacks = false;
                var keyArray = new object[dict.Count];
                var valueArray = new object[dict.Count];
                int counter = 0;
                foreach (var v in dict.Keys)
                {
                    var temp = v;
                    try
                    {
                        FixEnumValues(translator, mode, ref temp);
                        keyArray[counter++] = temp;
                    }
                    catch (MissingTranslationException)
                    {
                        keyArray[counter++] = null;
                    }

                    
                }

                counter = 0;
                foreach (var v in dict.Values)
                {
                    var temp = v;
                    FixEnumValues(translator, mode, ref temp);
                    valueArray[counter++] = temp;
                }

                dict.Clear();
                for (int i = 0; i < keyArray.Length; i++)
                {

                    if (keyArray[i] == null) continue;
                    dict[keyArray[i]] = valueArray[i]; 
                }
                DoDefaultTranslationFallbacks = true;
            });
        }
    }
}
