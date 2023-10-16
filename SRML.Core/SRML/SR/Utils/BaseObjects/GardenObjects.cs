using System.Collections.Generic;
using UnityEngine;
using SRML.Console;

namespace SRML.SR.Utils.BaseObjects
{
	public static class GardenObjects
	{
		// The Director
		private static LookupDirector Director => GameContext.Instance.LookupDirector;

		// Garden Positions
		public readonly static ObjectTransformValues[] spawnJoints = new ObjectTransformValues[]
		{
			// NORMAL SPAWN JOINTS
			new ObjectTransformValues(new Vector3(3.40f, 0.20f, 1.26f), new Vector3(0.00f, 151.60f, 0.00f), Vector3.one * 0.25f),
			new ObjectTransformValues(new Vector3(3.68f, 0.20f, 0.17f), new Vector3(0.00f, 316.05f, 0.00f), Vector3.one * 0.25f),
			new ObjectTransformValues(new Vector3(3.41f, 0.20f, -0.95f), new Vector3(0.00f, 135.40f, 0.00f), Vector3.one * 0.25f),
			new ObjectTransformValues(new Vector3(3.45f, 0.20f, -2.00f), new Vector3(0.00f, 320.33f, 0.00f), Vector3.one * 0.25f),
			new ObjectTransformValues(new Vector3(3.60f, 0.20f, 2.52f), new Vector3(0.00f, 55.68f, 0.00f), Vector3.one * 0.25f),
			new ObjectTransformValues(new Vector3(1.19f, 0.20f, 1.33f), new Vector3(0.00f, 58.41f, 0.00f), Vector3.one * 0.25f),
			new ObjectTransformValues(new Vector3(1.52f, 0.20f, 0.09f), new Vector3(0.00f, 170.08f, 0.00f), Vector3.one * 0.25f),
			new ObjectTransformValues(new Vector3(1.13f, 0.20f, -1.31f), new Vector3(0.00f, 135.40f, 0.00f), Vector3.one * 0.25f),
			new ObjectTransformValues(new Vector3(1.30f, 0.20f, -2.75f), new Vector3(0.00f, 175.18f, 0.00f), Vector3.one * 0.25f),
			new ObjectTransformValues(new Vector3(1.42f, 0.20f, 2.47f), new Vector3(0.00f, 44.95f, 0.00f), Vector3.one * 0.25f),
			new ObjectTransformValues(new Vector3(-0.88f, 0.20f, 2.65f), new Vector3(0.00f, 187.26f, 0.00f), Vector3.one * 0.25f),
			new ObjectTransformValues(new Vector3(-0.96f, 0.20f, -2.60f), new Vector3(0.00f, 320.33f, 0.00f), Vector3.one * 0.25f),
			new ObjectTransformValues(new Vector3(-1.13f, 0.20f, -1.33f), new Vector3(0.00f, 196.70f, 0.00f), Vector3.one * 0.25f),
			new ObjectTransformValues(new Vector3(-0.74f, 0.20f, 0.00f), new Vector3(0.00f, 337.57f, 0.00f), Vector3.one * 0.25f),
			new ObjectTransformValues(new Vector3(-1.01f, 0.20f, 1.38f), new Vector3(0.00f, 24.60f, 0.00f), Vector3.one * 0.25f),
			new ObjectTransformValues(new Vector3(-3.25f, 0.20f, 1.03f), new Vector3(0.00f, 107.21f, 0.00f), Vector3.one * 0.25f),
			new ObjectTransformValues(new Vector3(-3.07f, 0.20f, 0.11f), new Vector3(0.00f, 337.57f, 0.00f), Vector3.one * 0.25f),
			new ObjectTransformValues(new Vector3(-3.32f, 0.20f, -1.35f), new Vector3(0.00f, 135.40f, 0.00f), Vector3.one * 0.25f),
			new ObjectTransformValues(new Vector3(-3.13f, 0.20f, -2.63f), new Vector3(0.00f, 320.33f, 0.00f), Vector3.one * 0.25f),
			new ObjectTransformValues(new Vector3(-3.19f, 0.20f, 2.24f), new Vector3(0.00f, 55.68f, 0.00f), Vector3.one * 0.25f),

			// DELUXE SPAWN POINTS
			new ObjectTransformValues(new Vector3(-3.51f, 0.90f, 4.39f), new Vector3(0.00f, 100.26f, 0.00f), Vector3.one * 0.25f),
			new ObjectTransformValues(new Vector3(-3.88f, 0.90f, 3.44f), new Vector3(0.00f, 19.41f, 0.00f), Vector3.one * 0.25f),
			new ObjectTransformValues(new Vector3(-4.25f, 0.90f, 3.56f), new Vector3(0.00f, 177.25f, 0.00f), Vector3.one * 0.25f),
			new ObjectTransformValues(new Vector3(-4.46f, 0.90f, 2.99f), new Vector3(0.00f, 10.87f, 0.00f), Vector3.one * 0.25f),
			new ObjectTransformValues(new Vector3(-2.93f, 0.92f, 4.63f), new Vector3(0.00f, 89.06f, 0.00f), Vector3.one * 0.25f),
			new ObjectTransformValues(new Vector3(-3.80f, 0.94f, 3.89f), new Vector3(0.00f, 37.93f, 0.00f), Vector3.one * 0.25f),
			new ObjectTransformValues(new Vector3(-3.31f, 0.92f, 3.99f), new Vector3(0.00f, 2.38f, 0.00f), Vector3.one * 0.25f),

			new ObjectTransformValues(new Vector3(3.51f, 0.90f, -4.39f), new Vector3(0.00f, 280.26f, 0.00f), Vector3.one * 0.25f),
			new ObjectTransformValues(new Vector3(3.88f, 0.90f, -3.44f), new Vector3(0.00f, 199.42f, 0.00f), Vector3.one * 0.25f),
			new ObjectTransformValues(new Vector3(4.25f, 0.90f, -3.56f), new Vector3(0.00f, 357.25f, 0.00f), Vector3.one * 0.25f),
			new ObjectTransformValues(new Vector3(4.46f, 0.90f, -2.99f), new Vector3(0.00f, 190.87f, 0.00f), Vector3.one * 0.25f),
			new ObjectTransformValues(new Vector3(2.93f, 0.92f, -4.63f), new Vector3(0.00f, 269.06f, 0.00f), Vector3.one * 0.25f),
			new ObjectTransformValues(new Vector3(3.80f, 0.94f, -3.89f), new Vector3(0.00f, 217.93f, 0.00f), Vector3.one * 0.25f),
			new ObjectTransformValues(new Vector3(3.31f, 0.92f, -3.99f), new Vector3(0.00f, 182.38f, 0.00f), Vector3.one * 0.25f)
		};

