using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.Translation
{
    public class SlimePediaEntryTranslation : PediaEntryTranslation
    {
        public SlimePediaEntryTranslation(PediaDirector.Id id) : base(id)
        {
        }

        const string DIET_PREFIX = "m.diet.";
        const string FAVORITE_PREFIX = "m.favorite.";
        const string SLIMEOLOGY_PREFIX = "m.slimeology.";
        const string RISKS_PREFIX = "m.risks.";
        const string PLORTONOMICS_PREFIX = "m.plortonomics.";

        public string DietKey => DIET_PREFIX + StringKey;
        public string FavoriteKey => FAVORITE_PREFIX + StringKey;
        public string SlimeologyKey => SLIMEOLOGY_PREFIX + StringKey;
        public string RisksKey => RISKS_PREFIX + StringKey;
        public string PlortonomicsKey => PLORTONOMICS_PREFIX + StringKey;

        public SlimePediaEntryTranslation SetDietTranslation(string dietString)
        {
            TranslationPatcher.AddPediaTranslation(DietKey, dietString);
            return this;
        }

        public SlimePediaEntryTranslation SetFavoriteTranslation(string favorite)
        {
            TranslationPatcher.AddPediaTranslation(FavoriteKey, favorite);
            return this;
        }

        public SlimePediaEntryTranslation SetPlortonomicsTranslation(string plortonomics)
        {
            TranslationPatcher.AddPediaTranslation(PlortonomicsKey, plortonomics);
            return this;
        }

        public SlimePediaEntryTranslation SetSlimeologyTranslation(string slimeology)
        {
            TranslationPatcher.AddPediaTranslation(SlimeologyKey, slimeology);
            return this;
        }

        public SlimePediaEntryTranslation SetRisksTranslation(string risks)
        {
            TranslationPatcher.AddPediaTranslation(RisksKey, risks);
            return this;
        }

        public new SlimePediaEntryTranslation SetTitleTranslation(string name)
        {
            TranslationPatcher.AddPediaTranslation(TitleKey, name);
            return this;
        }

        public new SlimePediaEntryTranslation SetIntroTranslation(string intro)
        {
            TranslationPatcher.AddPediaTranslation(IntroKey, intro);
            return this;
        }

        [Obsolete]
        public new SlimePediaEntryTranslation SetDescriptionTranslation(string description)
        {
            TranslationPatcher.AddPediaTranslation(DescriptionKey, description);
            return this;
        }
    }
}
