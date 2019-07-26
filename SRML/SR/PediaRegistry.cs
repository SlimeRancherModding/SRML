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
        internal static Dictionary<PediaTab, SRMod> customTabs = new Dictionary<PediaTab, SRMod>();

        internal static IPediaRenderer activeRenderer;
        internal static ITabRenderer activeTabRenderer;

        static Dictionary<DisplaySetting, DefaultPediaRenderer> defaultRenderers = new Dictionary<DisplaySetting, DefaultPediaRenderer>();
        
        static PediaRegistry()
        {
            ModdedIDRegistry.RegisterIDRegistry(moddedIds);
        }

        public static IPediaRenderer GetDefaultRenderer(DisplaySetting setting)
        {
            if(!defaultRenderers.TryGetValue(setting, out var renderer))
            {
                renderer = new DefaultPediaRenderer(setting);
                defaultRenderers.Add(setting, renderer);
                
            }
            return renderer;
        }
        public static PediaDirector.Id CreatePediaId(object value, string name)
        {
            if (SRModLoader.CurrentLoadingStep > SRModLoader.LoadingStep.PRELOAD)
                throw new Exception("Can't register slimepedia entries outside of the PreLoad step");
            return moddedIds.RegisterValueWithEnum((PediaDirector.Id)value, name);
        }

        public static void RegisterPediaTab(PediaTab tab)
        {
            customTabs.Add(tab, SRMod.GetCurrentMod());
        }

        public static void RegisterIdEntry(PediaDirector.IdEntry entry)
        {
            customEntries.Add(entry);
        }
        
        public static void RegisterIdEntry(PediaDirector.Id id, Sprite icon)
        {
            RegisterIdEntry(new PediaDirector.IdEntry() { id = id, icon = icon });
        }
        
        public static void RegisterRenderer(PediaDirector.Id id, IPediaRenderer renderer)
        {
            customPediaRenderers.Add(id, renderer);
        }


        internal static IPediaRenderer GetRenderer(PediaDirector.Id id)
        {
            if(!customPediaRenderers.TryGetValue(id, out var renderer))
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

        public static void RegisterInitialPediaEntry(PediaDirector.Id id)
        {
            initialEntries.Add(id);
        }
        
        public static void RegisterIdentifiableMapping(PediaDirector.IdentMapEntry entry)
        {
            customIdentifiableLinks.Add(entry);
        }

        public static void RegisterIdentifiableMapping(PediaDirector.Id pedia, Identifiable.Id ident)
        {
            RegisterIdentifiableMapping(new PediaDirector.IdentMapEntry() { identId = ident, pediaId = pedia });
        }

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

        public static void SetPediaCategory(PediaDirector.Id id, PediaCategory category)
        {
            var cat = GetCategory(category).ToList();
            cat.Add(id);
            GetCategory(category) = cat.ToArray();
        }
 
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

            public virtual string GetLowerName()
            {
                return CurrentID.ToString().ToLower();
            }
            
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
