using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;

namespace SRML
{
    public interface IModEntryPoint
    {
        /// <summary>
        /// Called before <see cref="GameContext.Awake"/>; few assets are loaded at this time.<br />
        /// Useful for setting up things required by the Load step like enums and translations.<br />
        /// Any <see cref="Identifiable.Id"/> or <see cref="Gadget.Id"/> registered here will automatically be categorized unless categorized manually.
        /// </summary>
        void PreLoad();

        /// <summary>
        /// Called before <see cref="GameContext.Start"/>.<br/>
        /// Useful for when you need to reference game assets and registries, like <see cref="LookupDirector"/>.<br />
        /// Every registered <see cref="SlimeDiet.EatMap"/> is recalculated after this step
        /// </summary>
        void Load();

        /// <summary>
        /// Called after <see cref="GameContext.Start"/>.<br/>
        /// Useful for interaction between mods.
        /// </summary>
        void PostLoad();
    }

    public abstract class ModEntryPoint : IModEntryPoint
    {
        /// <summary>
        /// An automatically-generated Harmony instance with the id "net.[author].[modid]".
        /// </summary>
        public Harmony HarmonyInstance => HarmonyPatcher.GetInstance();

        /// <summary>
        /// A console instance with the name set in the mod info.
        /// </summary>
        public Console.Console.ConsoleInstance ConsoleInstance { get; internal set; }

        public virtual void PreLoad()
        {
        }

        public virtual void Load()
        {
        }

        public virtual void PostLoad()
        {
        }

        /// <summary>
        /// Runs after config gets reloaded with the reload command.
        /// </summary>
        public virtual void Reload()
        {
        }

        /// <summary>
        /// Runs on <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnApplicationQuit.html">OnApplicationQuit</seealso>
        /// </summary>
        public virtual void Unload()
        {
        }

        /// <summary>
        /// Runs on <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html">Update</seealso>
        /// </summary>
        public virtual void Update()
        {
        }

        /// <summary>
        /// Runs on <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.FixedUpdate.html">FixedUpdate</seealso>
        /// </summary>
        public virtual void FixedUpdate()
        {
        }

        /// <summary>
        /// Runs on <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.LateUpdate.html">LateUpdate</seealso>
        /// </summary>
        public virtual void LateUpdate()
        {
        }
    }
}