		public readonly static ObjectTransformValues[] beds = new ObjectTransformValues[]
		{
			// NORMAL BEDS
			new ObjectTransformValues(new Vector3(-3.15f, 0.00f, 0.00f), new Vector3(0.00f, 0.00f, 0.00f), Vector3.one),
			new ObjectTransformValues(new Vector3(-1.15f, 0.00f, 0.00f), new Vector3(0.00f, 180.00f, 0.00f), Vector3.one),
			new ObjectTransformValues(new Vector3(1.24f, 0.00f, 0.00f), new Vector3(0.00f, 0.00f, 0.00f), Vector3.one),
			new ObjectTransformValues(new Vector3(3.45f, 0.00f, 0.00f), new Vector3(0.00f, 180.00f, 0.00f), Vector3.one),

			// DELUXE BEDS
			new ObjectTransformValues(new Vector3(-3.80f, 0.90f, 3.80f), new Vector3(0.00f, 45.00f, 0.00f), Vector3.one),
			new ObjectTransformValues(new Vector3(3.80f, 0.90f, -3.80f), new Vector3(0.00f, 225.00f, 0.00f), Vector3.one),
		};

		public readonly static ObjectTransformValues[] bedSprouts = new ObjectTransformValues[]
		{
			// NORMAL SPROUTS
			new ObjectTransformValues(new Vector3(0.00f, 0.10f, -2.97f), new Vector3(351.24f, 0.00f, 0.00f), Vector3.one * 0.144f),
			new ObjectTransformValues(new Vector3(0.43f, 0.04f, -1.28f), new Vector3(354.85f, 6.16f, 348.38f), Vector3.one * 0.13392f),
			new ObjectTransformValues(new Vector3(-0.46f, 0.10f, 1.56f), new Vector3(347.64f, 76.44f, 357.13f), Vector3.one * 0.13392f),
			new ObjectTransformValues(new Vector3(0.33f, 0.11f, 2.42f), new Vector3(3.45f, 353.14f, 354.31f), Vector3.one * 0.1041814f),

			// DELUXE SPROUTS
			new ObjectTransformValues(new Vector3(0.00f, 0.02f, -1.23f), new Vector3(12.65f, 206.18f, 3.77f), Vector3.one * 0.144f),
			new ObjectTransformValues(new Vector3(0.27f, 0.03f, 0.10f), new Vector3(354.85f, 58.21f, 348.38f), Vector3.one * 0.1026823f),
			new ObjectTransformValues(new Vector3(0.22f, 0.03f, 1.33f), new Vector3(359.31f, 30.23f, 348.55f), Vector3.one * 0.127224f)
		};

