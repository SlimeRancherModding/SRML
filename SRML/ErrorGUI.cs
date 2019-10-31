using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SRML.SR;
using SRML.SR.UI.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SRML
{
    /// <summary>
    /// Class used to generate an error message 
    /// </summary>
    internal class ErrorGUI : MonoBehaviour
    {


        public static void CreateError(string error, bool doAbort=true)
        {
            SRCallbacks.OnMainMenuLoaded+=((u) =>
            {


                var mainmen = MainMenuUtils.DisplayBlankPanel<BaseUI>(u, "SRML ERROR", () => Application.Quit());
                var g = GameObject.Instantiate(mainmen.transform.GetChild(0).Find("Title").gameObject);
                MonoBehaviour.Destroy(g.GetComponent<XlateText>());
                g.GetComponent<TMP_Text>().text = error;
                g.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Top;
                g.GetComponent<TMP_Text>().fontSize *= .8f;
                g.GetComponent<TMP_Text>().enableWordWrapping = true;
                if (doAbort)
                {
                    var h = GameObject.Instantiate(mainmen.transform.GetChild(0).Find("Title").gameObject);
                    MonoBehaviour.Destroy(h.GetComponent<XlateText>());
                    h.GetComponent<TMP_Text>().enableWordWrapping = true;

                    h.GetComponent<TMP_Text>().text = "Aborting mod loading...";
                    h.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Bottom;

                    h.transform.SetParent(mainmen.transform.GetChild(0), false);
                    var rect2 = h.GetComponent<RectTransform>();
                    rect2.anchorMin = new Vector2(0, 0);
                    rect2.anchorMax = new Vector2(1, 1);
                    rect2.offsetMax = new Vector2(-50, -100);
                    rect2.offsetMin = new Vector2(50, 30);
                   
                }

                g.transform.SetParent(mainmen.transform.GetChild(0), false);
                var rect = g.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(1,1);
                rect.offsetMax = new Vector2(-50, -100);
                rect.offsetMin = new Vector2(50,30);
            });
        }
    }

    
}
