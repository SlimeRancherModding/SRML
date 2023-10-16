using System.Collections.Generic;
using MonomiPark.SlimeRancher.Regions;
using SRML.SR;
using SRML.SR.Utils.BaseObjects;
using UnityEngine;

namespace SRML.SR.Templates.Identifiables
{
    /// <summary>
    /// A template to create new plorts
    /// </summary>
    public class PlortTemplate : ModPrefab<PlortTemplate>
    {
        // Base for Identifiables
        protected Identifiable.Id ID;
        protected Vacuumable.Size vacSize = Vacuumable.Size.NORMAL;

        // The Material
        protected Material[] materials;

        // Plort scale
        protected Vector3 scale = Vector3.one * 0.3f;

        /// <summary>
        /// Template to create new plorts
        /// </summary>
        /// <param name="name">The name of the object (prefixes are recommended, but not needed)</param>
        /// <param name="ID">The Identifiable ID for this plort</param>
        /// <param name="materials">The materials that compose this plort's model</param>
        public PlortTemplate(string name, Identifiable.Id ID, Material[] materials) : base(name)
        {
            this.ID = ID;
            this.materials = materials;
        }

        /// <summary>
        /// Sets the vacuumable size
        /// </summary>
        /// <param name="vacSize">The vac size to set</param>
        public PlortTemplate SetVacSize(Vacuumable.Size vacSize)
        {
            this.vacSize = vacSize;
            return this;
        }

        /// <summary>
        /// Sets the translation for this plort's name
        /// </summary>
        /// <param name="name">The translated name</param>
        public override PlortTemplate SetTranslation(string name)
        {
            TranslationPatcher.AddActorTranslation("l." + ID.ToString().ToLower(), name);
            return this;
        }

        /// <summary>
        /// Sets the scale for the plort
        /// </summary>
        /// <param name="scale">The scale to set</param>
        public PlortTemplate SetScale(Vector3 scale)
        {
            this.scale = scale;
            return this;
        }

        /// <summary>
        /// Creates the object of the template (To get the prefab version use .ToPrefab() after calling this)
        /// </summary>
        public override PlortTemplate Create()
        {
            // Create main object
            mainObject.AddComponents(
                new Create<MeshFilter>((filter) => filter.sharedMesh = BaseObjects.originMesh["plort"]),
                new Create<MeshRenderer>((render) => render.sharedMaterials = materials),
                new Create<Identifiable>((ident) => ident.id = ID),
                new Create<Vacuumable>((vac) => vac.size = vacSize),
                new Create<Rigidbody>((body) =>
                {
                    body.drag = 0.2f;
                    body.angularDrag = 0.5f;
                    body.mass = 0.3f;
                    body.useGravity = true;
                }),
                new Create<SphereCollider>((col) =>
                {
                    col.center = Vector3.zero;
                    col.radius = 1;
                    col.sharedMaterial = BaseObjects.originPhysMaterial["Slime"];
                }),
                new Create<DragFloatReactor>((drag) => drag.floatDragMultiplier = 10),
                new Create<CollisionAggregator>(null),
                new Create<RegionMember>((rg) => rg.canHibernate = true),
                new Create<PlortInvulnerability>((plort) => plort.invulnerabilityPeriod = 3),
                new Create<PlaySoundOnHit>((hit) =>
                {
                    hit.hitCue = EffectObjects.cueHitPlort;
                    hit.minTimeBetween = 0.2f;
                    hit.minForce = 1;
                    hit.includeControllerCollisions = false;
                }),
                new Create<DestroyOnIgnite>((dest) => dest.igniteFX = EffectObjects.fxPlortDespawn),
                new Create<DestroyPlortAfterTime>((dest) =>
                {
                    dest.lifeTimeHours = 24;
                    dest.destroyFX = EffectObjects.fxPlortDespawn;
                }),
                new Create<PlortInstability>((insta) =>
                {
                    insta.lifetimeHours = 0.5f;
                    insta.explodePower = 400;
                    insta.explodeRadius = 7;
                    insta.explodeFX = EffectObjects.fxExplosion;
                })
            ).SetTransform(Vector3.zero, Vector3.zero, scale)
            .AddAfterChildren(SetShield);

            // Create model
            mainObject.AddChild(new GameObjectTemplate("Shield",
                new Create<MeshFilter>((filter) => filter.sharedMesh = BaseObjects.originMesh["plort_shell"]),
                new Create<MeshRenderer>((render) => render.sharedMaterials = BaseObjects.originMaterial["plortShield"].Group())
            ).SetTransform(Vector3.zero, Vector3.zero, Vector3.one * 1.1f));

            // Create delaunch
            mainObject.AddChild(new GameObjectTemplate("DelaunchTrigger",
                new Create<SphereCollider>((col) =>
                {
                    col.center = Vector3.zero;
                    col.radius = 0.1f;
                    col.isTrigger = true;
                }),
                new Create<VacDelaunchTrigger>(null)
            ).SetTransform(Vector3.zero, Vector3.zero, Vector3.one * 3));

            mainObject.Layer = BaseObjects.layers["Actor"];

            return this;
        }

        protected void SetShield(GameObject obj)
        {
            obj.GetComponent<PlortInvulnerability>().activateObj = obj.FindChild("Shield");
        }
    }
}
