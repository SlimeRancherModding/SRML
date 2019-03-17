using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using SRML;
using Harmony;
namespace SampleMod
{
    public class Main : ModEntryPoint
    {
        public override void PreLoad(HarmonyInstance instance)
        {
            Debug.Log("We did it!");
        }

        public override void PostLoad()
        {
            Debug.Log("We did it! Again!");
        }
    }
}
