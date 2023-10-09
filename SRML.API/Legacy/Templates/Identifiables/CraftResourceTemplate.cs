using MonomiPark.SlimeRancher.Regions;
using SRML.SR;
using SRML.SR.Utils.BaseObjects;
using UnityEngine;

namespace SRML.SR.Templates.Identifiables
{
    /// <summary>
    /// A template to create new Craft Resources
    /// </summary>
    public class CraftResourceTemplate : ModPrefab<CraftResourceTemplate>
    {
        // Base for Identifiables
        protected Identifiable.Id ID;
        protected Vacuumable.Size vacSize = Vacuumable.Size.NORMAL;

        // The Mesh and Materials
        protected Mesh mesh;
        protected Material[] materials;

        // Craft Resource Stuff
        protected PediaDirector.Id pediaID;
        protected float colRadius = 0.25f;
        protected ObjectTransformValues modelTrans = new ObjectTransformValues(Vector3.up * -0.2f, Vector3.zero, Vector3.one * 0.7f);

        // Component Configs
        protected Vector3 delaunchScale = Vector3.one;

        /// <summary>
        /// Template to create new Craft Resources
        /// </summary>
        /// <param name="name">The name of the object (prefixes are recommend, but not needed)</param>
        /// <param name="ID">The Identifiable ID for this resource</param>
        /// <param name="pediaID">The pedia ID for this resource</param>
        /// <param name="mesh">The model's mesh for this resource</param>
        /// <param name="materials">The materials that compose this resource's model</param>
        public CraftResourceTemplate(string name, Identifiable.Id ID, PediaDirector.Id pediaID, Mesh mesh, Material[] materials) : base(name)
        {
            this.ID = ID;
            this.pediaID = pediaID;
            this.mesh = mesh;
            this.materials = materials;
        }

        /// <summary>
        /// Sets the vacuumable size
        /// </summary>
        /// <param name="vacSize">The vac size to set</param>
        public CraftResourceTemplate SetVacSize(Vacuumable.Size vacSize)
        {
            this.vacSize = vacSize;
            return this;
        }

        /// <summary>
        /// Sets the transform values for the model
        /// </summary>
        /// <param name="trans">New values to set</param>
        public CraftResourceTemplate SetModelTrans(ObjectTransformValues trans)
        {
            modelTrans = trans;
            return this;
        }

        /// <summary>
        /// Sets the collider radius for this craft resource (Use this to ajust the collider to the model)
        /// </summary>
        /// <param name="colRadius">The radius of the collider</param>
        public CraftResourceTemplate SetColliderRadius(float colRadius)
        {
            this.colRadius = colRadius;
            return this;
        }

        /// <summary>
        /// Sets the scale for the Delaunch Trigger (do not change if you don't know what you are doing)
        /// </summary>
        /// <param name="delaunchScale">The new scale to set</param>
        public CraftResourceTemplate SetDelaunchScale(Vector3 delaunchScale)
        {
            this.delaunchScale = delaunchScale;
            return this;
        }

        /// <summary>
        /// Sets the translation for this resource's name
        /// </summary>
        /// <param name="name">The translated name</param>
        public override CraftResourceTemplate SetTranslation(string name)
        {
            TranslationPatcher.AddActorTranslation("l." + ID.ToString().ToLower(), name);
            return this;
        }

        /// <summary>
        /// Creates the object of the template (To get the prefab version use .ToPrefab() after calling this)
        /// </summary>
        public override CraftResourceTemplate Create()
        {
            // Sets up the stuff for the Pedia Entry
            PediaRegistry.RegisterIdentifiableMapping(pediaID, ID);

            // Create main object
            mainObject.AddComponents(
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
                new Create<SphereCollider>((col) =>
                {
                    col.center = Vector3.zero;
                    col.radius = colRadius;
                }),
                new Create<CollisionAggregator>(null),
                new Create<RegionMember>((rg) => rg.canHibernate = true)
            );

            // Create model
            mainObject.AddChild(new GameObjectTemplate("resource_ld",
                new Create<MeshFilter>((filter) => filter.sharedMesh = mesh),
                new Create<MeshRenderer>((render) => render.sharedMaterials = materials)
            ).SetTransform(modelTrans));

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
    }
}
