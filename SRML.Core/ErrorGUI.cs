using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SRML.SR;
using SRML.SR.UI.Utils;
using TMPro;
using UnityEngine;

namespace SRML.Core
{
    /// <summary>
    /// Class used to generate an error message 
    /// </summary>
    public class ErrorGUI : BaseUI
    {
        internal static Dictionary<string, (ErrorType, Exception)> errors = new Dictionary<string, (ErrorType, Exception)>();
        internal static GameObject extendedUI;
        internal static GameObject extendedError;

        [SerializeField]
        private Transform errorContainer;
        internal bool fallback;

        public readonly static Dictionary<ErrorType, string> messageForType = new Dictionary<ErrorType, string>()
        {
            { ErrorType.LoadMod, "{0} loading mod {1}" },
            { ErrorType.LoadModInfo, "{0} parsing mod info for {1}" },
            { ErrorType.RegisterModLoader, "{0} registering mod loader of type {1}" },
            { ErrorType.RegisterModType, "{0} registering mod type {1}" }
        };

        public static void TryCreateExtendedError(MainMenuUI ui)
        {
            GameObject gui = null;
            try
            {
                gui = ui.InstantiateAndWaitForDestroy(extendedUI);
                ErrorGUI eg = gui.GetComponent<ErrorGUI>();
                foreach (var exception in errors)
                    Instantiate(extendedError, eg.errorContainer).GetComponent<IndividualExceptionUI>()
                        .GenerateMessage(exception.Value.Item1, exception.Value.Item2, exception.Key);
            }
            catch (Exception e)
            {
                if (gui?.GetComponent<ErrorGUI>() != null)
                    gui.GetComponent<ErrorGUI>().fallback = true;
                gui?.Destroy();

                UnityEngine.Debug.LogError(e);
                UnityEngine.Debug.LogError("Fatal error encountered, unable to display extended error information. Displaying basic information ...");
                UnityEngine.Debug.LogError(errors.First().Value.Item2);
                CreateBasicError($"{errors.First().Value.Item2}", ui);
            }
        }

        public static void CreateBasicError(string error, MainMenuUI menu, bool doAbort = true)
        {
            GameObject mainmen;

            if (CoreTranslator.Instance != null)
                mainmen = MainMenuUtils.DisplayBlankPanelWithTranslation<ErrorGUI>(menu, "ErrorUI", "t.srml_error", () => Application.Quit());
            else
                mainmen = MainMenuUtils.DisplayBlankPanel<ErrorGUI>(menu, "SRML ERROR", () => Application.Quit());

            var panel = mainmen.transform.GetChild(0);
            var title = panel.Find("Title").gameObject;
            var g = Instantiate(title);
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

                if (CoreTranslator.Instance != null)
                    h.GetComponent<XlateText>().SetKey("t.srml_error.abort");
                else
                {
                    Destroy(h.GetComponent<XlateText>());
                    h.GetComponent<TMP_Text>().text = "Aborting mod loading...";
                }
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

        public static void CreateExtendedErrorOnMenu() => SRCallbacks.OnMainMenuLoaded += ui => TryCreateExtendedError(ui);

        public static void CreateBasicErrorOnMenu(string error, bool doAbort = true)
        {
            SRCallbacks.OnMainMenuLoaded += (u) =>
            {
                CreateBasicError(error, u, doAbort);
            };
        }

        public static void AddError(string id, ErrorType step, Exception e)
        {
            int rec = 1;
            while (errors.ContainsKey($"{id} ({rec})"))
                rec++;
            errors.Add($"{id} ({rec})", (step, e));
        }

        public void OpenModsFolder() => Process.Start(Path.GetFullPath(FileSystem.ModPath));

        public void OpenLogsFolder() => Process.Start(Path.GetFullPath(Main.StorageProvider.SavePath()));

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (!fallback)
                Application.Quit();
        }

        public enum ErrorType
        {
            LoadModInfo,
            LoadMod,
            RegisterModType,
            RegisterModLoader,
            LoadSRML
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

        public void GenerateMessage(ErrorGUI.ErrorType type, Exception exception, string id)
        {
            titleText = string.Format(ErrorGUI.messageForType[type], exception.GetType(), id ?? "<unknown>");
            title.SetText($"{titleText}");
            extended.SetText(exception.ToString());

            Console.Console.Instance.LogError(exception);
        }

        public void GenerateMessageWithoutId(ErrorGUI.ErrorType type, Exception exception) => GenerateMessage(type, exception, "<unknown>");

        public void ExtendOrRetract()
        {
            isExtended = !isExtended;
            extended.gameObject.SetActive(isExtended);
            arrow.sprite = isExtended ? down : right;
        }
    }
}
