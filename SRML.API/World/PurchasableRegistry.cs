using SRML.Core.API.BuiltIn;
using System;
using System.Collections.Generic;

namespace SRML.API.World
{
    public class PurchasableRegistry : GenericRegistry<PurchasableRegistry, Type, Func<BaseUI, PurchaseUI.Purchasable>>
    {
        public delegate void PurchaseableUIManipulator(BaseUI ui, ref PurchaseUI.Purchasable[] purchasables);

        internal Dictionary<Type, List<PurchaseableUIManipulator>> manipulators = new Dictionary<Type, List<PurchaseableUIManipulator>>();
            
        public override void Initialize()
        {
        }

        public void RegisterManipulator<T>(PurchaseableUIManipulator manipulator) where T : BaseUI => RegisterManipulator(typeof(T), manipulator);

        public void RegisterManipulator(Type type, PurchaseableUIManipulator manipulator)
        {
            if (!manipulators.ContainsKey(type))
                manipulators[type] = new List<PurchaseableUIManipulator>();

            manipulators[type].Add(manipulator);
        }

        public void Register<T>(Func<BaseUI, PurchaseUI.Purchasable> purchasable) where T : BaseUI => Register(typeof(T), purchasable);

        public override bool IsValid(Type toRegister, Func<BaseUI, PurchaseUI.Purchasable> toRegister2) =>
            toRegister.IsSubclassOf(typeof(BaseUI));
    }
}
