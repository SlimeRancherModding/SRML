using SRML.Core.API;
using SRML.Core.ModLoader;
using System;
using System.Collections.Generic;

namespace SRML.API.Player
{
    public class TargetingRegistry :
        Registry<TargetingRegistry, Predicate<global::Identifiable.Id>, Func<global::Identifiable.Id, string>, 
            Func<global::Identifiable.Id, string>>
    {
        public override void Initialize()
        {
        }

        public override bool IsRegistered(Predicate<global::Identifiable.Id> registered, 
            Func<global::Identifiable.Id, string> registered2, Func<global::Identifiable.Id, string> registered3) =>
            _registered.Contains((registered, registered2, registered3));


        public override void Register(Predicate<global::Identifiable.Id> condition, 
            Func<global::Identifiable.Id, string> nameFunc = null, Func<global::Identifiable.Id, string> descFunc = null)
        {
            if (IsRegistered(condition, nameFunc, descFunc))
                return;

            _registered.AddIfDoesNotContain((condition, nameFunc, descFunc));
            IMod mod = CoreLoader.Instance.GetExecutingModContext();
            if (mod != null)
            {
                if (!registeredForMod.ContainsKey(mod))
                    registeredForMod.Add(mod, new List<(Predicate<global::Identifiable.Id>, 
                        Func<global::Identifiable.Id, string>, Func<global::Identifiable.Id, string>)>());

                registeredForMod[mod].Add((condition, nameFunc, descFunc));
            }
        }
    }
}
