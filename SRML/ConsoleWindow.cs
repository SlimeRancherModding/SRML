using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SRML.ConsoleSystem
{
	/// <summary>
	/// Draws the window for the in-game console
	/// </summary>
	public class ConsoleWindow : MonoBehaviour
	{
		// CONTROL VARIABLES
		internal static bool updateDisplay = false;
		private static bool showWindow = false;
		private static bool canPress = true;
		private static Vector2 oldRes = new Vector2(Screen.width, Screen.height);
		private static Vector2 windowSize = new Vector2(Screen.width + 20, Screen.height / 2);
		private static Vector2 windowPosition = new Vector2(-10, -20);
		private static Rect windowRect = new Rect(windowPosition, windowSize);

		private static readonly string openUnityLog = "Unity Log";
		private static readonly string openSRMLLog = "SRML Log";

		private static readonly string runMods = "Mods";
		private static readonly string runCommands = "Help";
		private static readonly string runReload = "Reload";

		private static Vector2 consoleScroll = Vector2.zero;

		internal static string cmdsText = string.Empty;
		internal static string modsText = string.Empty;
		internal static string fullText = string.Empty;
		private static string cmdText = string.Empty;

		private static GUIStyle textArea;
		private static GUIStyle window;
		private static Font consoleFont = Font.CreateDynamicFontFromOSFont(new string[] { "Lucida Console", "Monaco" }, 13);

		private static GraphicRaycaster[] cachedCasters;
		internal static int currHistory = -1;

		/// <summary>
		/// Attachs the window to a scene
		/// </summary>
		public static void AttachWindow(Scene oldScene, Scene newScene)
		{
			GameObject window = new GameObject("_ConsoleWindow", typeof(ConsoleWindow));
			DontDestroyOnLoad(window);

			SceneManager.activeSceneChanged -= AttachWindow;
			Console.Log("Attached Console Window successfully!");
		}

		// TO ENSURE IT GOT CREATED
		private void Start()
		{
			Console.Log("Console Window running.");
			Console.Log("Use command 'help' for a list of all commands");
			Console.Log("Use command 'mods' for a list of all mods loaded");
			Console.Log("You can also check the menu on the right");

			foreach (SRModInfo info in SRModLoader.LoadedMods)
			{
				modsText += $"{(modsText.Equals(string.Empty) ? "" : "\n")}<color=#8ab7ff>{info.Name}</color> [<color=#8ab7ff>Author:</color> {info.Author} | <color=#8ab7ff>ID:</color> {info.Id} | <color=#8ab7ff>Version:</color> {info.Version}]";
			}
		}

		// JUST TO CHECK FOR A KEY
		private void Update()
		{
			if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)) && Input.GetKeyDown(KeyCode.Tab))
				ToggleWindow();

			if (oldRes.x != Screen.width || oldRes.y != Screen.height)
			{
				oldRes = new Vector2(Screen.width, Screen.height);
				windowSize = new Vector2(Screen.width + 20, Screen.height / 2);
				windowRect = new Rect(windowPosition, windowSize);
			}

			canPress = true;
		}

		// DRAWS THE WINDOW
		private void OnGUI()
		{
			// UNITY PREVENTS "GUI" STUFF FROM BEING CALLED OUTSIDE "OnGUI"
			if (textArea == null) textArea = new GUIStyle(GUI.skin.label);
			if (window == null) window = new GUIStyle(GUI.skin.window);

			window.active.background = window.normal.background;
			window.hover.background = window.normal.background;
			window.focused.background = window.normal.background;
			window.onActive.background = window.normal.background;
			window.onHover.background = window.normal.background;
			window.onFocused.background = window.normal.background;

			if (showWindow)
			{
				GUI.Window(1234567890, windowRect, DrawWindow, string.Empty, window);
				GUI.BringWindowToFront(1234567890);
			}
		}

		private void DrawWindow(int id)
		{
			Font defFont = GUI.skin.font;
			GUI.skin.font = consoleFont;

			Rect leftGroup = new Rect(15, 25, windowRect.width - 150, windowRect.height - 30);
			Rect rightGroup = new Rect(leftGroup.width + 25, leftGroup.y, 110, leftGroup.height - 5);

			// CONSOLE AREA
			GUI.BeginGroup(leftGroup);

			float cmdLineY = leftGroup.height - 25;
			cmdText = GUI.TextField(new Rect(0, cmdLineY, leftGroup.width, 20), cmdText);

			textArea.wordWrap = true;
			textArea.clipping = TextClipping.Clip;
			textArea.richText = true;
			textArea.padding = new RectOffset(5, 5, 5, 5);

			GUIContent fullContent = new GUIContent(fullText.ToString());
			Vector2 textSize = textArea.CalcSize(fullContent);

			Rect oRect = new Rect(0, 0, leftGroup.width, cmdLineY - 5);
			Rect sRect = new Rect(5, 7, oRect.width - 15, cmdLineY - 20);
			Rect tRect = new Rect(0, 0, textSize.x, textSize.y);
			GUI.BeginGroup(oRect, GUI.skin.textArea);
			consoleScroll = GUI.BeginScrollView(sRect, consoleScroll, tRect, false, true);
			GUI.Label(tRect, fullContent, textArea);
			GUI.EndScrollView();
			GUI.EndGroup();

			GUI.EndGroup();

			// MENU AREA
			GUI.BeginGroup(rightGroup, GUI.skin.textArea);

			if (GUI.Button(new Rect(10, 7, 90, 25), openUnityLog))
				System.Diagnostics.Process.Start(Console.unityLogFile);

			if (GUI.Button(new Rect(10, 37, 90, 25), openSRMLLog))
				System.Diagnostics.Process.Start(Console.srmlLogFile);

			if (GUI.Button(new Rect(10, 67, 90, 25), runMods))
				Console.ProcessInput("mods", true);

			if (GUI.Button(new Rect(10, 97, 90, 25), runCommands))
				Console.ProcessInput("help", true);

			if (GUI.Button(new Rect(10, 127, 90, 25), runReload))
				Console.ProcessInput("reload", true);

			GUI.EndGroup();

			// KEY LISTENING DONE HERE CAUSE UNITY COSUMES MOST KEY INPUTS WHEN A TEXTFIELD IS FOCUSED
			if (Event.current.isKey)
			{
				// SUBMITS THE TEXTFIELD
				if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
				{
					Console.ProcessInput(cmdText);
					cmdText = string.Empty;
				}

				// CLOSES THE WINDOW
				if ((Event.current.command || Event.current.control) && Event.current.keyCode == KeyCode.Tab && canPress)
					ToggleWindow();

				// CYCLES HISTORY UP
				if (Event.current.keyCode == KeyCode.UpArrow)
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
							currHistory = currHistory - 1;
						}
					}
				}

				// CYCLES HISTORY DOWN
				if (Event.current.keyCode == KeyCode.DownArrow)
				{
					if (currHistory != -1)
					{
						if (currHistory < (Console.history.Count - 1))
						{
							cmdText = Console.history[currHistory + 1];
							currHistory = currHistory + 1;
						}
						else
						{
							cmdText = "";
							currHistory = -1;
						}
					}
				}
			}

			// UPDATES THE SCROLL POSITION FOR THE CONSOLE TO SHOW LATEST MESSAGES
			if (updateDisplay)
			{
				consoleScroll.y = textArea.CalcSize(new GUIContent(fullText.ToString())).y;
				updateDisplay = false;
			}

			GUI.skin.font = defFont;
		}

		private void ToggleWindow()
		{
			canPress = false;
			showWindow = !showWindow;

			if (showWindow)
			{
				SceneContext.Instance.TimeDirector.Pause(true);

				cachedCasters = FindObjectsOfType<GraphicRaycaster>();
				foreach (GraphicRaycaster caster in cachedCasters)
				{
					caster.enabled = false;
				}
			}
			else
			{
				SceneContext.Instance.TimeDirector.Unpause(true);

				foreach (GraphicRaycaster caster in cachedCasters)
				{
					caster.enabled = true;
				}
				cachedCasters = null;
			}
		}
	}
}
