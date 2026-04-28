#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// After clone, missing UPM packages can leave scripts uncompilable (issue #6). Logs a clear hint if AR Foundation types are absent.
/// </summary>
[InitializeOnLoad]
static class ArCarDependencyHint
{
    static ArCarDependencyHint()
    {
        EditorApplication.delayCall += Check;
    }

    static void Check()
    {
        try
        {
            Type t = Type.GetType("UnityEngine.XR.ARFoundation.ARTrackedImageManager, Unity.XR.ARFoundation");
            if (t == null)
                Debug.LogWarning(
                    "[ar-car] XR packages appear missing. From Unity: Window → Package Manager → refresh, or re-open the project so manifest.json dependencies resolve.");
        }
        catch
        {
            /* ignore */
        }
    }
}
#endif
