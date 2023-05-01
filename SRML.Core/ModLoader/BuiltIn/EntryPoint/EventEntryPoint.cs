using HarmonyLib;
using System;
using System.Reflection;

namespace SRML.Core.ModLoader.BuiltIn.EntryPoint
{
    public abstract class EventEntryPoint : IEntryPoint
    {
        public abstract EntryEvent[] Events();

        public virtual void Initialize()
        {
            foreach (EntryEvent toRegister in Events())
            {
                HarmonyMethod method = new HarmonyMethod(toRegister.toExecute.Method);

                Main.HarmonyInstance.Patch(toRegister.executeOn,
                    toRegister.prefix ? method : null,
                    toRegister.prefix ? null : method, null);
            }
        }

        public class EntryEvent
        {
            public readonly bool prefix;
            public readonly MethodInfo executeOn;
            public readonly Method toExecute;

            public delegate void Method();

            public EntryEvent(bool prefix, MethodInfo executeOn, Method toExecute)
            {
                this.prefix = prefix;
                this.executeOn = executeOn;
                this.toExecute = toExecute;
            }
        }
    }
}
