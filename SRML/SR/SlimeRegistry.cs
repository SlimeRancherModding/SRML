using HarmonyLib;
using SRML.SR.Utils;
using SRML.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

namespace SRML.SR
{
    public static class SlimeRegistry
    {
        internal static Dictionary<SlimeDefinition, SRMod> slimeDefinitions = new Dictionary<SlimeDefinition, SRMod>();
        internal static List<KeyValuePair<Identifiable.Id, Identifiable.Id>> preventLargoTransforms = new List<KeyValuePair<Identifiable.Id, Identifiable.Id>>();
        internal static int stripeShaderId = Shader.PropertyToID("_StripeTexture");
        private static SlimeAppearance quantumApp;
        private static float defaultRadius;
        private static bool initialized;
        public static Dictionary<string, SlimeAppearanceElement> replaceElements = new Dictionary<string, SlimeAppearanceElement>();
        public static List<Shader> dontReplaceShaders = new List<Shader>();

        internal static void Initialize(SlimeDefinitions defs)
        {
            if (initialized) return;
            quantumApp = defs.GetAppearanceById(Identifiable.Id.QUANTUM_SLIME).QubitAppearance;
            defaultRadius = GameContext.Instance.LookupDirector.GetPrefab(Identifiable.Id.PINK_SLIME).GetComponent<SphereCollider>().radius;
            
            replaceElements.Add("Rad Aura", defs.GetAppearanceById(Identifiable.Id.PINK_RAD_LARGO).Structures[1].Element);
            replaceElements.Add("Rad Exotic Aura", (SlimeAppearanceElement)Resources.Load("dlc/secret_style/assets/actor/slime/element/RadAuraLargoExotic"));

            dontReplaceShaders.Add(Shader.Find("SR/AMP/FX/RadAura"));
            dontReplaceShaders.Add(Shader.Find("SR/Null Render"));

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
        /// <param name="refreshEatMaps">Whether or not to refresh the EatMaps of the slime and its bases.</param>
        public static void RegisterSlimeDefinition(SlimeDefinition definition, bool refreshEatMaps = true)
        {
            // TODO: Upgrade to new system
            //slimeDefinitions[definition] = SRMod.GetCurrentMod();
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

            if (definition.IsLargo && definition.BaseSlimes != null && definition.BaseSlimes.Length > 1)
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
                if (definition.BaseSlimes != null) 
                    foreach (SlimeDefinition child in definition.BaseSlimes) 
                        child.Diet.RefreshEatMap(definitions, child);
            }
        }

        /// <summary>
        /// Registers a <see cref="SlimeAppearance"/>.
        /// </summary>
        /// <param name="def">The <see cref="SlimeDefinition"/> that the <see cref="SlimeAppearance"/> is assigned to.</param>
        /// <param name="app">The <see cref="SlimeAppearance"/> to be registered.</param>
        public static void RegisterAppearance(SlimeDefinition def, SlimeAppearance app)
        {
            SceneContext.Instance.SlimeAppearanceDirector.RegisterDependentAppearances(def, app);
            SceneContext.Instance.SlimeAppearanceDirector.UpdateChosenSlimeAppearance(def, app);
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
            INHERIT_STRIPE_FROM_SLIME2 = 131072,
            RECOLOR_EYES_AS_SLIME1 = 262144,
            RECOLOR_EYES_AS_SLIME2 = 524288,
            RECOLOR_MOUTH_AS_SLIME1 = 1048576,
            RECOLOR_MOUTH_AS_SLIME2 = 2097152
        }

        // beware, all who enter here