		public readonly static ObjectTransformValues[] droneNodes = new ObjectTransformValues[]
		{
			// NORMAL NODES
			new ObjectTransformValues(new Vector3(-2.2f, -0.6f, 0), Vector3.zero, Vector3.one),
			new ObjectTransformValues(new Vector3(2.2f, -0.6f, 0), Vector3.zero, Vector3.one),
			new ObjectTransformValues(new Vector3(0, -0.6f, 2.2f), Vector3.zero, Vector3.one),
			new ObjectTransformValues(new Vector3(0, -0.6f, -2.2f), Vector3.zero, Vector3.one)
		};

		// STUFF TO BE FOUND OR CREATED
		public static Mesh dirtMesh;
		public static Material[] dirtMaterials;

		public static Mesh rockMesh;
		public static Material[] rockMaterials;

		public static Mesh deluxeDirtMesh;
		public static Material[] deluxeDirtMaterials;

		public static Mesh deluxeRockMesh;
		public static Material[] deluxeRockMaterials;

		public readonly static Dictionary<Identifiable.Id, Mesh> modelMeshs = new Dictionary<Identifiable.Id, Mesh>();
		public readonly static Dictionary<Identifiable.Id, Material[]> modelMaterials = new Dictionary<Identifiable.Id, Material[]>();

		public readonly static Dictionary<SpawnResource.Id, Mesh> modelSproutMeshs = new Dictionary<SpawnResource.Id, Mesh>();
		public readonly static Dictionary<SpawnResource.Id, Material[]> modelSproutMaterials = new Dictionary<SpawnResource.Id, Material[]>();

		public readonly static Dictionary<SpawnResource.Id, Mesh> modelTreeCols = new Dictionary<SpawnResource.Id, Mesh>();
		public readonly static Dictionary<SpawnResource.Id, Mesh> modelTreeMeshs = new Dictionary<SpawnResource.Id, Mesh>();
		public readonly static Dictionary<SpawnResource.Id, Material[]> modelTreeMaterials = new Dictionary<SpawnResource.Id, Material[]>();

		public readonly static Dictionary<SpawnResource.Id, Mesh> modelLeavesCols = new Dictionary<SpawnResource.Id, Mesh>();
		public readonly static Dictionary<SpawnResource.Id, Mesh> modelLeavesMeshs = new Dictionary<SpawnResource.Id, Mesh>();
		public readonly static Dictionary<SpawnResource.Id, Material[]> modelLeavesMaterials = new Dictionary<SpawnResource.Id, Material[]>();

