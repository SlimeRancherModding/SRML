using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.Editor
{
    public abstract class GameObjectReplacer : MonoBehaviour
    {
        public virtual GameObject GetReplacement()
        {
            return new GameObject();
        }
    }
}
