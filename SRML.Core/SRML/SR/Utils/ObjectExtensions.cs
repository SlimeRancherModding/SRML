using System.Linq;
using System.Reflection;

/// <summary>
/// Contains extension methods for <see cref="object"/>
/// </summary>
public static class ObjectExtensions
{
    private const string MIDDLE = " cannot be converted to type ";

    /// <summary>
    /// Performs a TRUE null-check.
    /// </summary>
    /// <param name="obj">An object to check.</param>
    /// <returns>Returns <see langword="true"/> if object is null, <see langword="false"/> otherwise.</returns>
    public static bool IsNull(this object obj)
    {
        return obj == null;
    }

    /// <summary>
    /// Performs a TRUE not-null-check.
    /// </summary>
    /// <param name="obj">An object to check.</param>
    /// <returns>Returns <see langword="false"/> if object is null, <see langword="true"/> otherwise.</returns>
    public static bool IsNotNull(this object obj)
    {
        return !IsNull(obj);
    }

    /// <summary>
    /// Invokes a method
    /// </summary>
    /// <param name="obj">The object you are invoking the method in</param>
    /// <param name="name">The name of the method</param>
    /// <param name="list">parameters</param>
    public static object InvokeMethod(this object obj, string name, params object[] list)
    {
        try
        {
            MethodInfo method = obj.GetType().GetInstanceMethod(name, list.Select(o => o.GetType()).ToArray());

            if (method == null) return null;

            return method?.Invoke(obj, list);
        }
        catch
        {
            // ignored
        }

        return null;
    }

    /// <summary>
    /// Invokes a method
    /// </summary>
    /// <typeparam name="T">Type of return</typeparam>
    /// <param name="obj">The object you are invoking the method in</param>
    /// <param name="name">The name of the method</param>
    /// <param name="list">parameters</param>
    public static T InvokeMethod<T>(this object obj, string name, params object[] list)
    {
        try
        {
            MethodInfo method = obj.GetType().GetInstanceMethod(name, list.Select(o => o.GetType()).ToArray());

            if (method == null) return default;

            return (T)method?.Invoke(obj, list);
        }
        catch
        {
            // ignored
        }

        return default;
    }

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
    /// Sets the value of a field
    /// </summary>
    /// <param name="obj">The object to set the field value of</param>
    /// <param name="name">The name of the field</param>
    /// <param name="value">The value to set</param>
    public static void SetField(this object obj, string name, object value)
    {
        try
        {
            FieldInfo field = obj.GetType().GetInstanceField(name);

            if (field == null) return;

            field?.SetValue(obj, value);
        }
        catch
        {
            // ignored
        }
    }

