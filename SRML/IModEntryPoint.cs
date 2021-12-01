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
        /// Called before MainScript.Awake<br/>
        /// You want to register new things and enum values here, as well as do all your harmony patching
        /// </summary>
        void PreLoad();

        /// <summary>
        /// Called before MainScript.Start<br/>
        /// Used for registering things that require a loaded gamecontext
        /// </summary>
        void Load();

        /// <summary>
        /// Called after all mods Load's have been called<br/>
        /// Used for editing existing assets in the game, not a registry step
        /// </summary>
        void PostLoad();

        /// <summary>
        /// Called when the reload command/button is used<br/>
        /// Configs are reloaded right before this.
        /// </summary>
        void ReLoad();

        /// <summary>
        /// Called when the game is exited
        /// </summary>
        void UnLoad();

        /// <summary>
        /// Called every frame, if <see cref="SRModLoader.CurrentLoadingStep"/> equals <see cref="SRModLoader.LoadingStep.FINISHED"/>
        /// </summary>
        void Update();

        /// <summary>
        /// Called every fixed frame-rate frame, if <see cref="SRModLoader.CurrentLoadingStep"/> equals <see cref="SRModLoader.LoadingStep.FINISHED"/>
        /// </summary>
        void FixedUpdate();

        /// <summary>
        /// Called every frame after all mods' Update functions have been called, if <see cref="SRModLoader.CurrentLoadingStep"/> equals <see cref="SRModLoader.LoadingStep.FINISHED"/>
        /// </summary>
        void LateUpdate();
    }

    public abstract class ModEntryPoint : IModEntryPoint
    {
        public Harmony HarmonyInstance => HarmonyPatcher.GetInstance();

        public virtual void Load()
        {
        }

        public virtual void PostLoad()
        {
        }

        public virtual void PreLoad()
        {
        }

        public virtual void ReLoad()
        {
        }

        public virtual void UnLoad()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void FixedUpdate()
        {
        }

        public virtual void LateUpdate()
        {
        }
    }
}
