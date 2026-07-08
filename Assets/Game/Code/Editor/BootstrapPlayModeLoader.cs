using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR

namespace Code.Editor
{
    [InitializeOnLoad]
    public static class BootstrapPlayModeLoader
    {
        private const string BootstrapScenePath = "Assets/Game/Scenes/Bootstrap.unity";

        static BootstrapPlayModeLoader()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.ExitingEditMode)
            {
                return;
            }
            
            SceneAsset bootstrapScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(BootstrapScenePath);
            
            EditorPrefs.SetString("StartupScene", SceneManager.GetActiveScene().path);
            
            EditorSceneManager.playModeStartScene = bootstrapScene;
        }
    }
}

#endif