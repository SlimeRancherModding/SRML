using System.Collections.Generic;
using MonomiPark.SlimeRancher.Regions;
using SRML.SR;
using SRML.SR.Utils.BaseObjects;
using UnityEngine;

namespace SRML.SR.Templates.Identifiables
{
    /// <summary>
    /// A template to create new foods
    /// </summary>
    public class FoodTemplate : ModPrefab<FoodTemplate>
    {
        // The Type Enum
        public enum Type
        {
            FRUIT,
            VEGGIE,
            CUSTOM
        }

        // Base for Identifiables
        protected Identifiable.Id ID;
        protected Vacuumable.Size vacSize = Vacuumable.Size.NORMAL;

        // The Mesh and Materials
        protected Mesh mesh;
        protected Material[] materials;

        // Resource Cycle Stuff
        protected Material rottenMaterial;

        protected float unripeHours = 6;
        protected float ripeHours = 6;
        protected float edibleHours = 36;
        protected float rottenHours = 6;

        // Specific to the Type
        protected PediaDirector.Id pediaID;
        protected Type foodType = Type.CUSTOM;
        protected SECTR_AudioCue releaseCue = null;
        protected SECTR_AudioCue hitCue = null;

        // Component Configs
        protected Vector3 delaunchScale = Vector3.one;
        protected Vector3 modelScale = Vector3.one;
        protected bool ejectWhenMature = false;

        /// <summary>
        /// Template to create new foods
        /// </summary>
        /// <param name="name">The name of the object (prefixes are recommended, but not needed)</param>
        /// <param name="ID">The Identifiable ID for this food</param>
        /// <param name="pediaID">The pedia ID for this resource</param>
        /// <param name="type">The type of food</param>
        /// <param name="mesh">The model's mesh for this food</param>
        /// <param name="materials">The materials that compose this food's model</param>
        public FoodTemplate(string name, Identifiable.Id ID, PediaDirector.Id pediaID, Type type, Mesh mesh, Material[] materials) : base(name)
        {
            this.ID = ID;
            foodType = type;
            this.mesh = mesh;
            this.materials = materials;

            if (type != Type.CUSTOM)
            {
                releaseCue = type == Type.VEGGIE ? EffectObjects.cueVeggieRelease : EffectObjects.cueFruitRelease;
                hitCue = type == Type.VEGGIE ? EffectObjects.cueHitVeggie : EffectObjects.cueHitFruit;
                ejectWhenMature = type == Type.VEGGIE;
            }
        }

        /// <summary>
        /// Sets the vacuumable size
        /// </summary>
        /// <param name="vacSize">The vac size to set</param>
        public FoodTemplate SetVacSize(Vacuumable.Size vacSize)
        {
            this.vacSize = vacSize;
            return this;
        }

        /// <summary>
        /// Sets the info of the resource cycle
        /// </summary>
        /// <param name="unripeGameHours">The number of game hours before the food is ready to collect</param>
        /// <param name="ripeGameHours">The number of game hours the food stays in place to be riped</param>
        /// <param name="edibleGameHours">The number of game hours the food can be eaten</param>
        /// <param name="rottenGameHours">The number of game hours the food takes to rot</param>
        public FoodTemplate SetResourceInfo(float unripeGameHours, float ripeGameHours, float edibleGameHours = 36, float rottenGameHours = 6)
        {
            unripeHours = unripeGameHours;
            ripeHours = ripeGameHours;
            edibleHours = edibleGameHours;
            rottenHours = rottenGameHours;
            return this;
        }

        /// <summary>
        /// Sets the rotten material
        /// </summary>
        /// <param name="rotten">The rotten material</param>
        public FoodTemplate SetRottenMaterial(Material rotten)
        {
            rottenMaterial = rotten;
            return this;
        }

        /// <summary>
        /// Sets the Audio Cues for the food (Only needed for type CUSTOM)
        /// </summary>
        /// <param name="releaseCue">The cue triggered when the food is released from a farmable zone</param>
        /// <param name="hitCue">The cue triggered when the food hits something</param>
        public FoodTemplate SetCues(SECTR_AudioCue releaseCue, SECTR_AudioCue hitCue)
        {
            this.releaseCue = releaseCue;
            this.hitCue = hitCue;
            return this;
        }

