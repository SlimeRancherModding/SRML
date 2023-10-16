using MonomiPark.SlimeRancher.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.Console.Commands
{
    public class SpawnCommand : ConsoleCommand
    {
        public override string ID => "spawn";

        public override string Usage => "spawn <id> [count]";

        public override string Description => "Spawns actor";

        public override bool Execute(string[] args)
        {
            if(args.Length < 1 || args.Length > 2)
            {
                Console.Instance.LogError("Incorrect number of arguments!");
                return false;
            }

            Identifiable.Id id;
            GameObject prefab;
            try
            {
                id = (Identifiable.Id)Enum.Parse(typeof(Identifiable.Id), args[0], true);
                prefab = GameContext.Instance.LookupDirector.GetPrefab(id);
            }
            catch
            {
                Console.Instance.LogError("Invalid ID!");
                return false;
            }

            int count = 0;

            if (args.Length != 2 || !Int32.TryParse(args[1], out count)) count = 1;

            for(int i = 0; i < count; i++)
            {

                

                if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out var hit))
                {
                    var spawned = SRBehaviour.InstantiateActor(prefab, SceneContext.Instance.Player.GetComponent<RegionMember>().setId, true);
                    spawned.transform.position = hit.point+hit.normal*PhysicsUtil.CalcRad(spawned.GetComponent<Collider>());
                    var delta = -(hit.point - Camera.main.transform.position).normalized;
                    var newForward = (delta - Vector3.Project(delta, hit.normal)).normalized;
                    spawned.transform.rotation = Quaternion.LookRotation(delta, hit.normal);
                }
            }
            return true;

        }

        protected override bool ArgsOutOfBounds(int argCount, int min = 0, int max = 0)
        {
            return base.ArgsOutOfBounds(argCount, min, max);
        }

        public override List<string> GetAutoComplete(int argIndex, string argText)
        {
            if(argIndex == 0)
            {
                return GameContext.Instance.LookupDirector.identifiablePrefabs.Select(x=>Identifiable.GetId(x).ToString()).ToList();
            }
            return base.GetAutoComplete(argIndex, argText);
        }
    }
}
