using SRML.API.Gadget;
using System;

namespace SRML.SR
{
    [Obsolete]
    public static class GadgetRegistry
    {
        public delegate GadgetDirector.BlueprintLocker BlueprintLockCreateDelegate(GadgetDirector director);

        /// <summary>
        /// Creates a <see cref="Gadget.Id"/>.
        /// </summary>
        /// <param name="value">What value is assigned to the <see cref="Gadget.Id"/>.</param>
        /// <param name="name">The name of the <see cref="Gadget.Id"/>.</param>
        /// <returns>The created <see cref="Gadget.Id"/>.</returns>
        /// <exception cref="Exception">Throws if ran outside of PreLoad</exception>
        public static Gadget.Id CreateGadgetId(object value, string name) => API.Gadget.GadgetRegistry.Instance.RegisterAndParse(name, value);

        /// <summary>
        /// Check if a <see cref="Gadget.Id"/> belongs to a modded gadget.
        /// </summary>
        /// <param name="id">The <see cref="Gadget.Id"/> to check.</param>
        /// <returns>True if <see cref="Gadget.Id"/> belongs to a modded gadget, otherwise false.</returns>
        public static bool IsModdedGadget(this Gadget.Id id) => API.Gadget.GadgetRegistry.Instance.IsRegistered(id);

        /// <summary>
        /// Associates an <see cref="Gadget.Id"/> with a <see cref="Identifiable.Id"/>
        /// </summary>
        /// <param name="id">The <see cref="Gadget.Id"/> to associate.</param>
        /// <param name="id2">The <see cref="Identifiable.Id"/> to associate</param>
        public static void RegisterIdentifiableMapping(Gadget.Id id, Identifiable.Id id2) =>
            API.Gadget.GadgetRegistry.Instance.RegisterIdentifiableTranslationMapping(id, id2);

        /// <summary>
        /// Registers a locker for a blueprint.
        /// </summary>
        /// <param name="id">The <see cref="Gadget.Id"/> of the blueprint.</param>
        /// <param name="creator">The locker for the blueprint.</param>
        public static void RegisterBlueprintLock(Gadget.Id id, BlueprintLockCreateDelegate creator) =>
            BlueprintRegistry.Instance.RegisterBlueprintLock(id, new BlueprintRegistry.BlueprintLock(creator));

        /// <summary>
        /// Register a blueprint.
        /// </summary>
        /// <param name="id">The <see cref="Gadget.Id"/> of the blueprint.</param>
        public static void RegisterDefaultBlueprint(Gadget.Id id) => BlueprintRegistry.Instance.RegisterDefaultUnlockedBlueprint(id);

        /// <summary>
        /// Register a blueprint that's automatically available.
        /// </summary>
        /// <param name="id">The <see cref="Gadget.Id"/> of the blueprint.</param>
        public static void RegisterDefaultAvailableBlueprint(Gadget.Id id) => BlueprintRegistry.Instance.RegisterDefaultAvailableBlueprint(id);


        /// <summary>
        /// Manually set the <see cref="GadgetCategorization.Rule"/> of the <see cref="Gadget.Id"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rule"></param>
        public static void Categorize(this Gadget.Id id, GadgetCategorization.Rule rule) =>
            API.Gadget.GadgetRegistry.Instance.Categorize(id, rule);

        /// <summary>
        /// Remove all instances of an <see cref="Gadget.Id"/> from every class in <see cref="Gadget"/>
        /// </summary>
        /// <param name="id"></param>
        public static void Uncategorize(this Gadget.Id id) => API.Gadget.GadgetRegistry.Instance.Decategorize(id);

        /// <summary>
        /// Puts a <see cref="Gadget.Id"/> into one of the vanilla categories based on its name prefix (see <see cref="Gadget"/>)<br />
        /// WARNING: Will not categorize decorations properly!
        /// </summary>
        /// <param name="id"></param>
        public static void CategorizeId(Gadget.Id id) => API.Gadget.GadgetRegistry.Instance.Categorize(id);

        /// <summary>
        /// Put an <see cref="Gadget.Id"/> into one of the vanilla categories
        /// </summary>
        /// <param name="id"></param>
        /// <param name="category"></param>
        public static void CategorizeId(Gadget.Id id, GadgetCategorization.Rule category) => API.Gadget.GadgetRegistry.Instance.Categorize(id, category);

        [Obsolete]
        public static void ClassifyGadget(Gadget.Id gadget, GadgetClassification classification)
        {
            switch (classification)
            {
                case GadgetClassification.MISC:
                    Gadget.MISC_CLASS.Add(gadget);
                    break;
                case GadgetClassification.EXTRACTOR:
                    Gadget.EXTRACTOR_CLASS.Add(gadget);
                    break;
                case GadgetClassification.TELEPORTER:
                    Gadget.TELEPORTER_CLASS.Add(gadget);
                    break;
                case GadgetClassification.WARP_DEPOT:
                    Gadget.WARP_DEPOT_CLASS.Add(gadget);
                    break;
                case GadgetClassification.ECHO_NET:
                    Gadget.ECHO_NET_CLASS.Add(gadget);
                    break;
                case GadgetClassification.LAMP:
                    Gadget.LAMP_CLASS.Add(gadget);
                    break;
                case GadgetClassification.FASHION_POD:
                    Gadget.FASHION_POD_CLASS.Add(gadget);
                    Gadget.RegisterFashion(gadget);
                    break;
                case GadgetClassification.SNARE:
                    Gadget.SNARE_CLASS.Add(gadget);
                    break;
                case GadgetClassification.DECO:
                    Gadget.DECO_CLASS.Add(gadget);
                    break;
                case GadgetClassification.DRONE:
                    Gadget.DRONE_CLASS.Add(gadget);
                    break;
            }
        }

        [Obsolete]
        public enum GadgetClassification
        {
            MISC,
            EXTRACTOR,
            TELEPORTER,
            WARP_DEPOT,
            ECHO_NET,
            LAMP,
            FASHION_POD,
            SNARE,
            DECO,
            DRONE
        }
    }
}
