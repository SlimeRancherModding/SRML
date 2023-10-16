using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SRML.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace SRML.SR.UI.Utils
{
    public static class MainMenuUtils
    {
        public static GameObject DisplayBlankPanel<T>(MainMenuUI mainMenu, string title, Action onClose = null) where T : BaseUI
        {
            var h = GameObject.Instantiate(mainMenu.optionsUI);
            h.name = title;
            Component.DestroyImmediate(h.GetComponent<OptionsUI>());

            for (int i = 0; i < h.transform.GetChild(0).childCount; i++)
            {
                var v = h.transform.GetChild(0).GetChild(i).gameObject;
                if (v.name == "CloseButton")
                {
                    v.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
                    v.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        GameObject.Destroy(h);
                        onClose?.Invoke();
                    });
                }
                else if (v.name == "Title" && title != null)
                {
                    GameObject.DestroyImmediate(v.GetComponent<XlateText>());
                    v.GetComponent<TMP_Text>().text = title;
                }
                else
                {
                    GameObject.Destroy(v);
                }
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

        public static GameObject DisplayBlankPanelWithTranslation<T>(MainMenuUI mainMenu, string name, string key, Action onClose = null) where T : BaseUI
        {
            var h = GameObject.Instantiate(mainMenu.optionsUI);
            h.name = name;
            Component.DestroyImmediate(h.GetComponent<OptionsUI>());

            for (int i = 0; i < h.transform.GetChild(0).childCount; i++)
            {
                var v = h.transform.GetChild(0).GetChild(i).gameObject;
                if (v.name == "CloseButton")
                {
                    v.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
                    v.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        GameObject.Destroy(h);
                        onClose?.Invoke();
                    });
                }
                else if (v.name == "Title" && key != null)
                {
                    v.GetComponent<XlateText>().SetKey(key);
                }
                else
                {
                    GameObject.Destroy(v);
                }
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

        public static GameObject AddMainMenuButton(MainMenuUI mainMenu, string text, Action onClicked)
        {
            var mode = mainMenu.transform.Find("StandardModePanel/OptionsButton");
            var g = GameObject.Instantiate(mode.gameObject);
            g.name = text;
            g.transform.SetParent(mode.parent, false);
            g.transform.localPosition = new Vector3(0, 0);
            MonoBehaviour.Destroy(g.GetComponent<XlateText>());
            var button = g.GetComponent<Button>();
            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener(new UnityAction(onClicked));

            g.GetComponentInChildren<TMP_Text>().text = text;
            return g;
        }

        public static GameObject AddMainMenuButtonWithTranslation(MainMenuUI mainMenu, string name, string key, Action onClicked)
        {
            var mode = mainMenu.transform.Find("StandardModePanel/OptionsButton");
            var g = GameObject.Instantiate(mode.gameObject);
            g.name = name;
            g.transform.SetParent(mode.parent, false);
            g.transform.localPosition = new Vector3(0, 0);
            var button = g.GetComponent<Button>();
            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener(new UnityAction(onClicked));

            g.GetComponentInChildren<XlateText>().SetKey(key);
            return g;
        }
    }
}
