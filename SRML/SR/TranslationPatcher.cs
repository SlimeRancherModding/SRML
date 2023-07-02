using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Converters;
using SRML.Core.ModLoader;
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

        internal static Dictionary<KeyValuePair<string,string>, IMod> keyToMod = new Dictionary<KeyValuePair<string, string>, IMod>();

        internal static void SetModForTranslationKey(string bundlename, string key, IMod mod) => keyToMod[new KeyValuePair<string, string>(bundlename, key)] = mod;

        internal static void AddTranslationKey(string bundlename, string key, string value, IMod mod)
        {
            if (GetPatchesFor(bundlename).ContainsKey(key)) Debug.LogWarning($"Translation key '{key}' for bundle '{bundlename}' is already taken by {keyToMod[new KeyValuePair<string, string>(bundlename, key)].ModInfo.Id}! Overwriting...");
            GetPatchesFor(bundlename)[key] = value;
            SetModForTranslationKey(bundlename, key, mod);
        }

        /// <summary>
        /// Add a plaintext translation for a localization key
        /// </summary>
        /// <param name="bundlename">Key bundle the localization key is located in</param>
        /// <param name="key">The localization key</param>
        /// <param name="value">The plain text translation</param>
        public static void AddTranslationKey(string bundlename, string key, string value) =>
            AddTranslationKey(bundlename, key, value, CoreLoader.Instance.GetExecutingModContext());

        /// <summary>
        /// Add a plaintext translation for a localization key in the 'pedia' bundle
        /// </summary>
        /// <param name="key">The localization key</param>
        /// <param name="value">The plain text translation</param>
        public static void AddPediaTranslation(string key, string value) => AddTranslationKey("pedia", key, value);

        /// <summary>
        /// Add a plaintext translation for a localization key in the 'actor' bundle
        /// </summary>
        /// <param name="key">The localization key</param>
        /// <param name="value">The plain text translation</param>
        public static void AddActorTranslation(string key, string value) => AddTranslationKey("actor", key, value);

        /// <summary>
        /// Add a plaintext translation for a localization key in the 'ui' bundle
        /// </summary>
        /// <param name="key">The localization key</param>
        /// <param name="value">The plain text translation</param>
        public static void AddUITranslation(string key, string value) => AddTranslationKey("ui", key, value);

        /// <summary>
        /// Add a plaintext translation for a localization key in the 'achieve' bundle
        /// </summary>
        /// <param name="key">The localization key</param>
        /// <param name="value">The plain text translation</param>
        public static void AddAchievementTranslation(string key, string value) => AddTranslationKey("achieve", key, value);

        /// <summary>
        /// Add a plaintext translation for a localization key in the 'exchange' bundle
        /// </summary>
        /// <param name="key">The localization key</param>
        /// <param name="value">The plain text translation</param>
        public static void AddExchangeTranslation(string key, string value) => AddTranslationKey("exchange", key, value);

        /// <summary>
        /// Add a plaintext translation for a localization key in the 'global' bundle
        /// </summary>
        /// <param name="key">The localization key</param>
        /// <param name="value">The plain text translation</param>
        public static void AddGlobalTranslation(string key, string value) => AddTranslationKey("global", key, value);

        /// <summary>
        /// Add a plaintext translation for a localization key in the 'mail' bundle
        /// </summary>
        /// <param name="key">The localization key</param>
        /// <param name="value">The plain text translation</param>
        public static void AddMailTranslation(string key, string value) => AddTranslationKey("mail", key, value);

        /// <summary>
        /// Add a plaintext translation for a localization key in the 'tutorial' bundle
        /// </summary>
        /// <param name="key">The localization key</param>
        /// <param name="value">The plain text translation</param>
        public static void AddTutorialTranslation(string key, string value) => AddTranslationKey("tutorial", key, value);

        /// <summary>
        /// Add a plaintext translation for localization for SRML error messages
        /// </summary>
        /// <param name="language">The language to be translated to</param>
        /// <param name="errorText">The plain text translation for the text saying it's an error</param>
        /// <param name="abortText">The plain text translation for the text saying mod-loading is aborting</param>
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

        static internal IMod GetModForKey(string bundlename, string key) => GetModForKey(new KeyValuePair<string, string>(bundlename, key));

        static internal IMod GetModForKey(KeyValuePair<string,string> pair) => keyToMod.TryGetValue(pair, out var value) ? value : null;
    }
}
