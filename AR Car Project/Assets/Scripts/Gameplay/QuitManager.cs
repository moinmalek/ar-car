using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Exits play mode in the Editor; calls <see cref="Application.Quit"/> on device builds.
/// </summary>
public sealed class QuitManager : MonoBehaviour
{
    /// <summary>Called from UI button.</summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
