using HarmonyLib;
using System.Reflection;

namespace SRML.Core.ModLoader.BuiltIn.EntryPoint
{
    public abstract class EventEntryPoint : Core.ModLoader.EntryPoint
    {
        protected EventEntryPoint(IModInfo info) : base(info)
        {
        }

        public abstract EntryEvent[] Events();

        public override void Initialize()
        {
            foreach (EntryEvent toRegister in Events())
            {
                HarmonyMethod method = new HarmonyMethod(toRegister.toExecute.Method);

                HarmonyInstance.Patch(toRegister.executeOn,
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
