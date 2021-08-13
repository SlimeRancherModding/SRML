using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class ComponentExtensions
{
	// MATERIAL CONTROL
	public static Material SetInfo(this Material mat, Color color, string name)
	{
		mat.SetColor("_Color", color);
		mat.name = name;
		return mat;
	}

	// OBJECT CONTROL
	public static T[] Group<T>(this T obj) where T : Object
	{
		return new[] { obj };
	}

	public static T[] Group<T>(this T obj, params T[] others) where T : Object
	{
		List<T> objs = new List<T>
		{
			obj
		};
		objs.AddRange(others);
		return objs.ToArray();
	}

	// PRIVATE FIELDS STUFF
	[System.Obsolete("Use ObjectExtensions.SetField instead.")]
	public static T SetPrivateField<T>(this T comp, string name, object value) where T : Component
	{
		comp.SetField(name, value);

		return comp;
	}

	[System.Obsolete("Use ObjectExtensions.SetProperty instead.")]
	public static T SetPrivateProperty<T>(this T comp, string name, object value) where T : Component
	{
		comp.SetProperty(name, value);

		return comp;
	}

	[System.Obsolete("Use ObjectExtensions.GetField instead.")]
	public static E GetPrivateField<E>(this Component comp, string name) => comp.GetField<E>(name);

	[System.Obsolete("Use ObjectExtensions.GetProperty instead.")]
	public static E GetPrivateProperty<E>(this Component comp, string name) => comp.GetProperty<E>(name);

	public static void CopyAllTo<T>(this T comp, T otherComp) where T : Component
	{
		foreach (FieldInfo field in comp.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
		{
			try
			{
				field.SetValue(otherComp, field.GetValue(comp));
			}
			catch { continue; }
		}

		foreach (FieldInfo field in comp.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
		{
			if (field.GetCustomAttributes(typeof(SerializeField), false).Length > 0)
			{
				try
				{
					field.SetValue(otherComp, field.GetValue(comp));
				}
				catch { continue; }
			}
		}

		foreach (PropertyInfo field in comp.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
		{
			try
			{
				field.SetValue(otherComp, field.GetValue(comp, null), null);
			}
			catch { continue; }
		}
	}


	public static T GetOrAddComponent<T>(this Component component) where T : Component
	{
		var toGet = component.gameObject.GetComponent<T>();
		if (toGet != null) return toGet;
		return component.gameObject.AddComponent<T>();
	}

	/// <summary>
	/// Is the component present in the object?
	/// </summary>
	/// <param name="obj">Object to test</param>
	/// <param name="comp">The component if found, null if not</param>
	/// <typeparam name="T">The type of component</typeparam>
	/// <returns>True if the component is found, false otherwise</returns>
	public static bool HasComponent<T>(this Component obj, out T comp) where T : Component
	{
		comp = obj.GetComponent<T>();

		return comp != null;
	}

	/// <summary>
	/// Is the component present in the object?
	/// </summary>
	/// <param name="obj">Object to test</param>
	/// <typeparam name="T">The type of component</typeparam>
	/// <returns>True if the component is found, false otherwise</returns>
	public static bool HasComponent<T>(this Component obj) where T : Component
	{
		return obj.GetComponent<T>() != null;
	}
}