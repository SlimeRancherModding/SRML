using HarmonyLib;
using SRML.SR.SaveSystem;
using SRML.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static SRML.SR.PediaRegistry.PediaTab;

namespace SRML.SR
{
    public static class PediaRegistry
    {
        internal static IDRegistry<PediaDirector.Id> moddedIds = new IDRegistry<PediaDirector.Id>();

        internal static List<PediaDirector.IdentMapEntry> customIdentifiableLinks = new List<PediaDirector.IdentMapEntry>();
        internal static List<PediaDirector.IdEntry> customEntries = new List<PediaDirector.IdEntry>();
        internal static List<PediaDirector.Id> initialEntries = new List<PediaDirector.Id>();
        internal static Dictionary<PediaDirector.Id, PediaCategory> pediaMappings = new Dictionary<PediaDirector.Id, PediaCategory>();
        internal static Dictionary<PediaDirector.Id, IPediaRenderer> customPediaRenderers = new Dictionary<PediaDirector.Id, IPediaRenderer>();
        internal static Dictionary<PediaCategory, List<IComparer<PediaDirector.Id>>> pediaSorters = new Dictionary<PediaCategory, List<IComparer<PediaDirector.Id>>>();
        internal static Dictionary<PediaTab, SRMod> customTabs = new Dictionary<PediaTab, SRMod>();
        

        internal static IPediaRenderer activeRenderer;
        internal static ITabRenderer activeTabRenderer;

        static Dictionary<DisplaySetting, DefaultPediaRenderer> defaultRenderers = new Dictionary<DisplaySetting, DefaultPediaRenderer>();
        
        static PediaRegistry()
        {
            ModdedIDRegistry.RegisterIDRegistry(moddedIds);
        }

        /// <summary>
        /// Gets the default renderer for a Slimepedia entry.
        /// </summary>
        /// <param name="setting">The setting to get the renderer from.</param>
        /// <returns>The default renderer for the specified setting.</returns>
        public static IPediaRenderer GetDefaultRenderer(DisplaySetting setting)
        {
            if(!defaultRenderers.TryGetValue(setting, out var renderer))
            {
                renderer = new DefaultPediaRenderer(setting);
                defaultRenderers.Add(setting, renderer);
                
            }
            return renderer;
        }

        /// <summary>
        /// Creates a <see cref="PediaDirector.Id"/>.
        /// </summary>
        /// <param name="value">What value is assigned to the <see cref="PediaDirector.Id"/>.</param>
        /// <param name="name">The name of the <see cref="PediaDirector.Id"/>.</param>
        /// <returns>The created <see cref="PediaDirector.Id"/>.</returns>
        /// <exception cref="Exception">Throws if ran outside of PreLoad</exception>
        public static PediaDirector.Id CreatePediaId(object value, string name)
        {
            if (SRModLoader.CurrentLoadingStep > SRModLoader.LoadingStep.PRELOAD)
                throw new Exception("Can't register slimepedia entries outside of the PreLoad step");
            return moddedIds.RegisterValueWithEnum((PediaDirector.Id)value, name);
        }

        /// <summary>
        /// Registers a tab for the Slimepedia.
        /// </summary>
        /// <param name="tab">The tab to register</param>
        public static void RegisterPediaTab(PediaTab tab) => customTabs.Add(tab, SRMod.GetCurrentMod());

        /// <summary>
        /// Registers a <see cref="PediaDirector.IdEntry"/> into the Slimepedia.
        /// </summary>
        /// <param name="entry">The <see cref="PediaDirector.IdEntry"/> to register.</param>
        public static void RegisterIdEntry(PediaDirector.IdEntry entry) => customEntries.Add(entry);
        
        /// <summary>
        /// Creates and registers a <see cref="PediaDirector.IdEntry"/>.
        /// </summary>
        /// <param name="id">The <see cref="PediaDirector.Id"/> belonging to the entry.</param>
        /// <param name="icon">The icon belonging to the entry.</param>
        public static void RegisterIdEntry(PediaDirector.Id id, Sprite icon) => RegisterIdEntry(new PediaDirector.IdEntry() { id = id, icon = icon });
        
