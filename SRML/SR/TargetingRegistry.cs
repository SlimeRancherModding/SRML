using System;
using System.Collections.Generic;

namespace SRML.SR
{
    public static class TargetingRegistry
    {
        internal static List<(Predicate<Identifiable.Id>, (Func<Identifiable.Id, MessageBundle, MessageBundle, string>, Func<Identifiable.Id, MessageBundle, MessageBundle, string>))> customTargetingInfo = 
            new List<(Predicate<Identifiable.Id>, (Func<Identifiable.Id, MessageBundle, MessageBundle, string>, Func<Identifiable.Id, MessageBundle, MessageBundle, string>))>();

        public static void RegisterTargeter(Predicate<Identifiable.Id> condition, 
            Func<Identifiable.Id, MessageBundle, MessageBundle, string> name = null, Func<Identifiable.Id, MessageBundle, MessageBundle, string> desc = null) => 
            customTargetingInfo.Add((condition, (name, desc)));

        public static string ComposeDescription(MessageBundle bundle, string key1, string key2) => 
            bundle.Xlate(MessageUtil.Compose(key1, new string[1] { key2 }));
        
        public static string ComposeTypeDescription(MessageBundle bundle, string typeKey) => 
            bundle.Xlate(MessageUtil.Compose("m.hudinfo_type", new string[1] { typeKey }));

        public static string ComposeDietDescription(MessageBundle bundle, string dietKey) => 
            bundle.Xlate(MessageUtil.Compose("m.hudinfo_diet", new string[1] { dietKey }));
    }
}
