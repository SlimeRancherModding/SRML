using SRML.Core.API;
using SRML.Core.API.BuiltIn;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SRML.API.Identifiable.Slime
{
    public class FashionSlotRegistry : GenericRegistry<FashionSlotRegistry, Fashion.Slot, SlimeAppearance.SlimeBone, string>
    {
        public override void Initialize()
        {
        }
    }
}