        /// <summary>
        /// Combines two <see cref="SlimeAppearance"/>s into a largo <see cref="SlimeAppearance"/>.
        /// </summary>
        /// <param name="slime1">The base <see cref="SlimeAppearance"/>.</param>
        /// <param name="slime2">The addon <see cref="SlimeAppearance"/>.</param>
        /// <param name="set">The <see cref="SlimeAppearance.AppearanceSaveSet"/> to register the <see cref="SlimeAppearance"/> into.</param>
        /// <param name="props">The properties controlling the way the <see cref="SlimeAppearance"/>s are combined.</param>
        /// <param name="stripeShader">The stripe shader to be used, if any.</param>
        /// <returns>The created <see cref="SlimeAppearance"/>.</returns>
        public static SlimeAppearance CombineAppearances(SlimeAppearance slime1, SlimeAppearance slime2, SlimeAppearance.AppearanceSaveSet set, LargoProps props, Shader stripeShader = null)
        {
            SlimeAppearance appearance = ScriptableObject.CreateInstance<SlimeAppearance>();

            appearance.AnimatorOverride = slime1.AnimatorOverride ?? slime2.AnimatorOverride;
            appearance.DependentAppearances = new SlimeAppearance[2] { slime1, slime2 };

            bool swapEyes = (props & (LargoProps.SWAP_EYES)) != 0;
            bool swapMouth = (props & (LargoProps.SWAP_MOUTH)) != 0;

            bool recolorEyeSlime1 = (props & (LargoProps.RECOLOR_EYES_AS_SLIME1)) != 0;
            bool recolorEyeSlime2 = (props & (LargoProps.RECOLOR_EYES_AS_SLIME2)) != 0;
            bool recolorMouthSlime1 = (props & (LargoProps.RECOLOR_MOUTH_AS_SLIME1)) != 0;
            bool recolorMouthSlime2 = (props & (LargoProps.RECOLOR_MOUTH_AS_SLIME2)) != 0;

            bool sameEyes = slime1.Face.ExpressionFaces.Select(x => x.Eyes).SequenceEqual(slime2.Face.ExpressionFaces.Select(x => x.Eyes));
            bool sameMouth = slime1.Face.ExpressionFaces.Select(x => x.Mouth).SequenceEqual(slime2.Face.ExpressionFaces.Select(x => x.Mouth));

            if ((!swapEyes && !swapMouth) || (sameEyes && !swapMouth) || (!swapEyes && sameMouth))
                appearance.Face = slime1.Face;
            else if ((swapEyes && swapMouth) || (sameEyes && swapMouth) || (swapEyes && sameMouth))
                appearance.Face = slime2.Face;
            else
            {
                appearance.Face = UnityEngine.Object.Instantiate(slime1.Face);
                appearance.Face.ExpressionFaces = new SlimeExpressionFace[slime1.Face.ExpressionFaces.Length];
                for (int i = 0; i < appearance.Face.ExpressionFaces.Length; i++)
                    appearance.Face.ExpressionFaces[i] = new SlimeExpressionFace { SlimeExpression = slime1.Face.ExpressionFaces[i].SlimeExpression };
            }

            for (int i = 0; i < appearance.Face.ExpressionFaces.Length; i++)
                ReplaceRecolorFaceMats(appearance.Face.ExpressionFaces[i], slime1.Face.ExpressionFaces[i], slime2.Face.ExpressionFaces[i],
                    swapEyes, swapMouth, recolorEyeSlime1, recolorEyeSlime2, recolorMouthSlime1, recolorMouthSlime2);

            appearance.Face.OnEnable();

            appearance.NameXlateKey = string.Empty;
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
                if (i == base1 || structures.Any(x => x.Element == slime1.Structures[i].Element)) continue;
                SlimeAppearanceStructure structure = slime1.Structures[i].Clone();
                if (replaceElements.ContainsKey(structure.Element.Name)) structure.Element = replaceElements[structure.Element.Name];
                ReplaceRecolorStructureMats((props & (LargoProps.REPLACE_SLIME1_ADDON_MATS)) != 0, (props & (LargoProps.RECOLOR_SLIME1_ADDON_MATS)) != 0, structure, structures[0]);
                structures.Add(structure);
            }
            for (int i = 0; i < slime2.Structures.Length; i++)
            {
                if (i == base2 || structures.Any(x => x.Element == slime2.Structures[i].Element)) continue;
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
                List<SlimeAppearanceStructure> qStructures = qubit.Structures.Where(x => x.DefaultMaterials != null && x.DefaultMaterials.Length > 0 && 
                    x.DefaultMaterials[0].HasProperty("_TopColor"))?.ToList();
                if (qStructures != null)
                {
                    Material mat = Material.Instantiate(quantumApp.Structures[0].DefaultMaterials[0]);
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
                else
                    appearance.QubitAppearance = quantumApp;
            }
            return appearance;
        }

        /// <summary>
        /// Creates a largo <see cref="SlimeDefinition"/> from two base <see cref="SlimeDefinition"/>s.
        /// </summary>
        /// <param name="id">The <see cref="Identifiable.Id"/> belonging to the resulting largo.</param>
        /// <param name="slime1">The <see cref="SlimeDefinition"/> belonging to the base slime.</param>
        /// <param name="slime2">The <see cref="SlimeDefinition"/> belonging to the addon slime.</param>
        /// <param name="props">The properties controlling the way the <see cref="SlimeDefinition"/>s are combined.</param>
        /// <returns>The created <see cref="SlimeDefinition"/>.</returns>
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

            slimeDefinition.FavoriteToys = new Identifiable.Id[0];
            if (slime1.FavoriteToys != null)
                slimeDefinition.FavoriteToys = slimeDefinition.FavoriteToys.Union(slime1.FavoriteToys).ToArray();
            if (slime2.FavoriteToys != null)
                slimeDefinition.FavoriteToys = slimeDefinition.FavoriteToys.Union(slime2.FavoriteToys).ToArray();

