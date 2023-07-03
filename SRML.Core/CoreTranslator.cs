using System.Collections.Generic;

namespace SRML.Core
{
    public class CoreTranslator
    {
        public static CoreTranslator Instance { get; private set; }

        internal MessageDirector.Lang currLang;
        internal Dictionary<string, Dictionary<string, string>> cachedBundles = new Dictionary<string, Dictionary<string, string>>();
        internal Dictionary<MessageDirector.Lang, Dictionary<string, Dictionary<string, string>>> patchesForLang =
            new Dictionary<MessageDirector.Lang, Dictionary<string, Dictionary<string, string>>>();

        internal Dictionary<MessageDirector.Lang, KeyValuePair<string, string>> srmlErrorMessages =
            new Dictionary<MessageDirector.Lang, KeyValuePair<string, string>>()
            {
                { MessageDirector.Lang.DE, new KeyValuePair<string, string>("SRML-FEHLER", "Ladevorgang des mod wird abgebrochen...") },
                { MessageDirector.Lang.ES, new KeyValuePair<string, string>("SRML ERROR", "Cargando mod abortado...") },
                { MessageDirector.Lang.FR, new KeyValuePair<string, string>("SRML ERREUR", "Annulation du chargement du mod...") },
                { MessageDirector.Lang.RU, new KeyValuePair<string, string>("SRML ОШИБКА", "Прерывается загрузка модификация...") },
                { MessageDirector.Lang.SV, new KeyValuePair<string, string>("SRML FEL", "Avbrytande laddning av modifiering...") },
                { MessageDirector.Lang.ZH, new KeyValuePair<string, string>("SRML 错误", "正在中止模组加载...") },
                { MessageDirector.Lang.JA, new KeyValuePair<string, string>("SRMLのエラー", "MODのロードを中断しています...") },
                { MessageDirector.Lang.PT, new KeyValuePair<string, string>("SRML ERROR", "Carregamento mod abortado...") },
                { MessageDirector.Lang.KO, new KeyValuePair<string, string>("SRML 오류", "모드 로드 중단 중...") },
                { MessageDirector.Lang.EN, new KeyValuePair<string, string>("SRML ERROR", "Aborting mod loading...") },
            };

        internal void ClearCache(MessageDirector.Lang newLang)
        {
            cachedBundles.Clear();
            currLang = newLang;
        }

        public void AddTranslation(MessageDirector.Lang lang, string bundle, string key, string value)
        {
            if (!patchesForLang.ContainsKey(lang))
                patchesForLang[lang] = new Dictionary<string, Dictionary<string, string>>();
            if (!patchesForLang[lang].ContainsKey(bundle))
                patchesForLang[lang][bundle] = new Dictionary<string, string>();

            patchesForLang[lang][bundle][key] = value;
        }

        public void AddPediaTranslation(MessageDirector.Lang lang, string key, string value) =>
            AddTranslation(lang, "pedia", key, value);
        public void AddActorTranslation(MessageDirector.Lang lang, string key, string value) =>
            AddTranslation(lang, "actor", key, value);
        public void AddUITranslation(MessageDirector.Lang lang, string key, string value) =>
            AddTranslation(lang, "ui", key, value);
        public void AddAchievementTranslation(MessageDirector.Lang lang, string key, string value) =>
            AddTranslation(lang, "achieve", key, value);
        public void AddExchangeTranslation(MessageDirector.Lang lang, string key, string value) =>
            AddTranslation(lang, "exchange", key, value);
        public void AddGlobalTranslation(MessageDirector.Lang lang, string key, string value) =>
            AddTranslation(lang, "global", key, value);
        public void AddMailTranslation(MessageDirector.Lang lang, string key, string value) =>
            AddTranslation(lang, "mail", key, value);
        public void AddTutorialTranslation(MessageDirector.Lang lang, string key, string value) =>
            AddTranslation(lang, "tutorial", key, value);

        public void AddSRMLErrorTranslation(MessageDirector.Lang lang, string errorTitle, string errorAborting) =>
            srmlErrorMessages[lang] = new KeyValuePair<string, string>(errorTitle, errorAborting);

        internal CoreTranslator() => Instance = this;
    }
}
