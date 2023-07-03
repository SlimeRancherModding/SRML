using System;
using SRML.Core;

namespace SRML.SR
{
    /// <summary>
    /// Localization
    /// </summary>
    [Obsolete]
    public static class TranslationPatcher
    {
        /// <summary>
        /// Add a plaintext translation for a localization key
        /// </summary>
        /// <param name="bundlename">Key bundle the localization key is located in</param>
        /// <param name="key">The localization key</param>
        /// <param name="value">The plain text translation</param>
        public static void AddTranslationKey(string bundlename, string key, string value) =>
            CoreTranslator.Instance.AddTranslation(MessageDirector.Lang.EN, bundlename, key, value);

        /// <summary>
        /// Add a plaintext translation for a localization key in the 'pedia' bundle
        /// </summary>
        /// <param name="key">The localization key</param>
        /// <param name="value">The plain text translation</param>
        public static void AddPediaTranslation(string key, string value) =>
            CoreTranslator.Instance.AddPediaTranslation(MessageDirector.Lang.EN, key, value);

        /// <summary>
        /// Add a plaintext translation for a localization key in the 'actor' bundle
        /// </summary>
        /// <param name="key">The localization key</param>
        /// <param name="value">The plain text translation</param>
        public static void AddActorTranslation(string key, string value) =>
            CoreTranslator.Instance.AddActorTranslation(MessageDirector.Lang.EN, key, value);

        /// <summary>
        /// Add a plaintext translation for a localization key in the 'ui' bundle
        /// </summary>
        /// <param name="key">The localization key</param>
        /// <param name="value">The plain text translation</param>
        public static void AddUITranslation(string key, string value) =>
            CoreTranslator.Instance.AddUITranslation(MessageDirector.Lang.EN, key, value);

        /// <summary>
        /// Add a plaintext translation for a localization key in the 'achieve' bundle
        /// </summary>
        /// <param name="key">The localization key</param>
        /// <param name="value">The plain text translation</param>
        public static void AddAchievementTranslation(string key, string value) =>
            CoreTranslator.Instance.AddAchievementTranslation(MessageDirector.Lang.EN, key, value);

        /// <summary>
        /// Add a plaintext translation for a localization key in the 'exchange' bundle
        /// </summary>
        /// <param name="key">The localization key</param>
        /// <param name="value">The plain text translation</param>
        public static void AddExchangeTranslation(string key, string value) =>
            CoreTranslator.Instance.AddExchangeTranslation(MessageDirector.Lang.EN, key, value);

        /// <summary>
        /// Add a plaintext translation for a localization key in the 'global' bundle
        /// </summary>
        /// <param name="key">The localization key</param>
        /// <param name="value">The plain text translation</param>
        public static void AddGlobalTranslation(string key, string value) =>
            CoreTranslator.Instance.AddGlobalTranslation(MessageDirector.Lang.EN, key, value);

        /// <summary>
        /// Add a plaintext translation for a localization key in the 'mail' bundle
        /// </summary>
        /// <param name="key">The localization key</param>
        /// <param name="value">The plain text translation</param>
        public static void AddMailTranslation(string key, string value) =>
            CoreTranslator.Instance.AddMailTranslation(MessageDirector.Lang.EN, key, value);

        /// <summary>
        /// Add a plaintext translation for a localization key in the 'tutorial' bundle
        /// </summary>
        /// <param name="key">The localization key</param>
        /// <param name="value">The plain text translation</param>
        public static void AddTutorialTranslation(string key, string value) =>
            CoreTranslator.Instance.AddTutorialTranslation(MessageDirector.Lang.EN, key, value);

        /// <summary>
        /// Add a plaintext translation for localization for SRML error messages
        /// </summary>
        /// <param name="language">The language to be translated to</param>
        /// <param name="errorText">The plain text translation for the text saying it's an error</param>
        /// <param name="abortText">The plain text translation for the text saying mod-loading is aborting</param>
        public static void AddSRMLErrorUITranslation(MessageDirector.Lang language, string errorText, string abortText) => 
            CoreTranslator.Instance.AddSRMLErrorTranslation(language, errorText, abortText);
    }
}
