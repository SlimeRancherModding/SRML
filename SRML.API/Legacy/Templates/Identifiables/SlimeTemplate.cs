using System.Collections.Generic;
using MonomiPark.SlimeRancher.Regions;
using SRML.SR;
using SRML.SR.Templates.Identifiables;
using SRML.SR.Utils;
using SRML.SR.Utils.BaseObjects;
using UnityEngine;

namespace SRML.SR.Templates.Identifiables
{
    /// <summary>
    /// A template to create new slimes
    /// </summary>
    public class SlimeTemplate : ModPrefab<SlimeTemplate>
    {
        // Base for Identifiables
        protected Identifiable.Id ID;
        protected Vacuumable.Size vacSize = Vacuumable.Size.NORMAL;

        // Base for Slimes
        protected SlimeDefinition definition;
        protected FearProfile fearProfile;

        protected SlimeEmotions.EmotionState hungerState = new SlimeEmotions.EmotionState(SlimeEmotions.Emotion.HUNGER, 0, 0, 0, 0);
        protected SlimeEmotions.EmotionState agitationState = new SlimeEmotions.EmotionState(SlimeEmotions.Emotion.AGITATION, 0, 0, 0, 0);
        protected SlimeEmotions.EmotionState fearState = new SlimeEmotions.EmotionState(SlimeEmotions.Emotion.FEAR, 0, 0, 0, 0);

        protected float driveToEat = 0.333f;
        protected float drivePerEat = 0.333f;

        protected float agitPerEat = 0.15f;
        protected float agitPerFavEat = 0.3f;

        protected int maxHealth = 20;

        protected bool canBeFeral = true;
        protected bool canBeAGlitch = true;
        protected bool isGordo = false;

        protected GameObject slimeModule;
        protected GameObject boneStructure = null;

        // Component Configs
        protected Vector3 delaunchScale = Vector3.one;
        protected readonly List<ICreateComponent> extras = new List<ICreateComponent>();

        // Property to get the module
        public GameObject Module => slimeModule;

        /// <summary>
        /// Template to create new slimes
        /// </summary>
        /// <param name="name">The name of the object (prefixes are recommend, but not needed)</param>
        /// <param name="definition">The definition for this slime</param>
        public SlimeTemplate(string name, SlimeDefinition definition) : base(name)
        {
            ID = definition.IdentifiableId;
            this.definition = definition;

            if (definition.Name.StartsWith("roamGordo."))
                isGordo = true;
        }

        /// <summary>
        /// Sets the vacuumable size
        /// </summary>
        /// <param name="vacSize">The vac size to set</param>
        public SlimeTemplate SetVacSize(Vacuumable.Size vacSize)
        {
            this.vacSize = vacSize;
            return this;
        }

        /// <summary>
        /// Sets the eat info
        /// </summary>
        /// <param name="driveToEat">The min. drive required for the slime to eat</param>
        /// <param name="drivePerEat">How much the drive is reduced once fed</param>
        /// <param name="agitPerEat">How much the agitation is reduced once fed</param>
        /// <param name="agitPerFavEat">How much the agitation is reduced once fed it's favorite</param>
        public SlimeTemplate SetEatInfo(float driveToEat, float drivePerEat, float agitPerEat = 0.15f, float agitPerFavEat = 0.3f)
        {
            this.drivePerEat = drivePerEat;
            this.driveToEat = driveToEat;
            this.agitPerEat = agitPerEat;
            this.agitPerFavEat = agitPerFavEat;
            return this;
        }

        /// <summary>
        /// Sets the max. health of this slime
        /// </summary>
        /// <param name="health">The max. health to set</param>
        public SlimeTemplate SetHealth(int health)
        {
            maxHealth = health;
            return this;
        }

        /// <summary>
        /// Sets the possibility of having a feral state
        /// </summary>
        /// <param name="canBeFeral">Can the slime be afected by a feral state</param>
        public SlimeTemplate SetFeralState(bool canBeFeral)
        {
            this.canBeFeral = canBeFeral;
            return this;
        }

        /// <summary>
        /// Sets the possibility of turning into a glitch if spawned in the Slimeulation
        /// </summary>
        /// <param name="canBeAGlitch">Can the slime be turned into a glitch inside the Slimeulation</param>
        public SlimeTemplate SetGlitchState(bool canBeAGlitch)
        {
            this.canBeAGlitch = canBeAGlitch;
            return this;
        }

