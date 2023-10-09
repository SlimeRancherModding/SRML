using System.Collections.Generic;
using SRML.Console;
using SRML.SR.Utils.BaseObjects;
using SRML.SR.Utils.Debug;
using UnityEngine;

namespace SRML.SR.Templates.Plantables
{
    /// <summary>
    /// A template to create new veggie plantables
    /// </summary>
    public class VeggiePlantableTemplate : ModPrefab<VeggiePlantableTemplate>
    {
        // Base for plantables
        protected readonly bool isDeluxe = false;

        protected readonly Identifiable.Id ID;
        protected readonly SpawnResource.Id resID;

        // Growth values
        protected int minSpawn = 15;
        protected int maxSpawn = 20;
        protected float minHours = 18;
        protected float maxHours = 24;
        protected float minNutrient = 20;
        protected float waterHours = 23;

        // Bonus values
        protected int minBonusSelection = 4;
        protected float bonusChance = 0.01f;

        // Spawning list
        protected List<GameObject> toSpawn = new List<GameObject>();
        protected List<GameObject> bonusToSpawn = new List<GameObject>();

        // Sprouts
        protected SpawnResource.Id sproutID = SpawnResource.Id.CARROT_PATCH;
        protected Mesh sprout;
        protected Material[] sproutMaterials;

        // Actual model and materials (to display when growing)
        protected Mesh modelMesh;
        protected Material[] modelMaterials;

        // Custom joints if needed
        protected List<ObjectTransformValues> customSpawnJoints = null;

        /// <summary>
        /// Template to create new veggie plantables
        /// </summary>
        /// <param name="name">The name of the object (prefixes are recommended, but not needed)</param>
        /// <param name="isDeluxe">Is this plantable for the deluxe version of the garden?</param>
        /// <param name="ID">The ID of the identifiable spawned by this plantable</param>
        /// <param name="resID">The spawn resource id for this plantable</param>
        /// <param name="toSpawn">The list of things to spawn (null to get it from the ID provided)</param>
        public VeggiePlantableTemplate(string name, bool isDeluxe, Identifiable.Id ID, SpawnResource.Id resID, List<GameObject> toSpawn = null) : base(name)
        {
            this.isDeluxe = isDeluxe;
            this.ID = ID;
            this.resID = resID;

            if (toSpawn == null)
                this.toSpawn.Add(GameContext.Instance.LookupDirector.GetPrefab(ID));
            else
                this.toSpawn = toSpawn;
        }

        /// <summary>
        /// Sets the bonus info
        /// </summary>
        /// <param name="minBonusSelection">The min. amount selected from the bonus list</param>
        /// <param name="bonusChance">The change to select extras</param>
        public VeggiePlantableTemplate SetBonusInfo(int minBonusSelection, float bonusChance = 0.01f)
        {
            this.minBonusSelection = minBonusSelection;
            this.bonusChance = bonusChance;
            return this;
        }

        /// <summary>
        /// Sets the list of bonus spawns
        /// </summary>
        /// <param name="bonusToSpawn">The list to set</param>
        public VeggiePlantableTemplate SetBonusSpawn(List<GameObject> bonusToSpawn)
        {
            this.bonusToSpawn = bonusToSpawn;
            return this;
        }

        /// <summary>
        /// Adds a new bonus spawn to the list
        /// </summary>
        /// <param name="ID">The ID for the spawnable</param>
        public VeggiePlantableTemplate AddBonusSpawn(Identifiable.Id ID)
        {
            bonusToSpawn.Add(GameContext.Instance.LookupDirector.GetPrefab(ID));
            return this;
        }

        /// <summary>
        /// Adds a new bonus spawwn to the list
        /// </summary>
        /// <param name="obj">The game object of the identifiable to spawn</param>
        public VeggiePlantableTemplate AddBonusSpawn(GameObject obj)
        {
            bonusToSpawn.Add(obj);
            return this;
        }

