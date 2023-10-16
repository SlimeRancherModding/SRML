using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.Editor.Runtime
{
    public class FieldReplacer : IFieldReplacer
    {
        public IInstanceInfo InstanceInfo { get; private set; }
    

        public bool ReplaceInChildren { get; private set; }

        List<IFieldReplacement> replacements = new List<IFieldReplacement>();

        public ICollection<IFieldReplacement> FieldReplacements => replacements;



        public FieldReplacer(IInstanceInfo rinfo, bool replaceInChildren, params IFieldReplacement[] replacements)
        {
            this.InstanceInfo = rinfo;
            this.replacements.AddRange(replacements);
        }
    }
}