        /// <summary>
        /// Sets the fear profile
        /// </summary>
        /// <param name="profile">The fear profile to set</param>
        public SlimeTemplate SetFearProfile(FearProfile profile)
        {
            fearProfile = profile;
            return this;
        }

        /// <summary>
        /// Sets the starting emotions of this slime
        /// </summary>
        /// <param name="hungerState">The initial state for Hunger</param>
        /// <param name="agitationState">The initial state for Agitation</param>
        /// <param name="fearState">The initial state for Fear</param>
        public SlimeTemplate SetEmotions(SlimeEmotions.EmotionState hungerState, SlimeEmotions.EmotionState agitationState, SlimeEmotions.EmotionState fearState)
        {
            this.hungerState = hungerState;
            this.agitationState = agitationState;
            this.fearState = fearState;
            return this;
        }

        /// <summary>
        /// Sets the scale for the Delaunch Trigger (do not change if you don't know what you are doing)
        /// </summary>
        /// <param name="delaunchScale">The new scale to set</param>
        public SlimeTemplate SetDelaunchScale(Vector3 delaunchScale)
        {
            this.delaunchScale = delaunchScale;
            return this;
        }

        /// <summary>
        /// Adds a new behaviour to the slime
        /// </summary>
        /// <param name="component">The component containing the new behaviour</param>
        public SlimeTemplate AddBehaviour(ICreateComponent component)
        {
            extras.Add(component);
            return this;
        }

        /// <summary>
        /// Adds a new behaviour to the slime
        /// </summary>
        /// <param name="component">The component containing the new behaviour</param>
        /// <param name="comps">A list of components containing the new behaviour</param>
        public SlimeTemplate AddBehaviour(ICreateComponent component, params ICreateComponent[] comps)
        {
            extras.Add(component);
            extras.AddRange(comps);
            return this;
        }

        /// <summary>
        /// Sets a new bone structure for the slime (Starting at the Appearance part)
        /// </summary>
        /// <param name="boneStructure">The new bone structure as a game object (to add to the prefab)</param>
        public SlimeTemplate SetBoneStructure(GameObject boneStructure)
        {
            this.boneStructure = boneStructure;
            return this;
        }

        /// <summary>
        /// Sets the translation for this slime's name
        /// </summary>
        /// <param name="name">The translated name</param>
        public override SlimeTemplate SetTranslation(string name)
        {
            TranslationPatcher.AddActorTranslation("l." + ID.ToString().ToLower(), name);
            return this;
        }

