using System.Collections.Generic;
using SRML.Console;
using SRML.SR.Utils.Debug;
using SRML.SR.Templates.Misc;
using SRML.SR.Utils.BaseObjects;
using UnityEngine;

namespace SRML.SR.Templates.Plots
{
    /// <summary>
    /// A template to create new plots
    /// </summary>
    public class CustomPlotTemplate : ModPrefab<CustomPlotTemplate>
    {
        // Base for Plots
        protected List<IModPrefab> attachments = new List<IModPrefab>();
        protected LandPlot.Id ID;

        // Tech Activator
        protected ObjectTransformValues techActivatorTrans = new ObjectTransformValues(new Vector3(-5.5f, -0.1f, -5.5f), new Vector3(0, 255, 0), Vector3.one);
        protected GameObject prefabUI;

        /// <summary>
        /// Template to create new plots
        /// </summary>
        /// <param name="name">The name of the object (prefixes are recommended, but not needed)</param>
        /// <param name="ID">The Plot ID for this plot</param>
        public CustomPlotTemplate(string name, LandPlot.Id ID) : base(name)
        {
            this.ID = ID;
        }

        /// <summary>
        /// Sets the Activator Transform values
        /// </summary>
        /// <param name="position">The position to set</param>
        /// <param name="rotation">The rotation to set</param>
        /// <param name="scale">The scale to set</param>
        public CustomPlotTemplate SetActivatorTransform(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            techActivatorTrans = new ObjectTransformValues(position, rotation, scale);
            return this;
        }

        /// <summary>
        /// Sets the Activator Transform values
        /// </summary>
        /// <param name="trans">The values to set</param>
        public CustomPlotTemplate SetActivatorTransform(ObjectTransformValues trans)
        {
            techActivatorTrans = trans;
            return this;
        }

        // TODO: Finish documentation
        public CustomPlotTemplate AddAttachment(IModPrefab attachment)
        {
            attachments.Add(attachment);
            return this;
        }

        public CustomPlotTemplate AddAttachment(IModPrefab attachment, params IModPrefab[] attachments)
        {
            this.attachments.Add(attachment);
            this.attachments.AddRange(attachments);
            return this;
        }

        /// <summary>
        /// Sets the game object for the UI of this plot
        /// </summary>
        /// <param name="plotUI">The game object to set</param>
        public CustomPlotTemplate SetPlotUI(GameObject plotUI)
        {
            prefabUI = plotUI;
            return this;
        }

        /// <summary>
        /// Creates the object of the template (To get the prefab version use .ToPrefab() after calling this)
        /// </summary>
        public override CustomPlotTemplate Create()
        {
            // Create main object
            mainObject.AddComponents(
                new Create<LandPlot>((plot) => plot.typeId = ID),
                new Create<Recolorizer>(null)
            );

            // Add Attachments
            foreach (IModPrefab attach in attachments)
            {
                if (attach is IPlotUpgradeTemplate upgrade)
                {
                    mainObject.AddComponents(upgrade.GetUpgrader());
                    mainObject.AddAfterChildren(upgrade.GetIdentifyAction());
                }

                mainObject.AddChild(attach.AsTemplateClone());
            }

            // Add Tech Activator
            mainObject.AddChild(new TechActivatorTemplate("techActivator", prefabUI).AsTemplateClone().SetTransform(techActivatorTrans));

            // Add Plot Structure (Frame & Dirt)
            mainObject.AddChild(new PlotFrameTemplate("Frame").AsTemplate());

            mainObject.AddChild(new GameObjectTemplate("Dirt",
                new Create<MeshFilter>((filter) => filter.sharedMesh = RanchObjects.dirtMesh),
                new Create<MeshRenderer>((render) => render.sharedMaterials = RanchObjects.dirtMaterials)
            ).SetTransform(new Vector3(4.8f, 0, -4.8f), Vector3.zero, Vector3.one)
            .AddChild(new GameObjectTemplate("rocks",
                new Create<MeshFilter>((filter) => filter.sharedMesh = RanchObjects.rocksMesh),
                new Create<MeshRenderer>((render) => render.sharedMaterials = RanchObjects.rocksMaterials)
            ))
            .AddChild(new GameObjectTemplate("plane",
                new Create<MeshFilter>((filter) => filter.sharedMesh = RanchObjects.plotPlane),
                new Create<MeshRenderer>((render) => render.sharedMaterials = RanchObjects.plotPlaneMaterials)
            ).SetTransform(new Vector3(-4.8f, 0, 4.8f), Vector3.right * 90, Vector3.one * 9f)
            ));

            return this;
        }

    }
}
