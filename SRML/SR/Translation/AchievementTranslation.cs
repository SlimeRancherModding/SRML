using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRML.SR.Translation
{
    public class AchievementTranslation : ITranslatable<AchievementsDirector.Achievement>
    {
        public const string NamePrefix = "t.";
        public const string DescriptionPrefix = "m.reqmt.";

        public AchievementsDirector.Achievement Key { get; protected set; }

        public string StringKey => Key.ToString().ToLower();

        public string NameKey => NamePrefix + StringKey;

        public string DescriptionKey => DescriptionPrefix + StringKey;

        public AchievementTranslation SetDescriptionTranslation(string desc)
        {
            TranslationPatcher.AddAchievementTranslation(DescriptionKey, desc);
            return this;
        }

        public AchievementTranslation SetNameTranslation(string name)
        {
            TranslationPatcher.AddAchievementTranslation(NameKey, name);
            return this;
        }

        public AchievementTranslation(AchievementsDirector.Achievement achievement)
        {
            this.Key = achievement;
        }
    }

    public static class AchievementTranslationExtensions
    {
        public static AchievementTranslation GetTranslation(this AchievementsDirector.Achievement achievement)
        {
            return new AchievementTranslation(achievement);
        }
    }
}