            if ((props & (LargoProps.PREVENT_SLIME1_EATMAP_TRANSFORM)) != 0)
                preventLargoTransforms.Add(new KeyValuePair<Identifiable.Id, Identifiable.Id>(slime1.IdentifiableId, id));
            if ((props & (LargoProps.PREVENT_SLIME2_EATMAP_TRANSFORM)) != 0)
                preventLargoTransforms.Add(new KeyValuePair<Identifiable.Id, Identifiable.Id>(slime2.IdentifiableId, id));

            return slimeDefinition;
        }

        /// <summary>
        /// Combines the <see cref="GameObject"/>s of the bases of a <see cref="SlimeDefinition"/> into one <see cref="GameObject"/>.
        /// </summary>
        /// <param name="def">The largo's <see cref="SlimeDefinition"/>.</param>
        /// <returns>The created <see cref="GameObject"/>.</returns>
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
            largoPrefab.GetComponent<AweTowardsLargos>()?.Destroy();

            if (largoPrefab.TryGetComponent(out PlayWithToys t))
                t.slimeDefinition = def;
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
                col.radius = slime2Prefab.GetComponent<SphereCollider>().radius == defaultRadius ? 
                    col.radius : slime2Prefab.GetComponents<SphereCollider>().First(x => !x.isTrigger).radius;
                col.center = slime2Prefab.GetComponent<SphereCollider>().radius == defaultRadius ? 
                    col.center : slime2Prefab.GetComponents<SphereCollider>().First(x => !x.isTrigger).center;
            }

            foreach (Transform transform in slime2Prefab.transform)
            {
                if (largoPrefab.transform.Find(transform.name) == null)
                    slime2Prefab.GetChildCopy(transform.name).transform.SetParent(largoPrefab.transform);
            }

            return largoPrefab;
        }

        /// <summary>
        /// Combines two slimes into a largo.
        /// </summary>
        /// <param name="largoId">The <see cref="Identifiable.Id"/> belonging to the resulting largo.</param>
        /// <param name="slime1">The <see cref="Identifiable.Id"/> belonging to the base slime.</param>
        /// <param name="slime2">The <see cref="Identifiable.Id"/> belonging to the addon slime.</param>
        /// <param name="props">The properties controlling the way the slimes are combined.</param>
        /// <param name="slime1SSProps">The properties controlling the base Secret Style and the addon normal <see cref="SlimeAppearance"/>s are combined.</param>
        /// <param name="slime2SSProps">The properties controlling the base normal and the addon Secret Style <see cref="SlimeAppearance"/>s are combined.</param>
        /// <param name="slime12SSProps">The properties controlling the base Secret Style and the addon Secret Style <see cref="SlimeAppearance"/>s are combined.</param>
        public static void CraftLargo(Identifiable.Id largoId, Identifiable.Id slime1, Identifiable.Id slime2,
            LargoProps props, LargoProps slime1SSProps = LargoProps.NONE, LargoProps slime2SSProps = LargoProps.NONE, LargoProps slime12SSProps = LargoProps.NONE) =>
            CraftLargo(largoId, slime1, slime2, props, out var largoDefinition, out var largoAppearance, out var largoObject, slime1SSProps, slime2SSProps, slime12SSProps);

        /// <summary>
        /// Combines two slimes into a largo.
        /// </summary>
        /// <param name="largoId">The <see cref="Identifiable.Id"/> belonging to the resulting largo.</param>
        /// <param name="slime1">The <see cref="Identifiable.Id"/> belonging to the base slime.</param>
        /// <param name="slime2">The <see cref="Identifiable.Id"/> belonging to the addon slime.</param>
        /// <param name="props">The properties controlling the way the slimes are combined.</param>
        /// <param name="slime1SSProps">The properties controlling the base Secret Style and the addon normal <see cref="SlimeAppearance"/>s are combined.</param>
        /// <param name="slime2SSProps">The properties controlling the base normal and the addon Secret Style <see cref="SlimeAppearance"/>s are combined.</param>
        /// <param name="slime12SSProps">The properties controlling the base Secret Style and the addon Secret Style <see cref="SlimeAppearance"/>s are combined.</param>
        /// <param name="largoDefinition">The <see cref="SlimeDefinition"/> of the created largo.</param>
        /// <param name="largoAppearance">The <see cref="SlimeAppearance"/> of the created largo.</param>
        /// <param name="largoObject">The <see cref="GameObject"/> of the created largo.</param>
        public static void CraftLargo(Identifiable.Id largoId, Identifiable.Id slime1, Identifiable.Id slime2, LargoProps props,
            out SlimeDefinition largoDefinition, out SlimeAppearance largoAppearance, out GameObject largoObject,
            LargoProps slime1SSProps = LargoProps.NONE, LargoProps slime2SSProps = LargoProps.NONE, LargoProps slime12SSProps = LargoProps.NONE)
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
                        if (def.GetAppearanceForSet(SlimeAppearance.AppearanceSaveSet.SECRET_STYLE) != null)
                            return;

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

            largoDefinition = def;
            largoAppearance = app;
            largoObject = largoOb;
        }

        internal static void InheritStripe(this Material mat, Material inherited, Shader shader = null)
        {
            if (!inherited.HasProperty(stripeShaderId)) return;

            if (mat.HasProperty(stripeShaderId))
            {
                mat.SetTexture("_Stripe2Texture", inherited.GetTexture("_StripeTexture"));
                mat.SetFloat("_Stripe2UV1", inherited.GetFloat("_StripeUV1"));
            }
            else
            {
                mat.shader = shader == null ? (mat.shader.name == "SR/AMP/Slime/Body/Default" ? 
                    Shader.Find("SR/AMP/Slime/Body/Stripe") : Shader.Find(mat.shader.name + " Stripe")) : shader;
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
            if (replace && !structure.DefaultMaterials.Any(x => dontReplaceShaders.Contains(x.shader))) 
                structure.DefaultMaterials = structure.DefaultMaterials.DuplicateMats(reference.DefaultMaterials);
            else 
                structure.DefaultMaterials = structure.DefaultMaterials.DuplicateMats();

            if (recolor)
            {
                Material mat = reference.DefaultMaterials.FirstOrDefault(x => x.HasProperty("_TopColor"));
                if (mat == null)
                    return;

                int z = 0;
                foreach (Material mat1 in structure.DefaultMaterials)
                {
                    if (mat1.HasProperty("_TopColor"))
                    {
                        Material cloned1 = GameObject.Instantiate(mat1);
                        cloned1.SetColor("_TopColor", mat.GetColor("_TopColor"));
                        cloned1.SetColor("_MiddleColor", mat.GetColor("_MiddleColor"));
                        cloned1.SetColor("_BottomColor", mat.GetColor("_BottomColor"));
                        structure.DefaultMaterials[z] = cloned1;
                    }
                    z++;
                }
            }
        }

        internal static void ReplaceRecolorFaceMats(SlimeExpressionFace ex, SlimeExpressionFace ref1, SlimeExpressionFace ref2,
            bool swapEyes, bool swapMouth, bool recolorEyesSlime1, bool recolorEyesSlime2, bool recolorMouthSlime1, bool recolorMouthSlime2)
        {
            ex.Eyes = swapEyes ? ref2.Eyes : ref1.Eyes;
            ex.Mouth = swapMouth ? ref2.Mouth : ref1.Mouth;

            if (ex.Eyes != null && (recolorEyesSlime1 || recolorEyesSlime2))
            {
                ex.Eyes = UnityEngine.Object.Instantiate(ex.Eyes);
                
                Material refMat;
                if (recolorEyesSlime1)
                    refMat = ref1.Eyes;
                else
                    refMat = ref2.Eyes;

                if (refMat != null)
                {
                    ex.Eyes.SetColor("_EyeRed", refMat.GetColor("_EyeRed"));
                    ex.Eyes.SetColor("_EyeGreen", refMat.GetColor("_EyeGreen"));
                    ex.Eyes.SetColor("_EyeBlue", refMat.GetColor("_EyeBlue"));
                    ex.Eyes.SetColor("_GlowColor", refMat.GetColor("_GlowColor"));
                }
            }

            if (ex.Mouth != null && (recolorMouthSlime1 || recolorMouthSlime2))
            {
                ex.Mouth = UnityEngine.Object.Instantiate(ex.Mouth);
                
                Material refMat;
                if (recolorMouthSlime1)
                    refMat = ref1.Mouth;
                else
                    refMat = ref2.Mouth;

                if (refMat != null)
                {
                    ex.Mouth.SetColor("_MouthBot", refMat.GetColor("_MouthBot"));
                    ex.Mouth.SetColor("_MouthMid", refMat.GetColor("_MouthMid"));
                    ex.Mouth.SetColor("_MouthTop", refMat.GetColor("_MouthTop"));
                }
            }
        }

        /// <summary>
        /// Takes an <see cref="Identifiable.Id"/> and formats it into a largo name.
        /// </summary>
        /// <param name="id">The <see cref="Identifiable.Id"/> belonging to the largo.</param>
        /// <returns>The resulting name.</returns>
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
