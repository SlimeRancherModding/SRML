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
