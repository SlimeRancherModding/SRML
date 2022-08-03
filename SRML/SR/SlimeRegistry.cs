using HarmonyLib;
using SRML.SR.Utils;
using SRML.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SRML.SR
{
    public static class SlimeRegistry
    {
        internal static Dictionary<SlimeDefinition, SRMod> slimeDefinitions = new Dictionary<SlimeDefinition, SRMod>();
        internal static List<KeyValuePair<Identifiable.Id, Identifiable.Id>> preventLargoTransforms = new List<KeyValuePair<Identifiable.Id, Identifiable.Id>>();
        private static Material quantumMat;
        private static float defaultRadius;
        private static bool initialized;
        public static Dictionary<string, SlimeAppearanceElement> replaceElements = new Dictionary<string, SlimeAppearanceElement>();
        public static List<string> dontReplaceMats = new List<string>();

        internal static void Initialize(SlimeDefinitions defs)
        {
            if (initialized) return;
            quantumMat = defs.GetAppearanceById(Identifiable.Id.QUANTUM_SLIME).QubitAppearance.Structures[0].DefaultMaterials[0];
            defaultRadius = GameContext.Instance.LookupDirector.GetPrefab(Identifiable.Id.PINK_SLIME).GetComponent<SphereCollider>().radius;
            replaceElements.Add("Rad Aura", defs.GetAppearanceById(Identifiable.Id.PINK_RAD_LARGO).Structures[1].Element);
            replaceElements.Add("Rad Exotic Aura", (SlimeAppearanceElement)Resources.Load("dlc/secret_style/assets/actor/slime/element/RadAuraLargoExotic"));
            initialized = true;
        }

        /// <summary>
        /// Register a slime definition in the <see cref="SlimeDefinitions"/> database
        /// </summary>
        /// <param name="definition">Slime definition to register</param>
        public static void RegisterSlimeDefinition(SlimeDefinition definition) => RegisterSlimeDefinition(definition, true);

        /// <summary>
        /// Register a slime definition in the <see cref="SlimeDefinitions"/> database
        /// </summary>
        /// <param name="definition">Slime definition to register</param>
        public static void RegisterSlimeDefinition(SlimeDefinition definition, bool refreshEatMaps = true)
        {
            slimeDefinitions[definition] = SRMod.GetCurrentMod();
            SlimeDefinitions definitions;
            switch (SRModLoader.CurrentLoadingStep)
            {
                case SRModLoader.LoadingStep.PRELOAD:
                    definitions = UnityEngine.Object.FindObjectOfType<GameContext>().SlimeDefinitions;
                    break;
                default:
                    definitions = GameContext.Instance.SlimeDefinitions;
                    break;
            }

            if (definition.IsLargo && definition.BaseSlimes != null)
            {
                if (definition.BaseSlimes[0].Diet.ProducePlorts() && definition.BaseSlimes[1].Diet.ProducePlorts())
                    definitions.largoDefinitionByBasePlorts.AddOrChange(new SlimeDefinitions.PlortPair(definition.BaseSlimes[0].Diet.Produces[0], definition.BaseSlimes[1].Diet.Produces[0]), definition);
                definitions.largoDefinitionByBaseDefinitions.AddOrChange(new SlimeDefinitions.SlimeDefinitionPair(definition.BaseSlimes[0], definition.BaseSlimes[1]), definition);
            }
            definitions.slimeDefinitionsByIdentifiable.AddOrChange(definition.IdentifiableId, definition);
            definitions.Slimes = definitions.Slimes.Where(x => x.IdentifiableId != definition.IdentifiableId).AddItem(definition).ToArray();
            if (refreshEatMaps)
            {
                definition.Diet.RefreshEatMap(definitions, definition);
                if (definition.BaseSlimes != null) foreach (SlimeDefinition child in definition.BaseSlimes) child.Diet.RefreshEatMap(definitions, child);
            }
        }

        [Flags]
        public enum LargoProps
        {
            NONE = 0,
            SWAP_EYES = 1,
            SWAP_MOUTH = 2,
            SWAP_BASE = 4,
            SWAP_SOUNDS = 8,
            REPLACE_BASE_MAT_AS_SLIME1 = 16,
            REPLACE_BASE_MAT_AS_SLIME2 = 32,
            RECOLOR_SLIME1_ADDON_MATS = 64,
            RECOLOR_SLIME2_ADDON_MATS = 128,
            REPLACE_SLIME1_ADDON_MATS = 256,
            REPLACE_SLIME2_ADDON_MATS = 512,
            RECOLOR_BASE_MAT_AS_SLIME1 = 1024,
            RECOLOR_BASE_MAT_AS_SLIME2 = 2048,
            PREVENT_SLIME1_EATMAP_TRANSFORM = 4096,
            PREVENT_SLIME2_EATMAP_TRANSFORM = 8192,
            GENERATE_NAME = 16384,
            GENERATE_SECRET_STYLES = 32768,
            INHERIT_STRIPE_FROM_SLIME1 = 65536,
            INHERIT_STRIPE_FROM_SLIME2 = 131072
        }

        public static SlimeAppearance CombineAppearances(SlimeAppearance slime1, SlimeAppearance slime2, SlimeAppearance.AppearanceSaveSet set, LargoProps props, Shader stripeShader = null)
        {
            SlimeAppearance appearance = ScriptableObject.CreateInstance<SlimeAppearance>();
            appearance.AnimatorOverride = slime1.AnimatorOverride ?? slime2.AnimatorOverride;
            appearance.DependentAppearances = new SlimeAppearance[2] { slime1, slime2 };
            appearance.Face = (SlimeFace)PrefabUtils.DeepCopyObject(slime1.Face);
            appearance.Face._expressionToFaceLookup.Clear();
            foreach (KeyValuePair<SlimeFace.SlimeExpression, SlimeExpressionFace> kvp in slime1.Face._expressionToFaceLookup)
            {
                SlimeExpressionFace face = new SlimeExpressionFace();
                face.SlimeExpression = kvp.Key;
                face.Eyes = (props & (LargoProps.SWAP_EYES)) != 0 ? slime2.Face._expressionToFaceLookup[kvp.Key].Eyes : slime1.Face._expressionToFaceLookup[kvp.Key].Eyes;
                face.Mouth = (props & (LargoProps.SWAP_MOUTH)) != 0 ? slime2.Face._expressionToFaceLookup[kvp.Key].Mouth : slime1.Face._expressionToFaceLookup[kvp.Key].Mouth;
                appearance.Face._expressionToFaceLookup.Add(kvp.Key, face);
            }
            appearance.Face.ExpressionFaces = appearance.Face._expressionToFaceLookup.Values.ToArray();
            appearance.NameXlateKey = slime1.NameXlateKey;
            appearance.SaveSet = set;
            List<SlimeAppearanceStructure> structures = new List<SlimeAppearanceStructure>();
            int base1 = slime1.Structures.IndexOfItem(slime1.Structures.FirstOrDefault(x => x.Element.Name.Contains("Body")));
            int base2 = slime2.Structures.IndexOfItem(slime2.Structures.FirstOrDefault(x => x.Element.Name.Contains("Body")));
            base1 = base1 == -1 ? 0 : base1;
            base2 = base2 == -1 ? 0 : base2;
            structures.Add((props & (LargoProps.SWAP_BASE)) != 0 ? slime2.Structures[base2].Clone() : slime1.Structures[base1].Clone());
            ReplaceRecolorStructureMats((props & (LargoProps.REPLACE_BASE_MAT_AS_SLIME1)) != 0, false, structures[0], slime1.Structures[0]);
            ReplaceRecolorStructureMats((props & (LargoProps.REPLACE_BASE_MAT_AS_SLIME2)) != 0, false, structures[0], slime2.Structures[0]);
            ReplaceRecolorStructureMats(false, (props & (LargoProps.RECOLOR_BASE_MAT_AS_SLIME1)) != 0, structures[0], slime1.Structures[0]);
            ReplaceRecolorStructureMats(false, (props & (LargoProps.RECOLOR_BASE_MAT_AS_SLIME2)) != 0, structures[0], slime2.Structures[0]);
            if ((props & (LargoProps.INHERIT_STRIPE_FROM_SLIME1)) != 0) structures[0].DefaultMaterials[0].InheritStripe(slime1.Structures[0].DefaultMaterials[0], stripeShader);
            if ((props & (LargoProps.INHERIT_STRIPE_FROM_SLIME2)) != 0) structures[0].DefaultMaterials[0].InheritStripe(slime2.Structures[0].DefaultMaterials[0], stripeShader);
            for (int i = 0; i < slime1.Structures.Length; i++)
            {
                if (i == base1) continue;
                SlimeAppearanceStructure structure = slime1.Structures[i].Clone();
                if (replaceElements.ContainsKey(structure.Element.Name)) structure.Element = replaceElements[structure.Element.Name];
                ReplaceRecolorStructureMats((props & (LargoProps.REPLACE_SLIME1_ADDON_MATS)) != 0, (props & (LargoProps.RECOLOR_SLIME1_ADDON_MATS)) != 0, structure, structures[0]);
                structures.Add(structure);
            }
            for (int i = 0; i < slime2.Structures.Length; i++)
            {
                if (i == base2) continue;
                SlimeAppearanceStructure structure = slime2.Structures[i].Clone();
                if (replaceElements.ContainsKey(structure.Element.Name)) structure.Element = replaceElements[structure.Element.Name];
                ReplaceRecolorStructureMats((props & (LargoProps.REPLACE_SLIME2_ADDON_MATS)) != 0, (props & (LargoProps.RECOLOR_SLIME2_ADDON_MATS)) != 0, structure, structures[0]);
                structures.Add(structure);
            }
            appearance.Structures = structures.ToArray();
            appearance.ColorPalette = SlimeAppearance.Palette.FromMaterial(appearance.Structures[0].DefaultMaterials[0]);
            appearance.CrystalAppearance = slime1.CrystalAppearance ?? slime2.CrystalAppearance;
            appearance.DeathAppearance = slime1.DeathAppearance ?? slime2.DeathAppearance;
            appearance.ExplosionAppearance = slime1.ExplosionAppearance ?? slime2.ExplosionAppearance;
            appearance.GlintAppearance = slime1.GlintAppearance ?? slime2.GlintAppearance;
            appearance.ShockedAppearance = slime1.ShockedAppearance ?? slime2.ShockedAppearance;
            appearance.TornadoAppearance = slime1.TornadoAppearance ?? slime2.TornadoAppearance;
            appearance.VineAppearance = slime1.VineAppearance ?? slime2.VineAppearance;
            if (slime1.QubitAppearance != null || slime2.QubitAppearance != null)
            {
                SlimeAppearance qubit = (SlimeAppearance)PrefabUtils.DeepCopyObject(appearance);
                List<SlimeAppearanceStructure> qStructures = qubit.Structures.Where(x => x.DefaultMaterials[0].HasProperty("_TopColor")).ToList();
                Material mat = Material.Instantiate(quantumMat);
                mat.SetFloat("_GhostToggle", 1f);
                for (int i = 0; i < qStructures.Count; i++)
                {
                    qStructures[i] = qStructures[i].Clone();
                    qStructures[i].DefaultMaterials = qStructures[i].DefaultMaterials.DuplicateMats();
                    for (int j = 0; j < qStructures[i].DefaultMaterials.Length; j++)
                        qStructures[i].DefaultMaterials[j] = mat;
                    ReplaceRecolorStructureMats(false, true, qStructures[i], appearance.Structures[0]);
                }
                qubit.Structures = qStructures.ToArray();
                appearance.QubitAppearance = qubit;
            }
            return appearance;
        }

        public static void RegisterAppearance(SlimeDefinition def, SlimeAppearance app)
        {
            SceneContext.Instance.SlimeAppearanceDirector.RegisterDependentAppearances(def, app);
            SceneContext.Instance.SlimeAppearanceDirector.UpdateChosenSlimeAppearance(def, app);
        }

        internal static void InheritStripe(this Material mat, Material inherited, Shader shader = null)
        {
            if (!inherited.shader.name.Contains("Stripe")) return;
            if (mat.shader.name.Contains("Stripe"))
            {
                mat.SetTexture("_Stripe2Texture", inherited.GetTexture("_StripeTexture"));
                mat.SetFloat("_Stripe2UV1", inherited.GetFloat("_StripeUV1"));
            }
            else
            {
                mat.shader = shader == null ? (mat.shader.name == "SR/AMP/Slime/Body/Default" ? Shader.Find("SR/AMP/Slime/Body/Stripe") : Shader.Find(mat.shader.name + " Stripe")) : shader;
                mat.SetTexture("_StripeTexture", inherited.GetTexture("_StripeTexture"));
                mat.SetFloat("_StripeUV1", inherited.GetFloat("_StripeUV1"));
            }
            mat.SetFloat("_StripeSpeed", inherited.GetFloat("_StripeSpeed"));
        }

        internal static Material[] DuplicateMats(this Material[] mats, Material[] source = null)
        {
            Material[] arr = source == null ? mats : source;
            Material[] newMats = new Material[arr.Length];
            Array.Copy(arr, newMats, arr.Length);
            mats = newMats;
            return mats;
        }

        internal static void ReplaceRecolorStructureMats(bool replace, bool recolor, SlimeAppearanceStructure structure, SlimeAppearanceStructure reference)
        {
            if (replace && !structure.DefaultMaterials.Any(x => dontReplaceMats.Contains(x.name))) 
                structure.DefaultMaterials = structure.DefaultMaterials.DuplicateMats(reference.DefaultMaterials);
            else 
                structure.DefaultMaterials = structure.DefaultMaterials.DuplicateMats();

            if (recolor)
            {
                Material mat = reference.DefaultMaterials[0];
                int z = 0;
                foreach (Material mat1 in structure.DefaultMaterials)
                {
                    Material cloned1 = GameObject.Instantiate(mat1);
                    if (cloned1.HasProperty("_TopColor"))
                    {
                        cloned1.SetColor("_TopColor", mat.GetColor("_TopColor"));
                        cloned1.SetColor("_MiddleColor", mat.GetColor("_MiddleColor"));
                        cloned1.SetColor("_BottomColor", mat.GetColor("_BottomColor"));
                    }
                    structure.DefaultMaterials[z] = cloned1;
                    z++;
                }
            }
        }

        public static SlimeDefinition CombineDefinitions(Identifiable.Id id, SlimeDefinition slime1, SlimeDefinition slime2, LargoProps props)
        {
            SlimeDefinition slimeDefinition = ScriptableObject.CreateInstance<SlimeDefinition>();

            slimeDefinition.BaseSlimes = new SlimeDefinition[] { slime1, slime2 };
            slimeDefinition.CanLargofy = false;
            slimeDefinition.IdentifiableId = id;
            slimeDefinition.IsLargo = true;
            slimeDefinition.Name = $"{slime1.Name} {slime2.Name}";
            slimeDefinition.PrefabScale = 2;
            slimeDefinition.Sounds = ((props & (LargoProps.SWAP_SOUNDS)) != 0) ? slime2.Sounds : slime1.Sounds;

            slimeDefinition.LoadLargoDiet();
            slimeDefinition.LoadFavoriteToysFromBaseSlimes();

            if ((props & (LargoProps.PREVENT_SLIME1_EATMAP_TRANSFORM)) != 0) 
                preventLargoTransforms.Add(new KeyValuePair<Identifiable.Id, Identifiable.Id>(slime1.IdentifiableId, id));
            if ((props & (LargoProps.PREVENT_SLIME2_EATMAP_TRANSFORM)) != 0) 
                preventLargoTransforms.Add(new KeyValuePair<Identifiable.Id, Identifiable.Id>(slime2.IdentifiableId, id));

            return slimeDefinition;
        }

        public static GameObject CombineSlimePrefabs(SlimeDefinition def)
        {
            GameObject largoPrefab = PrefabUtils.CopyPrefab(def.BaseSlimes[0].GetPrefab());
            GameObject slime2Prefab = def.BaseSlimes[1].GetPrefab();
            largoPrefab.name = GenerateLargoName(def.IdentifiableId).Replace(" ", string.Empty).Replace("Largo", string.Empty);
            largoPrefab.transform.localScale = Vector3.one * def.PrefabScale;

            largoPrefab.GetComponent<SlimeAppearanceApplicator>().SlimeDefinition = def;
            largoPrefab.GetComponent<SlimeEat>().slimeDefinition = def;
            largoPrefab.GetComponent<Identifiable>().id = def.IdentifiableId;
            largoPrefab.GetComponent<Vacuumable>().size = Vacuumable.Size.LARGE;
            largoPrefab.GetComponent<Rigidbody>().mass += slime2Prefab.GetComponent<Rigidbody>().mass;
            GameObject.Destroy(largoPrefab.GetComponent<AweTowardsLargos>());

            if (largoPrefab.HasComponent<PlayWithToys>())
                largoPrefab.GetComponent<PlayWithToys>().slimeDefinition = def;
            if (largoPrefab.HasComponent<ReactToToyNearby>())
                largoPrefab.GetComponent<ReactToToyNearby>().slimeDefinition = def;

            foreach (Component component in slime2Prefab.GetComponents(typeof(Component)))
                if (!largoPrefab.HasComponent(component.GetType()))
                    largoPrefab.AddComponent(component.GetType()).GetCopyOf(component);

            if (def.Sounds != null)
                largoPrefab.GetComponent<SlimeAudio>().slimeSounds = def.Sounds;

            SphereCollider col = largoPrefab.GetComponent<SphereCollider>();
            if (col != null && col.radius == defaultRadius)
            {
                col.radius = slime2Prefab.GetComponent<SphereCollider>().radius == defaultRadius ? col.radius : slime2Prefab.GetComponent<SphereCollider>().radius;
                col.center = slime2Prefab.GetComponent<SphereCollider>().radius == defaultRadius ? col.center : slime2Prefab.GetComponent<SphereCollider>().center;
            }

            foreach (Transform transform in slime2Prefab.transform)
            {
                if (largoPrefab.transform.Find(transform.name) == null)
                    slime2Prefab.GetChildCopy(transform.name).transform.SetParent(largoPrefab.transform);
            }

            return largoPrefab;
        }

        public static void CraftLargo(Identifiable.Id largoId, Identifiable.Id slime1, Identifiable.Id slime2, LargoProps props, LargoProps slime1SSProps = LargoProps.NONE, LargoProps slime2SSProps = LargoProps.NONE, LargoProps slime12SSProps = LargoProps.NONE)
        {
            SlimeDefinition slime1Def = slime1.GetSlimeDefinition();
            SlimeDefinition slime2Def = slime2.GetSlimeDefinition();

            SlimeDefinition def = CombineDefinitions(largoId, slime1Def, slime2Def, props);
            SlimeAppearance app = CombineAppearances(slime1Def.AppearancesDefault[0], slime2Def.AppearancesDefault[0], SlimeAppearance.AppearanceSaveSet.CLASSIC, props);
            GameObject largoOb = CombineSlimePrefabs(def);

            if ((props & (LargoProps.GENERATE_NAME)) != 0)
                TranslationPatcher.AddActorTranslation("l." + largoId.ToString().ToLower(), GenerateLargoName(largoId));

            if ((props & (LargoProps.GENERATE_SECRET_STYLES)) != 0)
            {
                GameContext.Instance.DLCDirector.onPackageInstalled += (x) =>
                {
                    if (x == DLCPackage.Id.SECRET_STYLE)
                    {
                        SlimeAppearance secretSlime1 = slime1Def.GetAppearanceForSet(SlimeAppearance.AppearanceSaveSet.SECRET_STYLE);
                        SlimeAppearance secretSlime2 = slime2Def.GetAppearanceForSet(SlimeAppearance.AppearanceSaveSet.SECRET_STYLE);

                        if (secretSlime1 != null && secretSlime2 != null)
                        {
                            RegisterAppearance(def, CombineAppearances(secretSlime1, secretSlime2, SlimeAppearance.AppearanceSaveSet.SECRET_STYLE, slime12SSProps == default ? props : slime12SSProps));
                            RegisterAppearance(def, CombineAppearances(slime1Def.AppearancesDefault[0], secretSlime2, SlimeAppearance.AppearanceSaveSet.SECRET_STYLE, slime2SSProps == default ? props : slime2SSProps));
                            RegisterAppearance(def, CombineAppearances(secretSlime1, slime2Def.AppearancesDefault[0], SlimeAppearance.AppearanceSaveSet.SECRET_STYLE, slime1SSProps == default ? props : slime1SSProps));
                        }
                        else if (secretSlime1 != null)
                            RegisterAppearance(def, CombineAppearances(secretSlime1, slime2Def.AppearancesDefault[0], SlimeAppearance.AppearanceSaveSet.SECRET_STYLE, slime1SSProps == default ? props : slime1SSProps));
                        else if (secretSlime2 != null)
                            RegisterAppearance(def, CombineAppearances(slime1Def.AppearancesDefault[0], secretSlime2, SlimeAppearance.AppearanceSaveSet.SECRET_STYLE, slime2SSProps == default ? props : slime2SSProps));
                    }
                };
            }

            def.AppearancesDefault = new SlimeAppearance[1] { app };
            LookupRegistry.RegisterIdentifiablePrefab(largoOb);
            RegisterAppearance(def, app);
            RegisterSlimeDefinition(def);
        }

        public static string GenerateLargoName(Identifiable.Id id)
        {
            string[] name = id.ToString().ToLower().Split('_');
            int i = 0;
            foreach (string namePiece in name)
            {
                name[i] = namePiece[0].ToString().ToUpper() + namePiece.Substring(1);
                i++;
            }
            return string.Join(" ", name).Replace("Slime", "Largo");
        }
    }
}
