using SRML.SR.SaveSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR
{
    public static class GadgetRegistry
    {
        internal static IDRegistry<Gadget.Id> moddedGadgets = new IDRegistry<Gadget.Id>();
        internal static Dictionary<Gadget.Id, GadgetCategorization.Rule> rules = new Dictionary<Gadget.Id, GadgetCategorization.Rule>();
        public delegate GadgetDirector.BlueprintLocker BlueprintLockCreateDelegate(GadgetDirector director);

        internal static Dictionary<Gadget.Id, BlueprintLockCreateDelegate> customBlueprintLocks = new Dictionary<Gadget.Id, BlueprintLockCreateDelegate>();
        internal static List<Gadget.Id> defaultBlueprints = new List<Gadget.Id>();
        internal static List<Gadget.Id> defaultAvailBlueprints = new List<Gadget.Id>();

        static GadgetRegistry() => ModdedIDRegistry.RegisterIDRegistry(moddedGadgets);

        public static Gadget.Id CreateGadgetId(object value, string name)
        {
            if (SRModLoader.CurrentLoadingStep > SRModLoader.LoadingStep.PRELOAD)
                throw new Exception("Can't register gadgets outside of the PreLoad step");
            return moddedGadgets.RegisterValueWithEnum((Gadget.Id) value, name);
        }

        public static bool IsModdedGadget(this Gadget.Id id) => moddedGadgets.ContainsKey(id);

        public static void RegisterIdentifiableMapping(Gadget.Id id, Identifiable.Id id2) => Identifiable.GADGET_NAME_DICT.Add(id2, id);

        public static void RegisterBlueprintLock(Gadget.Id id, BlueprintLockCreateDelegate creator) => customBlueprintLocks.Add(id, creator);

        public static void RegisterDefaultBlueprint(Gadget.Id id) => defaultBlueprints.Add(id);

        public static void RegisterDefaultAvailableBlueprint(Gadget.Id id) => defaultAvailBlueprints.Add(id);


        /// <summary>
        /// Manually set the <see cref="GadgetCategorization.Rule"/> of the <see cref="Gadget.Id"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rule"></param>
        public static void Categorize(this Gadget.Id id, GadgetCategorization.Rule rule)
        {
            if (SRModLoader.CurrentLoadingStep > SRModLoader.LoadingStep.PRELOAD)
            {
                CategorizeId(id, rule);
                return;
            }
            rules.Add(id, rule);
        }

        /// <summary>
        /// Remove all instances of an <see cref="Gadget.Id"/> from every class in <see cref="Gadget"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rule"></param>
        public static void Uncategorize(this Gadget.Id id)
        {
            Gadget.DECO_CLASS.Remove(id);
            Gadget.DRONE_CLASS.Remove(id);
            Gadget.ECHO_NET_CLASS.Remove(id);
            Gadget.EXTRACTOR_CLASS.Remove(id);
            Gadget.FASHION_POD_CLASS.Remove(id);
            Gadget.LAMP_CLASS.Remove(id);
            Gadget.MISC_CLASS.Remove(id);
            Gadget.SNARE_CLASS.Remove(id);
            Gadget.TELEPORTER_CLASS.Remove(id);
            Gadget.WARP_DEPOT_CLASS.Remove(id);
        }

        internal static void CategorizeAllIds()
        {
            foreach (Gadget.Id id in moddedGadgets.Keys)
            {
                if (rules.ContainsKey(id)) continue;
                CategorizeId(id);
            }
            foreach (Gadget.Id id in rules.Keys)
                CategorizeId(id, rules[id]);
        }

        /// <summary>
        /// Puts a <see cref="Gadget.Id"/> into one of the vanilla categories based on its name prefix (see <see cref="Gadget"/>)<br />
        /// WARNING: Will not categorize decorations properly!
        /// </summary>
        /// <param name="id"></param>
        public static void CategorizeId(Gadget.Id id)
        {
            string name = Enum.GetName(typeof(Gadget.Id), id);
            if (name.StartsWith("EXTRACTOR_"))
                CategorizeId(id, GadgetCategorization.Rule.EXTRACTOR);
            else if (name.StartsWith("TELEPORTER_"))
                CategorizeId(id, GadgetCategorization.Rule.TELEPORTER);
            else if (name.StartsWith("WARP_DEPOT_"))
                CategorizeId(id, GadgetCategorization.Rule.WARP_DEPOT);
            else if (name.StartsWith("ECHO_NET"))
                CategorizeId(id, GadgetCategorization.Rule.ECHO_NET);
            else if (name.StartsWith("LAMP_"))
                CategorizeId(id, GadgetCategorization.Rule.LAMP);
            else if (name.StartsWith("FASHION_POD_"))
                CategorizeId(id, GadgetCategorization.Rule.FASHION_POD);
            else if (name.StartsWith("GORDO_SNARE_"))
                CategorizeId(id, GadgetCategorization.Rule.SNARE);
            else if (name.StartsWith("DRONE"))
                CategorizeId(id, GadgetCategorization.Rule.DRONE);
            else
                CategorizeId(id, GadgetCategorization.Rule.MISC);
        }

        /// <summary>
        /// Put an <see cref="Gadget.Id"/> into one of the vanilla categories
        /// </summary>
        /// <param name="id"></param>
        /// <param name="category"></param>
        public static void CategorizeId(Gadget.Id id, GadgetCategorization.Rule category)
        {
            string name = Enum.GetName(typeof(Gadget.Id), id);
            if ((category & (GadgetCategorization.Rule.MISC)) != 0) Gadget.MISC_CLASS.Add(id);
            if ((category & (GadgetCategorization.Rule.EXTRACTOR)) != 0) Gadget.EXTRACTOR_CLASS.Add(id);
            if ((category & (GadgetCategorization.Rule.TELEPORTER)) != 0) Gadget.TELEPORTER_CLASS.Add(id);
            if ((category & (GadgetCategorization.Rule.WARP_DEPOT)) != 0) Gadget.WARP_DEPOT_CLASS.Add(id);
            if ((category & (GadgetCategorization.Rule.ECHO_NET)) != 0) Gadget.ECHO_NET_CLASS.Add(id);
            if ((category & (GadgetCategorization.Rule.LAMP)) != 0) Gadget.LAMP_CLASS.Add(id);
            if ((category & (GadgetCategorization.Rule.FASHION_POD)) != 0) Gadget.FASHION_POD_CLASS.Add(id);
            if ((category & (GadgetCategorization.Rule.SNARE)) != 0) Gadget.SNARE_CLASS.Add(id);
            if ((category & (GadgetCategorization.Rule.DECO)) != 0) Gadget.DECO_CLASS.Add(id);
            if ((category & (GadgetCategorization.Rule.DRONE)) != 0) Gadget.DRONE_CLASS.Add(id);
        }

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
