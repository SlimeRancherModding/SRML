using System.Collections.Generic;
using MonomiPark.SlimeRancher.Regions;
using SRML.SR;
using SRML.SR.Utils.BaseObjects;
using UnityEngine;

namespace SRML.SR.Templates.Identifiables
{
    /// <summary>
    /// A template to create new animals
    /// </summary>
    public class AnimalTemplate : ModPrefab<AnimalTemplate>
    {
        // Base for Identifiables
        protected Identifiable.Id ID;
        protected Vacuumable.Size vacSize = Vacuumable.Size.NORMAL;

        // The Model and Materials
        protected Mesh mesh;
        protected Material[] materials;

        // Move component to add
        protected ICreateComponent moveComponent;

        protected float minRepHours = 8;
        protected float maxRepHours = 16;

        // Reproduction stuff
        protected Identifiable mate;
        protected GameObject child;
        protected GameObject elder;

        // The game object with the skinned mesh
        protected GameObject skinnedMesh;
        protected Create<Animator> animator;
        protected GameObject bones;

        // Child Control
        protected bool isChild;
        protected float childHours;

        protected float eggPeriod;
        protected GameObject egg;

        /// <summary>
        /// Template to create new animals
        /// </summary>
        /// <param name="name">The name of the object (prefixes are recommended, but not needed)</param>
        /// <param name="ID">The Identifiable ID for this animal</param>
        /// <param name="mesh">The model's mesh for this animal</param>
        /// <param name="materials">The materials that compose this animal's model</param>
        /// <param name="animator">The animator used by the animal</param>
        /// <param name="isChild">Is this animal a child?</param>
        public AnimalTemplate(string name, Identifiable.Id ID, Mesh mesh, Material[] materials, Create<Animator> animator, bool isChild = false) : base(name)
        {
            this.ID = ID;
            this.mesh = mesh;
            this.materials = materials;
            this.animator = animator;
            this.isChild = isChild;

            moveComponent = new Create<ChickenRandomMove>((move) =>
            {
                move.maxJump = isChild ? 0.7f : 1f;
                move.walkForwardForce = isChild ? 2.5f : 3.333f;
                move.flapCue = EffectObjects.cueFlap;
            });

            skinnedMesh = BaseObjects.originSkinnedMesh["HenSkinned"];
            bones = BaseObjects.originBones["HenBones"];
        }

        /// <summary>
        /// Sets the reproduction objects (not needed for childs or males)
        /// </summary>
        /// <param name="mate">The male mate of this female</param>
        /// <param name="child">The resulting child</param>
        public AnimalTemplate SetReproduceObjects(Identifiable mate, GameObject child)
        {
            this.mate = mate;
            this.child = child;
            return this;
        }

        /// <summary>
        /// Sets the elder version of this (not needed for childs)
        /// </summary>
        /// <param name="elder">The elder version</param>
        public AnimalTemplate SetElder(GameObject elder)
        {
            this.elder = elder;
            return this;
        }

        /// <summary>
        /// Sets the movement component to a new one (Default is the Chicken Movement)
        /// </summary>
        /// <param name="comp">New component to use</param>
        public AnimalTemplate SetMoveComponent(ICreateComponent comp)
        {
            moveComponent = comp;
            return this;
        }

        /// <summary>
        /// Sets the vacuumable size
        /// </summary>
        /// <param name="vacSize">The vac size to set</param>
        public AnimalTemplate SetVacSize(Vacuumable.Size vacSize)
        {
            this.vacSize = vacSize;
            return this;
        }

        /// <summary>
        /// Sets the reproduction infos (not needed for childs or males)
        /// </summary>
        /// <param name="minReproduceGameHours">Min. Reproduction hours</param>
        /// <param name="maxReproduceGameHours">Max. Reproduction hours</param>
        public AnimalTemplate SetReproduceInfo(float minReproduceGameHours, float maxReproduceGameHours)
        {
            minRepHours = minReproduceGameHours;
            maxRepHours = maxReproduceGameHours;
            return this;
        }

        /// <summary>
        /// Sets the bones and skinned mesh of this animal (default are the ones from the Chicken/Hen)
        /// <para>The skinned mesh would be an object like mesh_body1 of the Hen's Prefab</para>
        /// <para>The bones would be an object like root of the Hen's Prefab</para>
        /// </summary>
        /// <param name="skinnedMesh">The new skinned mesh to use for the animal</param>
        /// <param name="bones">The new bones to use for the animal</param>
        public AnimalTemplate SetBones(GameObject skinnedMesh, GameObject bones)
        {
            this.skinnedMesh = skinnedMesh;
            this.bones = bones;
            return this;
        }

        /// <summary>
        /// Sets the child info (only needed for childs)
        /// </summary>
        /// <param name="delayGameHours">The number of hours before becoming an adult</param>
        /// <param name="eggPeriod">The number of hours before hatching the egg</param>
        /// <param name="egg">The egg object</param>
        public AnimalTemplate SetChildInfo(float delayGameHours, float eggPeriod, GameObject egg)
        {
            childHours = delayGameHours;
            this.eggPeriod = eggPeriod;
            this.egg = egg;
            return this;
        }

        /// <summary>
        /// Sets the translation for this animal's name
        /// </summary>
        /// <param name="name">The translated name</param>
        public override AnimalTemplate SetTranslation(string name)
        {
            TranslationPatcher.AddActorTranslation("l." + ID.ToString().ToLower(), name);
            return this;
        }

