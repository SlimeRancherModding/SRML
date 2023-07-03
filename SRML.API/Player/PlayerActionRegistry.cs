using InControl;
using SRML.Core;
using SRML.Core.API.BuiltIn;
using SRML.SR;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SRML.API.Player
{
    public class PlayerActionRegistry : GenericRegistry<PlayerActionRegistry, PlayerAction>
    {
        internal List<PlayerAction> registeredBindings = new List<PlayerAction>();

        public void RegisterBindingLine(PlayerAction toRegister) => registeredBindings.Add(toRegister);
        public void AddTranslation(PlayerAction toRegister, MessageDirector.Lang lang, string translated) => 
            CoreTranslator.Instance.AddUITranslation(lang, $"key.{toRegister.Name.ToLowerInvariant()}", translated);

        public void RegisterAllActions<T>() => RegisterAllActions(typeof(T));
        
        public void RegisterAllActions(Type t)
        {
            foreach (FieldInfo f in t.GetFields())
            {
                if (!f.FieldType.IsInstanceOfType(typeof(PlayerAction)))
                    continue;

                object action = f.GetValue(null);
                if (action != null)
                    Register((PlayerAction)action);
            }
        }

        public override void Initialize()
        {
        }
    }
}
