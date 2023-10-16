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

	public static T SetPrivateProperty<T>(this T comp, string name, object value) where T : Component
	{
		comp.SetProperty(name, value);
		return comp;
	}

	public static E GetPrivateField<E>(this Component comp, string name) => comp.GetField<E>(name);

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
}