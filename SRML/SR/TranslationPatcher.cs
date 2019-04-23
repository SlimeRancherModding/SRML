using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR
{
    public static class TranslationPatcher
    {
        internal static Dictionary<string,Dictionary<string,string>> patches = new Dictionary<string, Dictionary<string, string>>();

        public static void AddTranslationKey(string bundlename, string key, string value)
        {
            GetPatchesFor(bundlename).Add(key,value);
        }

        public static void AddPediaTranslation(string key, string value)
        {
            AddTranslationKey("pedia",key,value);
        }

        public static void AddActorTranslation(string key, string value)
        {
            AddTranslationKey("actor",key,value);
        }

        public static void AddUITranslation(string key, string value)
        {
            AddTranslationKey("ui",key,value);
        }

        internal static Dictionary<string, string> GetPatchesFor(string bundleName)
        {
            if (!patches.ContainsKey(bundleName))
            {
                patches.Add(bundleName, new Dictionary<string,string>());
            }

            return patches[bundleName];
        }
    }
}
