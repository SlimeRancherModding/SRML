using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SRML.Core.ModLoader;
using System.Linq;
using SRML.Core.ModLoader.BuiltIn.ModInfo;

namespace SRML.Console
{
    /// <summary>
    /// Draws the window for the in-game console
    /// </summary>
    public class ConsoleWindow : MonoBehaviour
    {
        // CONTROL VARIABLES
        internal static bool updateDisplay = false;
        private static bool showWindow = false;
        private static bool focus = false;
        private static bool autoComplete = false;
        private static bool hasAlreadyPaused = false;
        private static SRInput.InputMode previousInput;

        // TEXT VARIABLES
        private static readonly string cmdName = "cmdLine";
        private static readonly string openUnityLog = "Unity Log";
        private static readonly string openSRMLLog = "SRML Log";
        private static readonly string toggleAutoAC = " Auto-Open Auto Complete";
        private static readonly string commands = "<b><size=16>Command Menu</size></b>";
        private static readonly string acTitle = "Auto Complete";

        // SCROLL VIEWS
        private static Vector2 consoleScroll = Vector2.zero;
        private static Vector2 commandScroll = Vector2.zero;
        private static Vector2 aCompleteScroll = Vector2.zero;

        // TEXTS TO DISPLAY
        internal static string cmdsText = string.Empty;
        internal static string modsText = string.Empty;
        internal static string fullText = string.Empty;
        internal static string cmdText = string.Empty;

        // STYLE CONTROL
        private static GUIStyle textArea;
        private static GUIStyle window;
        private static readonly Font consoleFont = Font.CreateDynamicFontFromOSFont(new string[] { "Lucida Console", "Monaco" }, 13);

        // DESIGN VARIABLES
        private static Vector2 oldRes = new Vector2(Screen.width, Screen.height);
        private static Vector2 windowSize = new Vector2(Screen.width + 20, Screen.height / 2);
        private static Vector2 windowPosition = new Vector2(-10, -20);
        private static Rect windowRect = new Rect(windowPosition, windowSize);

        private static Rect leftGroup = new Rect(15, 25, windowRect.width - 290, windowRect.height - 30);
        private static Rect rightGroupA = new Rect(leftGroup.width + 25, leftGroup.y, 250, 100);
        private static Rect rightGroupB = new Rect(leftGroup.width + 25, rightGroupA.y + rightGroupA.height + 5, 250, leftGroup.height - rightGroupA.height - 10);

        private static float cmdLineY = leftGroup.height - 25;
        private static Rect cmdRect = new Rect(0, cmdLineY, leftGroup.width, 20);

        private static GUIContent FullContent => new GUIContent(fullText.ToString());
        private static Vector2 TextSize => textArea?.CalcSize(FullContent) ?? Vector2.zero;

        private static Rect oRect = new Rect(0, 0, leftGroup.width, cmdLineY - 5);
        private static Rect sRect = new Rect(5, 7, oRect.width - 15, cmdLineY - 20);
        private static Rect tRect = new Rect(0, 0, 0, 0);

        private static readonly Rect ulRect = new Rect(10, 7, rightGroupA.width - 20, 25);
        private static readonly Rect slRect = new Rect(10, 37, rightGroupA.width - 20, 25);
        private static readonly Rect tacRect = new Rect(10, 67, rightGroupA.width - 20, 25);
        private static readonly Rect cmRect = new Rect(10, 7, rightGroupB.width - 20, 25);

        private static Rect csRect = new Rect(10, 37, rightGroupB.width - 15, rightGroupB.height - 45);
        private static Rect caRect = new Rect(0, 0, csRect.width - 20, (30 * Console.cmdButtons.Count) + 5);

        private static Rect btnRect = new Rect(0, 5, caRect.width - 5, 25);

        // UI RAYCASTERS (TO DISABLE UI INTEREACTION)
        private static GraphicRaycaster[] cachedCasters;

        // HISTORY INDEX
        internal static int currHistory = -1;

        // AUTO COMPLETE - CONTROL VARIABLES
        private static float CursorX => ((TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl)).graphicalCursorPos.x;
        private static bool forceClose = false;
        private static bool justActivated = false;
        private static bool moveCursor = false;
        private static int completeIndex = 0;
        private static string selectComplete = string.Empty;

