using System;
using System.Collections.Generic;

namespace SRML.SR
{
    public static class TargetingRegistry
    {
        internal static List<(Predicate<Identifiable.Id>, (Func<Identifiable.Id, MessageBundle, string>, Func<Identifiable.Id, MessageBundle, string>))> customTargetingInfo = new List<(Predicate<Identifiable.Id>, (Func<Identifiable.Id, MessageBundle, string>, Func<Identifiable.Id, MessageBundle, string>))>();

        public static void RegisterTargeter(Predicate<Identifiable.Id> condition, Func<Identifiable.Id, MessageBundle, string> name = null, Func<Identifiable.Id, MessageBundle, string> desc = null) => customTargetingInfo.Add((condition, (name, desc)));
    }
}
