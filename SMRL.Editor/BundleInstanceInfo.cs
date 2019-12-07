using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.Editor
{
    public struct BundleInstanceInfo : IInstanceInfo
    {
        [SerializeField]
        public IDType idtype;
        [SerializeField]
        public int id;
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