        private static bool autoAC = true;
        private static bool oldAutoAC = true;

        private static Rect completeRect = new Rect(3, windowSize.y - 25, 200, 300);

        private static Rect acsRect = new Rect(10, 25, completeRect.width - 15, completeRect.height - 30);
        private static Rect intRect = new Rect(5, 0, acsRect.width - 20, 0);
        private static Rect cBtnRect = new Rect(0, 0, intRect.width - 5, 20);

        private static readonly List<string> cachedAC = new List<string>();
        private static string oldCmdText = null;

        /// <summary>
        /// Attachs the window to a scene
        /// </summary>
        public static void AttachWindow(Scene oldScene, Scene newScene)
        {
            GameObject window = new GameObject("_ConsoleWindow", typeof(ConsoleWindow));
            DontDestroyOnLoad(window);

            SceneManager.activeSceneChanged -= AttachWindow;
            Console.Instance.Log("Attached Console Window successfully!");
        }

        // TO ENSURE IT GOT CREATED
        private void Start()
        {
            Console.Instance.Log("Console Window running.");
            Console.Instance.Log("Use command '<color=#77DDFF>help</color>' for a list of all commands");
            Console.Instance.Log("Use command '<color=#77DDFF>mods</color>' for a list of all mods loaded");
            Console.Instance.Log("You can also check the menu on the right");

            foreach (IModInfo info in CoreLoader.Instance.Mods.Select(x => x.ModInfo))
            {
                if (info is BasicModInfo basicInfo)
                    modsText += $"{(modsText.Equals(string.Empty) ? "" : "\n")}<color=#77DDFF>{basicInfo.Name}</color> [<color=#77DDFF>Author:</color> {basicInfo.Author} | <color=#77DDFF>ID:</color> {info.Id} | <color=#77DDFF>Version:</color> {info.Version}]";
                else
                    modsText += $"{(modsText.Equals(string.Empty) ? "" : "\n")}<color=#77DDFF>{info.Id}</color> [<color=#77DDFF>ID:</color> {info.Id} | <color=#77DDFF>Version:</color> {info.Version}]";
            }
        }

        // TO CHECK FOR RESOLUTON CHANGES
        private void Update()
        {
            if (oldRes.x != Screen.width || oldRes.y != Screen.height)
            {
                oldRes = new Vector2(Screen.width, Screen.height);
                windowSize = new Vector2(Screen.width + 20, Screen.height / 2);
                windowRect = new Rect(windowPosition, windowSize);

                leftGroup = new Rect(15, 25, windowRect.width - 190, windowRect.height - 30);
                rightGroupA = new Rect(leftGroup.width + 25, leftGroup.y, 150, 70);
                rightGroupB = new Rect(leftGroup.width + 25, rightGroupA.y + rightGroupA.height + 5, 150, leftGroup.height - rightGroupA.height - 10);

                cmdLineY = leftGroup.height - 25;
                cmdRect = new Rect(0, cmdLineY, leftGroup.width, 20);

                oRect = new Rect(0, 0, leftGroup.width, cmdLineY - 5);
                sRect = new Rect(5, 7, oRect.width - 15, cmdLineY - 20);

                csRect = new Rect(10, 37, rightGroupB.width - 15, rightGroupB.height - 45);
                caRect = new Rect(0, 0, csRect.width - 20, (30 * Console.cmdButtons.Count) + 5);
            }
        }

