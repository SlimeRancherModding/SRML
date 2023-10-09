using System.Collections.Generic;
using MonomiPark.SlimeRancher.Regions;
using SRML.SR;
using SRML.SR.Utils.BaseObjects;
using UnityEngine;

namespace SRML.SR.Templates.Identifiables
{
    /// <summary>
    /// A template to create new gordos
    /// </summary>
    public class GordoTemplate : ModPrefab<GordoTemplate>
    {
        // Base for Identifiables
        protected Identifiable.Id ID;

        // Base for Slimes
        protected SlimeDefinition definition;
        protected int targetCount = 30;

        protected GameObject gordoMarker;
        protected GameObject boneStructure = null;

        // Component Configs
        protected Material[] materials;
        protected readonly List<ICreateComponent> extras = new List<ICreateComponent>();

        protected readonly List<GameObject> rewards = new List<GameObject>();
        protected readonly List<GordoRewards.RewardOverride> rewardOverrides = new List<GordoRewards.RewardOverride>();

        /// <summary>
        /// Template to create new gordos
        /// </summary>
        /// <param name="name">The name of the object (prefixes are recommend, but not needed)</param>
        /// <param name="ID">The identifiable ID for this gordo</param>
        /// <param name="definition">The definition of the slime this gordo is based on</param>
        /// <param name="materials">The materials to use on the model of this gordo</param>
        public GordoTemplate(string name, Identifiable.Id ID, SlimeDefinition definition, Material[] materials) : base(name)
        {
            ID = definition.IdentifiableId;
            this.definition = definition;
            this.materials = materials;
        }

        /// <summary>
        /// Adds a new behaviour to the slime
        /// </summary>
        /// <param name="component">The component containing the new behaviour</param>
        public GordoTemplate AddBehaviour(ICreateComponent component)
        {
            extras.Add(component);
            return this;
        }

        /// <summary>
        /// Adds a new behaviour to the slime
        /// </summary>
        /// <param name="component">The component containing the new behaviour</param>
        /// <param name="comps">A list of components containing the new behaviour</param>
        public GordoTemplate AddBehaviour(ICreateComponent component, params ICreateComponent[] comps)
        {
            extras.Add(component);
            extras.AddRange(comps);
            return this;
        }

        /// <summary>
        /// Sets a new bone structure for the slime (Starting at the Vibrating part)
        /// </summary>
        /// <param name="boneStructure">The new bone structure as a game object (to add to the prefab)</param>
        public GordoTemplate SetBoneStructure(GameObject boneStructure)
        {
            this.boneStructure = boneStructure;
            return this;
        }

        /// <summary>
        /// Sets the translation for this gordo's name
        /// </summary>
        /// <param name="name">The translated name</param>
        public override GordoTemplate SetTranslation(string name)
        {
            TranslationPatcher.AddActorTranslation("l." + ID.ToString().ToLower(), name);
            return this;
        }

