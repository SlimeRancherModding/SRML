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
                var mainmen = MainMenuUtils.DisplayBlankPanelWithTranslation<BaseUI>(u, "ErrorUI", "t.srml_error", () => Application.Quit());
                var panel = mainmen.transform.GetChild(0);
                var title = panel.Find("Title").gameObject;
                var g = GameObject.Instantiate(title);
                g.name = "ErrorText";
                MonoBehaviour.Destroy(g.GetComponent<XlateText>());
                g.GetComponent<TMP_Text>().text = error;
                g.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Top;
                g.GetComponent<TMP_Text>().fontSize *= .8f;
                g.GetComponent<TMP_Text>().enableWordWrapping = true;
                if (doAbort)
                {
                    var h = GameObject.Instantiate(title);
                    h.name = "AbortText";
                    h.GetComponent<TMP_Text>().enableWordWrapping = true;

                    h.GetComponent<XlateText>().SetKey("t.srml_abort");
                    h.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Bottom;

                    h.transform.SetParent(panel, false);
                    var rect2 = h.GetComponent<RectTransform>();
                    rect2.anchorMin = new Vector2(0, 0);
                    rect2.anchorMax = new Vector2(1, 1);
                    rect2.offsetMax = new Vector2(-50, -100);
                    rect2.offsetMin = new Vector2(50, 30);
                   
                }

                g.transform.SetParent(panel, false);
                var rect = g.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(1,1);
                rect.offsetMax = new Vector2(-50, -100);
                rect.offsetMin = new Vector2(50,30);
            });
        }
    }

    
}
