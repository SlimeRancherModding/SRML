using System.Collections.Generic;
using System.Reflection;
using SRML;
using SRML.SR.Templates.Components;
using SRML.SR.Utils.BaseObjects;
using SRML.SR.Utils.Debug;
using UnityEngine;

public static class GameObjectExtensions
{
	// INITIALIZE STUFF
	public static T Initialize<T>(this T obj, System.Action<T> action) where T : Object
	{
		action(obj);
		return obj;
	}

	public static GameObject AddStartAction(this GameObject obj, string actionID)
	{
		if (obj.GetComponent<ActionOnStart>() == null)
			obj.AddComponent<ActionOnStart>().actions.Add(actionID);
		else
			obj.GetComponent<ActionOnStart>().actions.Add(actionID);

		return obj;
	}

	// DEBUG STUFF
	public static GameObject GetReadyForMarker(this GameObject obj, MarkerType type, float scaleMult = 1f)
	{
		AddDebugMarker marker = obj.AddComponent<AddDebugMarker>();
		marker.type = type;
		marker.scaleMult = scaleMult;

		return obj;
	}

	public static GameObject SetDebugMarker(this GameObject obj, MarkerType type, float scaleMult = 1f)
	{
		if (BaseObjects.markerMaterials.ContainsKey(type))
		{
			GameObject marker = new GameObject("DebugMarker");
			marker.transform.parent = obj.transform;

			MeshFilter filter = marker.AddComponent<MeshFilter>();
			filter.sharedMesh = BaseObjects.cubeMesh;

			MeshRenderer render = marker.AddComponent<MeshRenderer>();
			render.sharedMaterial = BaseObjects.markerMaterials[type];

			DebugMarker dm = marker.AddComponent<DebugMarker>();
			dm.scaleMult = scaleMult;
			dm.type = type;

			marker.transform.localPosition = Vector3.zero;
			marker.transform.localEulerAngles = Vector3.zero;
		}

		return obj;
	}

	// CHILD STUFF
	public static GameObject FindChildWithPartialName(this GameObject obj, string name, bool noDive = false)
	{
		GameObject result = null;

		foreach (Transform child in obj.transform)
		{
			if (child.name.StartsWith(name))
			{
				result = child.gameObject;
				break;
			}

			if (child.childCount > 0 && !noDive)
			{
				result = child.gameObject.FindChildWithPartialName(name);
				if (result != null)
					break;
			}
		}

		return result;
	}

	public static GameObject FindChild(this GameObject obj, string name, bool dive = false)
	{
		if (!dive)
			return obj.transform.Find(name).gameObject;
		else
		{
			GameObject result = null;

			foreach (Transform child in obj?.transform)
			{
				if (child == null)
					continue;

				if (child.name.Equals(name))
				{
					result = child.gameObject;
					break;
				}

				if (child.childCount > 0)
				{
					result = child.gameObject.FindChild(name, dive);
					if (result != null)
						break;
				}
			}

			return result;
		}
	}

	public static GameObject[] FindChildrenWithPartialName(this GameObject obj, string name, bool noDive = false)
	{
		List<GameObject> result = new List<GameObject>();

		foreach (Transform child in obj.transform)
		{
			if (child.name.StartsWith(name))
				result.Add(child.gameObject);

			if (child.childCount > 0 && !noDive)
				result.AddRange(child.gameObject.FindChildrenWithPartialName(name));
		}

		return result.ToArray();
	}

	public static GameObject[] FindChildren(this GameObject obj, string name, bool noDive = false)
	{
		List<GameObject> result = new List<GameObject>();

		foreach (Transform child in obj.transform)
		{
			if (child.name.Equals(name))
				result.Add(child.gameObject);

			if (child.childCount > 0 && !noDive)
				result.AddRange(child.gameObject.FindChildren(name));
		}

		return result.ToArray();
	}

	// PARENT STUFF
	public static T FindComponentInParent<T>(this GameObject obj) where T : Component
	{
		return obj == null ? null : obj.transform.parent?.GetComponent<T>() ?? obj.transform.parent?.gameObject.FindComponentInParent<T>();
	}

	// OBTAIN CHILD
	public static GameObject GetChildCopy(this GameObject obj, string name)
	{
		GameObject copy = obj.CreatePrefabCopy();
		GameObject child = copy.FindChild(name);
		child.SetActive(false);
		child.transform.parent = null;

		SRML.Utils.GameObjectUtils.Prefabitize(child);
		Object.Destroy(copy);

		return child;
	}

	// COPY STUFF
	public static GameObject CreatePrefabCopy(this GameObject obj)
	{
		return SRML.Utils.PrefabUtils.CopyPrefab(obj);
	}


}
