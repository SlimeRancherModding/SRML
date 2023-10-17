using SRML.Core;
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
            CoreTranslator.Instance.AddPediaTranslation(Language, DietKey, dietString);
            return this;
        }

        public SlimePediaEntryTranslation SetFavoriteTranslation(string favorite)
        {
            CoreTranslator.Instance.AddPediaTranslation(Language, FavoriteKey, favorite);
            return this;
        }

        public SlimePediaEntryTranslation SetPlortonomicsTranslation(string plortonomics)
        {
            CoreTranslator.Instance.AddPediaTranslation(Language, PlortonomicsKey, plortonomics);
            return this;
        }

        public SlimePediaEntryTranslation SetSlimeologyTranslation(string slimeology)
        {
            CoreTranslator.Instance.AddPediaTranslation(Language, SlimeologyKey, slimeology);
            return this;
        }

        public SlimePediaEntryTranslation SetRisksTranslation(string risks)
        {
            CoreTranslator.Instance.AddPediaTranslation(Language, RisksKey, risks);
            return this;
        }

        public new SlimePediaEntryTranslation SetTitleTranslation(string name)
        {
            CoreTranslator.Instance.AddPediaTranslation(Language, TitleKey, name);
            return this;
        }

        public new SlimePediaEntryTranslation SetIntroTranslation(string intro)
        {
            CoreTranslator.Instance.AddPediaTranslation(Language, IntroKey, intro);
            return this;
        }

        [Obsolete]
        public new SlimePediaEntryTranslation SetDescriptionTranslation(string description)
        {
            CoreTranslator.Instance.AddPediaTranslation(Language, DescriptionKey, description);
            return this;
        }
    }
}
