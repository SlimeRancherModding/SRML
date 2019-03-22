using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.Editor
{
    [CreateAssetMenu(menuName = "SRML/Replacers/InstanceInfo")]
    public class BundleInstanceInfo : ScriptableObject, IInstanceInfo
    {
        [SerializeField]
        IDType idtype;
        [SerializeField]
        int id;
        public IDType idType => idtype;
        public int ID => id;

    }
    public enum IDType
    {
        IDENTIFIABLE,
        GADGET,
        LANDPLOT
        // to be continued
    }
    public interface IInstanceInfo
    {
        IDType idType { get; }
        int ID { get; }
    }
    
}