        // DRAWS THE WINDOW
        private void OnGUI()
        {
            //Font font = GUI.skin.font;
            //GUI.skin.font = consoleFont;

            // UNITY PREVENTS "GUI" STUFF FROM BEING CALLED OUTSIDE "OnGUI"
            if (textArea == null) textArea = new GUIStyle(GUI.skin.label);
            if (window == null) window = new GUIStyle(GUI.skin.window);

            window.active.background = window.normal.background;
            window.hover.background = window.normal.background;
            window.focused.background = window.normal.background;
            window.onActive.background = window.normal.background;
            window.onHover.background = window.normal.background;
            window.onFocused.background = window.normal.background;


            // FORCES WINDOW TO CLOSE IF THE GAME IS LOADING
            if (GameContext.Instance.AutoSaveDirector.IsLoadingGame() && showWindow)
            {
                SetWindowOff();
            }


            // LISTENS TO MAIN INPUT
            if (Event.current.isKey && Event.current.type == EventType.KeyDown)
            {
                // TOGGLES THE WINDOW
                if ((Event.current.modifiers == EventModifiers.Control || Event.current.modifiers == EventModifiers.Command) && Event.current.keyCode == KeyCode.Tab && !GameContext.Instance.AutoSaveDirector.IsLoadingGame())
                {
                    ToggleWindow();
                }
            }

            if (showWindow)
            {
                GUI.backgroundColor = new Color(0, 0, 0, 0.25f);
                GUI.Window(1234567892, new Rect(-20, -20, Screen.width + 20, Screen.height + 20), (id) => { }, string.Empty, window);
                GUI.BringWindowToBack(1234567892);
                GUI.backgroundColor = Color.white;

                GUI.Window(1234567890, windowRect, DrawWindow, string.Empty, window);
                GUI.BringWindowToFront(1234567890);
                GUI.FocusWindow(1234567890);

                if (autoComplete)
                {
                    completeRect.x = 4 + CursorX;
                    GUI.Window(1234567891, completeRect, DrawACWindow, acTitle, window);
                    GUI.BringWindowToFront(1234567891);
                }

                if (Event.current.isKey || Event.current.isMouse || Event.current.isScrollWheel)
                {
                    Event.current.Use();
                }
            }

            //GUI.skin.font = font;
        }

        private void DrawWindow(int id)
        {
            int bFontSize = GUI.skin.button.fontSize;
            int tfFontSize = GUI.skin.textField.fontSize;
            int tFontSize = GUI.skin.toggle.fontSize;
            int lFontSize = GUI.skin.label.fontSize;
            GUI.skin.button.fontSize = consoleFont.fontSize;
            GUI.skin.textField.fontSize = consoleFont.fontSize;
            GUI.skin.toggle.fontSize = consoleFont.fontSize;
            GUI.skin.label.fontSize = consoleFont.fontSize;

            if (cachedAC.Count == 0)
            {
                autoComplete = false;
            }

            // LISTENING TO THE KEY EVENTS
            if (Event.current.isKey && Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.Escape && autoComplete)
                {
                    forceClose = true;
                    autoComplete = false;

                    Event.current.Use();
                    return;
                }
                else if (Event.current.keyCode == KeyCode.Escape && !autoComplete)
                {
                    SetWindowOff();

                    Event.current.Use();
                    return;
                }

                // SUBMITS THE TEXTFIELD
                if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
                {
                    Console.ProcessInput(cmdText.TrimEnd(' '));
                    cmdText = string.Empty;
                    oldCmdText = null;

                    currHistory = -1;
                    completeIndex = 0;
                    autoComplete = false;

                    Event.current.Use();
                    return;
                }

                // AUTO COMPLETE TOGGLE
                if ((Event.current.modifiers == EventModifiers.Control || Event.current.modifiers == EventModifiers.Command) && Event.current.keyCode == KeyCode.Space)
                {
                    forceClose = false;

                    if (Regex.Matches(cmdText, "\"").Count % 2 == 0)
                        autoComplete = true;

                    justActivated = true;
                    oldCmdText = null;

                    Event.current.Use();
                    return;
                }

                // TAB AUTO COMPLETE
                if (Event.current.keyCode == KeyCode.Tab && Event.current.modifiers == EventModifiers.None && autoComplete)
                {
                    if (!selectComplete.Equals(string.Empty))
                    {
                        cmdText = selectComplete;
                        selectComplete = string.Empty;
                        moveCursor = true;

                        autoComplete = false;
                    }

                    Event.current.Use();
                    return;
                }

                // CYCLES HISTORY UP
                if (Event.current.keyCode == KeyCode.UpArrow && !autoComplete && Console.history.Count > 0)
                {
                    if (currHistory == -1)
                    {
                        cmdText = Console.history[Console.history.Count - 1];
                        currHistory = Console.history.Count - 1;
                    }
                    else
                    {
                        if (currHistory > 0)
                        {
                            cmdText = Console.history[currHistory - 1];
                            currHistory -= 1;
                        }
                    }

                    moveCursor = true;
                    Event.current.Use();
                    return;
                }
                else if (Event.current.keyCode == KeyCode.UpArrow && autoComplete)
                {
                    completeIndex--;

                    if (completeIndex < 0)
                        completeIndex = cachedAC.Count - 1;

                    aCompleteScroll.y = (25 * completeIndex) + 5;

                    Event.current.Use();
                    return;
                }

                // CYCLES HISTORY DOWN
                if (Event.current.keyCode == KeyCode.DownArrow && !autoComplete && Console.history.Count > 0)
                {
                    if (currHistory != -1)
                    {
                        if (currHistory < (Console.history.Count - 1))
                        {
                            cmdText = Console.history[currHistory + 1];
                            currHistory += 1;
                        }
                        else
                        {
                            cmdText = string.Empty;
                            currHistory = -1;
                        }
                    }

                    moveCursor = true;
                    Event.current.Use();
                    return;
                }
                else if (Event.current.keyCode == KeyCode.DownArrow && autoComplete)
                {
                    completeIndex++;

                    if (completeIndex > cachedAC.Count - 1)
                        completeIndex = 0;

                    aCompleteScroll.y = (25 * completeIndex) + 5;

                    Event.current.Use();
                    return;
                }

                // TRIGGER AUTO COMPLETE
                if (autoAC && Event.current.keyCode != KeyCode.None && Event.current.keyCode != KeyCode.Space && Event.current.keyCode != KeyCode.Escape && (!cmdText.Equals(oldCmdText) || cmdText.Equals(string.Empty)) && (Event.current.modifiers == EventModifiers.None || Event.current.modifiers == EventModifiers.Shift))
                {
                    if (cmdText.Equals(string.Empty))
                        forceClose = false;

                    autoComplete = !forceClose;
                }

                // FIXES SPACE && BACKSPACE
                if (autoAC && (Event.current.keyCode == KeyCode.Space) && Event.current.modifiers == EventModifiers.None)
                {
                    if (Regex.Matches(cmdText, "\"").Count % 2 == 0)
                        forceClose = false;

                    if (!forceClose)
                        autoComplete = true;
                }
            }

