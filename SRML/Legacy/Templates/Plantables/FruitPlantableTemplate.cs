using System.Collections.Generic;
using SRML.Console;
using SRML.SR.Utils.BaseObjects;
using SRML.SR.Utils.Debug;
using UnityEngine;

namespace SRML.SR.Templates.Plantables
{
    /// <summary>
    /// A template to create new fruit plantables
    /// </summary>
    public class FruitPlantableTemplate : ModPrefab<FruitPlantableTemplate>
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

        // Bonus value
        protected int minBonusSelection = 4;
        protected float bonusChance = 0.01f;

        // Spawning list
        protected List<GameObject> toSpawn = new List<GameObject>();
        protected List<GameObject> bonusToSpawn = new List<GameObject>();

        // Tree Objects
        protected SpawnResource.Id treeID = SpawnResource.Id.POGO_TREE;
        protected Mesh treeCol;
        protected Mesh tree;
        protected Material[] treeMaterials;

        // Leaves Objects
        protected SpawnResource.Id leavesID = SpawnResource.Id.POGO_TREE;
        protected Mesh leavesCol;
        protected Mesh leaves;
        protected Material[] leavesMaterials;

        // The position for the leaves
        protected Vector3 leavesPosition = new Vector3(0, 3.2f, 0);
        protected Vector3 leavesDeluxePosition = new Vector3(0, 3.1f, 0);

        // The leave scale
        protected bool customLeavesScale = false;

        protected Vector3 leavesScale = Vector3.one;
        protected Vector3 leavesDeluxeScale = Vector3.one;

        // The tree scale
        protected bool customTreeScale = false;

        protected Vector3 treeScale = Vector3.one * 0.7f;
        protected Vector3 treeDeluxeScale = Vector3.one * 0.7f;

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
        public FruitPlantableTemplate(string name, bool isDeluxe, Identifiable.Id ID, SpawnResource.Id resID, List<GameObject> toSpawn = null) : base(name)
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
        public FruitPlantableTemplate SetBonusInfo(int minBonusSelection, float bonusChance = 0.01f)
        {
            this.minBonusSelection = minBonusSelection;
            this.bonusChance = bonusChance;
            return this;
        }

        /// <summary>
        /// Sets the list of bonus spawns
        /// </summary>
        /// <param name="bonusToSpawn">The list to set</param>
        public FruitPlantableTemplate SetBonusSpawn(List<GameObject> bonusToSpawn)
        {
            this.bonusToSpawn = bonusToSpawn;
            return this;
        }

        /// <summary>
        /// Adds a new bonus spawn to the list
        /// </summary>
        /// <param name="ID">The ID for the spawnable</param>
        public FruitPlantableTemplate AddBonusSpawn(Identifiable.Id ID)
        {
            bonusToSpawn.Add(GameContext.Instance.LookupDirector.GetPrefab(ID));
            return this;
        }

        /// <summary>
        /// Adds a new bonus spawwn to the list
        /// </summary>
        /// <param name="obj">The game object of the identifiable to spawn</param>
        public FruitPlantableTemplate AddBonusSpawn(GameObject obj)
        {
            bonusToSpawn.Add(obj);
            return this;
        }

        /// <summary>
        /// Sets the list of spawns
        /// </summary>
        /// <param name="toSpawn">The list to set</param>
        public FruitPlantableTemplate SetSpawn(List<GameObject> toSpawn)
        {
            this.toSpawn = toSpawn;
            return this;
        }

        /// <summary>
        /// Adds a spawn to the list
        /// </summary>
        /// <param name="ID">The ID for the spawnable</param>
        public FruitPlantableTemplate AddSpawn(Identifiable.Id ID)
        {
            toSpawn.Add(GameContext.Instance.LookupDirector.GetPrefab(ID));
            return this;
        }

