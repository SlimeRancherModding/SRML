using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace SRML.SR.UI.Utils
{
    public static class UIUtils
    {
        public static void FixStyles(GameObject h)
        {
            foreach (var v in h.GetComponentsInChildren<Component>())
            {
                if (v.GetComponent<Scrollbar>())
                {
                    v.gameObject.AddComponent<ScrollbarStyler>().styleName =
                "Default";

                }
                else if (v.GetComponent<Text>())
                {
                    v.gameObject.AddComponent<TextStyler>().styleName = "Default";
                }
                else if (v.GetComponent<Image>() && !v.GetComponent<Button>())
                {
                    v.gameObject.AddComponent<PanelStyler>().styleName = "Default";
                }


            }

        }
    }
}