            // CONSOLE AREA
            GUI.BeginGroup(leftGroup);

            GUI.SetNextControlName(cmdName);
            cmdText = GUI.TextField(cmdRect, cmdText);

            if (cmdText.EndsWith(" ") && justActivated)
            {
                cmdText = cmdText.TrimEnd(' ');
                oldCmdText = null;
                justActivated = false;
            }

            if (!focus)
            {
                focus = true;
                GUI.FocusControl(cmdName);
            }

            textArea.wordWrap = false;
            textArea.clipping = TextClipping.Clip;
            textArea.richText = true;
            textArea.padding = new RectOffset(5, 5, 5, 5);
            textArea.font = consoleFont;

            tRect.width = TextSize.x;
            tRect.height = TextSize.y;

            GUI.BeginGroup(oRect, GUI.skin.textArea);
            consoleScroll = GUI.BeginScrollView(sRect, consoleScroll, tRect, false, true);
            GUI.Label(tRect, FullContent, textArea);
            GUI.EndScrollView();
            GUI.EndGroup();

            GUI.EndGroup();

            // MENU AREA
            GUI.BeginGroup(rightGroupA, GUI.skin.textArea);

            GUI.skin.button.fontStyle = FontStyle.Bold;

            if (GUI.Button(ulRect, openUnityLog))
                System.Diagnostics.Process.Start(Console.unityLogFile);

            if (GUI.Button(slRect, openSRMLLog))
                System.Diagnostics.Process.Start(Console.srmlLogFile);

            autoAC = GUI.Toggle(tacRect, autoAC, toggleAutoAC);

            if (autoAC != oldAutoAC)
            {
                if (autoAC)
                {
                    Console.Instance.LogSuccess("Auto complete will now open automatically.");
                }
                else
                {
                    Console.Instance.LogSuccess("Auto complete will only open by pressing CTRL+Space or CMD+Space");
                }

                oldAutoAC = autoAC;
            }

            GUI.EndGroup();

            GUI.BeginGroup(rightGroupB, GUI.skin.textArea);

            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUI.Label(cmRect, commands);
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;

            caRect.height = (30 * Console.cmdButtons.Count) + 5;
            commandScroll = GUI.BeginScrollView(csRect, commandScroll, caRect, false, true);
            GUI.BeginGroup(caRect);

