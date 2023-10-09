using System.Collections.Generic;
using SRML.Console;
using SRML.SR.Utils.Debug;
using SRML.SR.Utils.BaseObjects;
using UnityEngine;

namespace SRML.SR.Templates.Misc
{
    /// <summary>
    /// A template ti create new Tech Activators
    /// </summary>
    public class TechActivatorTemplate : ModPrefab<TechActivatorTemplate>
    {
        // Activator Stuff
        protected GameObject prefabUI;

        /// <summary>
        /// Template to create new Tech Activators
        /// </summary>
        /// <param name="name">The name of the object (prefixes are recommended, but not needed)</param>
        /// <param name="prefabUI">The prefab UI to display</param>
        public TechActivatorTemplate(string name, GameObject prefabUI) : base(name)
        {
            this.prefabUI = prefabUI;
        }

        /// <summary>
        /// Creates the object of the template (To get the prefab version use .ToPrefab() after calling this)
        /// </summary>
        public override TechActivatorTemplate Create()
        {
            // Create the structure
            mainObject.AddChild(new GameObjectTemplate("techActivator",
                new Create<MeshFilter>((filter) => filter.sharedMesh = RanchObjects.techActivator),
                new Create<MeshRenderer>((render) => render.sharedMaterials = RanchObjects.techActivatorMaterials),
                new Create<CapsuleCollider>((col) =>
                {
                    col.center = new Vector3(0, 0.8f, 0);
                    col.radius = 0.2767721f;
                    col.height = 1.578097f;
                    col.direction = 1;
                })
            ));

            // Create the trigger
            mainObject.AddChild(new GameObjectTemplate("triggerActivate",
                new Create<SphereCollider>((col) =>
                {
                    col.center = Vector3.zero;
                    col.radius = 0.5f;
                }),
                new Create<UIActivator>((activ) =>
                {
                    activ.uiPrefab = prefabUI;
                    activ.blockInExpoPrefab = null;
                })
            ).SetTransform(Vector3.up * 1.4f, Vector3.zero, Vector3.one * 0.7f));

            return this;
        }
    }
}