        /// <summary>
        /// Adds a spawn to the list
        /// </summary>
        /// <param name="obj">The game object of the identifiable to spawn</param>
        public FruitPlantableTemplate AddSpawn(GameObject obj)
        {
            toSpawn.Add(obj);
            return this;
        }

        /// <summary>
        /// Sets the tree to be used
        /// </summary>
        /// <param name="tree">The tree's mesh</param>
        /// <param name="treeMaterials">The materials for that tree</param>
        /// <param name="treeCol">The tree's mesh for the collider</param>
        public FruitPlantableTemplate SetCustomTree(Mesh tree, Material[] treeMaterials, Mesh treeCol = null)
        {
            this.treeCol = treeCol ?? tree;
            this.tree = tree;
            this.treeMaterials = treeMaterials;
            return this;
        }

        /// <summary>
        /// Sets the tree to be used based on the SpawnResource ID (only works for those already in the game)
        /// </summary>
        /// <param name="ID">The ID to get the tree from</param>
        public FruitPlantableTemplate SetCustomTree(SpawnResource.Id ID)
        {
            if (GardenObjects.modelTreeMeshs.ContainsKey(ID))
                treeID = ID;
            return this;
        }

        /// <summary>
        /// Sets the leaves to be used
        /// </summary>
        /// <param name="leaves">The leaves's mesh</param>
        /// <param name="leavesMaterials">The materials for that leaves</param>
        /// <param name="leavesCol">The leaves's mesh for the collider</param>
        public FruitPlantableTemplate SetCustomLeaves(Mesh leaves, Material[] leavesMaterials, Mesh leavesCol = null)
        {
            this.leavesCol = leavesCol ?? leaves;
            this.leaves = leaves;
            this.leavesMaterials = leavesMaterials;
            return this;
        }

        /// <summary>
        /// Sets the leaves to be used based on the SpawnResource ID (only works for those already in the game)
        /// </summary>
        /// <param name="ID">The ID to get the leaves from</param>
        public FruitPlantableTemplate SetCustomLeaves(SpawnResource.Id ID)
        {
            if (GardenObjects.modelLeavesMeshs.ContainsKey(ID))
                leavesID = ID;
            return this;
        }

        /// <summary>
        /// Sets the position for the leaves (uses the same for normal and deluxe)
        /// </summary>
        /// <param name="position">The new position to set</param>
        public FruitPlantableTemplate SetLeavesPosition(Vector3 position)
        {
            SetLeavesPosition(position, position);
            return this;
        }

        /// <summary>
        /// Sets the position for the leaves (uses different ones for normal and deluxe versions)
        /// </summary>
        /// <param name="position">The new position to set for normal</param>
        /// <param name="deluxePosition">The new position to set for deluxe</param>
        public FruitPlantableTemplate SetLeavesPosition(Vector3 position, Vector3 deluxePosition)
        {
            leavesPosition = position;
            leavesDeluxePosition = deluxePosition;
            return this;
        }

        /// <summary>
        /// Sets the scale for the leaves (uses the same for normal and deluxe)
        /// </summary>
        /// <param name="scale">The new scale to set</param>
        public FruitPlantableTemplate SetLeavesScale(Vector3 scale)
        {
            SetLeavesScale(scale, scale);
            return this;
        }

        /// <summary>
        /// Sets the scale for the leaves (uses different ones for normal and deluxe versions)
        /// </summary>
        /// <param name="scale">The new scale to set for normal</param>
        /// <param name="deluxeScale">The new scale to set for deluxe</param>
        public FruitPlantableTemplate SetLeavesScale(Vector3 scale, Vector3 deluxeScale)
        {
            leavesScale = scale;
            leavesDeluxeScale = deluxeScale;
            customLeavesScale = true;
            return this;
        }

        /// <summary>
        /// Sets the scale for the tree (uses the same for normal and deluxe)
        /// </summary>
        /// <param name="scale">The new scale to set</param>
        public FruitPlantableTemplate SetTreeScale(Vector3 scale)
        {
            SetTreeScale(scale, scale);
            return this;
        }

