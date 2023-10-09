using SRML.SR;
using SRML.SR.Templates;
using UnityEngine;

namespace SRML.SR.Templates
{
    /// <summary>
    /// Interface used to make lists of Mod Prefabs (as lists can't have different generic constructs)
    /// </summary>
    public interface IModPrefab
    {
        GameObject ToPrefab();
        GameObjectTemplate AsTemplate();
        GameObjectTemplate AsTemplateClone();
    }

    /// <summary>
    /// Simple prefab like class, used to make the structure for templates
    /// </summary>
    public abstract class ModPrefab<T> : IModPrefab where T : ModPrefab<T>
    {
        protected GameObjectTemplate mainObject;
        private GameObject prefabVersion = null;

        private event System.Action<GameObjectTemplate> PrefabFunction;

        public ModPrefab(string name)
        {
            mainObject = new GameObjectTemplate(name);
        }

        /// <summary>
        /// Creates the object of the template (To get the prefab version use .ToPrefab() after calling this)
        /// </summary>
        public abstract T Create();

        /// <summary>
        /// Adds a new start action to the pile
        /// </summary>
        /// <param name="actionID">ID of the action (as registered in TemplateActions)</param>
        public T AddStartAction(string actionID)
        {
            mainObject.AddStartAction(actionID);
            return (T)this;
        }

        /// <summary>
        /// Adds a new awake action to the pile
        /// </summary>
        /// <param name="actionID">ID of the action (as registered in TemplateActions)</param>
        public T AddAwakeAction(string actionID)
        {
            mainObject.AddAwakeAction(actionID);
            return (T)this;
        }

        /// <summary>
        /// Adds a prefab function (they are executed when ToPrefab is called)
        /// </summary>
        /// <param name="action">Action to add to the pile</param>
        public T AddPrefabFunction(System.Action<GameObjectTemplate> action)
        {
            PrefabFunction += action;
            return (T)this;
        }

        /// <summary>
        /// Adds a prefab function (they are executed when ToPrefab is called)
        /// </summary>
        /// <param name="actions">Actions to add to the pile</param>
        public T AddPrefabFunction(params System.Action<GameObjectTemplate>[] actions)
        {
            foreach (System.Action<GameObjectTemplate> action in actions)
                PrefabFunction += action;

            return (T)this;
        }

        /// <summary>
        /// Sets the translation for the prefab, not all templates implement this, so it might do nothing.
        /// </summary>
        /// <param name="name">The translated name</param>
        public virtual T SetTranslation(string name)
        {
            return (T)this;
        }

        /// <summary>
        /// Turns this ModPrefab/Template into a runtime prefab
        /// </summary>
        public GameObject ToPrefab()
        {
            if (prefabVersion == null)
            {
                PrefabFunction?.Invoke(mainObject);
                prefabVersion = mainObject.ToGameObject(null);
            }

            return prefabVersion;
        }

        /// <summary>
        /// Returns this prefab as a template
        /// </summary>
        public GameObjectTemplate AsTemplate()
        {
            return mainObject;
        }

        /// <summary>
        /// Returns this prefab as a clone of the template
        /// </summary>
        public GameObjectTemplate AsTemplateClone()
        {
            return mainObject.Clone();
        }
    }
}
