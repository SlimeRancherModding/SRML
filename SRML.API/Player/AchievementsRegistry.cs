using SRML.Core.API.BuiltIn;
using System.Collections.Generic;

namespace SRML.API.Player
{
    public class AchievementsRegistry : EnumRegistry<AchievementsRegistry, AchievementsDirector.Achievement>
    {
        internal Dictionary<AchievementsDirector.Achievement, AchievementsDirector.Tracker> trackersForAchievements =
            new Dictionary<AchievementsDirector.Achievement, AchievementsDirector.Tracker>();

        public void RegisterTracker(AchievementsDirector.Achievement achievement, AchievementsDirector.Tracker tracker) =>
            trackersForAchievements[achievement] = tracker;

        public override void Process(AchievementsDirector.Achievement toProcess)
        {
        }
    }
}
