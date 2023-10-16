using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.Console.Commands
{
    class KillAllCommand : ConsoleCommand
    {
        public override string ID => "killall";

        public override string Usage => "killall [radius/type]";

        public override string Description => "Kills all of a type in a radius, or just all";

        public override bool Execute(string[] args)
        {
            int radius = -1;
            List<Identifiable.Id> toKill = new List<Identifiable.Id>(); 
            foreach(var v in args ?? new string[0])
            {
                if(uint.TryParse(v,out uint rad))
                {
                    radius = (int)rad;
                    continue;
                }
                else try
                    {
                        toKill.Add((Identifiable.Id)Enum.Parse(typeof(Identifiable.Id), v, true));
                    }
                    catch
                    {

                    }
                    
            }


            List<GameObject> toDestroy = new List<GameObject>();
            foreach(var v in SceneContext.Instance.GameModel.AllActors().Where(x=>radius==-1||Vector3.Distance(x.Value?.transform?.position ?? SceneContext.Instance.PlayerState.model.position, SceneContext.Instance.PlayerState.model.position)<radius))
            {
                if (toKill.Count == 0 || toKill.Contains(v.Value.ident))
                {
                    toDestroy.Add(v.Value?.transform?.gameObject);

                }
            }
            int deletedCount = 0;
            foreach (var v in toDestroy)
            {
                if (v)
                {
                    deletedCount++;
                    DeathHandler.Kill(v, DeathHandler.Source.UNDEFINED, SceneContext.Instance.Player, "KillAllCommand.Execute");
                }
            }
            Console.Instance.LogSuccess($"Destroyed {deletedCount} actors!");
            return true;
        }

        public override List<string> GetAutoComplete(int argIndex, string argText)
        {
            return Enum.GetNames(typeof(Identifiable.Id)).ToList();
        }
    }
}
