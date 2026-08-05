using UnityEngine;
using System.Collections.Generic;

public class CH_TextConsole : MonoBehaviour
{
    public static CH_TextConsole Instance { get; private set; }

    [Header("Console")]
    public KeyCode toggleKey = KeyCode.BackQuote;
    public Rect windowRect = new Rect(20f, 20f, 500f, 320f);
    public bool startOpen;

    private bool isOpen;
    private string inputText = string.Empty;
    private string outputText = "Console ready. Type 'help' to see the available commands.";
    private Vector2 scrollPosition;
    private readonly List<string> commandHistory = new List<string>();
    private int historyIndex = -1;
    private CH_HorrorManager horrorManager;
    private bool focusInput;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        if (Instance != null)
            return;

        GameObject consoleObject = new GameObject("CH_TextConsole");
        DontDestroyOnLoad(consoleObject);
        Instance = consoleObject.AddComponent<CH_TextConsole>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        isOpen = startOpen;
        horrorManager = FindObjectOfType<CH_HorrorManager>();

        if (horrorManager == null)
        {
            AppendLine("CH_HorrorManager not found in the scene.");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isOpen = !isOpen;
            focusInput = isOpen;
            CH_Manager.Instance.SetFPSControllerActive(!isOpen);
            CH_Manager.Instance.SetCursorLockState(!isOpen);
        }
        if (isOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void LateUpdate()
    {
        if (isOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void OnGUI()
    {
        if (!isOpen)
            return;

        windowRect = GUI.Window(0, windowRect, DrawConsoleWindow, "Text Console");
        GUILayout.Label($"Cursor: {Cursor.lockState} | Visible: {Cursor.visible} | KeyboardControl: {GUIUtility.keyboardControl} | CONSOLE: {isOpen} | FOCUS: {focusInput}" );
    }

    private void DrawConsoleWindow(int windowId)
    {
        GUIStyle logStyle = new GUIStyle(GUI.skin.box)
        {
            wordWrap = true,
            fontSize = 12
        };

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(220f));
        GUILayout.TextArea(outputText, logStyle, GUILayout.ExpandHeight(true));
        GUILayout.EndScrollView();

        GUILayout.Space(6f);

        if (focusInput)
        {
            GUI.SetNextControlName("ConsoleInput");
            GUI.FocusControl("ConsoleInput");
            focusInput = false;
        }
        else
        {
            GUI.SetNextControlName("ConsoleInput");
        }

        GUILayout.BeginHorizontal();
        inputText = GUILayout.TextField(inputText, GUILayout.MinHeight(24f));

        if (GUILayout.Button("Submit", GUILayout.MinWidth(80f), GUILayout.MinHeight(24f)))
        {
            ExecuteCommand(inputText);
            inputText = string.Empty;
            focusInput = true;
        }

        GUILayout.EndHorizontal();

        if (Event.current.type == EventType.KeyDown)
        {
            if (Event.current.keyCode == KeyCode.Return)
            {
                Event.current.Use();
                ExecuteCommand(inputText);
                inputText = string.Empty;
            }
            else if (Event.current.keyCode == KeyCode.UpArrow)
            {
                Event.current.Use();
                if (commandHistory.Count > 0)
                {
                    historyIndex = Mathf.Clamp(historyIndex + 1, 0, commandHistory.Count - 1);
                    inputText = commandHistory[historyIndex];
                }
            }
            else if (Event.current.keyCode == KeyCode.DownArrow)
            {
                Event.current.Use();
                if (commandHistory.Count > 0)
                {
                    historyIndex = Mathf.Clamp(historyIndex - 1, 0, commandHistory.Count - 1);
                    inputText = commandHistory[historyIndex];
                }
                else
                {
                    inputText = string.Empty;
                }
            }
        }

        GUI.DragWindow();
    }

    private void ExecuteCommand(string rawCommand)
    {
        if (string.IsNullOrWhiteSpace(rawCommand))
            return;

        string[] parts = rawCommand.Trim()
            .Split(new[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0)
            return;

        string command = parts[0].ToLowerInvariant();

        commandHistory.Add(rawCommand.Trim());
        historyIndex = commandHistory.Count;

        switch (command)
        {
            case "help":
                AppendLine("Available commands:");
                AppendLine("- help");
                AppendLine("- clear");
                AppendLine("- turnoffalllights");
                AppendLine("- spawnclient");
                AppendLine("- turnonalllights");
                break;

            case "clear":
                outputText = string.Empty;
                break;

            case "turnoffalllights":
                if (horrorManager != null)
                {
                    horrorManager.TurnOffAllLights();
                    AppendLine("TurnOffAllLights executed.");
                }
                else
                {
                    AppendLine("CH_HorrorManager is not available.");
                }
                break;

            case "turnonalllights":
                if (horrorManager != null)
                {
                    horrorManager.TurnOnAllLights();
                    AppendLine("TurnOnAllLights executed.");
                }
                else
                {
                    AppendLine("CH_HorrorManager is not available.");
                }
                break;

            case "spawnclient":
                CH_AIManager aiManager = CH_AIManager.Instance;
                if (aiManager != null)
                {
                    aiManager.InvokeAICharacter();
                    AppendLine("SpawnClient executed.");
                }
                else
                {
                    AppendLine("CH_AIManager is not available.");
                }
                break;

            case "disableroomlights":
                if (parts.Length < 2 || !int.TryParse(parts[1], out int roomIndex))
                {
                    AppendLine("Usage: disableroomlights <roomIndex>");
                    break;
                }

                CH_RoomManager roomManager = CH_RoomManager.Instance;
                if (roomManager != null)
                {
                    roomManager.SetRoomLights(roomIndex, false);
                    AppendLine("DisableRoomLights executed.");
                }
                else
                {
                    AppendLine("CH_RoomManager is not available.");
                }
                break;

            default:
                AppendLine($"Unknown command: {rawCommand}");
                break;
        }
    }

    private void AppendLine(string message)
    {
        outputText += message + "\n";
    }
}
