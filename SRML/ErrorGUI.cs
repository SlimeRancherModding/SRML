using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML
{
    internal class ErrorGUI : MonoBehaviour
    {
        public String errorString;
        private Rect windowRect=new Rect(0,0,Screen.width/4,Screen.height/4);
        void OnGUI()
        {

            GUILayout.Window(1234, windowRect, DrawError,"Mod Loader Error");
            windowRect.x = (int) (Screen.width * .5f - windowRect.width * .5f);
            windowRect.y = (int)(Screen.height * .5f - windowRect.height * .5f);
            GUILayout.Window(1234, windowRect, DrawError, "Mod Loader Error");
        }

        void DrawError(int id)
        {
            GUILayout.Label(errorString);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Ok"))
            {
                Destroy(this.gameObject);
                Debug.Log("Destroyed " +errorString);
            }
        }

        public static GameObject CreateError(string error)
        {
            var g = new GameObject();
            g.AddComponent<ErrorGUI>().errorString = error;
            return g;
        }
    }

    
}
