using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.Config.Parsing
{
    static class StringUtils
    {
        internal static string ToQuotedString(string str)
        {
            if (str == null) str = "";
            string newStr = "";
            newStr += '\"';
            foreach (var ch in str)
            {
                if (ch == '\"') newStr += "\\\"";
                else newStr += ch;
            }
            newStr += '\"';
            return newStr;
        }

        internal static string FromQuotedString(string str)
        {
            if (str == null) str = "";
            string newStr = "";
            str = str.Trim(' ', '\"');
            bool isescaped = false;
            foreach (var v in str)
            {
                if (!isescaped && v == '\\') isescaped = true;
                else if (v == '\\') { newStr += '\\'; isescaped = false; }
                else if (isescaped && v == '\"') { newStr += '\"'; isescaped = false; }
                else
                {
                    isescaped = false;
                    newStr += v;
                }
            }
            return newStr;
        }
    }
}
