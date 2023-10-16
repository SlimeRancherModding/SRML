using SRML.SR.Utils.BaseObjects;
using System.Collections.Generic;
using UnityEngine;

namespace SRML.SR.Templates.Identifiables
{
    /// <summary>
    /// A template to create new fashion attachments (the part that gets attached to the object)
    /// </summary>
    public class FashionAttachTemplate : ModPrefab<FashionAttachTemplate>
    {
        // LayerInfo Struct
        protected struct LayerInfo
        {
            public Mesh mesh;
            public Material[] materials;

            public LayerInfo(Mesh mesh, Material[] materials)
            {
                this.mesh = mesh;
                this.materials = materials;
            }
        }

        // The Mesh and Materials
        protected List<LayerInfo> layers = new List<LayerInfo>();

        // The model transform
        protected ObjectTransformValues modelTrans = new ObjectTransformValues(Vector3.zero, Vector3.zero, Vector3.one);

        /// <summary>
        /// Template to create new fashion attachments
        /// </summary>
        /// <param name="name">The name of the object (prefixes are recommend, but not needed)</param>
        /// <param name="ID">The Identifiable ID for this fashion attachment</param>
        /// <param name="mesh">The model's mesh for this fashion attachment</param>
        /// <param name="materials">The materials that compose this fashion attachment's model</param>
        public FashionAttachTemplate(string name, Mesh mesh, Material[] materials) : base(name)
        {
            layers.Add(new LayerInfo(mesh, materials));
        }

        /// <summary>
        /// Sets the transform values for the model
        /// </summary>
        /// <param name="trans">New values to set</param>
        public FashionAttachTemplate SetModelTrans(ObjectTransformValues trans)
        {
            modelTrans = trans;
            return this;
        }

        /// <summary>
        /// Adds a new layer
        /// </summary>
        /// <param name="mesh">The mesh to add to the layer</param>
        /// <param name="materials">The materials to add to the layer</param>
        public FashionAttachTemplate AddLayer(Mesh mesh, Material[] materials)
        {
            layers.Add(new LayerInfo(mesh, materials));
            return this;
        }

        /// <summary>
        /// Creates the object of the template (To get the prefab version use .ToPrefab() after calling this)
        /// </summary>
        public override FashionAttachTemplate Create()
        {
            // Create model
            GameObjectTemplate model = new GameObjectTemplate("model_fp").SetTransform(modelTrans);

            int num = 1;
            foreach (LayerInfo layer in layers)
            {
                model.AddChild(new GameObjectTemplate("layer" + num,
                    new Create<MeshFilter>((filter) => filter.sharedMesh = layer.mesh),
                    new Create<MeshRenderer>((render) => render.sharedMaterials = layer.materials)
                ));
            }

            mainObject.AddChild(model);

            return this;
        }
    }
}