        /// <summary>
        /// Creates the object of the template (To get the prefab version use .ToPrefab() after calling this)
        /// </summary>
        public override SlimeTemplate Create()
        {
            // Builds the slime required stuff
            if (!isGordo) BuildModule();
            RegisterDefinition();

            // Create conditional components
            Create<SlimeFeral> slimeFeral = new Create<SlimeFeral>((feral) =>
            {
                feral.auraPrefab = EffectObjects.fxFeralAura;
                feral.dynamicToFeral = false;
                feral.dynamicFromFeral = true;
                feral.feralLifetimeHours = 3;
            });

            // Create main object
            mainObject.AddComponents(
                new Create<SlimeAppearanceApplicator>((app) =>
                {
                    app.SlimeDefinition = definition;
                    app.SlimeExpression = SlimeFace.SlimeExpression.Happy;
                }),
                new Create<Identifiable>((ident) => ident.id = ID),
                new Create<RegionMember>((rg) => rg.canHibernate = true),
                new Create<SlimeVarietyModules>((modules) =>
                {
                    modules.baseModule = definition.BaseModule;
                    modules.slimeModules = slimeModule?.Group() ?? definition.SlimeModules;
                }),
                new Create<SlimeEat>((eat) =>
                {
                    eat.slimeDefinition = definition;
                    eat.damagePerAttack = 20;
                    eat.EatFX = EffectObjects.fxSlimeEat;
                    eat.EatFavoriteFX = EffectObjects.fxSlimeFavEat;
                    eat.ProduceFX = EffectObjects.fxSlimeProduce;
                    eat.TransformFX = EffectObjects.fxSlimeTrans;
                    eat.minDriveToEat = driveToEat;
                    eat.drivePerEat = drivePerEat;
                    eat.agitationPerEat = agitPerEat;
                    eat.agitationPerFavEat = agitPerFavEat;
                }),
                new Create<ReactToToyNearby>((react) => react.slimeDefinition = definition),
                new Create<SphereCollider>((col) =>
                {
                    col.center = Vector3.zero;
                    col.radius = 0.5f;
                    col.sharedMaterial = BaseObjects.originPhysMaterial["Slime"];
                }),
                new Create<Rigidbody>((body) =>
                {
                    body.drag = 0.2f;
                    body.angularDrag = 5f;
                    body.mass = 0.5f;
                    body.useGravity = true;
                }),
                new Create<Vacuumable>((vac) => vac.size = vacSize),
                new Create<GotoConsumable>((goT) =>
                {
                    goT.maxJump = 10;
                    goT.attemptTime = 10;
                    goT.giveUpTime = 10;
                    goT.minSearchRad = 5;
                    goT.maxSearchRad = 30;
                    goT.facingStability = 1;
                    goT.facingSpeed = 5;
                    goT.pursuitSpeedFactor = 1;
                    goT.minDist = 0;
                }),
                new Create<SlimeRandomMove>((move) =>
                {
                    move.verticalFactor = 1;
                    move.scootSpeedFactor = 1;
                }),
                new Create<SlimeHealth>((health) => health.maxHealth = maxHealth),
                new Create<SlimeEmotions>((emo) =>
                {
                    emo.initHunger = hungerState;
                    emo.initAgitation = agitationState;
                    emo.initFear = fearState;
                }),
                new Create<SlimeSubbehaviourPlexer>(null),
                new Create<KeepUpright>((keep) =>
                {
                    keep.stability = 0.7f;
                    keep.speed = 4f;
                }),
                new Create<FleeThreats>((flee) =>
                {
                    flee.fearProfile = fearProfile;
                    flee.driver = SlimeEmotions.Emotion.FEAR;
                    flee.maxJump = 4;
                    flee.facingStability = 0.2f;
                    flee.facingSpeed = 1;
                }),
                new Create<SlimeAudio>((audio) => audio.slimeSounds = definition.Sounds),
                new Create<SECTR_PointSource>(null),
                canBeFeral ? slimeFeral : null,
                new Create<EscapeStuck>(null),
                new Create<MaybeCullOnReenable>(null),
                new Create<DragFloatReactor>((drag) => drag.floatDragMultiplier = 25f),
                new Create<SplatOnImpact>((splat) =>
                {
                    splat.splatPrefab = BaseObjects.splatQuad;
                    splat.splatFXPrefab = EffectObjects.fxSplat;
                }),
                new Create<AweTowardsLargos>((awe) =>
                {
                    awe.minSearchRad = 5;
                    awe.maxSearchRad = 15;
                    awe.facingStability = 1;
                    awe.facingSpeed = 5;
                    awe.pursuitSpeedFactor = 1;
                    awe.minDist = 0;
                }),
                new Create<SlimeFaceAnimator>(null),
                new Create<CalmedByWaterSpray>((spray) =>
                {
                    spray.agitationReduction = 0.1f;
                    spray.calmedHours = 0.3333f;
                }),
                new Create<TotemLinkerHelper>(null),
                new Create<CollisionAggregator>(null),
                new Create<AweTowardsAttractors>((awe) =>
                {
                    awe.facingSpeed = 2.5f;
                    awe.facingStability = 1;
                }),
                new Create<AttachFashions>((fash) => fash.gordoMode = false),
                new Create<SlimeEyeComponents>(null),
                new Create<SlimeMouthComponents>(null),
                new Create<PlayWithToys>((play) =>
                {
                    play.slimeDefinition = definition;
                    play.onlyFloatingToys = false;
                    play.minSearchRad = 5;
                    play.maxSearchRad = 15;
                    play.facingSpeed = 5;
                    play.facingStability = 1;
                    play.pursuitSpeedFactor = 1;
                    play.minDist = 0;
                }),
                new Create<SlimeIgniteReact>((react) => react.igniteFX = EffectObjects.fxStars),
                canBeAGlitch ? new Create<GlitchSlime>(null) : null,
                new Create<Chomper>((chop) => chop.timePerAttack = 3)
            );

            mainObject
                .AddComponents(extras.ToArray())
                .AddAfterChildren(SetValuesAfterBuild)
                .SetTransform(Vector3.zero, Vector3.zero, Vector3.one * (isGordo ? 4 : definition.PrefabScale))
                .AddAwakeAction("buildSlime")
                .AddStartAction("populateSlime")
                .AddStartAction("fixSlime." + mainObject.Name);

            TemplateActions.RegisterAction("fixSlime." + mainObject.Name, ApplyTrueForm);

            // Create eat trigger
            mainObject.AddChild(new GameObjectTemplate("EatTrigger",
                new Create<SphereCollider>((col) =>
                {
                    col.center = Vector3.zero;
                    col.radius = 0.45f;
                    col.isTrigger = true;
                }),
                new Create<SlimeEatTrigger>(null)
            ));

            // Create totem linker
            mainObject.AddChild(new GameObjectTemplate("TotemLinker",
                new Create<SphereCollider>((col) =>
                {
                    col.center = Vector3.zero;
                    col.radius = 0.1f;
                    col.isTrigger = true;
                }),
                new Create<TotemLinker>((totem) =>
                {
                    totem.receptivenessProb = 0.25f;
                    totem.rethinkReceptivenessMin = 6;
                    totem.rethinkReceptivenessMax = 12;
                    totem.gravFactorWhileTotemed = 0.5f;
                })
            ));

            // Create delaunch
            mainObject.AddChild(new GameObjectTemplate("DelaunchTrigger",
                new Create<SphereCollider>((col) =>
                {
                    col.center = Vector3.zero;
                    col.radius = 0.1f;
                    col.isTrigger = true;
                }),
                new Create<VacDelaunchTrigger>(null)
            ).SetTransform(Vector3.zero, Vector3.zero, delaunchScale));

            mainObject.Layer = BaseObjects.layers["Actor"];

            return this;
        }

