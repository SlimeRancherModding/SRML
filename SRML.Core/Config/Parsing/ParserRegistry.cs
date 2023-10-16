using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.Config.Parsing
{
    public static class ParserRegistry
    {
        static readonly Dictionary<Type, IStringParser> parsers = new Dictionary<Type, IStringParser>()
        {
            {typeof(int),new DelegateStringParser<int>(x=>x.ToString(),int.Parse) },
            {typeof(bool),new DelegateStringParser<bool>(x=>x.ToString(),bool.Parse) },
            {typeof(float),new DelegateStringParser<float>(x=>x.ToString(),float.Parse) },
            {typeof(long),new DelegateStringParser<long>(x=>x.ToString(),long.Parse) },
            {typeof(double),new DelegateStringParser<double>(x=>x.ToString(),double.Parse) },
            {typeof(uint),new DelegateStringParser<uint>(x=>x.ToString(),uint.Parse) },
            {typeof(byte),new DelegateStringParser<byte>(x=>x.ToString(),byte.Parse) },
            {typeof(sbyte),new DelegateStringParser<sbyte>(x=>x.ToString(),sbyte.Parse) },
            {typeof(short),new DelegateStringParser<short>(x=>x.ToString(),short.Parse) },
            {typeof(ushort),new DelegateStringParser<ushort>(x=>x.ToString(),ushort.Parse) },
            {typeof(ulong),new DelegateStringParser<ulong>(x=>x.ToString(),ulong.Parse) },
            {typeof(string),new DelegateStringParser<string>(x=>StringUtils.ToQuotedString(x),x=>StringUtils.FromQuotedString(x)) }

        };



        public static IStringParser GetParser(Type type)
        {
            return type.IsArray?new ArrayParser(type):(type.IsEnum?GetEnumParser(type) : parsers[type]);
        }

        public static bool TryGetParser(Type type, out IStringParser parser)
        {
            try
            {
                parser = GetParser(type);
                return true;
            }
            catch
            {
                parser = null;
                return false;
            }

        }

        

        static IStringParser GetEnumParser(Type enumType)
        {
            if (!enumType.IsEnum) throw new Exception("Type is not an enum!");
            if(!parsers.TryGetValue(enumType, out var parser))
            {
                parser = new EnumStringParser(enumType);
                parsers[enumType] = parser;
            }
            return parser;
        }

        public static void RegisterParser(IStringParser parser)
        {
            parsers[parser.ParsedType] = parser;
        }

        internal class EnumStringParser : IStringParser
        {


            public Type ParsedType { get; }

            public EnumStringParser(Type enumType)
            {
                ParsedType = enumType;
            }

            public string EncodeObject(object obj)
            {
                return Enum.GetName(ParsedType, obj);
            }

            public string GetUsageString()
            {
                return ParsedType.ToString();
            }

            public object ParseObject(string str)
            {
                return Enum.Parse(ParsedType, str, true);
            }
        }
    }
}
