using HarmonyLib;
using SRML.SR.SaveSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRML.SR
{
    public static class ChromaRegistry
    {
        internal static IDRegistry<RanchDirector.Palette> moddedPalettes = new IDRegistry<RanchDirector.Palette>();
        internal static IDRegistry<RanchDirector.PaletteType> moddedPaletteTypes = new IDRegistry<RanchDirector.PaletteType>();
        internal static List<RanchDirector.PaletteEntry> customPalettes = new List<RanchDirector.PaletteEntry>();

        static ChromaRegistry()
        {
            ModdedIDRegistry.RegisterIDRegistry(moddedPalettes);
            ModdedIDRegistry.RegisterIDRegistry(moddedPaletteTypes);
        }

        /// <summary>
        /// Creates a <see cref="RanchDirector.Palette"/> and registers it into the enum
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static RanchDirector.Palette CreatePalette(object value, string name)
        {
            if (SRModLoader.CurrentLoadingStep > SRModLoader.LoadingStep.PRELOAD)
                throw new Exception("Can't register palettes outside of the PreLoad step");
            return moddedPalettes.RegisterValueWithEnum((RanchDirector.Palette)value, name);
        }

        /// <summary>
        /// Creates a <see cref="RanchDirector.PaletteType"/> and registers it into the enum
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static RanchDirector.PaletteType CreatePaletteType(object value, string name)
        {
            if (SRModLoader.CurrentLoadingStep > SRModLoader.LoadingStep.PRELOAD)
                throw new Exception("Can't register palette types outside of the PreLoad step");
            return moddedPaletteTypes.RegisterValueWithEnum((RanchDirector.PaletteType)value, name);
        }

        /// <summary>
        /// Registers a <see cref="RanchDirector.PaletteEntry"/>
        /// </summary>
        /// <param name="entry"></param>
        public static void RegisterPaletteEntry(RanchDirector.PaletteEntry entry) => customPalettes.Add(entry);

        /// <summary>
        /// See if a <see cref="RanchDirector.Palette"/> is modded
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if given <see cref="RanchDirector.Palette"/> is from a mod</returns>
        public static bool IsModdedPalette(RanchDirector.Palette id) => moddedPalettes.ContainsKey(id);

        /// <summary>
        /// See if a <see cref="RanchDirector.PaletteType"/> is modded
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if given <see cref="RanchDirector.PaletteType"/> is from a mod</returns>
        public static bool IsModdedPaletteType(RanchDirector.PaletteType id) => moddedPaletteTypes.ContainsKey(id);

        internal static RanchDirector.Palette GetDefaultPaletteForType(RanchDirector.PaletteType type)
        {
            switch (type)
            {
                case RanchDirector.PaletteType.OGDEN_HOUSE:
                case RanchDirector.PaletteType.OGDEN_TECH:
                    return RanchDirector.Palette.OGDEN;
                case RanchDirector.PaletteType.MOCHI_HOUSE:
                case RanchDirector.PaletteType.MOCHI_TECH:
                    return RanchDirector.Palette.MOCHI;
                case RanchDirector.PaletteType.VIKTOR_HOUSE:
                case RanchDirector.PaletteType.VIKTOR_TECH:
                    return RanchDirector.Palette.VIKTOR;
                default:
                    return RanchDirector.Palette.DEFAULT;
            }
        }
    }
}
