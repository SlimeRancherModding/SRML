using SRML.Config.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.Config
{
    [ConfigFile("test")]
    public static class TestConfig
    {
        public static float test_float = 1f;
        public static string TestString = "hello";

        public static sbyte aa = -1;

        public static string otherstring = "123 ";
    }
}
