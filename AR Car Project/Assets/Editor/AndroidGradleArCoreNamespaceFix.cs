#if UNITY_EDITOR
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor.Android;

/// <summary>
/// ARCore's arcore_client AAR and Unity's unityandroidpermissions AAR both declare
/// AGP namespace/package com.google.ar.core. AGP 9 rejects that during manifest merge.
/// Unity generates unityandroidpermissions/build.gradle next to unityLibrary; inject a
/// unique namespace there after the Gradle project is emitted.
/// </summary>
class AndroidGradleArCoreNamespaceFix : IPostGenerateGradleAndroidProject
{
    const string TargetNamespace = "com.unity3d.plugin.unityandroidpermissions";

    public int callbackOrder => 0;

    public void OnPostGenerateGradleAndroidProject(string path)
    {
        string gradleRoot = Path.GetFullPath(Path.Combine(path, ".."));
        string buildGradle = Path.Combine(gradleRoot, "unityandroidpermissions", "build.gradle");
        if (!File.Exists(buildGradle))
            return;

        string text = File.ReadAllText(buildGradle);
        if (text.Contains("namespace '" + TargetNamespace + "'") || text.Contains("namespace \"" + TargetNamespace + "\""))
            return;

        if (Regex.IsMatch(text, @"namespace\s+['""]com\.google\.ar\.core['""]"))
        {
            text = Regex.Replace(
                text,
                @"namespace\s+['""]com\.google\.ar\.core['""]",
                "namespace '" + TargetNamespace + "'");
            File.WriteAllText(buildGradle, text);
            return;
        }

        const string anchor = "android {";
        int i = text.IndexOf(anchor);
        if (i < 0)
            return;

        int insert = i + anchor.Length;
        string insertText = "\n    namespace '" + TargetNamespace + "'\n";
        File.WriteAllText(buildGradle, text.Insert(insert, insertText));
    }
}
#endif
