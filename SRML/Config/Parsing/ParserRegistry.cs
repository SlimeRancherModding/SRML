using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.Config.Parsing
{
    public static class ParserRegistry
    {
        static readonly Dictionary<Type, IStringParser> parsers = new Dictionary<Type, IStringParser>()
        {
            {typeof(int),new DelegateStringParser<int>(x=>x.ToString(),int.Parse) },
            {typeof(float),new DelegateStringParser<float>(x=>x.ToString(),float.Parse) },
            {typeof(long),new DelegateStringParser<long>(x=>x.ToString(),long.Parse) },
            {typeof(double),new DelegateStringParser<double>(x=>x.ToString(),double.Parse) },
            {typeof(uint),new DelegateStringParser<uint>(x=>x.ToString(),uint.Parse) },
            {typeof(byte),new DelegateStringParser<byte>(x=>x.ToString(),byte.Parse) },
            {typeof(sbyte),new DelegateStringParser<sbyte>(x=>x.ToString(),sbyte.Parse) },
            {typeof(short),new DelegateStringParser<short>(x=>x.ToString(),short.Parse) },
            {typeof(ushort),new DelegateStringParser<ushort>(x=>x.ToString(),ushort.Parse) },
            {typeof(ulong),new DelegateStringParser<ulong>(x=>x.ToString(),ulong.Parse) },
            {typeof(string),new DelegateStringParser<string>(x=>x,x=>x) }

        };
        public static IStringParser GetParser(Type type)
        {
            return parsers[type];
        }

        public static bool TryGetParser(Type type, out IStringParser parser)
        {
            return parsers.TryGetValue(type, out parser);
        }

        public static void RegisterParser(IStringParser parser)
        {
            parsers[parser.ParsedType] = parser;
        }

    }
}