        /// <summary>
        /// Sets the scale for the tree (uses different ones for normal and deluxe versions)
        /// </summary>
        /// <param name="scale">The new scale to set for normal</param>
        /// <param name="deluxeScale">The new scale to set for deluxe</param>
        public FruitPlantableTemplate SetTreeScale(Vector3 scale, Vector3 deluxeScale)
        {
            treeScale = scale;
            treeDeluxeScale = deluxeScale;
            customTreeScale = true;
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
        public FruitPlantableTemplate SetSpawnInfo(int minSpawn, int maxSpawn, float minHours, float maxHours, float minNutrient = 20, float waterHours = 23)
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
        public FruitPlantableTemplate SetModel(Mesh modelMesh, Material[] modelMaterials)
        {
            this.modelMesh = modelMesh;
            this.modelMaterials = modelMaterials;
            return this;
        }

        /// <summary>
        /// Sets the spawn joints (the list needs to have 20 for non-deluxe and 34 for deluxe)
        /// </summary>
        /// <param name="spawnJoints">New spawn joints to set</param>
        public FruitPlantableTemplate SetSpawnJoints(List<ObjectTransformValues> spawnJoints)
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
        public override FruitPlantableTemplate Create()
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
                })
            ).AddAfterChildren(GrabJoints).AddAfterChildren(SetNetworkNodes);

            if (!isDeluxe)
            {
                mainObject.AddComponents(
                    new Create<ScaleMarker>((scale) => scale.doNotScaleAsReplacement = false)
                );
            }

            // Add network nodes
            GameObjectTemplate[] droneNetworkNodes = new GameObjectTemplate[4];

            for (int i = 0; i < 4; i++)
            {
                droneNetworkNodes[i] = new GameObjectTemplate($"DroneNetworkNode{(i + 1).ToString("00")}",
                    new Create<PathingNetworkNode>(null)
                ).AddChild(new GameObjectTemplate("NodeLoc").SetTransform(new Vector3(0, 2, 0), Vector3.zero, Vector3.one).SetDebugMarker(MarkerType.DroneNode))
                .AddAfterChildren((obj) => obj.GetComponent<PathingNetworkNode>().nodeLoc = obj.transform.GetChild(0))
                .SetTransform(GardenObjects.droneNodes[i]);
            }

            mainObject.AddChild(new GameObjectTemplate("DroneSubnetwork", new Create<GardenDroneSubnetwork>(null))
                .AddChild(droneNetworkNodes[0])
                .AddChild(droneNetworkNodes[1])
                .AddChild(droneNetworkNodes[2])
                .AddChild(droneNetworkNodes[3])
            );

            // Add tree
            GameObjectTemplate treeObj = new GameObjectTemplate("tree_bark",
                new Create<MeshFilter>((filter) => filter.sharedMesh = tree ?? GardenObjects.modelTreeMeshs[treeID] ?? GardenObjects.modelTreeMeshs[SpawnResource.Id.POGO_TREE]),
                new Create<MeshRenderer>((render) => render.sharedMaterials = treeMaterials ?? GardenObjects.modelTreeMaterials[treeID] ?? GardenObjects.modelTreeMaterials[SpawnResource.Id.POGO_TREE]),
                new Create<MeshCollider>((col) =>
                {
                    col.sharedMesh = treeCol ?? GardenObjects.modelTreeCols[treeID] ?? GardenObjects.modelTreeCols[SpawnResource.Id.POGO_TREE];
                    col.convex = treeCol == tree;
                })
            ).SetTransform(Vector3.zero, Vector3.zero, customTreeScale ? treeScale : Vector3.one * 0.7f);

            GameObjectTemplate leavesObj = new GameObjectTemplate("leaves_tree",
                new Create<MeshFilter>((filter) => filter.sharedMesh = leaves ?? GardenObjects.modelLeavesMeshs[leavesID] ?? GardenObjects.modelLeavesMeshs[SpawnResource.Id.POGO_TREE]),
                new Create<MeshRenderer>((render) => render.sharedMaterials = leavesMaterials ?? GardenObjects.modelLeavesMaterials[leavesID] ?? GardenObjects.modelLeavesMaterials[SpawnResource.Id.POGO_TREE]),
                new Create<MeshCollider>((col) =>
                {
                    col.sharedMesh = leavesCol ?? GardenObjects.modelLeavesCols[leavesID] ?? GardenObjects.modelLeavesCols[SpawnResource.Id.POGO_TREE];
                    col.convex = leavesCol == leaves;
                })
            ).SetTransform(leavesPosition, Vector3.zero, customLeavesScale ? leavesScale : Vector3.one);

            if (!isDeluxe)
            {
                mainObject.AddChild(treeObj.AddChild(leavesObj));
            }
            else
            {
                mainObject.AddChild(treeObj.AddComponents(
                    new Create<ScaleMarker>((scale) => scale.doNotScaleAsReplacement = false)
                ).AddChild(leavesObj));
            }

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
                ).SetTransform(customSpawnJoints == null ? GardenObjects.treeSpawnJoints[leavesID][i] : customSpawnJoints[i])
                .SetDebugMarker(MarkerType.SpawnPoint)
                );
            }

            // Add Deluxe Stuff
            if (isDeluxe)
            {
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
                    ).SetTransform(customSpawnJoints == null ? GardenObjects.treeSpawnJoints[leavesID][i] : customSpawnJoints[i])
                    .SetDebugMarker(MarkerType.SpawnPoint)
                    );
                }

                // Add trees
                mainObject.AddChild(treeObj.Clone().AddComponents(
                    new Create<ScaleMarker>((scale) => scale.doNotScaleAsReplacement = false)
                ).SetTransform(new Vector3(3.8f, 0.8f, -3.8f), new Vector3(0, 225, 0), customTreeScale ? treeDeluxeScale : new Vector3(0.4f, 0.5f, 0.4f))
                .AddChild(leavesObj.Clone().SetTransform(leavesDeluxePosition, Vector3.zero, customLeavesScale ? leavesDeluxeScale : new Vector3(1.3f, 0.9f, 1.3f))));

                mainObject.AddChild(treeObj.Clone().AddComponents(
                    new Create<ScaleMarker>((scale) => scale.doNotScaleAsReplacement = false)
                ).SetTransform(new Vector3(-3.8f, 0.8f, 3.8f), new Vector3(0, 45, 0), customTreeScale ? treeDeluxeScale : new Vector3(0.4f, 0.5f, 0.4f))
                .AddChild(leavesObj.Clone().SetTransform(leavesDeluxePosition, Vector3.zero, customLeavesScale ? leavesDeluxeScale : new Vector3(1.3f, 0.9f, 1.3f))));
            }

            return this;
        }

        protected void GrabJoints(GameObject obj)
        {
            obj.GetComponent<SpawnResource>().SpawnJoints = obj.GetComponentsInChildren<FixedJoint>();
        }

        protected void SetNetworkNodes(GameObject obj)
        {
            PathingNetworkNode[] nodes = obj.GetComponentsInChildren<PathingNetworkNode>();

            if (nodes.Length > 0)
            {
                foreach (PathingNetworkNode node in nodes)
                {
                    if (node.connections == null)
                        node.connections = new List<PathingNetworkNode>();
                    else
                        node.connections.Clear();
                }

                nodes[0].connections.Add(nodes[2]);
                nodes[0].connections.Add(nodes[3]);

                nodes[1].connections.Add(nodes[2]);
                nodes[1].connections.Add(nodes[3]);

                nodes[2].connections.Add(nodes[0]);
                nodes[2].connections.Add(nodes[1]);

                nodes[3].connections.Add(nodes[0]);
                nodes[3].connections.Add(nodes[1]);
            }
        }
    }
}
