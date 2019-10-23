using SRML.SR.Utils.Debug;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SRML.SR.Utils.Debug
{
    public class DebugSystem : MonoBehaviour
    {
        private RaycastHit mainHit;

        public void Update()
        {
            if (!DebugCommand.DebugMode)
                return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            bool def = Physics.queriesHitTriggers;
            Physics.queriesHitTriggers = true;

            foreach (RaycastHit hit in Physics.RaycastAll(ray, 5f))
            {
                if (hit.collider.GetComponent<DebugMarker>() != null)
                    hit.collider.GetComponent<DebugMarker>().SetHover();
            }

            Physics.queriesHitTriggers = def;

            Physics.Raycast(ray, out mainHit, 3000f);
        }

        public void FixedUpdate()
        {

        }

        public void OnGUI()
        {
            if (!DebugCommand.DebugMode)
                return;

            GUILayout.BeginArea(new Rect(15, 200, 450, Screen.height - 400));

            // Title
            GUILayout.Label("<b>DEBUG MODE ACTIVE</b>");
            GUILayout.Space(5);

            // SCENE CONTEXT STUFF
            if (SceneManager.GetActiveScene().name.Equals("worldGenerated"))
            {
                // Vars
                ZoneDirector.Zone zone = SceneContext.Instance.PlayerZoneTracker.GetCurrentZone();

                // Player Info
                GUILayout.Label($"<b>Position: </b>{SceneContext.Instance.Player.transform.position}");
                GUILayout.Label($"<b>Zone: </b>{zone}");
                GUILayout.Label($"<b>Map Unlocked: </b>{SceneContext.Instance.PlayerState.HasUnlockedMap(zone)}");
                GUILayout.Label($"<b>EndGame Time: </b>{SceneContext.Instance.PlayerState.GetEndGameTime()}");
                GUILayout.Label($"<b>Ammo Mode: </b>{SceneContext.Instance.PlayerState.GetAmmoMode()}");
                GUILayout.Space(5);

                // View Raycast
                GUILayout.Label($"<b>Look At: </b>{mainHit.point}");
                GUILayout.Label($"<b>Object: </b>{mainHit.collider?.name ?? "null"}");
                GUILayout.Label($"<b>Parent: </b>{mainHit.collider?.transform.parent.name ?? "null"}");
            }

            GUILayout.EndArea();
        }
    }
}
