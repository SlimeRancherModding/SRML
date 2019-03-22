using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace SRML.Editor
{
    [CreateAssetMenu(menuName = "SRML/Replacers/FieldReplacer")]
    public class BundleFieldReplacer : ScriptableObject,IFieldReplacer
    {
        [SerializeField]
        BundleInstanceInfo instanceInfo;

        public IInstanceInfo InstanceInfo => instanceInfo;
        [SerializeField]
        bool replaceInChildren;

        public bool ReplaceInChildren => replaceInChildren;

        [SerializeField]
        List<BundleFieldReplacement> fieldReplacements;// = new List<Replacement>();

        public ICollection<IFieldReplacement> FieldReplacements => fieldReplacements.Select((x)=>(IFieldReplacement)x).ToList();

    }

    public interface IFieldReplacer
    {
        IInstanceInfo InstanceInfo { get; }
        bool ReplaceInChildren { get; }
        ICollection<IFieldReplacement> FieldReplacements { get; }
    }

    public interface IFieldReplacement
    {
        bool TryResolveSource(out FieldInfo field);
        bool TryResolveTarget(out FieldInfo field);
    }
    [CreateAssetMenu(menuName = "SRML/Replacers/FieldReplacement")]
    public class BundleFieldReplacement : ScriptableObject, IFieldReplacement //if this isnt a scriptable object unity absolutely refuses to serialize it correctly
    {
        [SerializeField]
        string fieldToReplaceType;
        [SerializeField]
        string fieldToReplaceFieldName;
        [SerializeField]
        string replacementSourceType;
        [SerializeField]
        string replacementSourceFieldName;

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
