using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.Translation
{
    public class PediaEntryTranslation : ITranslatable<PediaDirector.Id>
    {
        public PediaDirector.Id Key { get; protected set; }

        public string StringKey => Enum.GetName(typeof(PediaDirector.Id), Key).ToLower();

        public PediaEntryTranslation(PediaDirector.Id id)
        {
            this.Key = id;
        }

        public string TitleKey => TITLE_PREFIX + StringKey;
        public string IntroKey => INTRO_PREFIX + StringKey;
        public string DescriptionKey => DESCRIPTION_PREFIX + StringKey;

        const string TITLE_PREFIX = "t.";
        const string INTRO_PREFIX = "m.intro.";
        const string DESCRIPTION_PREFIX = "m.desc.";

        public PediaEntryTranslation SetTitleTranslation(string name)
        {
            TranslationPatcher.AddPediaTranslation(TitleKey, name);
            return this;
        }

        public PediaEntryTranslation SetIntroTranslation(string intro)
        {
            TranslationPatcher.AddPediaTranslation(IntroKey, intro);
            return this;
        }

        public PediaEntryTranslation SetDescriptionTranslation(string description)  
        {
            TranslationPatcher.AddPediaTranslation(DescriptionKey, description);
            return this;
        }

    }
}
