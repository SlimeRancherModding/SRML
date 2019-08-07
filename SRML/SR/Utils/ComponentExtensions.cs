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
	public static T SetPrivateField<T>(this T comp, string name, object value) where T : Component
	{
		try
		{
			FieldInfo field = comp.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
			field.SetValue(comp, value);
		}
		catch { }

		return comp;
	}

	public static T SetPrivateProperty<T>(this T comp, string name, object value) where T : Component
	{
		try
		{
			PropertyInfo field = comp.GetType().GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance);
			field.SetValue(comp, value, null);
		}
		catch { }

		return comp;
	}

	public static E GetPrivateField<E>(this Component comp, string name)
	{
		try
		{
			FieldInfo field = comp.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
			return (E)field.GetValue(comp);
		}
		catch { }

		return default;
	}

	public static E GetPrivateProperty<E>(this Component comp, string name)
	{
		try
		{
			PropertyInfo field = comp.GetType().GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance);
			return (E)field.GetValue(comp, null);
		}
		catch { }

		return default;
	}

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