using SRML.Core;
using System;

namespace SRML.SR.Translation
{
    public class PediaEntryTranslation : ITranslatable<PediaDirector.Id>
    {
        public PediaDirector.Id Key { get; protected set; }
        public MessageDirector.Lang Language { get; protected set; }

        public string StringKey => Enum.GetName(typeof(PediaDirector.Id), Key).ToLower();

        public PediaEntryTranslation(PediaDirector.Id id)
        {
            this.Key = id;
        }
        public PediaEntryTranslation(PediaDirector.Id id, MessageDirector.Lang lang)
        {
            this.Key = id;
            this.Language = lang;
        }

        public string TitleKey => TITLE_PREFIX + StringKey;
        public string IntroKey => INTRO_PREFIX + StringKey;
        public string DescriptionKey => DESCRIPTION_PREFIX + StringKey;

        const string TITLE_PREFIX = "t.";
        const string INTRO_PREFIX = "m.intro.";
        const string DESCRIPTION_PREFIX = "m.desc.";

        public PediaEntryTranslation SetTitleTranslation(string name)
        {
            CoreTranslator.Instance.AddPediaTranslation(Language, TitleKey, name);
            return this;
        }

        public PediaEntryTranslation SetIntroTranslation(string intro)
        {
            CoreTranslator.Instance.AddPediaTranslation(Language, IntroKey, intro);
            return this;
        }

        public PediaEntryTranslation SetDescriptionTranslation(string description)  
        {
            CoreTranslator.Instance.AddPediaTranslation(Language, DescriptionKey, description);
            return this;
        }

    }
}