            int y = 5;
            foreach (ConsoleButton button in Console.cmdButtons.Values)
            {
                btnRect.y = y;
                if (GUI.Button(btnRect, button.Text))
                    Console.ProcessInput(button.Command.TrimEnd(' '), true);

                y += 30;
            }

            GUI.EndGroup();
            GUI.EndScrollView();

            GUI.skin.button.fontStyle = FontStyle.Normal;

            GUI.EndGroup();

            // UPDATES THE SCROLL POSITION FOR THE CONSOLE TO SHOW LATEST MESSAGES
            if (updateDisplay)
            {
                fullText = string.Empty;
                for (int i = 0; i < Console.lines.Count; i++)
                {
                    fullText += $"{(i == 0 ? string.Empty : "\n")}{Console.lines[i]}";
                }

                consoleScroll.y = TextSize.y;
                updateDisplay = false;
            }

            // MOVES THE CURSOR AFTER AUTO COMPLETE
            if (moveCursor)
            {
                TextEditor txt = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                txt.MoveCursorToPosition(Vector2.one * 50000);

                autoComplete = false;
                moveCursor = false;
            }

            if (cmdText.Equals(string.Empty) && currHistory > -1)
                currHistory = -1;

            GUI.skin.button.fontSize = bFontSize;
            GUI.skin.textField.fontSize = tfFontSize;
            GUI.skin.toggle.fontSize = tFontSize;
            GUI.skin.label.fontSize = lFontSize;
        }

        private void DrawACWindow(int id)
        {
            int bFontSize = GUI.skin.button.fontSize;
            GUI.skin.button.fontSize = consoleFont.fontSize;

            bool spaces = cmdText.Contains(" ");
            string command = spaces ? cmdText.Substring(0, cmdText.IndexOf(' ')) : cmdText;

            TextEditor txt = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
            if (txt.hasSelection)
                command = string.Empty;

            intRect.height = (25 * cachedAC.Count) + 5;
            aCompleteScroll = GUI.BeginScrollView(acsRect, aCompleteScroll, intRect, false, true);
            GUI.BeginGroup(intRect);

            Texture2D bg = GUI.skin.textField.normal.background;
            GUI.skin.textField.normal.background = GUI.skin.button.active.background;
            GUI.skin.textField.richText = true;

            if (!spaces || txt.hasSelection)
            {
                if (!command.Equals(oldCmdText))
                {
                    cachedAC.RemoveAll(s => true); // .Clear() gets broken sometimes when you dynamically load an Assembly, so do the same another way
                    foreach (string cmd in Console.commands.Keys)
                    {
                        if (cmd.ToLowerInvariant().StartsWith(command.ToLowerInvariant()))
                            cachedAC.Add(cmd);
                    }

                    oldCmdText = command;
                }

                if (cachedAC.Count > 0)
                {
                    if (completeIndex >= cachedAC.Count)
                        completeIndex = 0;

                    selectComplete = cmdText.Substring(0, cmdText.Length - command.Length) + cachedAC[completeIndex];

                    int y = 5;
                    for (int i = 0; i < cachedAC.Count; i++)
                    {
                        GUI.backgroundColor = Color.white;
                        if (completeIndex == i)
                            GUI.backgroundColor = Color.yellow;

                        cBtnRect.y = y;
                        if (GUI.Button(cBtnRect, $"{(completeIndex == i ? "►" : string.Empty)}<b><color=#77DDFF>{cachedAC[i].Substring(0, command.Length)}</color></b>{cachedAC[i].Substring(command.Length)}", GUI.skin.textField))
                        {
                            cmdText = cmdText.Substring(0, cmdText.Length - command.Length) + cachedAC[i];
                            GUI.FocusControl(cmdName);
                            moveCursor = true;

                            autoComplete = false;
                        }

                        y += 25;
                    }
                }
            }
            else
            {
                string[] args = Console.StripArgs(cmdText, true);

                int count = args.Length;
                if (count > 0)
                {
                    string lastArg = args[count - 1];

                    if (Console.commands.ContainsKey(command))
                    {
                        List<string> autoC = Console.commands[command].GetAutoComplete(count - 1, lastArg);
                        if (autoC == null) autoC = Console.commands[command].GetAutoComplete(count - 1, args);
                        autoC?.RemoveAll(s => s.Contains(" "));

                        if (autoC == null || autoC.Count == 0 || Regex.Matches(cmdText, "\"").Count % 2 != 0)
                            autoComplete = false;

                        if (autoComplete)
                        {
                            if (!cmdText.Equals(oldCmdText))
                            {
                                cachedAC.RemoveAll(s => true); // .Clear() gets broken sometimes when you dynamically load an Assembly, so do the same another way
                                foreach (string cmd in autoC)
                                {
                                    if (cmd.ToLowerInvariant().StartsWith(lastArg.ToLowerInvariant()))
                                        cachedAC.Add(cmd);
                                }

                                oldCmdText = cmdText;
                            }

                            if (cachedAC.Count > 0)
                            {
                                if (completeIndex >= cachedAC.Count)
                                    completeIndex = 0;

                                selectComplete = cmdText.Substring(0, cmdText.Length - lastArg.Length) + cachedAC[completeIndex];

                                int y = 5;
                                for (int i = 0; i < cachedAC.Count; i++)
                                {
                                    GUI.backgroundColor = Color.white;
                                    if (completeIndex == i)
                                        GUI.backgroundColor = Color.yellow;

                                    cBtnRect.y = y;
                                    if (GUI.Button(cBtnRect, $"{(completeIndex == i ? "►" : string.Empty)}<b><color=#77DDFF>{cachedAC[i].Substring(0, lastArg.Length)}</color></b>{cachedAC[i].Substring(lastArg.Length)}", GUI.skin.textField))
                                    {
                                        cmdText = cmdText.Substring(0, cmdText.Length - lastArg.Length) + cachedAC[i];
                                        GUI.FocusControl(cmdName);
                                        moveCursor = true;

                                        autoComplete = false;
                                    }

                                    y += 25;
                                }
                            }
                        }
                    }
                }
            }

            if (cachedAC.Count == 0)
                autoComplete = false;

            GUI.skin.textField.richText = false;
            GUI.skin.textField.normal.background = bg;

            GUI.EndGroup();
            GUI.EndScrollView();

            GUI.skin.button.fontSize = bFontSize;
        }

