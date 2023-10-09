using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR
{
    public static class AppearanceRegistry
    {
        public delegate SlimeAppearance AppearanceCombinerDelegate(SlimeAppearance appearance1, SlimeAppearance appearance2);
        public delegate bool AppearancePredicate(SlimeAppearance appearance1, SlimeAppearance appearance2);

        internal static Dictionary<AppearancePredicate, AppearanceCombinerData> combiners = new Dictionary<AppearancePredicate, AppearanceCombinerData>();

        public static SlimeAppearance CombineAppearances(SlimeAppearance appearance1, SlimeAppearance appearance2)
        {
            var combs = combiners.Where(x => x.Key(appearance1, appearance2)).Select(x => x.Value).ToList();
            combs.Sort((x, y) => x.Priority.CompareTo(y.Priority));
            return combs.First().Combiner(appearance1,appearance2);
        }

        public static void RegisterAppearanceCombiner(int priority, SlimeAppearance appearance, Func<SlimeAppearance,SlimeAppearance> func)
        {
            combiners.Add((x, y) => x == appearance || y == appearance,new AppearanceCombinerData()
            {
                Priority = priority,
                Combiner = (x, y) =>
                {
                    if (x == appearance)
                    {
                        x = y;
                    }
                    return func(x);
                }
            });
        }

        internal class AppearanceCombinerData
        {
            public int Priority;
            public AppearanceCombinerDelegate Combiner;
        }
    }
}
