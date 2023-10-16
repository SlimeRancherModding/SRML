using System.Collections.Generic;
using SRML.Console;
using SRML.SR.Utils.Debug;
using SRML.SR.Utils.BaseObjects;
using UnityEngine;

namespace SRML.SR.Templates.Plots
{
    /// <summary>
    /// A template to create a plot frame (use this if you want to create a custom frame by adding prefab functions to it)
    /// </summary>
    public class PlotFrameTemplate : ModPrefab<PlotFrameTemplate>
    {
        /// <summary>
        /// Template to create a plot frame
        /// </summary>
        /// <param name="name">The name of the object</param>
        public PlotFrameTemplate(string name) : base(name) { }

        /// <summary>
        /// Creates the object of the template (To get the prefab version use .ToPrefab() after calling this)
        /// </summary>
        public override PlotFrameTemplate Create()
        {
            // Create a base post
            GameObjectTemplate post = new GameObjectTemplate("Post")
            .AddChild(new GameObjectTemplate("plotBase",
                new Create<MeshFilter>((filter) => filter.sharedMesh = RanchObjects.plotBase),
                new Create<MeshRenderer>((render) => render.sharedMaterials = RanchObjects.plotBaseMaterials)
            ))
            .AddChild(new GameObjectTemplate("plotPost",
                new Create<MeshFilter>((filter) => filter.sharedMesh = RanchObjects.plotPost),
                new Create<MeshRenderer>((render) => render.sharedMaterials = RanchObjects.plotBaseMaterials),
                new Create<CapsuleCollider>((col) =>
                {
                    col.center = Vector3.up * 2.3f;
                    col.radius = 0.4455906f;
                    col.height = 4.199063f;
                    col.direction = 1;
                })
            ));

            // Create main object
            mainObject.AddChild(post.Clone("Post 1").SetTransform(new Vector3(4.8f, 0, -4.8f), Vector3.zero, Vector3.one));
            mainObject.AddChild(post.Clone("Post 2").SetTransform(new Vector3(-4.8f, 0, -4.8f), Vector3.up * 90, Vector3.one));
            mainObject.AddChild(post.Clone("Post 3").SetTransform(new Vector3(-4.8f, 0, 4.8f), Vector3.up * 180, Vector3.one));
            mainObject.AddChild(post.Clone("Post 4").SetTransform(new Vector3(4.8f, 0, 4.8f), Vector3.up * 270, Vector3.one));

            return this;
        }
    }
}
