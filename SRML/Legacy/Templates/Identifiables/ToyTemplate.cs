using System.Collections.Generic;
using MonomiPark.SlimeRancher.Regions;
using SRML.SR;
using SRML.SR.Utils.BaseObjects;
using UnityEngine;

namespace SRML.SR.Templates.Identifiables
{
    /// <summary>
    /// A template to create new toys
    /// </summary>
    public class ToyTemplate : ModPrefab<ToyTemplate>
    {
        // Base for Identifiables
        protected Identifiable.Id ID;
        protected Vacuumable.Size vacSize = Vacuumable.Size.LARGE;

        // The Mesh and Materials
        protected Mesh mesh;
        protected Material[] materials;

        // Toy Specific
        protected SECTR_AudioCue hitCue;
        protected Identifiable.Id fashion = Identifiable.Id.NONE;

        /// <summary>
        /// Template to create new toys
        /// </summary>
        /// <param name="name">The name of the object (prefixes are recommended, but not needed)</param>
        /// <param name="ID">The Identifiable ID for this toy</param>
        /// <param name="mesh">The model's mesh for this toy</param>
        /// <param name="materials">The materials that compose this toy's model</param>
        /// <param name="hitCue">The audio cue when this toy hits something</param>
        public ToyTemplate(string name, Identifiable.Id ID, Mesh mesh, Material[] materials, SECTR_AudioCue hitCue) : base(name)
        {
            this.ID = ID;
            this.mesh = mesh;
            this.materials = materials;
            this.hitCue = hitCue;
        }

        /// <summary>
        /// Sets the vacuumable size
        /// </summary>
        /// <param name="vacSize">The vac size to set</param>
        public ToyTemplate SetVacSize(Vacuumable.Size vacSize)
        {
            this.vacSize = vacSize;
            return this;
        }

        /// <summary>
        /// Sets what fashion is required to react with this toy
        /// </summary>
        /// <param name="ID">The ID of said fashion (NONE to remove the required fashion)</param>
        public ToyTemplate SetRequiredFashion(Identifiable.Id ID)
        {
            fashion = ID;
            return this;
        }

        /// <summary>
        /// Sets the translation for this toy's name
        /// </summary>
        /// <param name="name">The translated name</param>
        public override ToyTemplate SetTranslation(string name)
        {
            TranslationPatcher.AddActorTranslation("l." + ID.ToString().ToLower(), name);
            return this;
        }

        /// <summary>
        /// Creates the object of the template (To get the prefab version use .ToPrefab() after calling this)
        /// </summary>
        public override ToyTemplate Create()
        {
            // Create main object
            mainObject.AddComponents(
                new Create<Identifiable>((ident) => ident.id = ID),
                new Create<Vacuumable>((vac) => vac.size = vacSize),
                new Create<Rigidbody>((body) =>
                {
                    body.drag = 0.2f;
                    body.angularDrag = 0.5f;
                    body.mass = 0.5f;
                    body.useGravity = true;
                }),
                new Create<SphereCollider>((col) =>
                {
                    col.center = Vector3.zero;
                    col.radius = 0.5f;
                }),
                new Create<DragFloatReactor>((drag) => drag.floatDragMultiplier = 25f),
                new Create<CollisionAggregator>(null),
                new Create<RegionMember>((rg) => rg.canHibernate = true),
                new Create<TotemLinkerHelper>(null),
                new Create<PlaySoundOnHit>((hit) =>
                {
                    hit.hitCue = hitCue;
                    hit.minTimeBetween = 0.3f;
                    hit.minForce = 0.1f;
                    hit.includeControllerCollisions = false;
                })
            ).SetTransform(Vector3.zero, Vector3.zero, Vector3.one * 1.3f);

            // Create Totem Linker
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
            ));

            // Create influence
            mainObject.AddChild(new GameObjectTemplate("Influence",
                new Create<SphereCollider>((col) =>
                {
                    col.center = Vector3.zero;
                    col.radius = 5f;
                    col.isTrigger = true;
                }),
                new Create<ToyProximityTrigger>((toy) => toy.fashion = fashion)
            ));

            // Create model
            mainObject.AddChild(new GameObjectTemplate("resource_ld",
                new Create<MeshFilter>((filter) => filter.sharedMesh = mesh),
                new Create<MeshRenderer>((render) => render.sharedMaterials = materials)
            ).SetTransform(new Vector3(0, -0.5f, 0), Vector3.zero, Vector3.one * 0.5f));

            mainObject.Layer = BaseObjects.layers["Actor"];

            return this;
        }
    }
}