        private void ToggleWindow()
        {
            showWindow = !showWindow;

            if (showWindow)
            {
                previousInput = SRInput.Instance.GetInputMode();
                SRInput.Instance.SetInputMode(SRInput.InputMode.NONE);

                autoComplete = false;
                currHistory = -1;
                forceClose = false;
                completeIndex = 0;

                focus = false;
                if (SceneManager.GetActiveScene().name.Equals("worldGenerated"))
                {
                    if (!SceneContext.Instance.TimeDirector.HasPauser())
                        SceneContext.Instance.TimeDirector.Pause(true);
                    else
                        hasAlreadyPaused = true;
                }

                cachedCasters = FindObjectsOfType<GraphicRaycaster>();
                foreach (GraphicRaycaster caster in cachedCasters)
                {
                    if (caster == null)
                        continue;

                    caster.enabled = false;
                }
            }
            else
            {
                SRInput.Instance.SetInputMode(previousInput);

                autoComplete = false;
                currHistory = -1;
                forceClose = false;
                completeIndex = 0;

                if (SceneManager.GetActiveScene().name.Equals("worldGenerated"))
                {
                    if (!hasAlreadyPaused)
                        SceneContext.Instance.TimeDirector.Unpause(true);
                }

                foreach (GraphicRaycaster caster in cachedCasters)
                {
                    if (!caster)
                        continue;

                    caster.enabled = true;
                }
                cachedCasters = null;
                hasAlreadyPaused = false;
            }
        }

        private void SetWindowOff()
        {
            SRInput.Instance.SetInputMode(previousInput);

            showWindow = false;
            autoComplete = false;
            currHistory = -1;
            forceClose = false;
            completeIndex = 0;

            if (SceneManager.GetActiveScene().name.Equals("worldGenerated"))
            {
                if (!hasAlreadyPaused)
                    SceneContext.Instance.TimeDirector.Unpause(true);
            }

            foreach (GraphicRaycaster caster in cachedCasters)
            {
                if (!caster)
                    continue;

                caster.enabled = true;
            }
            cachedCasters = null;
            hasAlreadyPaused = false;
        }
    }
}
