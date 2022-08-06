using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions
{
    public static float GetRandom(this Vector2 range) => range.GetRandom(Randoms.SHARED);

    public static float GetRandom(this Vector2 range, Randoms random) => random.GetInRange(range.x, range.y);

    public static float Lerp(this Vector2 range, float t) => Mathf.Lerp(range.x, range.y, t);

    public static bool IsNaN(this Vector3 instance) => float.IsNaN(instance.x) || float.IsNaN(instance.y) || float.IsNaN(instance.z);

    #region Set X/Y/Z/W

    // Set X

    public static Vector4 SetX(this Vector4 vector, float x)
    {
        return new Vector4(x, vector.y, vector.z, vector.w);
    }

    public static Vector3 SetX(this Vector3 vector, float x)
    {
        return new Vector3(x, vector.y, vector.z);
    }

    public static Vector2 SetX(this Vector2 vector, float x)
    {
        return new Vector2(x, vector.y);
    }

    public static void SetX(this Transform transform, float x)
    {
        transform.position = transform.position.SetX(x);
    }

    // Set Y

    public static Vector4 SetY(this Vector4 vector, float y)
    {
        return new Vector4(vector.x, y, vector.z, vector.w);
    }

    public static Vector3 SetY(this Vector3 vector, float y)
    {
        return new Vector3(vector.x, y, vector.z);
    }

    public static Vector2 SetY(this Vector2 vector, float y)
    {
        return new Vector2(vector.x, y);
    }

    public static void SetY(this Transform transform, float y)
    {
        transform.position = transform.position.SetY(y);
    }

    // Set Z

    public static Vector4 SetZ(this Vector4 vector, float z)
    {
        return new Vector4(vector.x, vector.y, z, vector.w);
    }

    public static Vector3 SetZ(this Vector3 vector, float z)
    {
        return new Vector3(vector.x, vector.y, z);
    }

    public static void SetZ(this Transform transform, float z)
    {
        transform.position = transform.position.SetZ(z);
    }

    // Set W

    public static Vector4 SetW(this Vector4 vector, float w)
    {
        return new Vector4(vector.x, vector.y, vector.z, w);
    }

    // Set XY

    public static Vector4 SetXY(this Vector4 vector, float x, float y)
    {
        return new Vector4(x, y, vector.z, vector.w);
    }

    public static Vector3 SetXY(this Vector3 vector, float x, float y)
    {
        return new Vector3(x, y, vector.z);
    }

    public static void SetXY(this Transform transform, float x, float y)
    {
        transform.position = transform.position.SetXY(x, y);
    }

    // Set XZ

    public static Vector4 SetXZ(this Vector4 vector, float x, float z)
    {
        return new Vector4(x, vector.y, z, vector.w);
    }

    public static Vector3 SetXZ(this Vector3 vector, float x, float z)
    {
        return new Vector3(x, vector.y, z);
    }

    public static void SetXZ(this Transform transform, float x, float z)
    {
        transform.position = transform.position.SetXZ(x, z);
    }

    // Set YZ

    public static Vector4 SetYZ(this Vector4 vector, float y, float z)
    {
        return new Vector4(vector.x, y, z, vector.w);
    }

    public static Vector3 SetYZ(this Vector3 vector, float y, float z)
    {
        return new Vector3(vector.x, y, z);
    }

    public static void SetYZ(this Transform transform, float y, float z)
    {
        transform.position = transform.position.SetYZ(y, z);
    }

    // Set XW

    public static Vector4 SetXW(this Vector4 vector, float x, float w)
    {
        return new Vector4(x, vector.y, vector.z, w);
    }

    // Set TW

    public static Vector4 SetYW(this Vector4 vector, float y, float w)
    {
        return new Vector4(vector.x, y, vector.z, w);
    }

    // Set ZW

    public static Vector4 SetZW(this Vector4 vector, float z, float w)
    {
        return new Vector4(vector.x, vector.y, z, w);
    }

    // Set XYW

    public static Vector4 SetXYW(this Vector4 vector, float x, float y, float w)
    {
        return new Vector4(x, y, vector.z, w);
    }

    // Set XZW

    public static Vector4 SetXZW(this Vector4 vector, float x, float z, float w)
    {
        return new Vector4(x, vector.y, z, w);
    }

    // Set YZW

    public static Vector4 SetYZW(this Vector4 vector, float y, float z, float w)
    {
        return new Vector4(vector.x, y, z, w);
    }

    //Reset

    /// <summary>
    /// Set position to Vector3.zero.
    /// </summary>
    public static void ResetPosition(this Transform transform)
    {
        transform.position = Vector3.zero;
    }


    // RectTransform 

    public static void SetPositionX(this RectTransform transform, float x)
    {
        transform.anchoredPosition = transform.anchoredPosition.SetX(x);
    }

    public static void SetPositionY(this RectTransform transform, float y)
    {
        transform.anchoredPosition = transform.anchoredPosition.SetY(y);
    }

    public static void OffsetPositionX(this RectTransform transform, float x)
    {
        transform.anchoredPosition = transform.anchoredPosition.OffsetX(x);
    }

    public static void OffsetPositionY(this RectTransform transform, float y)
    {
        transform.anchoredPosition = transform.anchoredPosition.OffsetY(y);
    }

    #endregion

    #region Set Width/Height

    //Reset

    /// <summary>
    /// Set sizeDelta to Vector2.Zero.
    /// </summary>
    public static void ResetSize(this RectTransform transform)
    {
        transform.sizeDelta = Vector2.zero;
    }

    // RectTransform 

    public static float GetWidth(this RectTransform transform)
    {
        return transform.sizeDelta.x;
    }

    public static float GetHeight(this RectTransform transform)
    {
        return transform.sizeDelta.y;
    }

    public static void SetWidth(this RectTransform transform, float width)
    {
        transform.sizeDelta = transform.sizeDelta.SetX(width);
    }

    public static void SetHeight(this RectTransform transform, float height)
    {
        transform.sizeDelta = transform.sizeDelta.SetY(height);
    }

    public static void OffsetWidth(this RectTransform transform, float width)
    {
        transform.sizeDelta = transform.sizeDelta.OffsetX(width);
    }

    public static void OffsetHeight(this RectTransform transform, float height)
    {
        transform.sizeDelta = transform.sizeDelta.OffsetY(height);
    }

    public static void SetWidthAndHeight(this RectTransform transform, float width, float height)
    {
        transform.sizeDelta = new Vector2(width, height);
    }

    #endregion


    #region Offset X/Y/Z/W

    public static Vector4 Offset(this Vector4 vector, Vector2 offset)
    {
        return new Vector4(vector.x + offset.x, vector.y + offset.y, vector.z, vector.w);
    }

    public static Vector4 Offset(this Vector4 vector, Vector3 offset)
    {
        return new Vector4(vector.x + offset.x, vector.y + offset.y, vector.z + offset.z, vector.w);
    }

    public static Vector4 Offset(this Vector4 vector, Vector4 offset)
    {
        return new Vector4(vector.x + offset.x, vector.y + offset.y, vector.z + offset.z, vector.w + offset.w);
    }

    public static Vector3 Offset(this Vector3 vector, Vector2 offset)
    {
        return new Vector3(vector.x + offset.x, vector.y + offset.y, vector.z);
    }

    public static Vector3 Offset(this Vector3 vector, Vector3 offset)
    {
        return new Vector3(vector.x + offset.x, vector.y + offset.y, vector.z + offset.z);
    }


    public static Vector4 OffsetX(this Vector4 vector, float x)
    {
        return new Vector4(vector.x + x, vector.y, vector.z, vector.w);
    }

    public static Vector3 OffsetX(this Vector3 vector, float x)
    {
        return new Vector3(vector.x + x, vector.y, vector.z);
    }

    public static Vector2 OffsetX(this Vector2 vector, float x)
    {
        return new Vector2(vector.x + x, vector.y);
    }

    public static void OffsetX(this Transform transform, float x)
    {
        transform.position = transform.position.OffsetX(x);
    }


    public static Vector4 OffsetY(this Vector4 vector, float y)
    {
        return new Vector4(vector.x, vector.y + y, vector.z, vector.w);
    }

    public static Vector3 OffsetY(this Vector3 vector, float y)
    {
        return new Vector3(vector.x, vector.y + y, vector.z);
    }

    public static Vector2 OffsetY(this Vector2 vector, float y)
    {
        return new Vector2(vector.x, vector.y + y);
    }

    public static void OffsetY(this Transform transform, float y)
    {
        transform.position = transform.position.OffsetY(y);
    }


    public static Vector4 OffsetZ(this Vector4 vector, float z)
    {
        return new Vector4(vector.x, vector.y, vector.z + z, vector.w);
    }

    public static Vector3 OffsetZ(this Vector3 vector, float z)
    {
        return new Vector3(vector.x, vector.y, vector.z + z);
    }

    public static void OffsetZ(this Transform transform, float z)
    {
        transform.position = transform.position.OffsetZ(z);
    }


    public static Vector4 OffsetW(this Vector4 vector, float w)
    {
        return new Vector4(vector.x, vector.y, vector.z, vector.w + w);
    }


    public static Vector4 OffsetXY(this Vector4 vector, float x, float y)
    {
        return new Vector4(vector.x + x, vector.y + y, vector.z, vector.w);
    }

    public static Vector3 OffsetXY(this Vector3 vector, float x, float y)
    {
        return new Vector3(vector.x + x, vector.y + y, vector.z);
    }

    public static void OffsetXY(this Transform transform, float x, float y)
    {
        transform.position = transform.position.OffsetXY(x, y);
    }

    public static Vector2 OffsetXY(this Vector2 vector, float x, float y)
    {
        return new Vector2(vector.x + x, vector.y + y);
    }


    public static Vector4 OffsetXZ(this Vector4 vector, float x, float z)
    {
        return new Vector4(vector.x + x, vector.y, vector.z + z, vector.w);
    }

    public static Vector3 OffsetXZ(this Vector3 vector, float x, float z)
    {
        return new Vector3(vector.x + x, vector.y, vector.z + z);
    }

    public static void OffsetXZ(this Transform transform, float x, float z)
    {
        transform.position = transform.position.OffsetXZ(x, z);
    }


    public static Vector4 OffsetYZ(this Vector4 vector, float y, float z)
    {
        return new Vector4(vector.x, vector.y + y, vector.z + z, vector.w);
    }

    public static Vector3 OffsetYZ(this Vector3 vector, float y, float z)
    {
        return new Vector3(vector.x, vector.y + y, vector.z + z);
    }

    public static void OffsetYZ(this Transform transform, float y, float z)
    {
        transform.position = transform.position.OffsetYZ(y, z);
    }


    public static Vector4 OffsetXW(this Vector4 vector, float x, float w)
    {
        return new Vector4(vector.x + x, vector.y, vector.z, vector.w + w);
    }
    public static Vector4 OffsetYW(this Vector4 vector, float y, float w)
    {
        return new Vector4(vector.x, vector.y + y, vector.z, vector.w + w);
    }
    public static Vector4 OffsetZW(this Vector4 vector, float z, float w)
    {
        return new Vector4(vector.x, vector.y, vector.z + z, vector.w + w);
    }

    public static Vector4 OffsetXYW(this Vector4 vector, float x, float y, float w)
    {
        return new Vector4(vector.x + x, vector.y + y, vector.z, vector.w + w);
    }
    public static Vector4 OffsetYZW(this Vector4 vector, float y, float z, float w)
    {
        return new Vector4(vector.x, vector.y + y, vector.z + z, vector.w + w);
    }
    public static Vector4 OffsetXZW(this Vector4 vector, float x, float z, float w)
    {
        return new Vector4(vector.x + x, vector.y, vector.z + z, vector.w + w);
    }

    #endregion


    #region Clamp X/Y/Z/W

    public static Vector4 ClampX(this Vector4 vector, float min, float max)
    {
        return vector.SetX(Mathf.Clamp(vector.x, min, max));
    }

    public static Vector3 ClampX(this Vector3 vector, float min, float max)
    {
        return vector.SetX(Mathf.Clamp(vector.x, min, max));
    }

    public static Vector2 ClampX(this Vector2 vector, float min, float max)
    {
        return vector.SetX(Mathf.Clamp(vector.x, min, max));
    }

    public static void ClampX(this Transform transform, float min, float max)
    {
        transform.SetX(Mathf.Clamp(transform.position.x, min, max));
    }


    public static Vector4 ClampY(this Vector4 vector, float min, float max)
    {
        return vector.SetY(Mathf.Clamp(vector.y, min, max));
    }

    public static Vector3 ClampY(this Vector3 vector, float min, float max)
    {
        return vector.SetY(Mathf.Clamp(vector.y, min, max));
    }

    public static Vector2 ClampY(this Vector2 vector, float min, float max)
    {
        return vector.SetY(Mathf.Clamp(vector.y, min, max));
    }

    public static void ClampY(this Transform transform, float min, float max)
    {
        transform.SetY(Mathf.Clamp(transform.position.y, min, max));
    }


    public static Vector4 ClampZ(this Vector4 vector, float min, float max)
    {
        return vector.SetZ(Mathf.Clamp(vector.z, min, max));
    }

    public static Vector3 ClampZ(this Vector3 vector, float min, float max)
    {
        return vector.SetZ(Mathf.Clamp(vector.z, min, max));
    }

    public static void ClampZ(this Transform transform, float min, float max)
    {
        transform.SetZ(Mathf.Clamp(transform.position.z, min, max));
    }


    public static Vector4 ClampW(this Vector4 vector, float min, float max)
    {
        return vector.SetW(Mathf.Clamp(vector.w, min, max));
    }
    #endregion


    #region Invert

    public static Vector4 Invert(this Vector4 vector)
    {
        return new Vector4(-vector.x, -vector.y, -vector.z, -vector.w);
    }

    public static Vector3 Invert(this Vector3 vector)
    {
        return new Vector3(-vector.x, -vector.y, -vector.z);
    }

    public static Vector2 Invert(this Vector2 vector)
    {
        return new Vector2(-vector.x, -vector.y);
    }


    public static Vector4 InvertX(this Vector4 vector)
    {
        return new Vector4(-vector.x, vector.y, vector.z, vector.w);
    }

    public static Vector3 InvertX(this Vector3 vector)
    {
        return new Vector3(-vector.x, vector.y, vector.z);
    }

    public static Vector2 InvertX(this Vector2 vector)
    {
        return new Vector2(-vector.x, vector.y);
    }


    public static Vector4 InvertY(this Vector4 vector)
    {
        return new Vector4(vector.x, -vector.y, vector.z, vector.w);
    }

    public static Vector3 InvertY(this Vector3 vector)
    {
        return new Vector3(vector.x, -vector.y, vector.z);
    }

    public static Vector2 InvertY(this Vector2 vector)
    {
        return new Vector2(vector.x, -vector.y);
    }


    public static Vector4 InvertZ(this Vector4 vector)
    {
        return new Vector4(vector.x, vector.y, -vector.z, vector.w);
    }

    public static Vector3 InvertZ(this Vector3 vector)
    {
        return new Vector3(vector.x, vector.y, -vector.z);
    }


    public static Vector4 InvertW(this Vector4 vector)
    {
        return new Vector4(vector.x, vector.y, vector.z, -vector.w);
    }
    #endregion


    #region Convert

    public static Vector2 ToVector2(this Vector3 vector)
    {
        return new Vector2(vector.x, vector.y);
    }

    public static Vector2 ToVector2(this Vector4 vector)
    {
        return new Vector2(vector.x, vector.y);
    }

    public static Vector3 ToVector3(this Vector2 vector)
    {
        return new Vector3(vector.x, vector.y);
    }

    public static Vector3 ToVector3(this Vector4 vector)
    {
        return new Vector3(vector.x, vector.y, vector.z);
    }

    public static Vector4 ToVector4(this Vector3 vector)
    {
        return new Vector4(vector.x, vector.y, vector.z);
    }

    public static Vector4 ToVector4(this Vector2 vector)
    {
        return new Vector4(vector.x, vector.y);
    }


    public static Vector2 ToVector2(this Vector2Int vector)
    {
        return new Vector2(vector.x, vector.y);
    }

    public static Vector3 ToVector3(this Vector3Int vector)
    {
        return new Vector3(vector.x, vector.y);
    }


    public static Vector2Int ToVector2Int(this Vector2 vector)
    {
        return new Vector2Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
    }

    public static Vector3Int ToVector3Int(this Vector3 vector)
    {
        return new Vector3Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), Mathf.RoundToInt(vector.z));
    }

    #endregion


    #region Snap

    /// <summary>
    /// Snap to grid of snapValue
    /// </summary>
    public static Vector4 SnapValue(this Vector4 val, float snapValue)
    {
        return new Vector4(
            val.x.Snap(snapValue),
            val.y.Snap(snapValue),
            val.z.Snap(snapValue),
            val.w.Snap(snapValue));
    }

    /// <summary>
    /// Snap to grid of snapValue
    /// </summary>
    public static Vector3 SnapValue(this Vector3 val, float snapValue)
    {
        return new Vector3(
            val.x.Snap(snapValue),
            val.y.Snap(snapValue),
            val.z.Snap(snapValue));
    }

    /// <summary>
    /// Snap to grid of snapValue
    /// </summary>
    public static Vector2 SnapValue(this Vector2 val, float snapValue)
    {
        return new Vector2(
            val.x.Snap(snapValue),
            val.y.Snap(snapValue));
    }

    /// <summary>
    /// Snap position to grid of snapValue
    /// </summary>
    public static void SnapPosition(this Transform transform, float snapValue)
    {
        transform.position = transform.position.SnapValue(snapValue);
    }

    /// <summary>
    /// Snap to one unit grid
    /// </summary>
    public static Vector2 SnapToOne(this Vector2 vector)
    {
        return new Vector2(Mathf.Round(vector.x), Mathf.Round(vector.y));
    }

    /// <summary>
    /// Snap to one unit grid
    /// </summary>
    public static Vector3 SnapToOne(this Vector3 vector)
    {
        return new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z));
    }

    /// <summary>
    /// Snap to one unit grid
    /// </summary>
    public static Vector4 SnapToOne(this Vector4 vector)
    {
        return new Vector4(Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z), Mathf.Round(vector.w));
    }

    #endregion


    #region Average

    public static Vector4 AverageVector(this Vector4[] vectors)
    {
        if (vectors.IsNullOrEmpty()) return Vector4.zero;

        float x = 0f, y = 0f, z = 0f, w = 0f;
        for (var i = 0; i < vectors.Length; i++)
        {
            x += vectors[i].x;
            y += vectors[i].y;
            z += vectors[i].z;
            w += vectors[i].w;
        }

        return new Vector4(x / vectors.Length, y / vectors.Length, z / vectors.Length, w / vectors.Length);
    }

    public static Vector3 AverageVector(this Vector3[] vectors)
    {
        if (vectors.IsNullOrEmpty()) return Vector3.zero;

        float x = 0f, y = 0f, z = 0f;
        for (var i = 0; i < vectors.Length; i++)
        {
            x += vectors[i].x;
            y += vectors[i].y;
            z += vectors[i].z;
        }

        return new Vector3(x / vectors.Length, y / vectors.Length, z / vectors.Length);
    }

    public static Vector2 AverageVector(this Vector2[] vectors)
    {
        if (vectors.IsNullOrEmpty()) return Vector2.zero;

        float x = 0f, y = 0f;
        for (var i = 0; i < vectors.Length; i++)
        {
            x += vectors[i].x;
            y += vectors[i].y;
        }

        return new Vector2(x / vectors.Length, y / vectors.Length);
    }

    #endregion


    #region Approximately

    public static bool Approximately(this Vector3 vector, Vector3 compared, float threshold = 0.1f)
    {
        var xDiff = Mathf.Abs(vector.x - compared.x);
        var yDiff = Mathf.Abs(vector.y - compared.y);
        var zDiff = Mathf.Abs(vector.z - compared.z);

        return xDiff <= threshold && yDiff <= threshold && zDiff <= threshold;
    }

    public static bool Approximately(this Vector2 vector, Vector2 compared, float threshold = 0.1f)
    {
        var xDiff = Mathf.Abs(vector.x - compared.x);
        var yDiff = Mathf.Abs(vector.y - compared.y);

        return xDiff <= threshold && yDiff <= threshold;
    }

    #endregion


    #region Get Closest 

    /// <summary>
    /// Finds the position closest to the given one.
    /// </summary>
    /// <param name="position">World position.</param>
    /// <param name="otherPositions">Other world positions.</param>
    /// <returns>Closest position.</returns>
    public static Vector3 GetClosest(this Vector3 position, IEnumerable<Vector3> otherPositions)
    {
        var closest = Vector3.zero;
        var shortestDistance = Mathf.Infinity;

        foreach (var otherPosition in otherPositions)
        {
            var distance = (position - otherPosition).sqrMagnitude;

            if (distance < shortestDistance)
            {
                closest = otherPosition;
                shortestDistance = distance;
            }
        }

        return closest;
    }

    public static Vector3 GetClosest(this IEnumerable<Vector3> positions, Vector3 position)
    {
        return position.GetClosest(positions);
    }

    #endregion


    #region To

    /// <summary>
    /// Get vector between source and destination
    /// </summary>
    public static Vector4 To(this Vector4 source, Vector4 destination) =>
        destination - source;

    /// <summary>
    /// Get vector between source and destination
    /// </summary>
    public static Vector3 To(this Vector3 source, Vector3 destination) =>
        destination - source;

    /// <summary>
    /// Get vector between source and destination
    /// </summary>
    public static Vector2 To(this Vector2 source, Vector2 destination) =>
        destination - source;

    /// <summary>
    /// Get vector between source and target
    /// </summary>
    public static Vector3 To(this Component source, Component target) =>
        source.transform.position.To(target.transform.position);

    /// <summary>
    /// Get vector between source and target
    /// </summary>
    public static Vector3 To(this Component source, GameObject target) =>
        source.transform.position.To(target.transform.position);

    /// <summary>
    /// Get vector between source and target
    /// </summary>
    public static Vector3 To(this GameObject source, Component target) =>
        source.transform.position.To(target.transform.position);

    /// <summary>
    /// Get vector between source and target
    /// </summary>
    public static Vector3 To(this GameObject source, GameObject target) =>
        source.transform.position.To(target.transform.position);

    #endregion


    #region SA
    /// <summary>
    /// Calculates a squared distance between current and given vectors.
    /// </summary>
    /// <param name="a">The current vector.</param>
    /// <param name="b">The given vector.</param>
    /// <returns>Returns squared distance between current and given vectors.</returns>
    public static float SqrDistance(this in Vector3 a, in Vector3 b)
    {
        var x = b.x - a.x;
        var y = b.y - a.y;
        var z = b.z - a.z;
        return ((x * x) + (y * y) + (z * z));
    }

    /// <summary>
    /// Multiplies each element in Vector3 by the given scalar.
    /// </summary>
    /// <param name="a">The current vector.</param>
    /// <param name="s">The given scalar.</param>
    /// <returns>Returns new Vector3 containing the multiplied components.</returns>
    public static Vector3 MultipliedBy(this in Vector3 a, float s)
    {
        return new Vector3(
            a.x * s,
            a.y * s,
            a.z * s);
    }

    /// <summary>
    /// Multiplies each element in Vector3 a by the corresponding element of b.
    /// </summary>
    /// <param name="a">The current vector.</param>
    /// <param name="b">The given vector.</param>
    /// <returns>Returns new Vector3 containing the multiplied components of the given vectors.</returns>
    public static Vector3 MultipliedBy(this in Vector3 a, Vector3 b)
    {
        b.x *= a.x;
        b.y *= a.y;
        b.z *= a.z;

        return b;
    }

    /// <summary>
    /// Smoothes a Vector3 that represents euler angles.
    /// </summary>
    /// <param name="current">The current Vector3 value.</param>
    /// <param name="target">The target Vector3 value.</param>
    /// <param name="velocity">A refernce Vector3 used internally.</param>
    /// <param name="smoothTime">The time to smooth, in seconds.</param>
    /// <returns>The smoothed Vector3 value.</returns>
    public static Vector3 SmoothDampEuler(this in Vector3 current, Vector3 target, ref Vector3 velocity, float smoothTime)
    {
        Vector3 v;

        v.x = Mathf.SmoothDampAngle(current.x, target.x, ref velocity.x, smoothTime);
        v.y = Mathf.SmoothDampAngle(current.y, target.y, ref velocity.y, smoothTime);
        v.z = Mathf.SmoothDampAngle(current.z, target.z, ref velocity.z, smoothTime);

        return v;
    }

    /// <summary>
    /// Calculates a squared distance between current and given vectors.
    /// </summary>
    /// <param name="a">The current vector.</param>
    /// <param name="b">The given vector.</param>
    /// <returns>Returns squared distance between current and given vectors.</returns>
    public static float SqrDistance(this in Vector2 a, in Vector2 b)
    {
        var x = b.x - a.x;
        var y = b.y - a.y;
        return ((x * x) + (y * y));
    }

    /// <summary>
    /// Multiplies each element in Vector2 by the given scalar.
    /// </summary>
    /// <param name="a">The current vector.</param>
    /// <param name="s">The given scalar.</param>
    /// <returns>Returns new Vector2 containing the multiplied components.</returns>
    public static Vector2 MultipliedBy(this in Vector2 a, float s)
    {
        return new Vector2(
            a.x * s,
            a.y * s);
    }

    /// <summary>
    /// Multiplies each element in Vector2 a by the corresponding element of b.
    /// </summary>
    /// <param name="a">The current vector.</param>
    /// <param name="b">The given vector.</param>
    /// <returns>Returns new Vector2 containing the multiplied components of the given vectors.</returns>
    public static Vector2 MultipliedBy(this in Vector2 a, Vector2 b)
    {
        b.x *= a.x;
        b.y *= a.y;

        return b;
    }
    #endregion
}