        /// <summary>
        /// Creates the object of the template (To get the prefab version use .ToPrefab() after calling this)
        /// </summary>
        public override GordoTemplate Create()
        {
            // Create main object
            mainObject.AddComponents(
                new Create<MeshCollider>((col) =>
                {
                    col.sharedMesh = BaseObjects.originMesh["slime_gordo_LOD1"];
                    col.convex = true;
                }),
                new Create<VacConeDetector>(null),
                new Create<GordoFaceAnimator>(null),
                new Create<GordoFaceComponents>((comp) =>
                {
                    comp.blinkEyes = BaseObjects.originMaterial["eyeBlink"];
                    comp.strainEyes = BaseObjects.originMaterial["eyeScared"];
                    comp.chompOpenMouth = BaseObjects.originMaterial["mouthElated"];
                    comp.strainMouth = BaseObjects.originMaterial["mouthStuffed"];
                    comp.happyMouth = BaseObjects.originMaterial["mouthHappy"];
                }),
                new Create<GordoEat>((eat) =>
                {
                    eat.slimeDefinition = definition;
                    eat.targetCount = targetCount;
                    eat.eatFX = EffectObjects.fxGordoEat;
                    eat.eatCue = EffectObjects.cueGordoGlup;
                    eat.destroyFX = EffectObjects.fxGordoSplat;
                    eat.growthFactor = 1.5f;
                    eat.vibrationFactor = 0.2f;
                    eat.strainCue = EffectObjects.cueGordoStrain;
                    eat.burstCue = EffectObjects.cueGordoBurst;
                    eat.EatFavoriteFX = EffectObjects.fxSlimeFavEat;
                }),
                new Create<GordoRewards>((rew) =>
                {
                    rew.rewardPrefabs = rewards.ToArray();
                    rew.rewardOverrides = rewardOverrides.ToArray();
                    rew.slimePrefab = GameContext.Instance.LookupDirector.GetPrefab(definition.IdentifiableId);
                    rew.slimeSpawnFXPrefab = EffectObjects.fxSplat;
                }),
                new Create<Animator>((anim) =>
                {
                    anim.runtimeAnimatorController = BaseObjects.originAnimators["GordoAnimator"];
                    anim.avatar = BaseObjects.originAvatars["model_slime_v3Avatar"];
                }),
                new Create<GordoIdentifiable>((gordo) => gordo.id = ID),
                new Create<AttachFashions>((fash) => fash.gordoMode = true),
                new Create<GordoDisplayOnMap>((gordo) => gordo.markerPrefab = gordoMarker.GetComponent<MapMarker>())
            );

            mainObject
                .AddAfterChildren(SetValuesAfterBuild)
                .AddComponents(extras.ToArray())
                .SetTransform(Vector3.zero, Vector3.zero, Vector3.one * 4);

            // Create eat trigger
            mainObject.AddChild(new GameObjectTemplate("EatTrigger",
                new Create<SphereCollider>((col) =>
                {
                    col.center = Vector3.zero;
                    col.radius = 0.5f;
                    col.isTrigger = true;
                }),
                new Create<GordoEatTrigger>(null)
            )).SetTransform(new Vector3(0, 0.4f, 0.4f), Vector3.zero, Vector3.one * 0.5f);

            // Create Slime Gordo
            mainObject.AddChild(new GameObjectTemplate("slime_gordo",
                new Create<SkinnedMeshRenderer>((render) =>
                {
                    render.sharedMesh = BaseObjects.originMesh["slime_gordo"];
                    render.sharedMaterials = materials;
                })
            ).SetTransform(Vector3.zero, Vector3.zero, Vector3.one * 0.3f));

            mainObject.Layer = BaseObjects.layers["ActorStatic"];

            return this;
        }

        protected void SetValuesAfterBuild(GameObject obj)
        {
            // Adds the bones
            GameObject bones = boneStructure?.CreatePrefabCopy() ?? BaseObjects.originBones["GordoBones"].CreatePrefabCopy();
            bones.transform.parent = obj.transform;
            bones.transform.localPosition = new Vector3(0, -0.5f, 0);
            bones.SetActive(true);

            bones.name = "Vibrating";

            if (bones.FindChild("EatTrigger") != null)
                Object.Destroy(bones.FindChild("EatTrigger"));

            // Add vibration object
            obj.GetComponent<GordoEat>().toVibrate = obj.FindChild("Vibrating").transform;

            // Add skinned mesh info
            SkinnedMeshRenderer render = obj.FindChild("slime_gordo").GetComponent<SkinnedMeshRenderer>();
            render.rootBone = bones.FindChild("bone_skin_top").transform;

            List<Transform> addBones = new List<Transform>();
            foreach (GameObject bone in bones.FindChildrenWithPartialName("bone_"))
                addBones.Add(bone.transform);

            render.bones = addBones.ToArray();
        }
    }
}
