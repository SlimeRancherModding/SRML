using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR
{
    public static class PurchasableUIRegistry
    {
        public delegate bool PurchasableUIPredicate(Type uiType, BaseUI ui);
        public delegate PurchaseUI.Purchasable PurchasableCreatorDelegate(BaseUI ui);
        public delegate PurchaseUI.Purchasable PurchasableCreatorDelegateGeneric<T>(T ui) where T : BaseUI;
        public delegate void PurchasableManipulatorDelegateGeneric<T>(T ui, ref PurchaseUI.Purchasable[] purchasables) where T : BaseUI;
        public delegate void PurchasableManipulatorDelegate(BaseUI ui, ref PurchaseUI.Purchasable[] purchasables);

        internal static Dictionary<PurchasableUIPredicate, PurchasableCreatorDelegate> customPurchasables = new Dictionary<PurchasableUIPredicate, PurchasableCreatorDelegate>();
        internal static Dictionary<PurchasableUIPredicate, PurchasableManipulatorDelegate> customManipulators = new Dictionary<PurchasableUIPredicate, PurchasableManipulatorDelegate>();
        public static void RegisterPurchasable(PurchasableUIPredicate pred, PurchasableCreatorDelegate creator)
        {
            customPurchasables.Add(pred, creator);
        }

        public static void RegisterManipulator(PurchasableUIPredicate pred, PurchasableManipulatorDelegate man)
        {
            customManipulators.Add(pred, man);
        }

        public static void RegisterPurchasable<T>(PurchasableUIPredicate pred, PurchasableCreatorDelegateGeneric<T> creator) where T : BaseUI
        {
            RegisterPurchasable(pred, FromGeneric(creator));
        }

        public static void RegisterManipulator<T>(PurchasableUIPredicate pred, PurchasableManipulatorDelegateGeneric<T> man) where T : BaseUI
        {
            RegisterManipulator(pred, FromGeneric(man));
        }

        public static void RegisterPurchasable(Type t, PurchasableCreatorDelegate creator)
        {
            if (!typeof(BaseUI).IsAssignableFrom(t)) throw new Exception($"{t} does not extend BaseUI!");
            RegisterPurchasable((otherType, x) => t.IsAssignableFrom(otherType), creator);
        }

        public static void RegisterManipulator(Type t, PurchasableManipulatorDelegate man)
        {   
            if (!typeof(BaseUI).IsAssignableFrom(t)) throw new Exception($"{t} does not extend BaseUI!");
            RegisterManipulator((otherType, x) => t.IsAssignableFrom(otherType), man);
        }

        public static void RegisterPurchasable<T>(PurchasableCreatorDelegateGeneric<T> creator) where T : BaseUI
        {
            RegisterPurchasable(typeof(T), FromGeneric(creator));
        }

        public static void RegisterManipulator<T>(PurchasableManipulatorDelegateGeneric<T> man) where T : BaseUI
        {
            RegisterManipulator(typeof(T), FromGeneric(man));
        }

        public static PurchasableCreatorDelegate FromGeneric<T>(PurchasableCreatorDelegateGeneric<T> creator) where T : BaseUI
        {
            return x => creator((T)x);
        }

        public static PurchasableManipulatorDelegate FromGeneric<T>(PurchasableManipulatorDelegateGeneric<T> manipulator) where T : BaseUI
        {
            return (BaseUI x, ref PurchaseUI.Purchasable[] array) =>
            {
                manipulator((T)x, ref array);
            };
        }

    }
}
