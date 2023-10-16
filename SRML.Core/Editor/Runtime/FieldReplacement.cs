using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SRML.Editor.Runtime
{
    public class FieldReplacement : IFieldReplacement
    {
        private FieldInfo source;
        private FieldInfo target;
        public bool TryResolveSource(out FieldInfo field)
        {
            field = source;
            return true;
        }

        public bool TryResolveTarget(out FieldInfo field)
        {
            field = target;
            return true;
        }

        public FieldReplacement(FieldInfo target, FieldInfo source)
        {
            this.target = target;
            this.source = source;
        }

        public FieldReplacement(string targetType, string targetField, string sourceType,string sourceField) : this(Type.GetType(targetType+", Assembly-CSharp").GetField(targetField), Type.GetType(sourceType + ", Assembly-CSharp").GetField(sourceField)) { }

        public FieldReplacement(string targetType, string targetField) : this(targetType, targetField, targetType,
            targetField)
        { }

        public FieldReplacement(FieldInfo info) : this(info, info) { }

    }

}
