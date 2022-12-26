using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using SRML.SR;
using SRML.SR.UI.Utils;
using TMPro;
using UnityEngine;

namespace SRML
{
    /// <summary>
    /// Class used to generate an error message 
    /// </summary>
    internal class ErrorGUI : BaseUI
    {
        public static List<ModLoadException> errors = new List<ModLoadException>();
        public static GameObject extendedUI;
        public static GameObject extendedError;

        public Transform errorContainer;
        public bool fallback;

        public static void TryCreateExtendedError()
        {
            SRCallbacks.OnMainMenuLoaded += u =>
            {
                GameObject gui = null;
                try
                {
                    gui = u.InstantiateAndWaitForDestroy(extendedUI);
                    ErrorGUI eg = gui.GetComponent<ErrorGUI>();
                    foreach (ModLoadException exception in errors)
                        Instantiate(extendedError, eg.errorContainer).GetComponent<IndividualExceptionUI>().GenerateMessage(exception);
                }
                catch
                {
                    if (gui?.GetComponent<ErrorGUI>() != null)
                        gui.GetComponent<ErrorGUI>().fallback = true;
                    gui?.Destroy();

                    UnityEngine.Debug.LogError("Fatal error encountered, unable to display extended error information. Displaying basic information ...");
                    CreateBasicError($"{errors[0].Message}:\n{errors[0].InnerException}\n--- End of inner exception stack trace ---", u);
                }
            };
        }

        public static void CreateBasicError(string error, MainMenuUI menu, bool doAbort = true)
        {
            var mainmen = MainMenuUtils.DisplayBlankPanelWithTranslation<BaseUI>(menu, "ErrorUI", "t.srml_error", () => Application.Quit());
            var panel = mainmen.transform.GetChild(0);
            var title = panel.Find("Title").gameObject;
            var g = Instantiate(title);
            g.name = "ErrorText";
            MonoBehaviour.Destroy(g.GetComponent<XlateText>());
            g.GetComponent<TMP_Text>().text = error;
            g.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Top;
            g.GetComponent<TMP_Text>().fontSize *= .8f;
            g.GetComponent<TMP_Text>().enableWordWrapping = true;

            mainmen.AddComponent<ErrorGUI>();

            if (doAbort)
            {
                var h = GameObject.Instantiate(title);
                h.name = "AbortText";
                h.GetComponent<XlateText>().SetKey("t.srml_error.abort");
                h.GetComponent<TMP_Text>().enableWordWrapping = true;
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
            rect.anchorMax = new Vector2(1, 1);
            rect.offsetMax = new Vector2(-50, -100);
            rect.offsetMin = new Vector2(50, 30);
        }

        public static void CreateBasicErrorOnMenu(string error, bool doAbort = true)
        {
            SRCallbacks.OnMainMenuLoaded += (u) =>
            {
                CreateBasicError(error, u, doAbort);
            };
        }

        public void OpenModsFolder() => Process.Start(Path.GetFullPath(FileSystem.ModPath));

        public void OpenLogsFolder() => Process.Start(Path.GetFullPath(Main.StorageProvider.SavePath()));

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (!fallback)
                Application.Quit();
        }
    }

    internal class IndividualExceptionUI : MonoBehaviour
    {
        public TMP_Text title;
        public TMP_Text extended;
        public UnityEngine.UI.Image arrow;
        public Sprite down;
        public Sprite right;

        private bool isExtended = false;
        private string titleText;

        public void GenerateMessage(ModLoadException exception)
        {
            titleText = $"{exception.InnerException.GetType().Name} thrown when {ModLoadException.LoadingStepToVerb(exception.step)} '{exception.modId}'";
            title.SetText($"{titleText}");
            extended.SetText(exception.InnerException.ToString());
        }

        public void ExtendOrRetract()
        {
            isExtended = !isExtended;
            extended.gameObject.SetActive(isExtended);
            arrow.sprite = isExtended ? down : right;
        }
    }
}
