using HarmonyLib; 

namespace SRML.SR.Patches
{
	[HarmonyPatch(typeof(RanchDirector), "SetColorsForPalette")]
	internal static class RanchDirectorSetColorPrefix
	{
		public static void Prefix(RanchDirector __instance, RanchDirector.PaletteType type, RanchDirector.Palette palette)
		{
			if (!__instance.paletteDict.ContainsKey(palette))
			{
				SRML.Console.Console.LogWarning("Ranch palette " + palette.ToString() + " not found! Reverting to default.");
				SRSingleton<SceneContext>.Instance.GameModel.GetRanchModel().selectedPalettes[type] = RanchDirector.Palette.DEFAULT;
			}
		}
	}
}
