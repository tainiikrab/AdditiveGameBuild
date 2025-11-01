using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugTools : EditorWindow
{
    private int currentPlotIndex;
    private bool fastForwardActive;

    private void Update()
    {
        if (!EditorApplication.isPlaying) return;

        if (Input.GetKey(KeyCode.Space))
        {
            if (!fastForwardActive)
            {
                Time.timeScale = 10f;
                fastForwardActive = true;
            }
        }
        else if (fastForwardActive)
        {
            ResetTime();
            fastForwardActive = false;
        }

        Repaint();
    }

    private void OnGUI()
    {
        GUILayout.Label("Scene Control", EditorStyles.boldLabel);

        // using (new EditorGUI.DisabledScope(!EditorApplication.isPlaying))
        // {
        if (GUILayout.Button("Reload Scene   [R]")) ReloadScene();

        GUILayout.Space(10);
        GUILayout.Label("Time Control", EditorStyles.boldLabel);
        GUILayout.Label("Hold [Space] → x10 speed, release → normal");

        if (fastForwardActive)
        {
            GUI.color = Color.red;
            GUILayout.Label("FAST FORWARD ACTIVE");
            GUI.color = Color.white;
        }

        if (GUILayout.Button("Reset TimeScale   [Shift+Space]")) ResetTime();

        GUILayout.Space(10);
        GUILayout.Label("Orders", EditorStyles.boldLabel);
        if (GUILayout.Button("Create Regular Order   [O]")) OrderManager.CreateRegularOrder();

        currentPlotIndex = EditorGUILayout.IntField("Plot Index", currentPlotIndex);
        if (GUILayout.Button("Create Plot Order   [Shift+P]")) OrderManager.CreatePlotOrder(currentPlotIndex);

        if (GUILayout.Button("Finish current order")) OrderManager.CompleteOrder();

        GUILayout.Space(10);
        GUILayout.Label("Saves", EditorStyles.boldLabel);
        if (GUILayout.Button("Delete saves"))
        {
            SaveManager.ResetSaves();
            OrderManager.CompleteOrder();
            if (Application.isPlaying)
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        GUILayout.Space(10);
        GUILayout.Label("Perfomance", EditorStyles.boldLabel);
        if (GUILayout.Button("Set max FPS to 30")) Application.targetFrameRate = 30;
        if (GUILayout.Button("Reset max FPS")) Application.targetFrameRate = -1;
        // }
    }

    // === ОКНО ===
    [MenuItem("Tools/DebugTools %#t")] // Ctrl+Shift+T
    public static void ShowWindow()
    {
        GetWindow<DebugTools>("Debug Tools");
    }

    // === ГЛОБАЛЬНЫЕ ХОТКЕИ (тоже только в Play Mode) ===
    [MenuItem("Tools/Debug/Reload Scene _r")]
    private static void ReloadScene()
    {
        if (EditorApplication.isPlaying)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    [MenuItem("Tools/Debug/Create Regular Order _o")]
    private static void CreateRegularOrder()
    {
        if (EditorApplication.isPlaying)
            OrderManager.CreateRegularOrder();
    }

    [MenuItem("Tools/Debug/Create Plot Order #p")]
    private static void CreatePlotOrder()
    {
        if (EditorApplication.isPlaying)
            OrderManager.CreatePlotOrder(0);
    }

    private static void ResetTime()
    {
        Time.timeScale = 1f;
    }
}