        protected void SetValuesAfterBuild(GameObject obj)
        {
            // Adds the bones
            GameObject bones = boneStructure?.CreatePrefabCopy() ?? BaseObjects.originBones["SlimeBones"].CreatePrefabCopy();
            bones.transform.parent = obj.transform;
            bones.transform.localPosition = new Vector3(0, -0.5f, 0);
            bones.transform.localScale = Vector3.one;
            bones.SetActive(true);
            bones.name = "Appearance";

            // Configures the appearance applicator
            SlimeAppearanceApplicator app = obj.GetComponent<SlimeAppearanceApplicator>();
            app.RootAppearanceObject = obj.FindChild("Appearance");
            app.LODGroup = app.RootAppearanceObject.GetComponent<LODGroup>();
            app.Bones = BoneMappingUtils.GetMaps(bones).ToArray();

            // Configures the Face Animator
            obj.GetComponent<SlimeFaceAnimator>().appearanceApplicator = app;
        }

        protected void ApplyTrueForm(GameObject obj)
        {
            if (!canBeAGlitch)
                Object.Destroy(obj.GetComponent<GlitchSlime>());

            if (!canBeFeral)
                Object.Destroy(obj.GetComponent<SlimeFeral>());
        }

        /// <summary>
        /// Generates the largos for this slime (use this if you want the slime to be largoable)
        /// </summary>
        /// <param name="canBeTarr">Can the largos from the slime become tarr?</param>
        /// <param name="extraLargoBehaviour">Use this to add extra behaviours to the largos</param>
        /// <returns>A list of all IDs made for the largos created</returns>
        public List<Identifiable.Id> MakeLargos(bool canBeTarr = true, System.Action<SlimeDefinition> extraLargoBehaviour = null)
        {
            if (isGordo)
                return null;

            List<Identifiable.Id> largoIDs = new List<Identifiable.Id>();

            definition.CanLargofy = true;

            foreach (Identifiable.Id id in GameContext.Instance.LookupDirector.GetSlimeIDs(ID))
                largoIDs.Add(CraftLargo(canBeTarr, ID, id, extraLargoBehaviour));

            largoIDs.RemoveAll((id) => id == Identifiable.Id.NONE);

            return largoIDs;
        }

