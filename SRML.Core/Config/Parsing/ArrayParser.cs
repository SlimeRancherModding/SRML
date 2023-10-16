using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.Config.Parsing
{
    public class ArrayParser : IStringParser
    {
        public Type ParsedType { get; }
        public ArrayParser(Type array)
        {
            ParsedType = array;
        }

        public string EncodeObject(object obj)
        {
            Array array = (Array)obj;
            if (array == null) array = Array.CreateInstance(ParsedType.GetElementType(), 0);
            IStringParser parser = ParserRegistry.GetParser(ParsedType.GetElementType());
            string str = "[";
            for(int i = 0; i < array.Length; i++)
            {
                str += parser.EncodeObject(array.GetValue(i));
                if (i != array.Length - 1) str += ", ";
            }
            str += ']';
            return str;
        }

        public string GetUsageString()
        {
            return ParsedType.ToString();
        }

        public object ParseObject(string str)
        {   
            var parser = ParserRegistry.GetParser(ParsedType.GetElementType());
            str = str.Trim(' ', '[', ']');
            List<object> objs = new List<object>();
            foreach (var s in str.Split(','))
            {
                objs.Add(parser.ParseObject(s.Trim()));
            }


            Array array = Array.CreateInstance(ParsedType.GetElementType(), objs.Count);
            for(int i = 0; i < objs.Count; i++)
            {
                array.SetValue(objs[i], i);
            }
            return array;
        }
    }
}
