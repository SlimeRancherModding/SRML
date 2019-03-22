using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using SRML.Editor;

namespace SRML.Editor
{
    internal class ResolvedReplacer
    {
        public ResolvedInstance InstanceInfo;
        public Dictionary<FieldInfo,FieldInfo> FieldToField = new Dictionary<FieldInfo, FieldInfo>();

        public static ResolvedReplacer Resolve(FieldReplacer replacer)
        {
            ResolvedReplacer rep = new ResolvedReplacer();
            rep.InstanceInfo = ResolvedInstance.Resolve(replacer.InstanceInfo);
            if (replacer.FieldReplacements == null) throw new Exception("No replacements found!");
            foreach (var v in replacer.FieldReplacements)
            {
                if (!v.TryResolveTarget(out var field1) || !v.TryResolveSource(out var field2))
                    throw new Exception($"Unable to resolve field! {v.replacementSourceFieldName}:{v.replacementSourceType} from {v.fieldToReplaceFieldName}:{v.fieldToReplaceType}");
                rep.FieldToField.Add(field1,field2);
            }

            return rep;
        }
    }
}
