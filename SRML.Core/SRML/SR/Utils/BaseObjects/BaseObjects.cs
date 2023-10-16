using System.Collections.Generic;
using System.Text.RegularExpressions;
using MonomiPark.SlimeRancher.DataModel;
using SRML.SR.Utils.Debug;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SRML.SR.Utils.BaseObjects
{
    public static class BaseObjects
	{
		// The Director
		private static LookupDirector Director => GameContext.Instance.LookupDirector;
		private static GameModel GameModel => SceneContext.Instance.GameModel;

		// Really Base Stuff
		public readonly static Dictionary<string, int> layers = new Dictionary<string, int>();

		// Markers
		public readonly static Material fadeMat = new Material(Shader.Find("SR/Particles/Additive"));

		public readonly static Dictionary<MarkerType, Material> markerMaterials = new Dictionary<MarkerType, Material>()
		{
			{MarkerType.SpawnPoint, new Material(fadeMat).SetTintInfo(new Color(1, 0, 0, 1f), "Spawn Point Marker")},
			{MarkerType.SlimeSpawnPoint, new Material(fadeMat).SetTintInfo(new Color(1, 0.25f, 0.25f, 1f), "Slime Spawn Point Marker")},
			{MarkerType.AnimalSpawnPoint, new Material(fadeMat).SetTintInfo(new Color(0.5f, 0, 0.5f, 1f), "Animal Spawn Point Marker")},
			{MarkerType.CrateSpawnPoint, new Material(fadeMat).SetTintInfo(new Color(0.5f, 0.25f, 0, 1f), "Crate Spawn Point Marker")},
			{MarkerType.ResourceSpawnPoint, new Material(fadeMat).SetTintInfo(new Color(1, 0, 0, 1f), "Resource Spawn Point Marker")},
			{MarkerType.Plot, new Material(fadeMat).SetTintInfo(new Color(0, 1, 1, 1f), "Plot Marker")},
			{MarkerType.GadgetLocation, new Material(fadeMat).SetTintInfo(new Color(0, 1f, 0, 1f), "Gadget Location Marker")},
			{MarkerType.DroneNode, new Material(fadeMat).SetTintInfo(new Color(1, 0, 0.5f, 1f), "Drone Node Marker")}
		};

		public readonly static Dictionary<MarkerType, Material> markerAreaMaterials = new Dictionary<MarkerType, Material>()
		{
			{MarkerType.Plot, new Material(fadeMat).SetTintInfo(new Color(0, 1, 1, 0.1f), "Plot Area")},
			{MarkerType.GadgetLocation, new Material(fadeMat).SetTintInfo(new Color(0.5f, 1f, 0.5f, 0.5f), "Gadget Location Area")},
		};

		public static Mesh cubeMesh;

		public static GameObject debugSystem;

		// Original Meshes and Materials
		public readonly static Dictionary<string, Mesh> originMesh = new Dictionary<string, Mesh>();
		public readonly static Dictionary<string, Material> originMaterial = new Dictionary<string, Material>();
		public readonly static Dictionary<string, PhysicMaterial> originPhysMaterial = new Dictionary<string, PhysicMaterial>();
		public readonly static Dictionary<string, Texture> originTexture = new Dictionary<string, Texture>();
		public readonly static Dictionary<string, Sprite> originSprite = new Dictionary<string, Sprite>();
		public readonly static Dictionary<string, AudioClip> originClips = new Dictionary<string, AudioClip>();
		public readonly static Dictionary<string, SECTR_AudioCue> originCues = new Dictionary<string, SECTR_AudioCue>();
		public readonly static Dictionary<string, SlimeSounds> originSounds = new Dictionary<string, SlimeSounds>();
		public readonly static Dictionary<string, GameObject> originFXs = new Dictionary<string, GameObject>();
		public readonly static Dictionary<string, RuntimeAnimatorController> originAnimators = new Dictionary<string, RuntimeAnimatorController>();
		public readonly static Dictionary<string, Avatar> originAvatars = new Dictionary<string, Avatar>();

		public readonly static Dictionary<string, GameObject> originSkinnedMesh = new Dictionary<string, GameObject>();
		public readonly static Dictionary<string, GameObject> originBones = new Dictionary<string, GameObject>();

		private readonly static List<string> materialBlacklist = new List<string>()
		{
			"Spawn Point Marker",
			"Slime Spawn Point Marker",
			"Animal Spawn Point Marker",
			"Crate Spawn Point Marker",
			"Resource Spawn Point Marker",
			"Plot Marker",
			"Gadget Location Marker",
			"Drone Node Marker",
			"Plot Area",
			"Gadget Location Area",
			"Standard"
		};

		// Single Objects
		public static GameObject splatQuad;

		// Populate blocker
		private static bool populated = false;

		private static Material SetTintInfo(this Material mat, Color color, string name)
		{
			Texture2D tex = new Texture2D(1, 1);
			tex.SetPixel(0, 0, color);
			tex.Apply();
			mat.mainTexture = tex;
			mat.name = name;
			return mat;
		}

		// Populates required values
		internal static void Populate()
		{
			// Gets unity stuff
			for (int i = 0; i < 32; i++)
			{
				string name = LayerMask.LayerToName(i);
				if (!name.Equals(string.Empty) && !layers.ContainsKey(name))
					layers.Add(name, i);
			}

			// Obtains objects loaded into the game that might be useful
			foreach (Mesh mesh in Resources.FindObjectsOfTypeAll<Mesh>())
			{
				if (!mesh.name.Equals(string.Empty) && !originMesh.ContainsKey(mesh.name.Replace("(Instance)", "")))
					originMesh.Add(mesh.name.Replace("(Instance)", ""), mesh);
			}

			foreach (Material mat in Resources.FindObjectsOfTypeAll<Material>())
			{
				if (!mat.name.Equals(string.Empty) && !originMaterial.ContainsKey(mat.name.Replace("(Instance)", "")) &&
					!mat.name.StartsWith("Hidden/") && !materialBlacklist.Contains(mat.name.Replace("(Instance)", "")))
					originMaterial.Add(mat.name.Replace("(Instance)", ""), mat);
			}

			foreach (PhysicMaterial mat in Resources.FindObjectsOfTypeAll<PhysicMaterial>())
			{
				if (!mat.name.Equals(string.Empty) && !originPhysMaterial.ContainsKey(mat.name.Replace("(Instance)", "")))
					originPhysMaterial.Add(mat.name.Replace("(Instance)", ""), mat);
			}

			foreach (Texture tex in Resources.FindObjectsOfTypeAll<Texture>())
			{
				if (!tex.name.Equals(string.Empty) && !originTexture.ContainsKey(tex.name.Replace("(Instance)", "")))
					originTexture.Add(tex.name.Replace("(Instance)", ""), tex);
			}

			foreach (Sprite sprite in Resources.FindObjectsOfTypeAll<Sprite>())
			{
				if (!sprite.name.Equals(string.Empty) && !originSprite.ContainsKey(sprite.name.Replace("(Instance)", "")))
					originSprite.Add(sprite.name.Replace("(Instance)", ""), sprite);
			}

			foreach (AudioClip clip in Resources.FindObjectsOfTypeAll<AudioClip>())
			{
				if (!clip.name.Equals(string.Empty) && !originClips.ContainsKey(clip.name.Replace("(Instance)", "")))
					originClips.Add(clip.name.Replace("(Instance)", ""), clip);
			}

			foreach (SECTR_AudioCue cue in Resources.FindObjectsOfTypeAll<SECTR_AudioCue>())
			{
				if (!cue.name.Equals(string.Empty) && !originCues.ContainsKey(cue.name.Replace("(Instance)", "")))
					originCues.Add(cue.name.Replace("(Instance)", ""), cue);
			}

			foreach (SlimeSounds sound in Resources.FindObjectsOfTypeAll<SlimeSounds>())
			{
				if (!sound.name.Equals(string.Empty) && !originSounds.ContainsKey(sound.name.Replace("(Instance)", "")))
					originSounds.Add(sound.name.Replace("(Instance)", ""), sound);
			}

			foreach (RuntimeAnimatorController animator in Resources.FindObjectsOfTypeAll<RuntimeAnimatorController>())
			{
				if (!animator.name.Equals(string.Empty) && !originAnimators.ContainsKey(animator.name.Replace("(Instance)", "")))
					originAnimators.Add(animator.name.Replace("(Instance)", ""), animator);
			}

			foreach (Avatar avatar in Resources.FindObjectsOfTypeAll<Avatar>())
			{
				if (!avatar.name.Equals(string.Empty) && !originAvatars.ContainsKey(avatar.name.Replace("(Instance)", "")))
					originAvatars.Add(avatar.name.Replace("(Instance)", ""), avatar);
			}

			foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>())
			{
				if (!(obj.name.StartsWith("FX ") || obj.name.StartsWith("fx")))
					continue;

				if (!obj.name.Equals(string.Empty) && !originFXs.ContainsKey(obj.name.Replace("(Instance)", "")))
					originFXs.Add(obj.name.Replace("(Instance)", ""), obj);
			}

			// Load Bones
			originSkinnedMesh.Add("HenSkinned", Director.GetPrefab(Identifiable.Id.HEN).GetChildCopy("Hen Hen/mesh_body1"));
			originBones.Add("HenBones", Director.GetPrefab(Identifiable.Id.HEN).GetChildCopy("Hen Hen/root"));

			originBones.Add("SlimeBones", Director.GetPrefab(Identifiable.Id.PINK_SLIME).GetChildCopy("Appearance"));
			originBones.Add("GordoBones", Director.GetGordo(Identifiable.Id.PINK_GORDO).GetChildCopy("Vibrating"));

			// Gets the cube for the markers
			cubeMesh = originMesh["Cube"];

			debugSystem = new GameObject("DebugSystem", typeof(DebugSystem));
			Object.DontDestroyOnLoad(debugSystem);

			// Adds markers to objects
			foreach (GameObject obj in Director.plotPrefabs)
			{
				obj.GetReadyForMarker(MarkerType.Plot, 8f);
			}

			foreach (GameObject obj in Director.resourceSpawnerPrefabs)
			{
				foreach (GameObject child in obj.FindChildrenWithPartialName("SpawnJoint"))
					child.GetReadyForMarker(MarkerType.SpawnPoint);

				foreach (GameObject child in obj.FindChildrenWithPartialName("NodeLoc"))
					child.GetReadyForMarker(MarkerType.DroneNode, 3f);
			}

			// Single Objects
			splatQuad = Director.GetPrefab(Identifiable.Id.PINK_SLIME).GetComponent<SplatOnImpact>().splatPrefab;

			// Register Global Template Actions
			TemplateActions.RegisterAction("buildSlime", AssembleModules);
			TemplateActions.RegisterAction("populateSlime", PopulateSlimeInfo);

			// Populates all other object classes
			GardenObjects.Populate();
			TheWildsObjects.Populate();
			RanchObjects.Populate();
			EffectObjects.Populate();

			// Adds Late Populate method
			SceneManager.sceneLoaded += LatePopulate;
		}

		internal static void LatePopulate(Scene sceneLoaded, LoadSceneMode mode)
		{
			if (populated)
				return;

			if (sceneLoaded.name.Equals("worldGenerated"))
			{
				// Obtains objects loaded into the game that might be useful
				foreach (Mesh mesh in Resources.FindObjectsOfTypeAll<Mesh>())
				{
					if (!mesh.name.Equals(string.Empty) && !originMesh.ContainsKey(mesh.name.Replace("(Instance)", "")) && 
						!(!mesh.name.EndsWith("(Instance)") && Regex.IsMatch(mesh.name, @".*\(.*\)")))
						originMesh.Add(mesh.name.Replace("(Instance)", ""), mesh);
				}

				foreach (Material mat in Resources.FindObjectsOfTypeAll<Material>())
				{
					if (!mat.name.Equals(string.Empty) && !originMaterial.ContainsKey(mat.name.Replace("(Instance)", "")) &&
						!mat.name.StartsWith("Hidden/") && !materialBlacklist.Contains(mat.name) &&
						!(!mat.name.EndsWith("(Instance)") && Regex.IsMatch(mat.name, @".*\(.*\)")))
						originMaterial.Add(mat.name.Replace("(Instance)", ""), mat);
				}

				foreach (PhysicMaterial mat in Resources.FindObjectsOfTypeAll<PhysicMaterial>())
				{
					if (!mat.name.Equals(string.Empty) && !originPhysMaterial.ContainsKey(mat.name.Replace("(Instance)", "")) &&
						!(!mat.name.EndsWith("(Instance)") && Regex.IsMatch(mat.name, @".*\(.*\)")))
						originPhysMaterial.Add(mat.name.Replace("(Instance)", ""), mat);
				}

				foreach (Texture tex in Resources.FindObjectsOfTypeAll<Texture>())
				{
					if (!tex.name.Equals(string.Empty) && !originTexture.ContainsKey(tex.name.Replace("(Instance)", "")) &&
						!(!tex.name.EndsWith("(Instance)") && Regex.IsMatch(tex.name, @".*\(.*\)")))
						originTexture.Add(tex.name.Replace("(Instance)", ""), tex);
				}

				foreach (Sprite sprite in Resources.FindObjectsOfTypeAll<Sprite>())
				{
					if (!sprite.name.Equals(string.Empty) && !originSprite.ContainsKey(sprite.name.Replace("(Instance)", "")) &&
						!(!sprite.name.EndsWith("(Instance)") && Regex.IsMatch(sprite.name, @".*\(.*\)")))
						originSprite.Add(sprite.name.Replace("(Instance)", ""), sprite);
				}

				foreach (AudioClip clip in Resources.FindObjectsOfTypeAll<AudioClip>())
				{
					if (!clip.name.Equals(string.Empty) && !originClips.ContainsKey(clip.name.Replace("(Instance)", "")) &&
						!(!clip.name.EndsWith("(Instance)") && Regex.IsMatch(clip.name, @".*\(.*\)")))
						originClips.Add(clip.name.Replace("(Instance)", ""), clip);
				}

				foreach (SECTR_AudioCue cue in Resources.FindObjectsOfTypeAll<SECTR_AudioCue>())
				{
					if (!cue.name.Equals(string.Empty) && !originCues.ContainsKey(cue.name.Replace("(Instance)", "")) &&
						!(!cue.name.EndsWith("(Instance)") && Regex.IsMatch(cue.name, @".*\(.*\)")))
						originCues.Add(cue.name.Replace("(Instance)", ""), cue);
				}

				foreach (SlimeSounds sound in Resources.FindObjectsOfTypeAll<SlimeSounds>())
				{
					if (!sound.name.Equals(string.Empty) && !originSounds.ContainsKey(sound.name.Replace("(Instance)", "")) &&
						!(!sound.name.EndsWith("(Instance)") && Regex.IsMatch(sound.name, @".*\(.*\)")))
						originSounds.Add(sound.name.Replace("(Instance)", ""), sound);
				}

				foreach (RuntimeAnimatorController animator in Resources.FindObjectsOfTypeAll<RuntimeAnimatorController>())
				{
					if (!animator.name.Equals(string.Empty) && !originAnimators.ContainsKey(animator.name.Replace("(Instance)", "")) &&
						!(!animator.name.EndsWith("(Instance)") && Regex.IsMatch(animator.name, @".*\(.*\)")))
						originAnimators.Add(animator.name.Replace("(Instance)", ""), animator);
				}

				foreach (Avatar avatar in Resources.FindObjectsOfTypeAll<Avatar>())
				{
					if (!avatar.name.Equals(string.Empty) && !originAvatars.ContainsKey(avatar.name.Replace("(Instance)", "")) &&
						!(!avatar.name.EndsWith("(Instance)") && Regex.IsMatch(avatar.name, @".*\(.*\)")))
						originAvatars.Add(avatar.name.Replace("(Instance)", ""), avatar);
				}

				foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>())
				{
					if (!(obj.name.StartsWith("FX ") || obj.name.StartsWith("fx")))
						continue;

					if (!obj.name.Equals(string.Empty) && !originFXs.ContainsKey(obj.name.Replace("(Instance)", "")) &&
						!(!obj.name.EndsWith("(Instance)") && Regex.IsMatch(obj.name, @".*\(.*\)")))
						originFXs.Add(obj.name.Replace("(Instance)", ""), obj);
				}

				// Adds markers for world objects
				foreach (GadgetSiteModel obj in GameModel.AllGadgetSites().Values)
					obj.transform.gameObject.GetReadyForMarker(MarkerType.GadgetLocation, 4f);

				foreach (KookadobaPatchNode node in Resources.FindObjectsOfTypeAll<KookadobaPatchNode>())
				{
					foreach (GameObject child in node.gameObject.FindChildrenWithPartialName("SpawnJoint"))
						child.GetReadyForMarker(MarkerType.SpawnPoint);
				}

				foreach (DirectedAnimalSpawnerModel spawner in GameModel.AllAnimalSpawners())
                {
					((DirectedAnimalSpawner)spawner.part).gameObject.GetReadyForMarker(MarkerType.AnimalSpawnPoint);
                }

				foreach (DirectedSlimeSpawner spawner in Object.FindObjectsOfType<DirectedSlimeSpawner>())
                {
					spawner.gameObject.GetReadyForMarker(MarkerType.SlimeSpawnPoint);
                }

				foreach (DirectedCrateSpawner spawner in Object.FindObjectsOfType<DirectedCrateSpawner>())
                {
					spawner.gameObject.GetReadyForMarker(MarkerType.CrateSpawnPoint);
                }

				foreach (SpawnResourceModel spawner in GameModel.AllResourceSpawners())
                {
					foreach (Joint joint in ((SpawnResource)spawner.part).SpawnJoints)
						joint.gameObject.GetReadyForMarker(MarkerType.ResourceSpawnPoint);
				}

				GardenObjects.LatePopulate();
				TheWildsObjects.LatePopulate();
				RanchObjects.LatePopulate();
				EffectObjects.LatePopulate();

				populated = true;
			}
		}

		// TEMPLATE ACTIONS
		internal static void AssembleModules(GameObject obj)
		{
			obj.GetComponent<SlimeVarietyModules>().Assemble();
			obj.GetComponent<SlimeAppearanceApplicator>().SlimeAppearanceDirector = SceneContext.Instance.SlimeAppearanceDirector;
		}

		internal static void PopulateSlimeInfo(GameObject obj)
		{
			bool isGordo = obj.GetComponent<SlimeEat>().slimeDefinition.Name.StartsWith("roamGordo.");
			SlimeEmotions emo = obj.GetComponent<SlimeEmotions>();
			emo?.SetEmotionEnabled(SlimeEmotions.Emotion.HUNGER, !isGordo);
		}
	}
}
