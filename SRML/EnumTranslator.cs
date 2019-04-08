using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML
{
    public partial class EnumTranslator
    {
        public Dictionary<Type,Dictionary<int,string>> MappedValues = new Dictionary<Type, Dictionary<int, string>>();

        public int GetFreeValue(Type enumType)
        {
            if (!MappedValues.ContainsKey(enumType)) return -1;
            return MappedValues[enumType].Keys.LastOrDefault()-1;
        }   
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

        public int TranslateTo(object val)
        {
            var type = val.GetType();
            if (!MappedValues.ContainsKey(type)) return ((int) val);
            var potential = MappedValues[type].FirstOrDefault((x) => Enum.GetName(type, val) == x.Value);
            return potential.Key<0?potential.Key : ((int)val);
        }

        public T TranslateFrom<T>(int val)
        {
            return (T)TranslateFrom(typeof(T), val);
        }

        public object TranslateFrom(Type enumType, int val)
        {
            return val<0?Enum.Parse(enumType,MappedValues[enumType][val]):Enum.ToObject(enumType,val);
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
            return TranslateEnum(this, mode, id);
        }

        public void FixEnumValues(TranslationMode mode, object toFix)
        {
            FixEnumValues(this,mode,toFix);
        }

        
    }

    public partial class EnumTranslator
    {
        public delegate void EnumFixerGenericDelegate<T>(EnumTranslator translator,TranslationMode mode, T toFix);
        public delegate void EnumFixerDelegate(EnumTranslator translator, TranslationMode mode, object toFix);
        static Dictionary<Type,EnumFixerDelegate> enumFixers = new Dictionary<Type, EnumFixerDelegate>();

        public static void RegisterEnumFixer<T>(EnumFixerGenericDelegate<T> del)
        {
            RegisterEnumFixer(typeof(T),((EnumTranslator translator,TranslationMode mode, object fix) => del(translator,mode,(T)fix)));
        }

        public static void RegisterEnumFixer(Type type, EnumFixerDelegate del)
        {
            enumFixers.Add(type,del);
        }

        static void FixEnumValues(EnumTranslator translator, TranslationMode mode, ref object toFix)
        {
            if (toFix == null) return;
            var type = toFix.GetType();
            if (type.IsEnum)
            {
                toFix = mode == TranslationMode.TOTRANSLATED
                    ? translator.TranslateTo(toFix)
                    : translator.TranslateFrom(type, (int)toFix);
            }
            else
                foreach (var v in enumFixers.Where((x) => x.Key.IsAssignableFrom(type)))
                {
                    v.Value(translator, mode, toFix);
                }
        }

        public static void FixEnumValues(EnumTranslator translator, TranslationMode mode, object toFix)
        {
            if (toFix == null) return;
            var type = toFix.GetType();
            if (type.IsEnum)
            {
                throw new Exception("ENUMS BELONG IN THE REF VERSION OF FIXENUMVALUES");
            }
            else
                foreach (var v in enumFixers.Where((x) => x.Key.IsAssignableFrom(type))) v.Value(translator, mode, toFix);
        }

        public static T TranslateEnum<T>(EnumTranslator translator,TranslationMode mode, T id)
        {
            return (mode == TranslationMode.TOTRANSLATED
                ? (T)(object)translator.TranslateTo(id)
                : translator.TranslateFrom<T>((int)(object)id));
        }

        public enum TranslationMode
        {
            TOTRANSLATED,
            FROMTRANSLATED
        }

        static EnumTranslator()
        {
            RegisterEnumFixer((EnumTranslator translator, TranslationMode mode, IList list) =>
            {
                for(int i = 0;i<list.Count;i++)
                {
                    var temp = list[i];
                    FixEnumValues(translator,mode,ref temp);
                    list[i] = temp;
                }
            });
            RegisterEnumFixer((EnumTranslator translator, TranslationMode mode, IDictionary dict) =>
            {
                var keyArray = new object[dict.Count];
                var valueArray = new object[dict.Count];
                int counter = 0;
                foreach (var v in dict.Keys)
                {
                    var temp = v;
                    FixEnumValues(translator,mode,ref temp);
                    keyArray[counter++] = temp;
                }

                counter = 0;
                foreach (var v in dict.Values)
                {
                    var temp = v;
                    FixEnumValues(translator, mode, ref temp);
                    valueArray[counter++] = temp;
                }

                for (int i = 0; i < keyArray.Length; i++)
                {
                    dict[keyArray[i]] = valueArray[i];
                }
            });
        }
    }
}
