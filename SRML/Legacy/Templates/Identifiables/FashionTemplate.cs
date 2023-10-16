using System.Collections.Generic;
using MonomiPark.SlimeRancher.Regions;
using SRML.SR;
using SRML.SR.Utils.BaseObjects;
using UnityEngine;
using UnityEngine.UI;

namespace SRML.SR.Templates.Identifiables
{
    /// <summary>
    /// A template to create new fashions (the actual fashion not the pod)
    /// </summary>
    public class FashionTemplate : ModPrefab<FashionTemplate>
    {
        // Base for Identifiables
        protected Identifiable.Id ID;
        protected Vacuumable.Size vacSize = Vacuumable.Size.NORMAL;

        // Fashion Info
        protected Fashion.Slot slot;
        protected GameObject attachPrefab;

        protected Sprite icon;

        /// <summary>
        /// Template to create new fashions
        /// </summary>
        /// <param name="name">The name of the object (prefixes are recommended, but not needed)</param>
        /// <param name="ID">The Identifiable ID for this fashion</param>
        /// <param name="attachPrefab">The prefab to attach to the target</param>
        /// <param name="icon">The icon for this fashion</param>
        /// <param name="slot">The slot it occupied when attached</param>
        public FashionTemplate(string name, Identifiable.Id ID, GameObject attachPrefab, Sprite icon, Fashion.Slot slot = Fashion.Slot.FRONT) : base(name)
        {
            this.ID = ID;
            this.attachPrefab = attachPrefab;
            this.icon = icon;
            this.slot = slot;
        }

        /// <summary>
        /// Sets the vacuumable size
        /// </summary>
        /// <param name="vacSize">The vac size to set</param>
        public FashionTemplate SetVacSize(Vacuumable.Size vacSize)
        {
            this.vacSize = vacSize;
            return this;
        }


        /// <summary>
        /// Sets the translation for this fashion's name
        /// </summary>
        /// <param name="name">The translated name</param>
        public override FashionTemplate SetTranslation(string name)
        {
            TranslationPatcher.AddActorTranslation("l." + ID.ToString().ToLower(), name);
            return this;
        }

