using HarmonyLib;
using SRML.Core.API.BuiltIn;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SRML.API.Gadget
{
    public class DroneTargetRegistry : ComponentRegistry<DroneTargetRegistry, global::Identifiable.Id, DroneGadget>
    {
        public override MethodInfo ComponentInitializeMethod => AccessTools.Method(typeof(DroneGadget), "Awake");
        public override bool Prefix => true;

        private readonly List<DroneMetadata> registeredMetadata = new List<DroneMetadata>();

        public override bool IsRegistered(global::Identifiable.Id registered, DroneGadget component) =>
            registeredMetadata.All(x => x.targets.Any(y => y.ident == registered));

        protected override void InitializeComponent(DroneGadget component)
        {
            Console.Console.Instance.Log($"initializign drone");
            if (!registeredMetadata.Contains(component.metadata))
            {
                registeredMetadata.Add(component.metadata);
                if (AlreadyRegistered)
                {
                    foreach (global::Identifiable.Id id in Registered)
                        Register(id, component.metadata);
                }
            }
        }

        public void Register(global::Identifiable.Id toRegister, DroneMetadata metadata)
        {
            Console.Console.Instance.Log($"registering {toRegister} into {metadata.name}");
            if (!metadata.targets.Any(x => x.ident == toRegister))
                metadata.targets = metadata.targets.AddToArray(new DroneMetadata.Program.Target.Basic(toRegister));
        }

        public void RegisterIntoAll(global::Identifiable.Id toRegister)
        {
            foreach (DroneMetadata metadata in registeredMetadata)
                Register(toRegister, metadata);
        }

        protected override void RegisterIntoComponent(global::Identifiable.Id toRegister, DroneGadget component) =>
            RegisterIntoAll(toRegister);
    }
}