        /// <summary>
        /// Creates the object of the template (To get the prefab version use .ToPrefab() after calling this)
        /// </summary>
        public override AnimalTemplate Create()
        {
            // Create main object
            mainObject.AddComponents(
                new Create<Identifiable>((ident) => ident.id = ID),
                new Create<Vacuumable>((vac) => vac.size = vacSize),
                new Create<Rigidbody>((body) =>
                {
                    body.drag = 0.2f;
                    body.angularDrag = 5f;
                    body.mass = 0.3f;
                    body.useGravity = true;
                }),
                new Create<DragFloatReactor>((drag) => drag.floatDragMultiplier = 10),
                new Create<BoxCollider>((col) =>
                {
                    col.center = Vector3.zero;
                    col.size = Vector3.one * 0.5f;
                }),
                new Create<CollisionAggregator>(null),
                new Create<RegionMember>((rg) => rg.canHibernate = true),
                new Create<SECTR_PointSource>((source) =>
                {
                    source.Loop = false;
                    source.PlayOnStart = false;
                    source.RestartLoopsOnEnabled = false;
                    source.Volume = 1;
                    source.Pitch = 1;
                }),
                new Create<PlaySoundOnHit>((hit) =>
                {
                    hit.hitCue = isChild ? EffectObjects.cueHitChick : EffectObjects.cueHitChicken;
                    hit.minTimeBetween = 0.2f;
                    hit.minForce = 1;
                    hit.includeControllerCollisions = false;
                }),
                new Create<SlimeSubbehaviourPlexer>(null),
                moveComponent,
                new Create<SphereCollider>((col) =>
                {
                    col.center = Vector3.zero;
                    col.radius = 0.5f;
                    col.isTrigger = true;
                }),
                new Create<SlimeAudio>((audio) => audio.slimeSounds = BaseObjects.originSounds["HenHen"]),
                new Create<KeepUpright>((keep) =>
                {
                    keep.stability = 0.9f;
                    keep.speed = 2;
                }),
                new Create<DestroyOnIgnite>(null),
                new Create<AttachFashions>((fash) => fash.gordoMode = false)
            ).AddAfterChildren(ConfigBones);

            if (mate != null)
            {
                mainObject.AddComponents(
                    new Create<Reproduce>((rep) =>
                    {
                        rep.nearMateId = mate;
                        rep.maxDistToMate = 10f;
                        rep.densityDist = 10;
                        rep.maxDensity = 12;
                        rep.deluxeDensityFactor = 2;
                        rep.minReproduceGameHours = minRepHours;
                        rep.maxReproduceGameHours = maxRepHours;
                        rep.produceFX = EffectObjects.fxStars;
                        rep.childPrefab = child;
                    })
                );
            }

            if (isChild)
            {
                mainObject.AddComponents(
                    new Create<TransformAfterTime>((trans) =>
                    {
                        trans.delayGameHours = childHours;
                        trans.transformFX = EffectObjects.fxStars;
                    }),
                    new Create<EggActivator>((egg) =>
                    {
                        egg.eggPeriod = eggPeriod;
                        egg.activateObj = this.egg;
                    })
                );
            }
            else
            {
                mainObject.AddComponents(
                    new Create<TransformChanceOnReproduce>((trans) =>
                    {
                        trans.transformChance = 0.05f;
                        trans.targetPrefab = elder;
                        trans.transformFX = EffectObjects.fxStars;
                    })
                );
            }

            // Create body
            mainObject.AddChild(new GameObjectTemplate("Body",
                new Create<MeshFilter>((filter) => filter.sharedMesh = mesh),
                new Create<MeshRenderer>((render) => render.sharedMaterials = materials)
            ).SetTransform(Vector3.zero, Vector3.zero, Vector3.one * 0.5f));

            // Create delaunch trigger
            mainObject.AddChild(new GameObjectTemplate("DelaunchTrigger",
                new Create<SphereCollider>((col) =>
                {
                    col.center = Vector3.zero;
                    col.radius = 0.1f;
                    col.isTrigger = true;
                }),
                new Create<VacDelaunchTrigger>(null)
            ).SetTransform(Vector3.zero, Vector3.zero, Vector3.one * 1.4f));

            // Create animal
            mainObject.AddChild(new GameObjectTemplate("Animal",
                animator,
                new Create<LODGroup>((lod) =>
                {
                    lod.localReferencePoint = new Vector3(0, 0.5f, -0.1f);
                    lod.size = 1.893546f;
                })
            ).SetTransform(Vector3.up * -0.5f, Vector3.zero, Vector3.one));

            mainObject.Layer = BaseObjects.layers["Actor"];

            return this;
        }

        protected void ConfigBones(GameObject obj)
        {
            // Setup Bones
            GameObject sm = skinnedMesh.CreatePrefabCopy();
            sm.name = "mesh_body";
            sm.transform.parent = obj.transform.Find("Animal");

            GameObject b = bones.CreatePrefabCopy();
            b.name = "root";
            b.transform.parent = obj.transform.Find("Animal");

            // Read all bones to the skinned mesh
            SkinnedMeshRenderer render = sm.GetComponent<SkinnedMeshRenderer>();
            render.rootBone = b.FindChild("bone_spine", true).transform;

            List<Transform> addBones = new List<Transform>();
            foreach (GameObject bone in b.FindChildrenWithPartialName("bone_"))
                addBones.Add(bone.transform);

            render.bones = addBones.ToArray();

            // Add bones to other components
            obj.GetComponent<AttachFashions>().attachmentFront = b.FindChild("bone_attachment_front", true).transform;
        }
    }
}
