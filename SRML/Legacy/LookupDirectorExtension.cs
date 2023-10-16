using System;
using System.Collections.Generic;
using SRML;
using SRML.SR;
using SRML.SR.Templates.Identifiables;
using UnityEngine;

public static class LookupDirectorExtension
{
	// Garden Resources
	internal readonly static Dictionary<Identifiable.Id, GameObject> gardenResources = new Dictionary<Identifiable.Id, GameObject>();

	// Get Prefab Methods
	public static GameObject GetGardenResourcePrefab(this LookupDirector director, Identifiable.Id ID)
	{
		return gardenResources.ContainsKey(ID) ? gardenResources[ID] : null;
	}

	public static GameObject GetLargoPrefab(this LookupDirector director, Identifiable.Id slimeA, Identifiable.Id slimeB)
	{
		Identifiable.Id largo = Identifiable.Id.NONE;

		string nameA = slimeA.ToString().Replace("_SLIME", "");
		string nameB = slimeB.ToString().Replace("_SLIME", "");

		string largoA = $"{nameA}_{nameB}_LARGO";
		string largoB = $"{nameB}_{nameA}_LARGO";

		object result = Enum.Parse(typeof(Identifiable.Id), largoA) ?? Enum.Parse(typeof(Identifiable.Id), largoB);

		if (result != null)
			largo = (Identifiable.Id)result;

		return largo == Identifiable.Id.NONE ? null : director.GetPrefab(largo);
	}

	// Get Component Methods
	public static Identifiable GetIdentifiable(this LookupDirector director, Identifiable.Id ID)
	{
		return director.GetPrefab(ID).GetComponent<Identifiable>();
	}

	// Get IDs Methods
	public static List<Identifiable.Id> GetSlimeIDs(this LookupDirector director, params Identifiable.Id[] exclusions)
	{
		List<Identifiable.Id> result = new List<Identifiable.Id>();
		List<Identifiable.Id> exclude = new List<Identifiable.Id>(exclusions);

		foreach (Identifiable.Id id in Enum.GetValues(typeof(Identifiable.Id)))
		{
			if (exclude.Contains(id))
				continue;

			if (!id.ToString().EndsWith("_SLIME"))
				continue;

			result.Add(id);
		}

		return result;
	}

	// Check Methods
	public static bool LargoExists(this LookupDirector director, Identifiable.Id slimeA, Identifiable.Id slimeB)
	{
		string nameA = slimeA.ToString().Replace("_SLIME", "");
		string nameB = slimeB.ToString().Replace("_SLIME", "");

		string largoA = $"{nameA}_{nameB}_LARGO";
		string largoB = $"{nameB}_{nameA}_LARGO";

		object result = Enum.Parse(typeof(Identifiable.Id), largoA) ?? Enum.Parse(typeof(Identifiable.Id), largoB);

		return result != null;
	}

	// Largo Creation System
	public static List<Identifiable.Id> MakeLargos(this LookupDirector director, Identifiable.Id slimeA, Action<SlimeDefinition> extraLargoBehaviour = null, Predicate<Identifiable.Id> canBeTarr = null, Predicate<Identifiable.Id> forceLargo = null)
	{
		List<Identifiable.Id> largoIDs = new List<Identifiable.Id>();

		foreach (Identifiable.Id id in GameContext.Instance.LookupDirector.GetSlimeIDs(slimeA))
			largoIDs.Add(director.CraftLargo(slimeA, id, extraLargoBehaviour, canBeTarr, forceLargo));

		largoIDs.RemoveAll((id) => id == Identifiable.Id.NONE);

		return largoIDs;
	}

	public static Identifiable.Id CraftLargo(this LookupDirector director, Identifiable.Id slimeA, Identifiable.Id slimeB, Action<SlimeDefinition> extraLargoBehaviour = null, Predicate<Identifiable.Id> canBeTarr = null, Predicate<Identifiable.Id> forceLargo = null)
	{
		if (director.LargoExists(slimeA, slimeB))
			return Identifiable.Id.NONE;

		string prefabName = "slime" + 
			slimeA.ToString().Replace("_SLIME", "").ToUpper()[0] + slimeA.ToString().Replace("_SLIME", "").ToLower().Substring(1) + 
			slimeB.ToString().Replace("_SLIME", "").ToUpper()[0] + slimeB.ToString().Replace("_SLIME", "").ToLower().Substring(1);

		string name = slimeA.ToString().Replace("_SLIME", "") + slimeB.ToString().Replace("_SLIME", "") + "_LARGO";
		Identifiable.Id largoID = IdentifiableRegistry.CreateIdentifiableId(EnumPatcher.GetFirstFreeValue(typeof(Identifiable.Id)), name);

		SlimeDefinitions defs = GameContext.Instance.SlimeDefinitions;

		SlimeDefinition curr = defs.GetSlimeByIdentifiableId(slimeA);
		SlimeDefinition other = defs.GetSlimeByIdentifiableId(slimeB);

		bool largofyState = curr.CanLargofy;
		curr.CanLargofy = true;

		if (!other.CanLargofy && !(forceLargo?.Invoke(slimeB) ?? false))
			return Identifiable.Id.NONE;

		bool largofyStateB = other.CanLargofy;
		other.CanLargofy = true;

		SlimeDefinition largoDef = defs.GetLargoByBaseSlimes(curr, other);
		largoDef.IdentifiableId = largoID;

		curr.CanLargofy = largofyState;
		other.CanLargofy = largofyStateB;

		if (!(canBeTarr?.Invoke(slimeB) ?? true))
		{
			largoDef.Diet.EatMap.RemoveAll((entry) => entry.becomesId == Identifiable.Id.TARR_SLIME);
			largoDef.Diet.EatMap.RemoveAll((entry) => entry.becomesId == Identifiable.Id.GLITCH_TARR_SLIME);
		}

		extraLargoBehaviour?.Invoke(largoDef);

		SlimeTemplate largoTemplate = new SlimeTemplate(prefabName, largoDef).SetVacSize(Vacuumable.Size.LARGE)
			.SetTranslation(curr.Name + " " + other.Name + " Largo").Create();

		LookupRegistry.RegisterIdentifiablePrefab(largoTemplate.ToPrefab());

		return largoID;
	}

