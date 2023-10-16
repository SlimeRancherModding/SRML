using System;
using System.Linq;
using System.Reflection;

public static class TypeExtensions
{
    private const string MIDDLE = " cannot be converted to type ";

    private static void LogConvert(System.ArgumentException e, string what = "field")
    {
        if (e.Message.Contains(MIDDLE))
        {
            string j = e.Message.Substring(15).Replace("'", "");
            string p = j.Substring(0, j.Length - 1);
            int i = p.IndexOf(MIDDLE);
            UnityEngine.Debug.LogWarning($"Cannot set {what} because the value ({p.Substring(0, i)}) cannot be converted to the {what} type ({p.Substring(i + MIDDLE.Length)}).");
        }
    }

    /// <summary>
    /// Invokes a static method
    /// </summary>
    /// <param name="type">Type to get method from</typeparam>
    /// <param name="name">The name of the method</param>
    /// <param name="list">parameters</param>
    public static object InvokeMethod(this Type type, string name, params object[] list)
    {
        try
        {
            MethodInfo method = type.GetStaticMethod(name, list.Select(o => o.GetType()).ToArray());

            if (method == null) return null;

            return method?.Invoke(null, list);
        }
        catch
        {
            // ignored
        }

        return null;
    }

    /// <summary>
    /// Invokes a static method
    /// </summary>
    /// <typeparam name="T">Type of return</typeparam>
    /// <param name="type">Type to get method from</typeparam>
    /// <param name="name">The name of the method</param>
    /// <param name="list">parameters</param>
    public static T InvokeMethod<T>(this Type type, string name, params object[] list)
    {
        try
        {
            MethodInfo method = type.GetStaticMethod(name, list.Select(o => o.GetType()).ToArray());

            if (method == null) return default;

            return (T)method?.Invoke(null, list);
        }
        catch
        {
            // ignored
        }

        return default;
    }

    /// <summary>
    /// Sets the value of a static field
    /// </summary>
    /// <param name="type">Type to set field value of</typeparam>
    /// <param name="name">The name of the field</param>
    /// <param name="value">The value to set</param>
    public static void SetField(this Type type, string name, object value)
    {
        try
        {
            FieldInfo field = type.GetStaticField(name);

            if (field == null) return;

            field?.SetValue(null, value);
        }
        catch (System.ArgumentException e)
        {
            LogConvert(e);
        }
        catch
        {
            // ignored
        }
    }

    /// <summary>
    /// Sets the value of a static field
    /// </summary>
    /// <typeparam name="T">Type of value</typeparam>
    /// <param name="type">Type to set field value of</typeparam>
    /// <param name="name">The name of the field</param>
    /// <param name="value">The value to set</param>
    public static void SetField<T>(this Type type, string name, T value)
    {
        try
        {
            FieldInfo field = type.GetStaticField(name);

            if (field == null) return;

            field?.SetValue(null, value);
        }
        catch (System.ArgumentException e)
        {
            LogConvert(e);
        }
        catch
        {
            // ignored
        }
    }

    /// <summary>
    /// Gets the value of a static field
    /// </summary>
    /// <param name="type">Type to get field value from</typeparam>
    /// <param name="name">The name of the field</param>
    public static object GetField(this Type type, string name)
    {
        try
        {
            FieldInfo field = type.GetStaticField(name);

            if (field == null) return default;

            return field?.GetValue(null);
        }
        catch
        {
            // ignored
        }

        return null;
    }

    /// <summary>
    /// Gets the value of a static field
    /// </summary>
    /// <typeparam name="T">Type of value</typeparam>
    /// <param name="type">Type to get field value from</typeparam>
    /// <param name="name">The name of the field</param>
    public static T GetField<T>(this Type type, string name)
    {
        try
        {
            FieldInfo field = type.GetStaticField(name);

            if (field == null) return default;

            return (T)field?.GetValue(null);
        }
        catch
        {
            // ignored
        }

        return default;
    }

    /// <summary>
    /// Sets the value of a static property
    /// </summary>
    /// <param name="type">Type to set property value of</typeparam>
    /// <param name="name">The name of the property</param>
    /// <param name="value">The value to set</param>
    public static void SetProperty(this Type type, string name, object value)
    {
        try
        {
            PropertyInfo field = type.GetStaticProperty(name);

            if (field == null) return;

            if (field.CanWrite)
                field.SetValue(null, value, null);
            else
                type.SetField($"<{name}>k__BackingField", value);
        }
        catch (System.ArgumentException e)
        {
            LogConvert(e, "property");
        }
        catch
        {
            // ignored
        }
    }

    /// <summary>
    /// Sets the value of a static property
    /// </summary>
    /// <param name="type">Type to set property value of</typeparam>
    /// <param name="name">The name of the property</param>
    /// <param name="value">The value to set</param>
    /// <typeparam name="T">Type of value</typeparam>
    public static void SetProperty<T>(this Type type, string name, T value)
    {
        try
        {
            PropertyInfo field = type.GetStaticProperty(name);

            if (field == null) return;

            if (field.CanWrite)
                field.SetValue(null, value, null);
            else
                type.SetField<T>($"<{name}>k__BackingField", value);
        }
        catch (System.ArgumentException e)
        {
            LogConvert(e, "property");
        }
        catch
        {
            // ignored
        }
    }

