using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class AudioClipClickPlayer : EditorWindow
{
    private static AudioClip currentlyPlayingClip;
    private static double lastPlayTime;
    
    static AudioClipClickPlayer()
    {
        Selection.selectionChanged += OnSelectionChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnSelectionChanged()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;
        
        StopPreview();
        
        if (Selection.activeObject is AudioClip selectedClip)
        {
            if (selectedClip == currentlyPlayingClip && 
                EditorApplication.timeSinceStartup - lastPlayTime < 0.5f)
                return;
                
            currentlyPlayingClip = selectedClip;
            lastPlayTime = EditorApplication.timeSinceStartup;
            PlayPreview(selectedClip);
        }
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode || 
            state == PlayModeStateChange.EnteredPlayMode)
        {
            StopPreview();
        }
    }

    private static void PlayPreview(AudioClip clip)
    {
        if (clip == null || !enabled) return;
        
        System.Reflection.Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        System.Type audioUtilType = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        
        System.Reflection.MethodInfo stopMethod = audioUtilType.GetMethod(
            "StopAllPreviewClips",
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public
        );
        stopMethod?.Invoke(null, null);
        
        // Play the new preview
        System.Reflection.MethodInfo playMethod = audioUtilType.GetMethod(
            "PlayPreviewClip",
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public,
            null,
            new System.Type[] { typeof(AudioClip), typeof(int), typeof(bool) },
            null
        );
        
        playMethod?.Invoke(null, new object[] { clip, 0, false });
        
        // Debug.Log($"Playing: {clip.name}");
    }

    private static void StopPreview()
    {
        if (currentlyPlayingClip == null) return;
        
        System.Reflection.Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        System.Type audioUtilType = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        System.Reflection.MethodInfo method = audioUtilType.GetMethod(
            "StopAllPreviewClips",
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public
        );
        
        method?.Invoke(null, null);
        currentlyPlayingClip = null;
        
        // Debug.Log("Audio preview stopped");
    }
    
    [MenuItem("Tools/Stop Audio Preview")]
    private static void StopAudioPreview()
    {
        StopPreview();
    }
    
    private static bool enabled = true;
    [MenuItem("Tools/Toggle Audio Preview On Click")]
    private static void ToggleAudioPreview()
    {
        enabled = !enabled;
        if (!enabled)
            StopPreview();
            
        Menu.SetChecked("Tools/Toggle Audio Preview On Click", enabled);
        // Debug.Log($"Audio preview on click: {(enabled ? "ENABLED" : "DISABLED")}");
    }
}