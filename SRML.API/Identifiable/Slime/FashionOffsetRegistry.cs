using SRML.Core.API.BuiltIn;
using System;
using UnityEngine;

namespace SRML.API.Identifiable.Slime
{
    public class FashionOffsetRegistry : GenericRegistry<FashionOffsetRegistry, global::Identifiable.Id, Vector3, Predicate<AttachFashions>>
    {
        public override void Initialize()
        {
        }
    }
}
