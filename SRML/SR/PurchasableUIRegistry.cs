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

        internal static Dictionary<PurchasableUIPredicate, PurchasableCreatorDelegate> customPurchasables = new Dictionary<PurchasableUIPredicate, PurchasableCreatorDelegate>();

        public static void RegisterPurchasable(PurchasableUIPredicate pred, PurchasableCreatorDelegate creator)
        {
            customPurchasables.Add(pred, creator);
        }

        public static void RegisterPurchasable<T>(PurchasableUIPredicate pred, PurchasableCreatorDelegateGeneric<T> creator) where T : BaseUI
        {
            RegisterPurchasable(pred, FromGeneric(creator));
        }

        public static void RegisterPurchasable(Type t, PurchasableCreatorDelegate creator)
        {
            if (!typeof(BaseUI).IsAssignableFrom(t)) throw new Exception($"{t} does not extend BaseUI!");
            RegisterPurchasable((otherType, x) => t.IsAssignableFrom(otherType), creator);
        }

        public static void RegisterPurchasable<T>(PurchasableCreatorDelegateGeneric<T> creator) where T : BaseUI
        {
            RegisterPurchasable(typeof(T), FromGeneric(creator));
        }

        public static PurchasableCreatorDelegate FromGeneric<T>(PurchasableCreatorDelegateGeneric<T> creator) where T : BaseUI
        {
            return x => creator((T)x);
        }

    }
}
