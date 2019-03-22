using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace SRML.Editor
{
    [CreateAssetMenu(menuName = "SRML/Replacers/FieldReplacer")]
    public class FieldReplacer : ScriptableObject
    {
        public InstanceInfo InstanceInfo;

        public bool ReplaceInChildren;

        [SerializeField]
        public List<FieldReplacement> FieldReplacements;// = new List<Replacement>();

    }
    [CreateAssetMenu(menuName = "SRML/Replacers/FieldReplacement")]
    public class FieldReplacement : ScriptableObject //if this isnt a scriptable object unity absolutely refuses to serialize it correctly
    {

        public string fieldToReplaceType;
        public string fieldToReplaceFieldName;

        public string replacementSourceType;
        public string replacementSourceFieldName;

        public bool TryResolveSource(out FieldInfo field)
        {
            return Resolve(replacementSourceType, replacementSourceFieldName, out field);
        }
        public bool TryResolveTarget(out FieldInfo field)
        {
            return Resolve(fieldToReplaceType,fieldToReplaceFieldName,out field);
        }

        private bool Resolve(String typeName, String fieldName, out FieldInfo field)
        {
            Debug.Log("Found "+ Type.GetType(typeName + ", Assembly-CSharp"));
            if (Type.GetType(typeName+", Assembly-CSharp") is System.Type type &&
                type.GetField(fieldName) is FieldInfo foundField)
            {
                field = foundField;
                return true;
            }

            field = null;
            return false;
        }
    }
}
