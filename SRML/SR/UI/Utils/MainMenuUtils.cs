using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace SRML.SR.UI.Utils
{
    public static class MainMenuUtils
    {
        public static GameObject DisplayBlankPanel<T>(MainMenuUI mainMenu,string title) where T : BaseUI
        {
            var h = GameObject.Instantiate(mainMenu.optionsUI);

            Component.DestroyImmediate(h.GetComponent<OptionsUI>());

            foreach (Transform v in h.transform.GetChild(0))
            {
                if (v.name == "CloseButton")
                {
                    v.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
                    v.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        GameObject.Destroy(h);
                    });
                }
                else if (v.name == "Title"&&title!=null)
                {
                    v.GetComponent<TMP_Text>().text = title;
                }
                else GameObject.Destroy(v.gameObject);
            }


            mainMenu.gameObject.SetActive(false);
            var baseUI = h.AddComponent<T>();
            baseUI.onDestroy += () =>
            {
                if (mainMenu)
                {
                    mainMenu.gameObject.SetActive(true);
                }
            };
            return h;
        }

        public static GameObject AddMainMenuButton(MainMenuUI mainMenu, String text, Action onClicked)
        {
            var mode = mainMenu.transform.Find("StandardModePanel/OptionsButton");
            var g = GameObject.Instantiate(mode.gameObject);
            g.transform.SetParent(mode.parent, false);
            g.transform.localPosition = new Vector3(0, 0);

            var button = g.GetComponent<Button>();
            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener(new UnityAction(onClicked));

            g.GetComponentInChildren<TMP_Text>().text = text;

            return g;
        }


    }

}
