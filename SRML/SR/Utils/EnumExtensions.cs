using SRML;
using System.Linq;
using UnityEngine;

public static class EnumExtensions
{
    internal static GameContext GetContextForStep()
    {
        GameContext context;
        if (SRModLoader.CurrentLoadingStep > SRModLoader.LoadingStep.PRELOAD)
            context = GameContext.Instance;
        else
            context = GameObject.FindObjectOfType<GameContext>();
        return context;
    }

    internal static SceneContext GetSceneContextForStep()
    {
        SceneContext context;
        if (SRModLoader.CurrentLoadingStep > SRModLoader.LoadingStep.PRELOAD)
            context = SceneContext.Instance;
        else
            context = SceneContext.FindObjectOfType<SceneContext>();
        return context;
    }

    public static GameObject GetPrefab(this Identifiable.Id id) => Identifiable.IsGordo(id) ? GetContextForStep().LookupDirector.GetGordo(id) : GetContextForStep().LookupDirector.GetPrefab(id);
    public static GameObject GetResourcePrefab(this SpawnResource.Id id) => GetContextForStep().LookupDirector.GetResourcePrefab(id);
    public static GameObject GetPlotPrefab(this LandPlot.Id id) => GetContextForStep().LookupDirector.GetPlotPrefab(id);
    public static Sprite GetIcon(this Identifiable.Id id) => GetContextForStep().LookupDirector.GetIcon(id);
    public static Sprite GetIcon(this PediaDirector.Id id) => GetSceneContextForStep().PediaDirector.entries.First(x => x.id == id).icon;
    public static Color GetColor(this Identifiable.Id id) => GetContextForStep().LookupDirector.GetColor(id);
    public static ToyDefinition GetToyDefinition(this Identifiable.Id id) => GetContextForStep().LookupDirector.GetToyDefinition(id);
    public static UpgradeDefinition GetUpgradeDefinition(this PlayerState.Upgrade id) => GetContextForStep().LookupDirector.GetUpgradeDefinition(id);
    public static SlimeDefinition GetSlimeDefinition(this Identifiable.Id id) => GetContextForStep().SlimeDefinitions.GetSlimeByIdentifiableId(id);
    public static GadgetDefinition GetGadgetDefinition(this Gadget.Id id) => GetContextForStep().LookupDirector.GetGadgetDefinition(id);
}