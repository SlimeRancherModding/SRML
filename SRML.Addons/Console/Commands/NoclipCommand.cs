using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.Console.Commands
{
    class NoclipCommand : ConsoleCommand
    {
        public override string ID => "noclip";

        public override string Usage => "noclip";

        public override string Description => "toggles noclip";

        public int originalLayer;

        public override bool Execute(string[] args)
        {
            bool noclipState = SceneContext.Instance.Player.GetComponentInChildren<vp_FPController>().MotorFreeFly;
            SceneContext.Instance.Player.GetComponentInChildren<vp_FPController>().MotorFreeFly = !noclipState;
            if (!noclipState) originalLayer = SceneContext.Instance.Player.layer;
            SceneContext.Instance.Player.layer = noclipState ? (originalLayer) : LayerMask.NameToLayer("RaycastOnly");
            return true;
        }
    }
}
