using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace SRML.SR.Editor.Replacers
{
    public class LookupGameObjectReplacerWithMyName : LookupGameObjectReplacer
    {
        private new string targetField;

        public override GameObject GetReplacement()
        {
            targetField = gameObject.name;
            return base.GetReplacement();
        }
        
    }
}
