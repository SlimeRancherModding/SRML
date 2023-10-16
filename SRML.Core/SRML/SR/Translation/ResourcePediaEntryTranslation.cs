using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.Translation
{
    class ResourcePediaEntryTranslation : PediaEntryTranslation
    {
        public ResourcePediaEntryTranslation(PediaDirector.Id id) : base(id)
        {
        }

        const string RESOURCE_TYPE_PREFIX = "m.resource_type.";
        const string FAVORED_BY_LABEL_PREFIX = "l.favored_by.";
        const string FAVORED_BY_PREFIX = "m.favored_by.";
        const string HOW_TO_USE_PREFIX = "m.how_to_use.";

        public string ResourceTypeKey => RESOURCE_TYPE_PREFIX + StringKey;
        public string FavoredByLabelKey => FAVORED_BY_LABEL_PREFIX + StringKey;
        public string FavoredByKey => FAVORED_BY_PREFIX + StringKey;
        public string HowToUseKey => HOW_TO_USE_PREFIX + StringKey;

        public ResourcePediaEntryTranslation SetResourceTypeTranslation(string name)
        {
            TranslationPatcher.AddPediaTranslation(ResourceTypeKey, name);
            return this;
        }

        public ResourcePediaEntryTranslation SetFavoredByLabelTranslation(string name)
        {
            TranslationPatcher.AddPediaTranslation(FavoredByLabelKey, name);
            return this;
        }

        public ResourcePediaEntryTranslation SetFavoredByTranslation(string name)
        {
            TranslationPatcher.AddPediaTranslation(FavoredByKey, name);
            return this;
        }

        public ResourcePediaEntryTranslation SetHowToUseTranslation(string name)
        {
            TranslationPatcher.AddPediaTranslation(HowToUseKey, name);
            return this;
        }
        public new ResourcePediaEntryTranslation SetTitleTranslation(string name)
        {
            TranslationPatcher.AddPediaTranslation(TitleKey, name);
            return this;
        }

        public new ResourcePediaEntryTranslation SetIntroTranslation(string intro)
        {
            TranslationPatcher.AddPediaTranslation(IntroKey, intro);
            return this;
        }

        public new ResourcePediaEntryTranslation SetDescriptionTranslation(string description)
        {
            TranslationPatcher.AddPediaTranslation(DescriptionKey, description);
            return this;
        }
    }
}
