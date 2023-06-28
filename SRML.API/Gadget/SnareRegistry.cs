using SRML.Core.API.BuiltIn;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SRML.API.Gadget
{
    public class SnareRegistry : GenericRegistry<SnareRegistry, global::Identifiable.Id>
    {
        public Type[] RemoveOnSnare { get => removeOnSnareTypes.ToArray(); }
        private List<Type> removeOnSnareTypes = new List<Type>()
        {
            typeof(SlimeFaceAnimator),
            typeof(SlimeEat),
            typeof(SlimeEatAsh),
            typeof(SlimeEatWater),
            typeof(SlimeEatTrigger),
            typeof(SlimeSubbehaviour),
            typeof(DestroyPlortAfterTime),
            typeof(PlortInvulnerability),
            typeof(PlaySoundOnHit),
            typeof(DestroyOnIgnite),
        };

        public void RegisterRemoveOnSnareType(Type toRemove)
        {
            if (!typeof(Component).IsAssignableFrom(toRemove))
                return;

            removeOnSnareTypes.Add(toRemove);
        }
        
        public override void Initialize()
        {
        }
    }
}