	// Gordo Creation System
	public static SlimeTemplate MakeRoamingGordo(this LookupDirector director, string name, Identifiable.Id gordoID, SlimeDefinition definition)
	{
		SlimeDefinition gordoDef = ScriptableObject.CreateInstance<SlimeDefinition>();
		gordoDef.AppearancesDefault = definition.AppearancesDefault;
		gordoDef.AppearancesDynamic = definition.AppearancesDynamic;
		gordoDef.BaseModule = definition.BaseModule;
		gordoDef.BaseSlimes = definition.BaseSlimes;
		gordoDef.CanLargofy = false;
		gordoDef.Diet = definition.Diet;
		gordoDef.FavoriteToys = new Identifiable.Id[0];
		gordoDef.IdentifiableId = gordoID;
		gordoDef.IsLargo = true;
		gordoDef.PrefabScale = 4f;
		gordoDef.SlimeModules = definition.SlimeModules;
		gordoDef.Sounds = definition.Sounds;
		gordoDef.Name = "roamGordo." + definition.Name;

		FearProfile prof = ScriptableObject.CreateInstance<FearProfile>();
		prof.threats = new List<FearProfile.ThreatEntry>();
		prof.GetType().GetMethod("OnEnable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(prof, new object[0]);

		SlimeTemplate gordo = new SlimeTemplate(name, gordoDef).SetVacSize(Vacuumable.Size.GIANT)
			.SetHealth(60).SetFeralState(false).SetGlitchState(false).SetFearProfile(prof).SetTranslation(definition.Name + " Gordo");

		Identifiable.SLIME_CLASS.Add(gordoID);

		return gordo;
	}

	public static GordoTemplate MakeStaticGordo(this LookupDirector director, string name, Identifiable.Id gordoID, SlimeDefinition definition, Material[] gordoMaterials)
	{
		return new GordoTemplate(name, gordoID, definition, gordoMaterials).SetTranslation(definition.Name + " Gordo");
	}

	// Initializes the Extension
	internal static void InitExtension(this LookupDirector director)
	{
		// Required prefabs
		GameObject gingerSpawnResource = director.GetPrefab(Identifiable.Id.GINGER_VEGGIE).CreatePrefabCopy();
		gingerSpawnResource.GetComponent<ResourceCycle>().unripeGameHours = 12;

		GameObject kookadobaSpawnResource = director.GetPrefab(Identifiable.Id.KOOKADOBA_FRUIT).CreatePrefabCopy();
		kookadobaSpawnResource.GetComponent<ResourceCycle>().unripeGameHours = 6;

		// Load garden resources
		gardenResources.Add(Identifiable.Id.BEET_VEGGIE, director.GetPrefab(Identifiable.Id.BEET_VEGGIE));
		gardenResources.Add(Identifiable.Id.CARROT_VEGGIE, director.GetPrefab(Identifiable.Id.CARROT_VEGGIE));
		gardenResources.Add(Identifiable.Id.GINGER_VEGGIE, gingerSpawnResource);
		gardenResources.Add(Identifiable.Id.OCAOCA_VEGGIE, director.GetPrefab(Identifiable.Id.OCAOCA_VEGGIE));
		gardenResources.Add(Identifiable.Id.ONION_VEGGIE, director.GetPrefab(Identifiable.Id.BEET_VEGGIE));
		gardenResources.Add(Identifiable.Id.PARSNIP_VEGGIE, director.GetPrefab(Identifiable.Id.PARSNIP_VEGGIE));
		gardenResources.Add(Identifiable.Id.CUBERRY_FRUIT, director.GetPrefab(Identifiable.Id.CUBERRY_FRUIT));
		gardenResources.Add(Identifiable.Id.KOOKADOBA_FRUIT, kookadobaSpawnResource);
		gardenResources.Add(Identifiable.Id.LEMON_FRUIT, director.GetPrefab(Identifiable.Id.LEMON_FRUIT));
		gardenResources.Add(Identifiable.Id.MANGO_FRUIT, director.GetPrefab(Identifiable.Id.MANGO_FRUIT));
		gardenResources.Add(Identifiable.Id.PEAR_FRUIT, director.GetPrefab(Identifiable.Id.PEAR_FRUIT));
		gardenResources.Add(Identifiable.Id.POGO_FRUIT, director.GetPrefab(Identifiable.Id.POGO_FRUIT));
	}
}