    /// <summary>
    /// Sets the value of a field
    /// </summary>
    /// <param name="obj">The object to set the field value of</param>
    /// <param name="name">The name of the field</param>
    /// <param name="value">The value to set</param>
    /// <typeparam name="T">Type of value</typeparam>
    public static void SetField<T>(this object obj, string name, T value)
    {
        try
        {
            FieldInfo field = obj.GetType().GetInstanceField(name);

            if (field == null) return;

            field?.SetValue(obj, value);
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
    /// Gets the value of a field
    /// </summary>
    /// <param name="obj">The object to get the value from</param>
    /// <param name="name">The name of the field</param>
    public static object GetField(this object obj, string name)
    {
        try
        {
            FieldInfo field = obj.GetType().GetInstanceField(name);

            if (field == null) return default;

            return field?.GetValue(obj);
        }
        catch
        {
            // ignored
        }

        return null;
    }

    /// <summary>
    /// Gets the value of a field
    /// </summary>
    /// <param name="obj">The object to get the value from</param>
    /// <param name="name">The name of the field</param>
    /// <typeparam name="T">Type of value</typeparam>
    public static T GetField<T>(this object obj, string name)
    {
        try
        {
            FieldInfo field = obj.GetType().GetInstanceField(name);

            if (field == null) return default;

            return (T)field?.GetValue(obj);
        }
        catch
        {
            // ignored
        }

        return default;
    }

    /// <summary>
    /// Sets the value of a property
    /// </summary>
    /// <param name="obj">The object to set the property value of</param>
    /// <param name="name">The name of the property</param>
    /// <param name="value">The value to set</param>
    /// <typeparam name="T">Type of value</typeparam>
    public static void SetProperty(this object obj, string name, object value)
    {
        try
        {
            PropertyInfo field = obj.GetType().GetInstanceProperty(name);

            if (field == null) return;

            if (field.CanWrite)
                field.SetValue(obj, value, null);
            else
                obj.SetField($"<{name}>k__BackingField", value);
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
    /// Sets the value of a property
    /// </summary>
    /// <param name="obj">The object to set the property value of</param>
    /// <param name="name">The name of the property</param>
    /// <param name="value">The value to set</param>
    /// <typeparam name="T">Type of value</typeparam>
    public static void SetProperty<T>(this object obj, string name, T value)
    {
        try
        {
            PropertyInfo field = obj.GetType().GetInstanceProperty(name);

            if (field == null) return;

            if (field.CanWrite)
                field.SetValue(obj, value, null);
            else
                obj.SetField<T>($"<{name}>k__BackingField", value);
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
    /// Gets the value of a property
    /// </summary>
    /// <param name="obj">The object to get the value from</param>
    /// <param name="name">The name of the property</param>
    public static object GetProperty(this object obj, string name)
    {
        try
        {
            PropertyInfo field = obj.GetType().GetInstanceProperty(name);

            if (field == null) return default;

            return field.CanRead ? field.GetValue(obj, null) : obj.GetField($"<{name}>k__BackingField");
        }
        catch
        {
            // ignored
        }

        return null;
    }


    /// <summary>
    /// Gets the value of a property
    /// </summary>
    /// <param name="obj">The object to get the value from</param>
    /// <param name="name">The name of the property</param>
    /// <typeparam name="T">Type of value</typeparam>
    public static T GetProperty<T>(this object obj, string name)
    {
        try
        {
            PropertyInfo field = obj.GetType().GetInstanceProperty(name);

            if (field == null) return default;

            return field.CanRead ? (T)field.GetValue(obj, null) : obj.GetField<T>($"<{name}>k__BackingField");
        }
        catch
        {
            // ignored
        }

        return default;
    }

    public static bool HasMethod(this object target, string methodName) => target.GetType().HasMethod(methodName);

    public static bool HasField(this object target, string fieldName) => target.GetType().HasField(fieldName);

    public static bool HasProperty(this object target, string propertyName) => target.GetType().HasProperty(propertyName);
}

/// <summary>
/// Contains extension methods for <see cref="UnityEngine.Object"/>
/// </summary>
public static class UnityObjectExtensions
{
    public static T Instantiate<T>(this T original, bool keepOriginalName = false) where T : UnityEngine.Object
    {
        T clone = UnityEngine.Object.Instantiate(original);
        if (keepOriginalName)
            clone.name = original.name;
        return clone;
    }

    public static T Instantiate<T>(this T original, UnityEngine.Transform parent, bool keepOriginalName = false) where T : UnityEngine.Object
    {
        T clone = UnityEngine.Object.Instantiate(original, parent);
        if (keepOriginalName)
            clone.name = original.name;
        return clone;
    }

    public static T Instantiate<T>(this T original, UnityEngine.Transform parent, bool worldPositionStays, bool keepOriginalName = false) where T : UnityEngine.Object
    {
        T clone = UnityEngine.Object.Instantiate(original, parent, worldPositionStays);
        if (keepOriginalName)
            clone.name = original.name;
        return clone;
    }

    public static T Instantiate<T>(this T original, UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, bool keepOriginalName = false) where T : UnityEngine.Object
    {
        T clone = UnityEngine.Object.Instantiate(original, position, rotation);
        if (keepOriginalName)
            clone.name = original.name;
        return clone;
    }

    public static T Instantiate<T>(this T original, UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, UnityEngine.Transform parent, bool keepOriginalName = false) where T : UnityEngine.Object
    {
        T clone = UnityEngine.Object.Instantiate(original, position, rotation, parent);
        if (keepOriginalName)
            clone.name = original.name;
        return clone;
    }

    public static void DontDestroyOnLoad<T>(this T target) where T : UnityEngine.Object => UnityEngine.Object.DontDestroyOnLoad(target);
    public static void Destroy<T>(this T obj) where T : UnityEngine.Object => UnityEngine.Object.Destroy(obj);
    public static void Destroy<T>(this T obj, float t) where T : UnityEngine.Object => UnityEngine.Object.Destroy(obj, t);
    public static void DestroyImmediate<T>(this T obj) where T : UnityEngine.Object => UnityEngine.Object.DestroyImmediate(obj);
    public static void DestroyImmediate<T>(this T obj, bool allowDestroyingAssets) where T : UnityEngine.Object => UnityEngine.Object.DestroyImmediate(obj, allowDestroyingAssets);

    /// <summary>
    /// Clones the Scriptable Object
    /// </summary>
    public static T CloneInstance<T>(this T obj) where T : UnityEngine.ScriptableObject
    {
        T newObj = UnityEngine.ScriptableObject.CreateInstance<T>();
        foreach (FieldInfo field in typeof(T).GetInstanceFields())
            field.SetValue(newObj, field.GetValue(obj));
        return newObj;
    }
}