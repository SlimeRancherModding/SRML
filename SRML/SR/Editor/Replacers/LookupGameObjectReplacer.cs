using System;
using SRML.Editor;
using UnityEngine;

namespace SRML.SR.Editor.Replacers
{
    public class LookupGameObjectReplacer : GameObjectReplacer
    {
        public int prefabId;
        public String targetComponent;
        public String targetField;
        public override GameObject GetReplacement()
        {
            GameObject targetPrefab = GameContext.Instance.LookupDirector.GetPrefab((Identifiable.Id) prefabId);
            if (targetComponent == null || targetComponent.Length == 0) return targetPrefab;
            Type targetCompType = Type.GetType(targetComponent+", Assembly-CSharp",true); // make a field for assembly name somewhere instead
            var targetFieldInfo = targetCompType.GetField(targetField);
            if (targetFieldInfo.FieldType != typeof(GameObject))
                throw new Exception("Target field does not take a GameObject");
            return (GameObject)targetFieldInfo.GetValue(targetPrefab.GetComponentInChildren(targetCompType));
        }
    }
}
