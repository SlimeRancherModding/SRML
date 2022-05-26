using System.Collections.Generic;
using SRML.Console;
using SRML.SR.Utils.Debug;
using UnityEngine;

namespace SRML.SR.Templates.Plots
{
    /// <summary>
    /// Interface used to make lists of PlotUpgradeTemplate (as lists can't have different generic constructs)
    /// </summary>
    public interface IPlotUpgradeTemplate
    {
        ICreateComponent GetUpgrader();
        System.Action<GameObject> GetIdentifyAction();
    }

    /// <summary>
    /// A template to create a plot upgrade
    /// <para>This template is a bit more complex than the others, it's structure is entirely built by you. It only exists for covinience when adding to custom plots.</para>
    /// </summary>
    public class PlotUpgradeTemplate<T> : ModPrefab<PlotUpgradeTemplate<T>>, IPlotUpgradeTemplate where T : PlotUpgrader
    {
        // Base for Plot Upgrade
        protected LandPlot.Upgrade upgrade;
        protected Create<T> upgrader;
        protected System.Action<GameObject> setupComponent;

        /// <summary>
        /// Template to create a plot upgrade
        /// </summary>
        /// <param name="name">The name of the object</param>
        /// <param name="upgrade">The upgrade for this land plot</param>
        /// <param name="upgrader">The upgrader component to add to the main object</param>
        /// <param name="setupComponent">The action to setup the component after creation</param>
        public PlotUpgradeTemplate(string name, LandPlot.Upgrade upgrade, Create<T> upgrader, System.Action<GameObject> setupComponent) : base(name)
        {
            this.upgrade = upgrade;
            this.upgrader = upgrader;
            this.setupComponent = setupComponent;
        }

#pragma warning disable 0809
        /// <summary>
        /// Creates the object of the template (To get the prefab version use .ToPrefab() after calling this)
        /// </summary>
        [System.Obsolete("For the plot upgrade use .Create(action) to customize the upgrade")]
        public override PlotUpgradeTemplate<T> Create()
        {
            return null;
        }
#pragma warning restore 0809

        /// <summary>
        /// Creates the object of the template (To get the prefab version use .ToPrefab() after calling this)
        /// </summary>
        /// <param name="action">The action to construct the template</param>
        public virtual PlotUpgradeTemplate<T> Create(System.Action<GameObjectTemplate> action)
        {
            action?.Invoke(mainObject);
            return this;
        }

        ICreateComponent IPlotUpgradeTemplate.GetUpgrader()
        {
            return upgrader;
        }

        System.Action<GameObject> IPlotUpgradeTemplate.GetIdentifyAction()
        {
            return setupComponent;
        }
    }
}
