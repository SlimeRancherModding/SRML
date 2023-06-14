using SRML.Core.ModLoader;
using System.Collections.Generic;

namespace SRML.Core.API.BuiltIn
{
    public abstract class GenericRegistry<R, T> : Registry<R, T>
        where R : GenericRegistry<R, T>
    {
        public sealed override void Register(T toRegister)
        {
            if (IsRegistered(toRegister))
                return;

            _registered.AddIfDoesNotContain(toRegister);
            IMod mod = CoreLoader.Instance.GetExecutingModContext();
            if (mod != null)
            {
                if (!registeredForMod.ContainsKey(mod))
                    registeredForMod.Add(mod, new List<T>());

                registeredForMod[mod].Add(toRegister);
            }
        }

        public sealed override bool IsRegistered(T registered) =>
            _registered.Contains(registered);
    }

    public abstract class GenericRegistry<R, T, T2> : Registry<R, T, T2>
        where R : GenericRegistry<R, T, T2>
    {
        public sealed override void Register(T toRegister, T2 toRegister2)
        {
            if (IsRegistered(toRegister, toRegister2))
                return;

            _registered.AddIfDoesNotContain((toRegister, toRegister2));
            IMod mod = CoreLoader.Instance.GetExecutingModContext();
            if (mod != null)
            {
                if (!registeredForMod.ContainsKey(mod))
                    registeredForMod.Add(mod, new List<(T, T2)>());

                registeredForMod[mod].Add((toRegister, toRegister2));
            }
        }

        public sealed override bool IsRegistered(T registered, T2 registered2) =>
            _registered.Contains((registered, registered2));
    }

    public abstract class GenericRegistry<R, T, T2, T3> : Registry<R, T, T2, T3>
        where R : GenericRegistry<R, T, T2, T3>
    {
        public sealed override void Register(T toRegister, T2 toRegister2, T3 toRegister3)
        {
            if (IsRegistered(toRegister, toRegister2, toRegister3))
                return;

            _registered.AddIfDoesNotContain((toRegister, toRegister2, toRegister3));
            IMod mod = CoreLoader.Instance.GetExecutingModContext();
            if (mod != null)
            {
                if (!registeredForMod.ContainsKey(mod))
                    registeredForMod.Add(mod, new List<(T, T2, T3)>());

                registeredForMod[mod].Add((toRegister, toRegister2, toRegister3));
            }
        }

        public sealed override bool IsRegistered(T registered, T2 registered2, T3 registered3) =>
            _registered.Contains((registered, registered2, registered3));
    }
}
