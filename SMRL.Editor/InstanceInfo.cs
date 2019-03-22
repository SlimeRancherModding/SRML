using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.Editor
{
    [CreateAssetMenu(menuName = "SRML/Replacers/InstanceInfo")]
    public class InstanceInfo : ScriptableObject
    {
        public IDType idType;
        public int ID;

        public enum IDType
        {
            IDENTIFIABLE,
            GADGET,
            LANDPLOT
            // to be continued
        }
    }
    
}
