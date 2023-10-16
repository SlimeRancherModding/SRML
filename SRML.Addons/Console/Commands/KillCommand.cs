using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.Console.Commands
{
    class KillCommand : ConsoleCommand
    {
        public override string ID => "kill";

        public override string Usage => "kill";

        public override string Description => "Kills what you're looking at";

        public override bool Execute(string[] args)
        {
            if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out var hit))
            {
                var gameobject = hit.collider.gameObject;
                if (gameobject.GetComponent<Identifiable>())
                {
                    DeathHandler.Kill(gameobject, DeathHandler.Source.UNDEFINED, SceneContext.Instance.Player, "RemoveCommand.Execute");
                    
                }
                else if (gameobject.GetComponentInParent<Gadget>())
                {
                    gameobject.GetComponentInParent<Gadget>().DestroyGadget();
                }
                else if (gameobject.GetComponentInParent<LandPlot>())
                {
                    gameobject.GetComponentInParent<LandPlotLocation>().Replace(gameobject.GetComponentInParent<LandPlot>(), GameContext.Instance.LookupDirector.GetPlotPrefab(LandPlot.Id.EMPTY));
                }
            }
            Console.Instance.LogError("Not looking at a valid object!");
            return false;
        }
    }
}