		public readonly static Dictionary<SpawnResource.Id, List<ObjectTransformValues>> treeSpawnJoints = new Dictionary<SpawnResource.Id, List<ObjectTransformValues>>()
		{
			{SpawnResource.Id.POGO_TREE, new List<ObjectTransformValues>()},
			{SpawnResource.Id.PEAR_TREE, new List<ObjectTransformValues>()},
			{SpawnResource.Id.CUBERRY_TREE, new List<ObjectTransformValues>()},
			{SpawnResource.Id.LEMON_TREE, new List<ObjectTransformValues>()},
			{SpawnResource.Id.MANGO_TREE, new List<ObjectTransformValues>()}
		};

		// Populates required values
		internal static void Populate()
		{
			// Populate Single Objects
			dirtMesh = BaseObjects.originMesh["dirt_long"];
			dirtMaterials = BaseObjects.originMaterial["veggieDirtClod01"].Group();

			rockMesh = BaseObjects.originMesh["rocks_long"];
			rockMaterials = BaseObjects.originMaterial["veggieDirtClod02"].Group();

			deluxeDirtMesh = BaseObjects.originMesh["dirt_mid"];
			deluxeDirtMaterials = dirtMaterials;

			deluxeRockMesh = BaseObjects.originMesh["rocks_mid"];
			deluxeRockMaterials = rockMaterials;

			// Gets all models and materials for veggies
			modelMeshs.Add(Identifiable.Id.BEET_VEGGIE, BaseObjects.originMesh["model_heartbeet"]);
			modelMeshs.Add(Identifiable.Id.CARROT_VEGGIE, BaseObjects.originMesh["model_carrot"]);
			modelMeshs.Add(Identifiable.Id.GINGER_VEGGIE, BaseObjects.originMesh["model_ginger"]);
			modelMeshs.Add(Identifiable.Id.OCAOCA_VEGGIE, BaseObjects.originMesh["model_ocaoca"]);
			modelMeshs.Add(Identifiable.Id.ONION_VEGGIE, BaseObjects.originMesh["model_oddOnion"]);
			modelMeshs.Add(Identifiable.Id.PARSNIP_VEGGIE, BaseObjects.originMesh["model_parsnip"]);

			modelMaterials.Add(Identifiable.Id.BEET_VEGGIE, BaseObjects.originMaterial["heartbeet"].Group());
			modelMaterials.Add(Identifiable.Id.CARROT_VEGGIE, BaseObjects.originMaterial["carrot"].Group());
			modelMaterials.Add(Identifiable.Id.GINGER_VEGGIE, BaseObjects.originMaterial["ginger"].Group());
			modelMaterials.Add(Identifiable.Id.OCAOCA_VEGGIE, BaseObjects.originMaterial["ocaoca"].Group());
			modelMaterials.Add(Identifiable.Id.ONION_VEGGIE, BaseObjects.originMaterial["onion"].Group());
			modelMaterials.Add(Identifiable.Id.PARSNIP_VEGGIE, BaseObjects.originMaterial["parsnip"].Group());

			modelSproutMeshs.Add(SpawnResource.Id.BEET_PATCH, BaseObjects.originMesh["sprout_beet"]);
			modelSproutMeshs.Add(SpawnResource.Id.CARROT_PATCH, BaseObjects.originMesh["sprout_carrot"]);
			modelSproutMeshs.Add(SpawnResource.Id.OCAOCA_PATCH, BaseObjects.originMesh["sprout_ocaoca"]);
			modelSproutMeshs.Add(SpawnResource.Id.PARSNIP_PATCH, BaseObjects.originMesh["sprout_parsnip"]);

			modelSproutMaterials.Add(SpawnResource.Id.BEET_PATCH, BaseObjects.originMaterial["heartbeet NoSway"].Group());
			modelSproutMaterials.Add(SpawnResource.Id.CARROT_PATCH, BaseObjects.originMaterial["carrot NoSway"].Group());
			modelSproutMaterials.Add(SpawnResource.Id.OCAOCA_PATCH, BaseObjects.originMaterial["ocaoca NoSway"].Group());
			modelSproutMaterials.Add(SpawnResource.Id.PARSNIP_PATCH, BaseObjects.originMaterial["parsnip NoSway"].Group());

			// Gets all models and materials for fruits
			modelMeshs.Add(Identifiable.Id.CUBERRY_FRUIT, BaseObjects.originMesh["model_cuberry"]);
			modelMeshs.Add(Identifiable.Id.KOOKADOBA_FRUIT, BaseObjects.originMesh["kook4"]);
			modelMeshs.Add(Identifiable.Id.LEMON_FRUIT, BaseObjects.originMesh["model_phaseLemon"]);
			modelMeshs.Add(Identifiable.Id.MANGO_FRUIT, BaseObjects.originMesh["model_mintmango"]);
			modelMeshs.Add(Identifiable.Id.PEAR_FRUIT, BaseObjects.originMesh["model_pricklepear"]);
			modelMeshs.Add(Identifiable.Id.POGO_FRUIT, BaseObjects.originMesh["model_pogofruit"]);

			modelMaterials.Add(Identifiable.Id.CUBERRY_FRUIT, BaseObjects.originMaterial["cuberry"].Group());
			modelMaterials.Add(Identifiable.Id.KOOKADOBA_FRUIT, BaseObjects.originMaterial["kookadoba"].Group());
			modelMaterials.Add(Identifiable.Id.LEMON_FRUIT, BaseObjects.originMaterial["phaseLemon"].Group());
			modelMaterials.Add(Identifiable.Id.MANGO_FRUIT, BaseObjects.originMaterial["mintmango"].Group());
			modelMaterials.Add(Identifiable.Id.PEAR_FRUIT, BaseObjects.originMaterial["pricklepear"].Group());
			modelMaterials.Add(Identifiable.Id.POGO_FRUIT, BaseObjects.originMaterial["pogo"].Group());

			modelTreeMeshs.Add(SpawnResource.Id.CUBERRY_TREE, BaseObjects.originMesh["tree_cube"]);
			modelTreeMeshs.Add(SpawnResource.Id.LEMON_TREE, BaseObjects.originMesh["tree_pogo"]);
			modelTreeMeshs.Add(SpawnResource.Id.MANGO_TREE, BaseObjects.originMesh["tree_mango"]);
			modelTreeMeshs.Add(SpawnResource.Id.PEAR_TREE, BaseObjects.originMesh["tree_pear"]);
			modelTreeMeshs.Add(SpawnResource.Id.POGO_TREE, BaseObjects.originMesh["tree_pogo"]);

			modelTreeMaterials.Add(SpawnResource.Id.CUBERRY_TREE, BaseObjects.originMaterial["objTreeBark_cube"].Group());
			modelTreeMaterials.Add(SpawnResource.Id.LEMON_TREE, BaseObjects.originMaterial["objTreeBark_lemon_alt"].Group());
			modelTreeMaterials.Add(SpawnResource.Id.MANGO_TREE, BaseObjects.originMaterial["objTreeBark_mango"].Group());
			modelTreeMaterials.Add(SpawnResource.Id.PEAR_TREE, BaseObjects.originMaterial["objTreeBark_pear"].Group());
			modelTreeMaterials.Add(SpawnResource.Id.POGO_TREE, BaseObjects.originMaterial["objTreeBark_pogo"].Group());

			modelTreeCols.Add(SpawnResource.Id.CUBERRY_TREE, BaseObjects.originMesh["tree_cube_COL"]);
			modelTreeCols.Add(SpawnResource.Id.LEMON_TREE, BaseObjects.originMesh["tree_pogo_COL"]);
			modelTreeCols.Add(SpawnResource.Id.MANGO_TREE, BaseObjects.originMesh["tree_mango_COL"]);
			modelTreeCols.Add(SpawnResource.Id.PEAR_TREE, BaseObjects.originMesh["tree_pear_COL"]);
			modelTreeCols.Add(SpawnResource.Id.POGO_TREE, BaseObjects.originMesh["tree_pogo_COL"]);

			modelLeavesMeshs.Add(SpawnResource.Id.CUBERRY_TREE, BaseObjects.originMesh["leaves_cube"]);
			modelLeavesMeshs.Add(SpawnResource.Id.LEMON_TREE, BaseObjects.originMesh["planeTree"]);
			modelLeavesMeshs.Add(SpawnResource.Id.MANGO_TREE, BaseObjects.originMesh["leaves_mango"]);
			modelLeavesMeshs.Add(SpawnResource.Id.PEAR_TREE, BaseObjects.originMesh["leaves_pear"]);
			modelLeavesMeshs.Add(SpawnResource.Id.POGO_TREE, BaseObjects.originMesh["leaves_pogo"]);

			modelLeavesMaterials.Add(SpawnResource.Id.CUBERRY_TREE, BaseObjects.originMaterial["objTreeLeaves_cube"].Group());
			modelLeavesMaterials.Add(SpawnResource.Id.LEMON_TREE, BaseObjects.originMaterial["objTreeLeaves_lemon_alt"].Group());
			modelLeavesMaterials.Add(SpawnResource.Id.MANGO_TREE, BaseObjects.originMaterial["objTreeLeaves_mango"].Group());
			modelLeavesMaterials.Add(SpawnResource.Id.PEAR_TREE, BaseObjects.originMaterial["objTreeLeaves_pear"].Group());
			modelLeavesMaterials.Add(SpawnResource.Id.POGO_TREE, BaseObjects.originMaterial["objTreeLeaves_pogo"].Group());

			modelLeavesCols.Add(SpawnResource.Id.CUBERRY_TREE, BaseObjects.originMesh["leaves_cube_COL"]);
			modelLeavesCols.Add(SpawnResource.Id.LEMON_TREE, BaseObjects.originMesh["planeTree"]);
			modelLeavesCols.Add(SpawnResource.Id.MANGO_TREE, BaseObjects.originMesh["leaves_mango_COL"]);
			modelLeavesCols.Add(SpawnResource.Id.PEAR_TREE, BaseObjects.originMesh["leaves_pear_COL"]);
			modelLeavesCols.Add(SpawnResource.Id.POGO_TREE, BaseObjects.originMesh["leaves_pogo_COL"]);

			// Get all tree joints
			foreach (GameObject joint in Director.GetResourcePrefab(SpawnResource.Id.POGO_TREE_DLX).FindChildrenWithPartialName("SpawnJoint"))
				treeSpawnJoints[SpawnResource.Id.POGO_TREE].Add(new ObjectTransformValues(joint.transform.localPosition, joint.transform.localEulerAngles, joint.transform.localScale));

			foreach (GameObject joint in Director.GetResourcePrefab(SpawnResource.Id.CUBERRY_TREE_DLX).FindChildrenWithPartialName("SpawnJoint"))
				treeSpawnJoints[SpawnResource.Id.CUBERRY_TREE].Add(new ObjectTransformValues(joint.transform.localPosition, joint.transform.localEulerAngles, joint.transform.localScale));

			foreach (GameObject joint in Director.GetResourcePrefab(SpawnResource.Id.LEMON_TREE_DLX).FindChildrenWithPartialName("SpawnJoint"))
				treeSpawnJoints[SpawnResource.Id.LEMON_TREE].Add(new ObjectTransformValues(joint.transform.localPosition, joint.transform.localEulerAngles, joint.transform.localScale));

			foreach (GameObject joint in Director.GetResourcePrefab(SpawnResource.Id.PEAR_TREE_DLX).FindChildrenWithPartialName("SpawnJoint"))
				treeSpawnJoints[SpawnResource.Id.PEAR_TREE].Add(new ObjectTransformValues(joint.transform.localPosition, joint.transform.localEulerAngles, joint.transform.localScale));

			foreach (GameObject joint in Director.GetResourcePrefab(SpawnResource.Id.MANGO_TREE_DLX).FindChildrenWithPartialName("SpawnJoint"))
				treeSpawnJoints[SpawnResource.Id.MANGO_TREE].Add(new ObjectTransformValues(joint.transform.localPosition, joint.transform.localEulerAngles, joint.transform.localScale));
		}

		internal static void LatePopulate() { }
	}
}