        /// <summary>
        /// Sets the list of spawns
        /// </summary>
        /// <param name="toSpawn">The list to set</param>
        public VeggiePlantableTemplate SetSpawn(List<GameObject> toSpawn)
        {
            this.toSpawn = toSpawn;
            return this;
        }

        /// <summary>
        /// Adds a spawn to the list
        /// </summary>
        /// <param name="ID">The ID for the spawnable</param>
        public VeggiePlantableTemplate AddSpawn(Identifiable.Id ID)
        {
            toSpawn.Add(GameContext.Instance.LookupDirector.GetPrefab(ID));
            return this;
        }

        /// <summary>
        /// Adds a spawn to the list
        /// </summary>
        /// <param name="obj">The game object of the identifiable to spawn</param>
        public VeggiePlantableTemplate AddSpawn(GameObject obj)
        {
            toSpawn.Add(obj);
            return this;
        }

        /// <summary>
        /// Sets the sprout to be used (null can also be provided to use the default one)
        /// </summary>
        /// <param name="sprout">The sprout's mesh</param>
        /// <param name="sproutMaterials">The material for that sprout</param>
        public VeggiePlantableTemplate SetCustomSprout(Mesh sprout, Material[] sproutMaterials)
        {
            this.sprout = sprout;
            this.sproutMaterials = sproutMaterials;
            return this;
        }

        /// <summary>
        /// Sets the sprout to be used based on the SpawnResource ID (only works for those already in the game)
        /// </summary>
        /// <param name="ID">The ID to get the sprout from</param>
        public VeggiePlantableTemplate SetCustomSprout(SpawnResource.Id ID)
        {
            if (GardenObjects.modelSproutMeshs.ContainsKey(ID))
                sproutID = ID;

            return this;
        }

        /// <summary>
        /// Sets the spawn info
        /// </summary>
        /// <param name="minSpawn">Min. number of items to spawn</param>
        /// <param name="maxSpawn">Max. number of items to spawn</param>
        /// <param name="minHours">Min. hours to be ready to spawn</param>
        /// <param name="maxHours">Max. hours to be ready to spawn</param>
        /// <param name="minNutrient">Min. value of nutrients, related to growth rate</param>
        /// <param name="waterHours">The number of hours the water lasts in the soil</param>
        public VeggiePlantableTemplate SetSpawnInfo(int minSpawn, int maxSpawn, float minHours, float maxHours, float minNutrient = 20, float waterHours = 23)
        {
            this.minSpawn = minSpawn;
            this.maxSpawn = maxSpawn;
            this.minHours = minHours;
            this.maxHours = maxHours;
            this.minNutrient = minNutrient;
            this.waterHours = waterHours;
            return this;
        }

        /// <summary>
        /// Sets the model of the spawn points (used to display the growth of the items)
        /// </summary>
        /// <param name="modelMesh">The mesh for the model</param>
        /// <param name="modelMaterials">The materials for the model</param>
        public VeggiePlantableTemplate SetModel(Mesh modelMesh, Material[] modelMaterials)
        {
            this.modelMesh = modelMesh;
            this.modelMaterials = modelMaterials;
            return this;
        }

        /// <summary>
        /// Sets the spawn joints (the list needs to have 20 for non-deluxe and 34 for deluxe)
        /// </summary>
        /// <param name="spawnJoints">New spawn joints to set</param>
        public VeggiePlantableTemplate SetSpawnJoints(List<ObjectTransformValues> spawnJoints)
        {
            if (spawnJoints.Count < 20 && !isDeluxe || spawnJoints.Count < 34 && isDeluxe)
            {
                Console.Console.Instance.LogError($"Tried to register spawn joints for '<color=white>{mainObject.Name}</color>' but they are of an invalid size (20 for normal; 34 for deluxe)");
            }

            customSpawnJoints = spawnJoints;

            return this;
        }