        /// <summary>
        /// Creates the object of the template (To get the prefab version use .ToPrefab() after calling this)
        /// </summary>
        public override FashionTemplate Create()
        {
            // Create main object
            mainObject.AddComponents(
                new Create<Identifiable>((ident) => ident.id = ID),
                new Create<Vacuumable>((vac) => vac.size = vacSize),
                new Create<Rigidbody>((body) =>
                {
                    body.drag = 0;
                    body.angularDrag = 0.05f;
                    body.mass = 0.1f;
                    body.useGravity = true;
                }),
                new Create<SphereCollider>((col) =>
                {
                    col.center = Vector3.zero;
                    col.radius = 0.5f;
                }),
                new Create<CollisionAggregator>(null),
                new Create<RegionMember>((rg) => rg.canHibernate = true),
                new Create<Fashion>((fash) =>
                {
                    fash.slot = slot;
                    fash.attachPrefab = attachPrefab;
                    fash.attachFX = EffectObjects.fxFashionApply;
                }),
                new Create<DestroyOnTouching>((dest) =>
                {
                    dest.hoursOfContactAllowed = 0;
                    dest.wateringRadius = 0;
                    dest.wateringUnits = 0;
                    dest.destroyFX = EffectObjects.fxFashionBurst;
                    dest.touchingWaterOkay = false;
                    dest.touchingAshOkay = false;
                    dest.reactToActors = false;
                    dest.liquidType = Identifiable.Id.WATER_LIQUID;
                })
            ).SetTransform(Vector3.zero, Vector3.zero, Vector3.one * 0.7f);

            // Create Icon UI content
            GameObjectTemplate imageBack = new GameObjectTemplate("Image Back",
                new Create<RectTransform>((trans) =>
                {
                    trans.anchorMin = Vector2.zero;
                    trans.anchorMax = Vector2.one;
                    trans.anchoredPosition = Vector2.zero;
                    trans.sizeDelta = Vector2.zero;
                    trans.pivot = Vector2.one * 0.5f;
                    trans.localEulerAngles = new Vector3(0, 180, 0);
                }),
                new Create<CanvasRenderer>(null),
                new Create<Image>((img) =>
                {
                    img.sprite = icon;
                    img.overrideSprite = icon;
                    img.type = Image.Type.Simple;
                    img.preserveAspect = true;
                    img.fillCenter = true;
                    img.fillMethod = Image.FillMethod.Radial360;
                    img.fillAmount = 1;
                    img.fillClockwise = true;
                    img.fillOrigin = 0;

                    img.material = BaseObjects.originMaterial["Digital Icon Medium"];
                    img.SetProperty("preferredWidth", 1024);
                    img.SetProperty("preferredHeight", 1024);
                })
            );

            GameObjectTemplate image = new GameObjectTemplate("Image",
                new Create<RectTransform>((trans) =>
                {
                    trans.anchorMin = Vector2.zero;
                    trans.anchorMax = Vector2.one;
                    trans.anchoredPosition = Vector2.zero;
                    trans.sizeDelta = Vector2.zero;
                    trans.pivot = Vector2.one * 0.5f;
                    trans.localEulerAngles = Vector3.zero;
                }),
                new Create<CanvasRenderer>(null),
                new Create<Image>((img) =>
                {
                    img.sprite = icon;
                    img.overrideSprite = icon;
                    img.type = Image.Type.Simple;
                    img.preserveAspect = true;
                    img.fillCenter = true;
                    img.fillMethod = Image.FillMethod.Radial360;
                    img.fillAmount = 1;
                    img.fillClockwise = true;
                    img.fillOrigin = 0;

                    img.material = BaseObjects.originMaterial["Digital Icon Medium"];
                    img.SetProperty("preferredWidth", 1024);
                    img.SetProperty("preferredHeight", 1024);
                })
            );

            GameObjectTemplate iconUI = new GameObjectTemplate("IconUI",
                new Create<RectTransform>((trans) =>
                {
                    trans.anchorMin = Vector2.zero;
                    trans.anchorMax = Vector2.zero;
                    trans.anchoredPosition = Vector2.zero;
                    trans.sizeDelta = Vector2.one * 80;
                    trans.pivot = Vector2.one * 0.5f;
                    trans.localEulerAngles = new Vector3(0, 180, 0);
                    trans.offsetMin = Vector2.one * -40f;
                    trans.offsetMax = Vector2.one * 40f;
                    trans.localScale = new Vector3(0, 0, 1.3f);
                }),
                new Create<Canvas>((canvas) => canvas.renderMode = RenderMode.WorldSpace),
                new Create<CanvasScaler>((scale) =>
                {
                    scale.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
                    scale.referenceResolution = new Vector2(800, 600);
                    scale.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                })
            ).AddChild(image).AddChild(imageBack);

            // Create Icon Pivot
            mainObject.AddChild(new GameObjectTemplate("Icon Pivot",
                new Create<CameraFacingBillboard>(null)
            ).AddChild(iconUI));

            // Create Surround Sphere
            mainObject.AddChild(new GameObjectTemplate("SurroundSphere",
                new Create<MeshFilter>((filter) => filter.sharedMesh = BaseObjects.originMesh["plort_shell"]),
                new Create<MeshRenderer>((render) => render.sharedMaterials = BaseObjects.originMaterial["EyeShine"].Group())
            ).SetTransform(Vector3.zero, new Vector3(45, 90, 0), Vector3.one * 0.5f));

            // Create delaunch
            mainObject.AddChild(new GameObjectTemplate("DelaunchTrigger",
                new Create<SphereCollider>((col) =>
                {
                    col.center = Vector3.zero;
                    col.radius = 0.1f;
                    col.isTrigger = true;
                }),
                new Create<VacDelaunchTrigger>(null)
            ));

            mainObject.Layer = BaseObjects.layers["Actor"];

            return this;
        }
    }
}
