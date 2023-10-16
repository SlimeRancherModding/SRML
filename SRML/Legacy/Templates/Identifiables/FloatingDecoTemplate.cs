using System.Collections.Generic;
using MonomiPark.SlimeRancher.Regions;
using SRML.SR;
using SRML.SR.Utils.BaseObjects;
using UnityEngine;

namespace SRML.SR.Templates.Identifiables
{
    /// <summary>
    /// A template to create floating decorations like ornaments or echos
    /// </summary>
    public class FloatingDecoTemplate : ModPrefab<FloatingDecoTemplate>
    {
        // The Type Enum
        public enum Type
        {
            ECHO,
            ECHO_NOTE,
            ORNAMENT,
            CUSTOM
        }

        // Base for Identifiables
        protected Identifiable.Id ID;
        protected Vacuumable.Size vacSize = Vacuumable.Size.NORMAL;

        // The Mesh and Materials
        protected Mesh mesh;
        protected Material[] materials;
        protected Color color = Color.clear;

        // Specific to the Type
        protected Type decoType = Type.CUSTOM;
        protected int clip;
        protected ObjectTransformValues trans;

        /// <summary>
        /// Template to create floating decorations
        /// </summary>
        /// <param name="name">The name of the object (prefixes are recommended, but not needed)</param>
        /// <param name="ID">The Identifiable ID for this decoration</param>
        /// <param name="type">The type of decoration</param>
        /// <param name="mesh">The model's mesh for this decoration</param>
        /// <param name="materials">The materials that compose this decoration's model</param>
        public FloatingDecoTemplate(string name, Identifiable.Id ID, Type type, Material[] materials = null, Mesh mesh = null) : base(name)
        {
            this.ID = ID;
            decoType = type;

            if (type == Type.ECHO)
            {
                this.materials = materials ?? BaseObjects.originMaterial["EchoBlue"].Group();
                this.mesh = BaseObjects.originMesh["Quad"];
                trans = new ObjectTransformValues(Vector3.zero, Vector3.zero, Vector3.one * 1.7f);
            }
            else if (type == Type.ECHO_NOTE)
            {
                this.materials = materials ?? BaseObjects.originMaterial["EchoNote1"].Group();
                this.mesh = BaseObjects.originMesh["Quad"];
                trans = new ObjectTransformValues(Vector3.zero, Vector3.zero, Vector3.one * 0.7f);
            }
            else if (type == Type.ORNAMENT)
            {
                this.materials = materials ?? BaseObjects.originMaterial["ornament_pink"].Group();
                this.mesh = BaseObjects.originMesh["quad_ornament"];
                trans = new ObjectTransformValues(Vector3.zero, Vector3.up * 180, Vector3.one * 0.8f);
            }
            else
            {
                this.materials = materials;
                this.mesh = mesh;
            }
        }

        /// <summary>
        /// Sets the vacuumable size
        /// </summary>
        /// <param name="vacSize">The vac size to set</param>
        public FloatingDecoTemplate SetVacSize(Vacuumable.Size vacSize)
        {
            this.vacSize = vacSize;
            return this;
        }

        /// <summary>
        /// Sets the clip (For the type ECHO_NOTE)
        /// </summary>
        /// <param name="clip">The number of clip to set</param>
        public FloatingDecoTemplate SetClip(int clip)
        {
            this.clip = clip;
            return this;
        }

        /// <summary>
        /// Sets the transform values
        /// </summary>
        /// <param name="trans">New values to set</param>
        public FloatingDecoTemplate SetTransValues(ObjectTransformValues trans)
        {
            this.trans = trans;
            return this;
        }

        /// <summary>
        /// Sets the color for the material (For the type ECHO or ECHO_NOTE) (only if you want to use the default material for echos and change the color)
        /// </summary>
        /// <param name="color">New color to set</param>
        public FloatingDecoTemplate SetColor(Color color)
        {
            this.color = color;
            return this;
        }

        /// <summary>
        /// Sets the translation for this deco's name
        /// </summary>
        /// <param name="name">The translated name</param>
        public override FloatingDecoTemplate SetTranslation(string name)
        {
            TranslationPatcher.AddActorTranslation("l." + ID.ToString().ToLower(), name);
            return this;
        }

        /// <summary>
        /// Creates the object of the template (To get the prefab version use .ToPrefab() after calling this)
        /// </summary>
        public override FloatingDecoTemplate Create()
        {
            // Create main object
            mainObject.AddComponents(
                new Create<Identifiable>((ident) => ident.id = ID),
                new Create<Vacuumable>((vac) => vac.size = vacSize),
                new Create<Rigidbody>((body) =>
                {
                    body.drag = 5f;
                    body.angularDrag = 0.5f;
                    body.mass = 0.3f;
                    body.useGravity = false;
                }),
                new Create<SphereCollider>((col) =>
                {
                    col.center = Vector3.zero;
                    col.radius = 0.25f;
                }),
                new Create<CollisionAggregator>(null),
                new Create<RegionMember>((rg) => rg.canHibernate = true),
                new Create<StopOnCollision>((stop) => stop.distFromCol = 0.25f)
            );

            // Create model
            if (color != Color.clear && (decoType == Type.ECHO || decoType == Type.ECHO_NOTE))
            {
                Material mat = new Material(materials[0]);
                mat.SetColor("_TintColor", color);

                materials = mat.Group();
            }

            mainObject.AddChild(new GameObjectTemplate("model",
                new Create<MeshFilter>((filter) => filter.sharedMesh = mesh),
                new Create<MeshRenderer>((render) =>
                {
                    render.sharedMaterials = materials;
                    render.receiveShadows = false;
                    render.shadowCastingMode = decoType == Type.ECHO || decoType == Type.ECHO_NOTE ? UnityEngine.Rendering.ShadowCastingMode.Off : UnityEngine.Rendering.ShadowCastingMode.On;
                })
            ).SetTransform(trans));

            // Create Note
            if (decoType == Type.ECHO_NOTE)
            {
                mainObject.AddAfterChildren(SetNoteRenderer);

                mainObject.AddChild(new GameObjectTemplate("echo_note",
                    new Create<SphereCollider>((col) =>
                    {
                        col.center = Vector3.zero;
                        col.radius = 0.75f;
                        col.isTrigger = true;
                    }),
                    new Create<EchoNote>((note) => note.clip = clip),
                    new Create<ResetLayerChanges>(null)
                ));
            }

            mainObject.Layer = BaseObjects.layers["ActorEchoes"];

            return this;
        }

        protected void SetNoteRenderer(GameObject obj)
        {
            obj.FindChild("echo_note").GetComponent<EchoNote>().renderer = obj.FindChild("model").GetComponent<MeshRenderer>();
        }
    }
}