        /// <summary>
        /// Creates the object of the template (To get the prefab version use .ToPrefab() after calling this)
        /// </summary>
        public override VeggiePlantableTemplate Create()
        {
            // Create main object
            mainObject.AddComponents(
                new Create<SpawnResource>((spawn) =>
                {
                    spawn.BonusChance = bonusChance;
                    spawn.forceDestroyLeftoversOnSpawn = false;
                    spawn.id = resID;
                    spawn.MaxActiveSpawns = 0;
                    spawn.MaxObjectsSpawned = maxSpawn;
                    spawn.MaxSpawnIntervalGameHours = maxHours;
                    spawn.MaxTotalSpawns = 0;
                    spawn.minBonusSelections = minBonusSelection;
                    spawn.MinNutrientObjectsSpawned = minNutrient;
                    spawn.MinObjectsSpawned = minSpawn;
                    spawn.MinSpawnIntervalGameHours = minHours;
                    spawn.wateringDurationHours = waterHours;
                    spawn.ObjectsToSpawn = toSpawn.ToArray();
                    spawn.BonusObjectsToSpawn = bonusToSpawn.ToArray();
                }),
                new Create<BoxCollider>((col) =>
                {
                    col.size = new Vector3(8, 0.1f, 8);
                    col.center = new Vector3(0, 0, 0.1f);
                    col.isTrigger = true;
                }),
                new Create<ScaleYOnlyMarker>((scale) => scale.doNotScaleAsReplacement = false)
            ).AddAfterChildren(GrabJoints);

            // Add spawn joints
            for (int i = 0; i < 20; i++)
            {
                mainObject.AddChild(new GameObjectTemplate($"SpawnJoint{(i + 1).ToString("00")}",
                    new Create<MeshFilter>((filter) => filter.sharedMesh = GardenObjects.modelMeshs.ContainsKey(ID) ? GardenObjects.modelMeshs[ID] : modelMesh),
                    new Create<MeshRenderer>((render) => render.sharedMaterials = GardenObjects.modelMaterials.ContainsKey(ID) ? GardenObjects.modelMaterials[ID] : modelMaterials),
                    new Create<Rigidbody>((body) =>
                    {
                        body.drag = 0;
                        body.angularDrag = 0.05f;
                        body.mass = 1;
                        body.useGravity = false;
                        body.isKinematic = true;
                    }),
                    new Create<FixedJoint>(null),
                    new Create<HideOnStart>(null)
                ).SetTransform(customSpawnJoints == null ? GardenObjects.spawnJoints[i] : customSpawnJoints[i])
                .SetDebugMarker(MarkerType.SpawnPoint)
                );
            }

            // Add beds
            GameObjectTemplate sprout = new GameObjectTemplate("Sprout",
                new Create<MeshFilter>((filter) => filter.sharedMesh = this.sprout ?? GardenObjects.modelSproutMeshs[sproutID] ?? GardenObjects.modelSproutMeshs[SpawnResource.Id.CARROT_PATCH]),
                new Create<MeshRenderer>((render) => render.sharedMaterials = sproutMaterials == null ? GardenObjects.modelSproutMaterials[sproutID] ?? GardenObjects.modelSproutMaterials[SpawnResource.Id.CARROT_PATCH] : sproutMaterials)
            );

            GameObjectTemplate dirt = new GameObjectTemplate("Dirt",
                new Create<MeshFilter>((filter) => filter.sharedMesh = GardenObjects.dirtMesh),
                new Create<MeshRenderer>((render) => render.sharedMaterials = GardenObjects.dirtMaterials)
            ).AddChild(new GameObjectTemplate("rocks_long",
                new Create<MeshFilter>((filter) => filter.sharedMesh = GardenObjects.rockMesh),
                new Create<MeshRenderer>((render) => render.sharedMaterials = GardenObjects.rockMaterials)
            )).SetTransform(Vector3.zero, Vector3.up * 90f, new Vector3(0.5f, 0.4f, 0.5f));

            GameObjectTemplate baseBeds = new GameObjectTemplate("BaseBeds");

            for (int i = 0; i < 4; i++)
            {
                (isDeluxe ? baseBeds : mainObject).AddChild(new GameObjectTemplate("Bed",
                    new Create<LODGroup>((group) =>
                    {
                        group.localReferencePoint = Vector3.one * 0.1f;
                        group.size = 8.657982f;
                        group.fadeMode = LODFadeMode.None;
                        group.animateCrossFading = false;
                    }),
                    new Create<CapsuleCollider>((col) =>
                    {
                        col.center = new Vector3(0, -0.6f, 0);
                        col.radius = 0.8f;
                        col.height = 8;
                        col.direction = 2;
                        col.contactOffset = 0.01f;
                    })
                ).SetTransform(GardenObjects.beds[i])
                .AddChild(dirt)
                .AddChild(sprout.Clone().SetTransform(GardenObjects.bedSprouts[0]))
                .AddChild(sprout.Clone().SetTransform(GardenObjects.bedSprouts[1]))
                .AddChild(sprout.Clone().SetTransform(GardenObjects.bedSprouts[2]))
                .AddChild(sprout.Clone().SetTransform(GardenObjects.bedSprouts[3]))
                );
            }

            // Add Deluxe Stuff
            if (isDeluxe)
            {
                mainObject.AddChild(baseBeds);

                // Add spawn joints
                for (int i = 20; i < 34; i++)
                {
                    mainObject.AddChild(new GameObjectTemplate($"SpawnJoint{(i + 1).ToString("00")}",
                        new Create<MeshFilter>((filter) => filter.sharedMesh = GardenObjects.modelMeshs.ContainsKey(ID) ? GardenObjects.modelMeshs[ID] : modelMesh),
                        new Create<MeshRenderer>((render) => render.sharedMaterials = GardenObjects.modelMaterials.ContainsKey(ID) ? GardenObjects.modelMaterials[ID] : modelMaterials),
                        new Create<Rigidbody>((body) =>
                        {
                            body.drag = 0;
                            body.angularDrag = 0.05f;
                            body.mass = 1;
                            body.useGravity = false;
                            body.isKinematic = true;
                        }),
                        new Create<FixedJoint>(null),
                        new Create<HideOnStart>(null)
                    ).SetTransform(customSpawnJoints == null ? GardenObjects.spawnJoints[i] : customSpawnJoints[i])
                    .SetDebugMarker(MarkerType.SpawnPoint)
                    );
                }

                // Add beds
                GameObjectTemplate dirtDeluxe = new GameObjectTemplate("Dirt",
                    new Create<MeshFilter>((filter) => filter.sharedMesh = GardenObjects.deluxeDirtMesh),
                    new Create<MeshRenderer>((render) => render.sharedMaterials = GardenObjects.deluxeDirtMaterials)
                ).AddChild(new GameObjectTemplate("rocks_long",
                    new Create<MeshFilter>((filter) => filter.sharedMesh = GardenObjects.deluxeRockMesh),
                    new Create<MeshRenderer>((render) => render.sharedMaterials = GardenObjects.deluxeRockMaterials)
                )).SetTransform(Vector3.zero, Vector3.up * 90f, new Vector3(0.5f, 0.3f, 0.5f));

                for (int i = 4; i < 6; i++)
                {
                    mainObject.AddChild(new GameObjectTemplate("Bed",
                        new Create<LODGroup>((group) =>
                        {
                            group.localReferencePoint = Vector3.one * 0.1f;
                            group.size = 8.657982f;
                            group.fadeMode = LODFadeMode.None;
                            group.animateCrossFading = false;
                        }),
                        new Create<ScaleYOnlyMarker>((scale) => scale.doNotScaleAsReplacement = false)
                    ).SetTransform(GardenObjects.beds[i])
                    .AddChild(dirtDeluxe)
                    .AddChild(sprout.Clone().SetTransform(GardenObjects.bedSprouts[4]))
                    .AddChild(sprout.Clone().SetTransform(GardenObjects.bedSprouts[5]))
                    .AddChild(sprout.Clone().SetTransform(GardenObjects.bedSprouts[6]))
                    );
                }
            }

            return this;
        }

        protected void GrabJoints(GameObject obj)
        {
            obj.GetComponent<SpawnResource>().SpawnJoints = obj.GetComponentsInChildren<FixedJoint>();
        }
    }
}