        /// <summary>
        /// Registers an entry renderer for the Slimepedia.
        /// </summary>
        /// <param name="id">The <see cref="PediaDirector.Id"/> assigned to the entry.</param>
        /// <param name="renderer">The renderer the Slimepedia entry should use.</param>
        public static void RegisterRenderer(PediaDirector.Id id, IPediaRenderer renderer) => customPediaRenderers.Add(id, renderer);

        internal static IPediaRenderer GetRenderer(PediaDirector.Id id)
        {
            if (!customPediaRenderers.TryGetValue(id, out var renderer))
            {
                var tab = GetCustomPediaTab(id);
                if (tab != null)
                {
                    return GetDefaultRenderer(tab.DefaultDisplaySetting);
                }
                return null;
            }
            return renderer;
        }

        internal static PediaTab GetCustomPediaTab(PediaDirector.Id id)
        {
            foreach(var v in customTabs)
            {
                if (v.Key.Entries.Contains(id) || v.Key.ID == id) return v.Key;
            }
            return null;
        }

        /// <summary>
        /// Registers an entry to already be unlocked in a new game
        /// </summary>
        /// <param name="id">The <see cref="PediaDirector.Id"/> that belongs to the entry.</param>
        public static void RegisterInitialPediaEntry(PediaDirector.Id id) => initialEntries.Add(id);
        
        /// <summary>
        /// Creates a link between an <see cref="Identifiable.Id"/> and a <see cref="PediaDirector.Id"/>.
        /// </summary>
        /// <param name="entry">The entry mapping both values.</param>
        public static void RegisterIdentifiableMapping(PediaDirector.IdentMapEntry entry) => customIdentifiableLinks.Add(entry);

        /// <summary>
        /// Creates a link between an <see cref="Identifiable.Id"/> and a <see cref="PediaDirector.Id"/>.
        /// </summary>
        /// <param name="pedia">The <see cref="PediaDirector.Id"/> to link.</param>
        /// <param name="ident">The <see cref="Identifiable.Id"/> to link.</param>
        public static void RegisterIdentifiableMapping(PediaDirector.Id pedia, Identifiable.Id ident) => 
            RegisterIdentifiableMapping(new PediaDirector.IdentMapEntry() { identId = ident, pediaId = pedia });

        static ref PediaDirector.Id[] GetCategory(PediaCategory cat)
        {
            switch (cat)
            {
                case PediaCategory.TUTORIALS:
                    return ref PediaUI.TUTORIALS_ENTRIES;
                case PediaCategory.SLIMES:
                    return ref PediaUI.SLIMES_ENTRIES;
                case PediaCategory.RESOURCES:
                    return ref PediaUI.RESOURCES_ENTRIES;
                case PediaCategory.RANCH:
                    return ref PediaUI.RANCH_ENTRIES;
                case PediaCategory.WORLD:
                    return ref PediaUI.WORLD_ENTRIES;
                case PediaCategory.SCIENCE:
                    return ref PediaUI.SCIENCE_ENTRIES;
            }
            throw new Exception();
        }

        /// <summary>
        /// Sorts a Slimepedia category based on a rule.
        /// </summary>
        /// <param name="category">The Slimepedia category to sort.</param>
        /// <param name="comparer">The rules to sort it.</param>
        public static void SortPediaCategory(PediaCategory category, IComparer<PediaDirector.Id> comparer)
        {
            var cat = GetCategory(category).ToList();
            cat.Sort(comparer);
            GetCategory(category) = cat.ToArray();
        }

        /// <summary>
        /// Assigns a <see cref="PediaDirector.Id"/> to a Slimepedia category.
        /// </summary>
        /// <param name="id">The <see cref="PediaDirector.Id"/> to assign.</param>
        /// <param name="category">The Slimepedia category to assign the <see cref="PediaDirector.Id"/> to.</param>
        public static void SetPediaCategory(PediaDirector.Id id, PediaCategory category) => GetCategory(category) = GetCategory(category).AddToArray(id);

        public enum PediaCategory
        {
            TUTORIALS,
            SLIMES,
            RESOURCES,
            RANCH,
            WORLD,
            SCIENCE
        }

