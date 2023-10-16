using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using SRML.Console;
using SRML.SR.Utils.Debug;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SRML.SR.Utils.BaseObjects
{
	public static class RanchObjects
	{
		// Base Plots
		public static Mesh techActivator;
		public static Material[] techActivatorMaterials;

		public static Mesh plotBase;
		public static Mesh plotPost;
		public static Material[] plotBaseMaterials;

		public static Mesh dirtMesh;
		public static Material[] dirtMaterials;

		public static Mesh rocksMesh;
		public static Material[] rocksMaterials;

		public static Mesh plotPlane;
		public static Material[] plotPlaneMaterials;

		//public static 

		// Populates required values
		internal static void Populate()
		{
			// Base Plots meshes and materials
			techActivator = BaseObjects.originMesh["techActivator"];
			techActivatorMaterials = BaseObjects.originMaterial["RanchTech"].Group();

			plotBase = BaseObjects.originMesh["corralBase"];
			plotPost = BaseObjects.originMesh["corralPost"];
			plotBaseMaterials = BaseObjects.originMaterial["Corral"].Group();

			dirtMesh = BaseObjects.originMesh["dirt"];
			dirtMaterials = BaseObjects.originMaterial["envReefSub02"].Group();

			rocksMesh = BaseObjects.originMesh["rocks"];
			rocksMaterials = BaseObjects.originMaterial["envRocky03"].Group();

			plotPlane = BaseObjects.originMesh["Quad"];
			plotPlaneMaterials = BaseObjects.originMaterial["envReefSub02"].Group();
		}

		internal static void LatePopulate()
		{
		}
	}
}