    /// <summary>
    /// Gets the value of a static property
    /// </summary>
    /// <param name="type">Type to get property value from</typeparam>
    /// <param name="name">The name of the property</param>
    public static object GetProperty(this Type type, string name)
    {
        try
        {
            PropertyInfo field = type.GetStaticProperty(name);

            if (field == null) return default;

            return field.CanRead ? field.GetValue(null, null) : type.GetField($"<{name}>k__BackingField");
        }
        catch
        {
            // ignored
        }

        return null;
    }

    /// <summary>
    /// Gets the value of a static property
    /// </summary>
    /// <param name="type">Type to get property value from</typeparam>
    /// <param name="name">The name of the property</param>
    /// <typeparam name="T">Type of value</typeparam>
    public static T GetProperty<T>(this Type type, string name)
    {
        try
        {
            PropertyInfo field = type.GetStaticProperty(name);

            if (field == null) return default;

            return field.CanRead ? (T)field.GetValue(null, null) : type.GetField<T>($"<{name}>k__BackingField");
        }
        catch
        {
            // ignored
        }

        return default;
    }

    public static bool Equals(this ParameterInfo[] parameters, Type[] types)
    {
        if (types == null)
            throw new ArgumentNullException(nameof(types));
        if (parameters.Length != types.Length)
            return false;
        for (int index = 0; index < types.Length; ++index)
        {
            if (types[index] == (Type)null)
                throw new ArgumentNullException(nameof(types));
            if (parameters[index].ParameterType != types[index])
                return false;
        }
        return true;
    }

    public static MethodInfo GetInstanceMethod(this Type type, string name) => type.GetInstanceMethods().FirstOrDefault(mi => mi.Name == name);
    public static MethodInfo GetInstanceMethod(this Type type, string name, Type[] parameters) => type.GetInstanceMethods().FirstOrDefault(mi => mi.Name == name && mi.GetParameters().Equals(parameters));
    public static MethodInfo[] GetInstanceMethods(this Type type) => type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
    public static MethodInfo GetStaticMethod(this Type type, string name) => type.GetStaticMethods().FirstOrDefault(mi => mi.Name == name);
    public static MethodInfo GetStaticMethod(this Type type, string name, Type[] parameters) => type.GetStaticMethods().FirstOrDefault(mi => mi.Name == name && mi.GetParameters().Equals(parameters));
    public static MethodInfo[] GetStaticMethods(this Type type) => type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);

    public static FieldInfo GetInstanceField(this Type type, string name) => type.GetInstanceFields().FirstOrDefault(fi => fi.Name == name);
    public static FieldInfo GetInstanceField<T>(this Type type, string name) => type.GetInstanceFields().FirstOrDefault(fi => fi.Name == name && fi.FieldType == typeof(T));
    public static FieldInfo[] GetInstanceFields(this Type type) => type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
    public static FieldInfo GetStaticField(this Type type, string name) => type.GetStaticFields().FirstOrDefault(fi => fi.Name == name);
    public static FieldInfo GetStaticField<T>(this Type type, string name) => type.GetStaticFields().FirstOrDefault(fi => fi.Name == name && fi.FieldType == typeof(T));
    public static FieldInfo[] GetStaticFields(this Type type) => type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);

    public static PropertyInfo GetInstanceProperty(this Type type, string name) => type.GetInstanceProperties().FirstOrDefault(pi => pi.Name == name);
    public static PropertyInfo GetInstanceProperty<T>(this Type type, string name) => type.GetInstanceProperties().FirstOrDefault(pi => pi.Name == name && pi.PropertyType == typeof(T));
    public static PropertyInfo[] GetInstanceProperties(this Type type) => type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
    public static PropertyInfo GetStaticProperty(this Type type, string name) => type.GetStaticProperties().FirstOrDefault(pi => pi.Name == name);
    public static PropertyInfo GetStaticProperty<T>(this Type type, string name) => type.GetStaticProperties().FirstOrDefault(pi => pi.Name == name && pi.PropertyType == typeof(T));
    public static PropertyInfo[] GetStaticProperties(this Type type) => type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);

    public static bool HasMethod<T>(this Type target, string methodName)
    {
        return target.GetInstanceMethod(methodName) != null || target.GetStaticMethod(methodName) != null;
    }

    public static bool HasField<T>(this Type target, string fieldName)
    {
        return target.GetInstanceField(fieldName) != null || target.GetStaticField(fieldName) != null;
    }

    public static bool HasProperty<T>(this Type target, string propertyName)
    {
        return target.GetInstanceProperty(propertyName) != null || target.GetStaticProperty(propertyName) != null;
    }
}