        public class DefaultPediaRenderer  : IReusablePediaRenderer
        {
            public DisplaySetting DisplaySetting { get; }
            public PediaDirector.Id CurrentID { get; set; }
            public DefaultPediaRenderer(DisplaySetting setting)
            {
                DisplaySetting = setting;
                
            }

            public virtual string GetLowerName() => CurrentID.ToString().ToLower();
            
            public void OnListingSelected(GameObject panelObj)
            {
                var pedia = panelObj.GetComponentInParent<PediaUI>();
                switch (DisplaySetting)
                {
                    case DisplaySetting.GENERIC:
                        pedia.PopulateGenericDesc(GetLowerName());
                        break;
                    case DisplaySetting.RANCH:
                        pedia.PopulateRanchDesc(GetLowerName());
                        break;
                    case DisplaySetting.RESOURCE:
                        pedia.PopulateResourcesDesc(GetLowerName());
                        break;
                    case DisplaySetting.SLIME:
                        pedia.PopulateSlimesDesc(GetLowerName());
                        break;
                    case DisplaySetting.VACPACK:
                        pedia.PopulateVacpackDesc(GetLowerName());
                        break;
                }
            }

            public void OnListingDeselected(GameObject panelObj)
            {
            }
        }

        public abstract class BasicPediaRenderer : IPediaRenderer
        {
            GameObject createdObject;

            public abstract GameObject CreateRenderGameObject(GameObject panelObj);

            void IPediaRenderer.OnListingSelected(GameObject panelObj)
            {
                createdObject = CreateRenderGameObject(panelObj);
                if (createdObject.transform.parent != panelObj.transform) createdObject.transform.SetParent(panelObj.transform);
            }

            void IPediaRenderer.OnListingDeselected(GameObject panelObj)
            {
                if (createdObject) GameObject.Destroy(createdObject);
            }
        }

        public interface IReusablePediaRenderer : IPediaRenderer
        {
            PediaDirector.Id CurrentID { get; set; }
        }

        public interface IPediaRenderer
        {
            void OnListingSelected(GameObject panelObj);
            void OnListingDeselected(GameObject panelObj);
        }

        public enum DisplaySetting
        {
            GENERIC,
            SLIME,
            RANCH,
            RESOURCE,
            VACPACK
        }

        public class PediaTab
        {
            public string NameKey => "b." + ID.ToString().ToLower();
            public PediaDirector.Id ID;
            public Func<bool> IsVisible;
            public List<PediaDirector.Id> Entries = new List<PediaDirector.Id>();
            public DisplaySetting DefaultDisplaySetting;
            public ITabRenderer TabRenderer;

            internal Toggle TabToggle { get; private set; }

            public void SetTabTranslation(string tabname)
            {
                TranslationPatcher.AddUITranslation(NameKey, tabname);
            }

            public void InitForPediaAwake(PediaUI ui)
            {
                if (!TabToggle)
                {
                    TabToggle = GenerateTab(ui.slimesTab);
                    ui.tabs.tabs = ui.tabs.tabs.AddToArray(TabToggle);
                }
            }

            public virtual Toggle.ToggleEvent GetOnSelectAction()
            {
                var newAction = new Toggle.ToggleEvent();
                newAction.AddListener((x)=>
                {
                    if(TabToggle?.isOn ?? false)
                    {
                        TabToggle.GetComponentInParent<PediaUI>().SelectEntry(Entries[0], true, Entries[0]);
                    }
                });
                return newAction;
            }

            public virtual Toggle GenerateTab(Toggle original)
            {
                var newUI= GameObjectUtils.InstantiateInactive(original.gameObject);
                newUI.GetComponentInChildren<XlateText>(true).key = NameKey;
                var toggle = newUI.GetComponent<Toggle>();
                toggle.onValueChanged = GetOnSelectAction();
                newUI.transform.SetParent(original.transform.parent,false);
                newUI.SetActive(true);
                return toggle;
            }

            public interface ITabRenderer
            {
                void OnTabSelected(GameObject ui);
                void OnTabDeselected(GameObject ui);
            }

        }
    }
}
