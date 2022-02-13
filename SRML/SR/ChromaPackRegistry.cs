using HarmonyLib;
using SRML.SR.SaveSystem;
using System.Collections.Generic;
using System.Linq;

namespace SRML.SR
{
    public static class ChromaPackRegistry
    {
        internal static IDRegistry<RanchDirector.Palette> moddedPalettes = new IDRegistry<RanchDirector.Palette>();
        internal static IDRegistry<RanchDirector.PaletteType> moddedPaletteTypes = new IDRegistry<RanchDirector.PaletteType>();
        internal static List<RanchDirector.PaletteEntry> moddedPaletteEntries = new List<RanchDirector.PaletteEntry>();

        static ChromaPackRegistry()
        {
            ModdedIDRegistry.RegisterIDRegistry(moddedPalettes);
            ModdedIDRegistry.RegisterIDRegistry(moddedPaletteTypes);
        }

        internal static RanchDirector.Palette GetDefaultPaletteForType(RanchDirector.PaletteType type)
        {
            switch(type)
            {
                case RanchDirector.PaletteType.OGDEN_HOUSE:
                case RanchDirector.PaletteType.OGDEN_TECH:
                    return RanchDirector.Palette.OGDEN;

                case RanchDirector.PaletteType.MOCHI_HOUSE:
                case RanchDirector.PaletteType.MOCHI_TECH:
                case RanchDirector.PaletteType.VALLEY:
                    return RanchDirector.Palette.MOCHI;

                case RanchDirector.PaletteType.VIKTOR_HOUSE:
                case RanchDirector.PaletteType.VIKTOR_TECH:
                    return RanchDirector.Palette.VIKTOR;

                case RanchDirector.PaletteType.HOUSE:
                case RanchDirector.PaletteType.RANCH_TECH:
                case RanchDirector.PaletteType.VAC:
                default:
                    return RanchDirector.Palette.DEFAULT;
            }
        }

        public static RanchDirector.Palette CreatePalette(object value, string name)
        {
            if (SRModLoader.CurrentLoadingStep > SRModLoader.LoadingStep.PRELOAD)
                throw new LoadingStepException("Can't register palettes outside of the PreLoad step");
            return moddedPalettes.RegisterValueWithEnum((RanchDirector.Palette)value, name);
        }

        public static RanchDirector.PaletteType CreatePaletteType(object value, string name)
        {
            if (SRModLoader.CurrentLoadingStep > SRModLoader.LoadingStep.PRELOAD)
                throw new LoadingStepException("Can't register palette types outside of the PreLoad step");
            return moddedPaletteTypes.RegisterValueWithEnum((RanchDirector.PaletteType)value, name);
        }

        /// <summary>
        /// Check if a <see cref="RanchDirector.Palette"/> was registered by a mod
        /// </summary>
        /// <param name="palette"></param>
        /// <returns></returns>
        public static bool IsModdedPalette(this RanchDirector.Palette palette)
        {
            return moddedPalettes.ContainsKey(palette);
        }

        /// <summary>
        /// Check if a <see cref="RanchDirector.PaletteType"/> was registered by a mod
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsModdedPaletteType(this RanchDirector.PaletteType type)
        {
            return moddedPaletteTypes.ContainsKey(type);
        }

        public static void RegisterPaletteEntry(RanchDirector.PaletteEntry paletteEntry)
        {
            moddedPaletteEntries.Add(paletteEntry);
        }
    }
}
