using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR
{
    public static class PurchasableUIRegistry
    {
        /// <summary>
        /// Predicate for choosing purchasable ui's
        /// </summary>
        /// <param name="uiType">The type of the Purchasable UI</param>
        /// <param name="ui">The UI instance</param>
        /// <returns></returns>
        public delegate bool PurchasableUIPredicate(Type uiType, BaseUI ui);

        /// <summary>
        /// Creates purchasables on demand
        /// </summary>
        /// <param name="ui">The <see cref="BaseUI"/> that the <see cref="PurchaseUI.Purchasable"/> will be added to</param>
        /// <returns></returns>
        public delegate PurchaseUI.Purchasable PurchasableCreatorDelegate(BaseUI ui);
        /// <summary>
        /// Creates a purchasable on demand
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ui">The UI that the <see cref="PurchaseUI.Purchasable"/> will be added to</param>
        /// <returns></returns>
        public delegate PurchaseUI.Purchasable PurchasableCreatorDelegateGeneric<T>(T ui) where T : BaseUI;
        /// <summary>
        /// Delegate for arbitarily manipulating a list of purchasables
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ui"></param>
        /// <param name="purchasables"></param>
        public delegate void PurchasableManipulatorDelegateGeneric<T>(T ui, ref PurchaseUI.Purchasable[] purchasables) where T : BaseUI;
        /// <summary>
        /// Delegate for arbitarily manipulating a list of purchasables
        /// </summary>
        public delegate void PurchasableManipulatorDelegate(BaseUI ui, ref PurchaseUI.Purchasable[] purchasables);

        internal static Dictionary<PurchasableUIPredicate, PurchasableCreatorDelegate> customPurchasables = new Dictionary<PurchasableUIPredicate, PurchasableCreatorDelegate>();
        internal static Dictionary<PurchasableUIPredicate, PurchasableManipulatorDelegate> customManipulators = new Dictionary<PurchasableUIPredicate, PurchasableManipulatorDelegate>();
        /// <summary>
        /// Register a <paramref name="creator"/> to all UI's pointed to by the <paramref name="pred"/>
        /// </summary>
        /// <param name="pred">The prediate for filtering the Purchasable UI</param>
        /// <param name="creator">The creator of the new purchasable</param>
        public static void RegisterPurchasable(PurchasableUIPredicate pred, PurchasableCreatorDelegate creator)
        {
            customPurchasables.Add(pred, creator);
        }

        /// <summary>
        /// Register a compararer to sort a list of purchasables
        /// </summary>
        /// <param name="pred">Filter for PurchasableUI's</param>
        /// <param name="comparer">Comparer to use when sorting</param>
        public static void RegisterEntrySorter(PurchasableUIPredicate pred, IComparer<PurchaseUI.Purchasable> comparer)
        {
            RegisterManipulator(pred, new PurchasableManipulatorDelegate((BaseUI x, ref PurchaseUI.Purchasable[] y) =>
            {
                var list = y.ToList();
                list.Sort(comparer);
                y = list.ToArray();
            }));
        }

        /// <summary>
        /// Register a compararer to sort a list of purchasables
        /// </summary>
        /// <typeparam name="T">The type of the UI</typeparam>
        /// <param name="comparer">Comparer used for sorting</param>
        public static void RegisterEntrySorter<T>(IComparer<PurchaseUI.Purchasable> comparer) where T : BaseUI
        {
            RegisterManipulator((x,y)=>typeof(T).IsAssignableFrom(x), new PurchasableManipulatorDelegate((BaseUI x, ref PurchaseUI.Purchasable[] y) =>
            {
                var list = y.ToList();
                list.Sort(comparer);
                y = list.ToArray();
            }));
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
