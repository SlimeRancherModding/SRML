using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TransformExtensions
{
    public static void SetDefaultScale(this RectTransform trans)
    {
        trans.localScale = new Vector3(1, 1, 1);
    }
    public static void SetPivotAndAnchors(this RectTransform trans, Vector2 aVec)
    {
        trans.pivot = aVec;
        trans.anchorMin = aVec;
        trans.anchorMax = aVec;
    }

    public static Vector2 GetSize(this RectTransform trans)
    {
        return trans.rect.size;
    }
    public static float GetWidth(this RectTransform trans)
    {
        return trans.rect.width;
    }
    public static float GetHeight(this RectTransform trans)
    {
        return trans.rect.height;
    }

    public static void SetPositionOfPivot(this RectTransform trans, Vector2 newPos)
    {
        trans.localPosition = new Vector3(newPos.x, newPos.y, trans.localPosition.z);
    }

    public static void SetLeftBottomPosition(this RectTransform trans, Vector2 newPos)
    {
        trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
    }
    public static void SetLeftTopPosition(this RectTransform trans, Vector2 newPos)
    {
        trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
    }
    public static void SetRightBottomPosition(this RectTransform trans, Vector2 newPos)
    {
        trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
    }
    public static void SetRightTopPosition(this RectTransform trans, Vector2 newPos)
    {
        trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
    }

    public static void SetSize(this RectTransform trans, Vector2 newSize)
    {
        Vector2 oldSize = trans.rect.size;
        Vector2 deltaSize = newSize - oldSize;
        trans.offsetMin = trans.offsetMin - new Vector2(deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
        trans.offsetMax = trans.offsetMax + new Vector2(deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y));
    }
    public static void SetWidth(this RectTransform trans, float newSize)
    {
        SetSize(trans, new Vector2(newSize, trans.rect.size.y));
    }
    public static void SetHeight(this RectTransform trans, float newSize)
    {
        SetSize(trans, new Vector2(trans.rect.size.x, newSize));
    }

    public static string GetPath(this Transform current)
    {
        if (current.parent == null)
            return "/" + current.name;
        return current.parent.GetPath() + "/" + current.name;
    }

    /// <summary>
    /// Resets `anchorMin`, `anchorMax`, `offsetMin`, `offsetMax` to `Vector2.zero`.
    /// </summary>
    /// <param name="rectTransform">RectTransform to operate with.</param>
    public static void Reset(this RectTransform rectTransform)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    /// <summary>
    /// Get's the screen rect of provided RectTransform.
    /// </summary>
    /// <param name="rectTransform">RectTransform to operate with.</param>
    /// <returns>Screen rect.</returns>
    public static Rect GetScreenRect(this RectTransform rectTransform)
    {
        var rtCorners = new Vector3[4];
        rectTransform.GetWorldCorners(rtCorners);
        var rtRect = new Rect(new Vector2(rtCorners[0].x, rtCorners[0].y), new Vector2(rtCorners[3].x - rtCorners[0].x, rtCorners[1].y - rtCorners[0].y));

        var canvas = rectTransform.GetComponentInParent<Canvas>();
        var canvasCorners = new Vector3[4];
        canvas.GetComponent<RectTransform>().GetWorldCorners(canvasCorners);
        var cRect = new Rect(new Vector2(canvasCorners[0].x, canvasCorners[0].y), new Vector2(canvasCorners[3].x - canvasCorners[0].x, canvasCorners[1].y - canvasCorners[0].y));

        var screenWidth = Screen.width;
        var screenHeight = Screen.height;

        var size = new Vector2(screenWidth / cRect.size.x * rtRect.size.x, screenHeight / cRect.size.y * rtRect.size.y);
        var rect = new Rect(screenWidth * ((rtRect.x - cRect.x) / cRect.size.x), screenHeight * ((-cRect.y + rtRect.y) / cRect.size.y), size.x, size.y);
        return rect;
    }

    /// <summary>
    /// Method to get Rect related to ScreenSpace, from given RectTransform.
    /// This will give the real position of this Rect on screen.
    /// </summary>
    /// <param name="transform">Original RectTransform of some object</param>
    /// <returns>New Rect instance.</returns>
    public static Rect ToScreenSpace(this RectTransform transform)
    {
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        Rect rect = new Rect(transform.position.x, Screen.height - transform.position.y, size.x, size.y);
        rect.x -= (transform.pivot.x * size.x);
        rect.y -= ((1.0f - transform.pivot.y) * size.y);
        return rect;
    }


    /// <summary>
    /// Sets <see cref="Transform.lossyScale"/> value.
    /// </summary>
    /// <param name="transform">Transform component.</param>
    /// <param name="lossyScale">New lossyScale value.</param>
    public static void SetLossyScale(this Transform transform, Vector3 lossyScale)
    {
        transform.localScale = Vector3.one;
        var currentLossyScale = transform.lossyScale;
        transform.localScale = new Vector3(lossyScale.x / currentLossyScale.x, lossyScale.y / currentLossyScale.y, lossyScale.z / currentLossyScale.z);
    }

    /// <summary>
    /// Reset <see cref="Transform"/> component position, scale and rotation.
    /// </summary>
    /// <param name="transform">Transform component.</param>
    public static void Reset(this Transform transform)
    {
        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// Removes all transform children.
    /// </summary>
    /// <param name="transform">Transform component.</param>
    /// <param name="activeOnly">Will ignore disabled game-objects when set to <c>true</c>. </param>
    /// <returns></returns>
    public static Transform Clear(this Transform transform, bool activeOnly = false)
    {
        if (transform.childCount == 0)
            return transform;


        var children = transform.GetComponentsInChildren<Transform>();

        foreach (var child in children)
        {
            if (child == transform || child == null) continue;
            if (activeOnly && !child.gameObject.activeSelf) continue;
            UnityEngine.Object.DestroyImmediate(child.gameObject);
        }
        return transform;
    }

    /// <summary>
    /// Find or create child with name.
    /// </summary>
    /// <param name="transform">Transform component.</param>
    /// <param name="name">Child name.</param>
    /// <returns>Child <see cref="Transform"/> component instance.</returns>
    public static Transform FindOrCreateChild(this Transform transform, string name)
    {
        var part = transform.Find(name);
        if (part == null)
        {
            part = new GameObject(name).transform;
            part.parent = transform;
            part.Reset();
        }
        return part;
    }

    public static string GetHierarchyString(this Transform transform) => string.Join("/", transform.GetHierarchy().Select(it => it.name).ToArray());

    public static IEnumerable<Transform> GetHierarchy(this Transform transform) => transform.GetAscendants().Reverse<Transform>();

    private static IEnumerable<Transform> GetAscendants(this Transform transform)
    {
        for (Transform current = transform; current != null; current = current.parent)
            yield return current;
    }

    public static List<T> ChildComponentsToList<T>(this Transform t) where T : Component => ((IEnumerable<T>)t.GetComponentsInChildren<T>()).ToList<T>();

    public static bool IsDescendant(this Transform potentialAncestor, Transform descendant)
    {
        if (descendant == null || potentialAncestor == null || descendant.parent == descendant)
            return false;
        return descendant.parent == potentialAncestor || descendant.parent.IsDescendant(potentialAncestor);
    }
    public static bool IsDescendantOf(this Transform descendant, Transform potentialAncestor)
    {
        return potentialAncestor.IsDescendant(descendant);
    }

    public static Transform GetTransformByNameInChildren(
      this Transform trans,
      string name,
      bool includeInactive = false,
      bool subString = false)
    {
        name = name.ToLower();
        foreach (Transform tran in trans)
        {
            if (!subString)
            {
                if (tran.name.ToLower() == name && (includeInactive || tran.gameObject.activeInHierarchy))
                    return tran;
            }
            else if (tran.name.ToLower().Contains(name) && (includeInactive || tran.gameObject.activeInHierarchy))
                return tran;
            Transform byNameInChildren = tran.GetTransformByNameInChildren(name, includeInactive, subString);
            if (byNameInChildren != null)
                return byNameInChildren;
        }
        return null;
    }

    public static Transform GetTransformByNameInAncestors(
      this Transform trans,
      string name,
      bool includeInactive = false,
      bool subString = false)
    {
        if (trans.parent == null)
            return null;
        name = name.ToLower();
        if (!subString)
        {
            if (trans.parent.name.ToLower() == name && (includeInactive || trans.gameObject.activeInHierarchy))
                return trans.parent;
        }
        else if (trans.parent.name.ToLower().Contains(name) && (includeInactive || trans.gameObject.activeInHierarchy))
            return trans.parent;
        Transform byNameInAncestors = trans.parent.GetTransformByNameInAncestors(name, includeInactive, subString);
        return byNameInAncestors != null ? byNameInAncestors : null;
    }
}