        /// <summary>
        /// Sets the scale for the Delaunch Trigger (do not change if you don't know what you are doing)
        /// </summary>
        /// <param name="delaunchScale">The new scale to set</param>
        public FoodTemplate SetDelaunchScale(Vector3 delaunchScale)
        {
            this.delaunchScale = delaunchScale;
            return this;
        }

        /// <summary>
        /// Sets the scale for the model, only needed if the default doesn't work
        /// </summary>
        /// <param name="modelScale">The new scale to set</param>
        public FoodTemplate SetModelScale(Vector3 modelScale)
        {
            this.modelScale = modelScale;
            return this;
        }

        /// <summary>
        /// Sets the option to eject the food when mature (only needed if the food is farmable)
        /// </summary>
        /// <param name="ejectWhenMature">The new state to set</param>
        public FoodTemplate SetEjectWhenMature(bool ejectWhenMature)
        {
            this.ejectWhenMature = ejectWhenMature;
            return this;
        }

        /// <summary>
        /// Sets the translation for this slime's name
        /// </summary>
        /// <param name="name">The translated name</param>
        public override FoodTemplate SetTranslation(string name)
        {
            TranslationPatcher.AddActorTranslation("l." + ID.ToString().ToLower(), name);
            return this;
        }

        /// <summary>
        /// Creates the object of the template (To get the prefab version use .ToPrefab() after calling this)
        /// </summary>
        public override FoodTemplate Create()
        {
            // Sets up the stuff for the Pedia Entry
            PediaRegistry.RegisterIdentifiableMapping(pediaID, ID);

            // Create main object
            mainObject.AddComponents(
                new Create<MeshFilter>((filter) => filter.sharedMesh = mesh),
                new Create<Identifiable>((ident) => ident.id = ID),
                new Create<Vacuumable>((vac) => vac.size = vacSize),
                new Create<Rigidbody>((body) =>
                {
                    body.drag = 0.2f;
                    body.angularDrag = 5f;
                    body.mass = 0.3f;
                    body.useGravity = true;
                }),
                new Create<DragFloatReactor>((drag) => drag.floatDragMultiplier = 10),
                new Create<MeshCollider>((col) =>
                {
                    col.sharedMesh = mesh;
                    col.convex = true;
                }),
                new Create<CollisionAggregator>(null),
                new Create<RegionMember>((rg) => rg.canHibernate = true),
                new Create<ResourceCycle>((res) =>
                {
                    res.unripeGameHours = unripeHours;
                    res.ripeGameHours = ripeHours;
                    res.edibleGameHours = edibleHours;
                    res.rottenGameHours = rottenHours;
                    res.rottenMat = rottenMaterial;
                    res.destroyFX = EffectObjects.fxRottenDespawn;
                    res.releaseCue = releaseCue;
                    res.vacuumableWhenRipe = true;
                    res.addEjectionForce = ejectWhenMature;
                    res.releasePrepTime = 0.5f;
                }),
                new Create<SECTR_PointSource>((source) =>
                {
                    source.Loop = false;
                    source.PlayOnStart = false;
                    source.RestartLoopsOnEnabled = true;
                    source.Volume = 1;
                    source.Pitch = 1;
                }),
                new Create<PlaySoundOnHit>((hit) =>
                {
                    hit.hitCue = hitCue;
                    hit.minTimeBetween = 0.2f;
                    hit.minForce = 1;
                    hit.includeControllerCollisions = false;
                })
            ).AddAfterChildren(AddShakeTransform);

            // Create delaunch trigger
            mainObject.AddChild(new GameObjectTemplate("DelaunchTrigger",
                new Create<SphereCollider>((col) =>
                {
                    col.center = Vector3.zero;
                    col.radius = 0.1f;
                    col.isTrigger = true;
                }),
                new Create<VacDelaunchTrigger>(null)
            ).SetTransform(Vector3.zero, Vector3.zero, delaunchScale));

            // Create model
            mainObject.AddChild(new GameObjectTemplate("model_food",
                new Create<MeshFilter>((filter) => filter.sharedMesh = mesh),
                new Create<MeshRenderer>((render) =>
                {
                    render.sharedMaterials = materials;
                    render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                    render.receiveShadows = true;
                })
            ).SetTransform(Vector3.zero, Vector3.zero, modelScale));

            mainObject.Layer = BaseObjects.layers["Actor"];

            return this;
        }

        protected void AddShakeTransform(GameObject obj)
        {
            obj.GetComponent<ResourceCycle>().toShake = obj.FindChild("model_food").transform;
        }
    }
}
