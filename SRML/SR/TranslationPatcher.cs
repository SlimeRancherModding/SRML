using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Converters;
using SRML.SR.SaveSystem.Data.Partial;
using UnityEngine;

namespace SRML.SR
{
    /// <summary>
    /// Localization
    /// </summary>
    public static class TranslationPatcher
    {

        internal static Dictionary<string, Dictionary<string, string>> doneDictionaries = new Dictionary<string, Dictionary<string, string>>();

        internal static Dictionary<string, Dictionary<string,string>> patches = new Dictionary<string, Dictionary<string, string>>();

        internal static Dictionary<MessageDirector.Lang, KeyValuePair<string, string>> srmlErrorMessages = new Dictionary<MessageDirector.Lang, KeyValuePair<string, string>>();

        internal static Dictionary<KeyValuePair<string,string>,SRMod> keyToMod = new Dictionary<KeyValuePair<String,String>,SRMod>();

        internal static void SetModForTranslationKey(string bundlename, string key, SRMod mod) => keyToMod[new KeyValuePair<string, string>(bundlename, key)] = mod;

        internal static void AddTranslationKey(string bundlename, string key, string value, SRMod mod)
        {
            if (GetPatchesFor(bundlename).ContainsKey(key)) Debug.LogWarning($"Translation key '{key}' for bundle '{bundlename}' is already taken by {keyToMod[new KeyValuePair<string, string>(bundlename, key)]}! Overwriting...");
            GetPatchesFor(bundlename)[key] = value;
            SetModForTranslationKey(bundlename,key,mod);
        }

        /// <summary>
        /// Add a plaintext translation for a localization key
        /// </summary>
        /// <param name="bundlename">Key bundle the localization key is located in</param>
        /// <param name="key">The localization key</param>
        /// <param name="value">The plain text translation</param>
        public static void AddTranslationKey(string bundlename, string key, string value) => AddTranslationKey(bundlename, key, value, SRMod.GetCurrentMod());

        /// <summary>
        /// Add a plaintext translation for a localization key in the 'pedia' bundle
        /// </summary>
        /// <param name="key">The localization key</param>
        /// <param name="value">The plain text translation</param>
        public static void AddPediaTranslation(string key, string value) => AddTranslationKey("pedia", key,value);

        /// <summary>
        /// Add a plaintext translation for a localization key in the 'actor' bundle
        /// </summary>
        /// <param name="key">The localization key</param>
        /// <param name="value">The plain text translation</param>
        public static void AddActorTranslation(string key, string value) => AddTranslationKey("actor",key,value);

        /// <summary>
        /// Add a plaintext translation for a localization key in the 'ui' bundle
        /// </summary>
        /// <param name="key">The localization key</param>
        /// <param name="value">The plain text translation</param>
        public static void AddUITranslation(string key, string value) => AddTranslationKey("ui",key,value);

        public static void AddSRMLErrorUITranslation(MessageDirector.Lang language, string errorText, string abortText) => srmlErrorMessages.Add(language, new KeyValuePair<string, string>(errorText, abortText));

        private static Dictionary<string, string> GetPatchesFor(string bundleName)
        {
            if (doneDictionaries.TryGetValue(bundleName, out var test)) return test;

            if (!patches.ContainsKey(bundleName))
            {
                patches.Add(bundleName, new Dictionary<string,string>());
            }

            return patches[bundleName];
        }

        internal static Dictionary<string, List<string>> GetKeysForMod(SRMod mod)
        {
            Dictionary<string, List<string>> output = new Dictionary<string, List<string>>();
            List<string> GetListForKey(string key)
            {
                return output.TryGetValue(key, out var list) ? list : output[key] = new List<string>();
            }

            foreach(var pair in keyToMod.Where((x)=>x.Value==mod).Select((x)=>x.Key)) GetListForKey(pair.Key).Add(pair.Value);
            
            return output;
        }

        static internal SRMod GetModForKey(string bundlename, string key) => GetModForKey(new KeyValuePair<string, string>(bundlename, key));

        static internal SRMod GetModForKey(KeyValuePair<string,string> pair) => keyToMod.TryGetValue(pair, out var value) ? value : null;
    }
}
