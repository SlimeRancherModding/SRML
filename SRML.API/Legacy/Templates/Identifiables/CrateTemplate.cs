using System.Collections.Generic;
using MonomiPark.SlimeRancher.Regions;
using SRML.SR.Utils.BaseObjects;
using UnityEngine;

namespace SRML.SR.Templates.Identifiables
{
    /// <summary>
    /// A template to create new crates
    /// </summary>
    public class CrateTemplate : ModPrefab<CrateTemplate>
    {
        // Base for Identifiables
        protected Identifiable.Id ID;
        protected Vacuumable.Size vacSize = Vacuumable.Size.LARGE;

        // The Materials
        protected Material[] materials;

        // The Spawn Options
        protected List<BreakOnImpact.SpawnOption> spawnOptions = new List<BreakOnImpact.SpawnOption>();
        protected int minSpawn = 4;
        protected int maxSpawn = 6;

        /// <summary>
        /// Template to create new crates
        /// </summary>
        /// <param name="name">The name of the object (prefixes are recommended, but not needed)</param>
        /// <param name="ID">The Identifiable ID for this crate</param>
        /// <param name="materials">The materials that compose this crate's model</param>
        public CrateTemplate(string name, Identifiable.Id ID, Material[] materials = null) : base(name)
        {
            this.ID = ID;
            this.materials = materials ?? BaseObjects.originMaterial["objWoodKit01"].Group();
        }

        /// <summary>
        /// Sets the vacuumable size
        /// </summary>
        /// <param name="vacSize">The vac size to set</param>
        public CrateTemplate SetVacSize(Vacuumable.Size vacSize)
        {
            this.vacSize = vacSize;
            return this;
        }

        /// <summary>
        /// Sets the spawn options for the crate
        /// </summary>
        /// <param name="spawnOptions">List of options</param>
        public CrateTemplate SetSpawnOptions(List<BreakOnImpact.SpawnOption> spawnOptions)
        {
            this.spawnOptions = spawnOptions;
            return this;
        }

        /// <summary>
        /// Sets the info for spawning items
        /// </summary>
        /// <param name="minSpawns">The min amount of things to spawn</param>
        /// <param name="maxSpawns">The max amount of things to spawn</param>
        public CrateTemplate SetSpawnInfo(int minSpawns, int maxSpawns)
        {
            minSpawn = minSpawns;
            maxSpawn = maxSpawns;
            return this;
        }

        /// <summary>
        /// Creates the object of the template (To get the prefab version use .ToPrefab() after calling this)
        /// </summary>
        public override CrateTemplate Create()
        {
            // Create main object
            mainObject.AddComponents(
                new Create<Rigidbody>((body) =>
                {
                    body.drag = 0.2f;
                    body.angularDrag = 0.5f;
                    body.mass = 1f;
                    body.useGravity = true;
                }),
                new Create<BoxCollider>((col) =>
                {
                    col.center = Vector3.zero;
                    col.size = Vector3.one;
                }),
                new Create<CollisionAggregator>(null),
                new Create<MeshFilter>((filter) => filter.sharedMesh = BaseObjects.originMesh["objCrate"]),
                new Create<MeshRenderer>((render) => render.sharedMaterials = materials),
                new Create<Identifiable>((ident) => ident.id = ID),
                new Create<Vacuumable>((vac) => vac.size = vacSize),
                new Create<DragFloatReactor>((drag) => drag.floatDragMultiplier = 10),
                new Create<RegionMember>((rg) => rg.canHibernate = true),
                new Create<BreakOnImpact>((imp) =>
                {
                    imp.minSpawns = minSpawn;
                    imp.maxSpawns = maxSpawn;
                    imp.breakFX = EffectObjects.fxCrateBrake;
                    imp.spawnOptions = spawnOptions;
                })
            );

            mainObject.Layer = BaseObjects.layers["Actor"];

            return this;
        }
    }
}