        internal Identifiable.Id CraftLargo(bool canBeTarr, Identifiable.Id slimeA, Identifiable.Id slimeB, System.Action<SlimeDefinition> extraLargoBehaviour = null)
        {
            if (GameContext.Instance.LookupDirector.LargoExists(slimeA, slimeB))
                return Identifiable.Id.NONE;

            string prefabName = mainObject.Name + slimeB.ToString().Replace("_SLIME", "").ToUpper()[0] + slimeB.ToString().Replace("_SLIME", "").ToLower().Substring(1);
            string name = slimeA.ToString().Replace("_SLIME", "") + slimeB.ToString().Replace("_SLIME", "") + "_LARGO";
            Identifiable.Id largoID = IdentifiableRegistry.CreateIdentifiableId(EnumPatcher.GetFirstFreeValue(typeof(Identifiable.Id)), name);

            SlimeDefinitions defs = GameContext.Instance.SlimeDefinitions;

            SlimeDefinition other = defs.GetSlimeByIdentifiableId(slimeB);

            if (!other.CanLargofy)
                return Identifiable.Id.NONE;

            SlimeDefinition largoDef = defs.GetLargoByBaseSlimes(definition, other);
            largoDef.IdentifiableId = largoID;

            if (!canBeTarr)
            {
                largoDef.Diet.EatMap.RemoveAll((entry) => entry.becomesId == Identifiable.Id.TARR_SLIME);
                largoDef.Diet.EatMap.RemoveAll((entry) => entry.becomesId == Identifiable.Id.GLITCH_TARR_SLIME);
            }

            extraLargoBehaviour?.Invoke(largoDef);

            SlimeTemplate largoTemplate = new SlimeTemplate(prefabName, largoDef).SetFeralState(canBeFeral).SetGlitchState(canBeAGlitch)
                .SetVacSize(Vacuumable.Size.LARGE).SetTranslation(definition.Name + " " + other.Name + " Largo").Create();

            LookupRegistry.RegisterIdentifiablePrefab(largoTemplate.ToPrefab());

            return largoID;
        }

        /// <summary>
        /// Generates a roaming Gordo Slime for this slime (use this if you want the slime to have a Gordo counter part)
        /// <para>Roaming gordos are normal slimes scaled as a gordo and contains almost no behaviours, also it doesnt eat. Use PrefabFunctions to give it
        /// behaviours</para>
        /// </summary>
        /// <param name="gordoID">The ID for the gordo</param>
        /// <returns>The slime template for the Roaming Gordo (YOU NEED TO CLASS .Create TO FINISH THE PROCESS)</returns>
        public SlimeTemplate MakeRoamingGordo(Identifiable.Id gordoID)
        {
            if (isGordo)
                return null;

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

            SlimeTemplate gordo = new SlimeTemplate("gordo" + mainObject.Name.Replace("slime", ""), gordoDef).SetVacSize(Vacuumable.Size.GIANT)
                .SetHealth(60).SetFeralState(false).SetGlitchState(false).SetFearProfile(prof).SetTranslation(definition.Name + " Gordo");

            gordo.isGordo = true;

            return gordo;
        }

        /// <summary>
        /// Generates a Gordo Slime for this slime (use this if you want the slime to have a Gordo counter part)
        /// </summary>
        /// <param name="gordoID">The ID for the gordo</param>
        /// <returns>The gordo template for the static gordo (YOU NEED TO CLASS .Create TO FINISH THE PROCESS)</returns>
        public GordoTemplate MakeStaticGordo(Identifiable.Id gordoID, Material[] gordoMaterials)
        {
            return isGordo ? null : new GordoTemplate("gordo" + mainObject.Name.Replace("slime", ""), gordoID, definition, gordoMaterials).SetTranslation(definition.Name + " Gordo");
        }

        /// <summary>
        /// Builds the module for this slime
        /// </summary>
        protected virtual void BuildModule()
        {
            GameObjectTemplate module = new GameObjectTemplate("module" + mainObject.Name);
            module.AddComponents(extras.ToArray());
            module.AddComponents(
                new Create<SlimeEat>((eat) =>
                {
                    eat.slimeDefinition = definition;
                    eat.damagePerAttack = 20;
                    eat.EatFX = EffectObjects.fxSlimeEat;
                    eat.EatFavoriteFX = EffectObjects.fxSlimeFavEat;
                    eat.ProduceFX = EffectObjects.fxSlimeProduce;
                    eat.TransformFX = EffectObjects.fxSlimeTrans;
                    eat.minDriveToEat = driveToEat;
                    eat.drivePerEat = drivePerEat;
                    eat.agitationPerEat = agitPerEat;
                    eat.agitationPerFavEat = agitPerFavEat;

                }),
                new Create<ReactToToyNearby>((react) => react.slimeDefinition = definition)
            );

            slimeModule = module.ToGameObject(null);
            definition.SlimeModules = slimeModule.Group();
        }

        /// <summary>
        /// Builds the definition for this slime
        /// </summary>
        protected virtual void RegisterDefinition()
        {
            SlimeRegistry.RegisterSlimeDefinition(definition);
        }
    }
}
