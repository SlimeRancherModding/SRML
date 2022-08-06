using System;
using System.Collections.Generic;

namespace SRML.SR
{
    public static class TargetingRegistry
    {
        internal static List<(Predicate<Identifiable.Id>, (Func<Identifiable.Id, MessageBundle, MessageBundle, string>, Func<Identifiable.Id, MessageBundle, MessageBundle, string>))> customTargetingInfo = 
            new List<(Predicate<Identifiable.Id>, (Func<Identifiable.Id, MessageBundle, MessageBundle, string>, Func<Identifiable.Id, MessageBundle, MessageBundle, string>))>();

        /// <summary>
        /// Registers a targeter.
        /// </summary>
        /// <param name="condition">The condition in which the targeter is used.</param>
        /// <param name="name">A function getting the name of the targeted object, with the first bundle being the UI bundle, and the second being the pedia bundle.</param>
        /// <param name="desc">A function getting the description of the targeted object, with the first bundle being the UI bundle, and the second being the pedia bundle.</param>
        public static void RegisterTargeter(Predicate<Identifiable.Id> condition, 
            Func<Identifiable.Id, MessageBundle, MessageBundle, string> name = null, Func<Identifiable.Id, MessageBundle, MessageBundle, string> desc = null) => 
            customTargetingInfo.Add((condition, (name, desc)));

        /// <summary>
        /// Combines two keys' resulting translations.
        /// </summary>
        /// <param name="bundle">The bundle containing the keys.</param>
        /// <param name="key1">The key to be added to.</param>
        /// <param name="key2">The key to be added.</param>
        /// <returns>The composed description.</returns>
        public static string ComposeDescription(MessageBundle bundle, string key1, string key2) => 
            bundle.Xlate(MessageUtil.Compose(key1, new string[1] { key2 }));
        
        /// <summary>
        /// Composes a type description translation.
        /// </summary>
        /// <param name="bundle">The bundle containing the type key.</param>
        /// <param name="typeKey">The key belonging to the type's translation.</param>
        /// <returns></returns>
        public static string ComposeTypeDescription(MessageBundle bundle, string typeKey) =>
            ComposeDescription(bundle, "m.hudinfo_type", typeKey);

        /// <summary>
        /// Composes a diet description translation.
        /// </summary>
        /// <param name="bundle">The bundle containing the diet key.</param>
        /// <param name="typeKey">The key belonging to the diet's translation.</param>
        public static string ComposeDietDescription(MessageBundle bundle, string dietKey) =>
            ComposeDescription(bundle, "m.hudinfo_diet", dietKey);
    }
}
