#if UNITY_EDITOR
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor.Android;

/// <summary>
/// Post-processes the Gradle project Unity exports before <c>gradlew</c> runs.
/// </summary>
class AndroidGradleArCoreNamespaceFix : IPostGenerateGradleAndroidProject
{
    const string TargetNamespace = "com.unity3d.plugin.unityandroidpermissions";
    const string UnityPlayerActivity = "com.unity3d.player.UnityPlayerActivity";

    public int callbackOrder => 0;

    public void OnPostGenerateGradleAndroidProject(string path)
    {
        string gradleRoot = Path.GetFullPath(Path.Combine(path, ".."));
        FixUnityAndroidPermissionsNamespace(gradleRoot);
        EnsureBuildAndRunLaunchesUnityPlayerActivity(gradleRoot);
    }

    /// <summary>
    /// ARCore's arcore_client AAR and Unity's unityandroidpermissions AAR both declare
    /// AGP namespace/package com.google.ar.core. AGP 9 rejects that during manifest merge.
    /// </summary>
    static void FixUnityAndroidPermissionsNamespace(string gradleRoot)
    {
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

    /// <summary>
    /// Unity Editor "Build And Run" starts <see cref="UnityPlayerActivity"/> via ADB. If the
    /// exported launcher manifest only declares <c>UnityPlayerGameActivity</c> (GameActivity
    /// entry point), install succeeds but launch fails with "Activity class ... does not exist".
    /// </summary>
    static void EnsureBuildAndRunLaunchesUnityPlayerActivity(string gradleRoot)
    {
        string manifestPath = Path.Combine(gradleRoot, "launcher", "src", "main", "AndroidManifest.xml");
        if (!File.Exists(manifestPath))
            return;

        string xml = File.ReadAllText(manifestPath);
        if (xml.Contains("android:name=\"" + UnityPlayerActivity + "\"") || xml.Contains("android:name='" + UnityPlayerActivity + "'"))
            return;

        // Only rewrite the MAIN/LAUNCHER activity so we do not disturb other activity declarations.
        var pattern = new Regex(
            @"<activity\b[^>]*android:name\s*=\s*""com\.unity3d\.player\.UnityPlayerGameActivity""[^>]*>[\s\S]*?<intent-filter>[\s\S]*?android\.intent\.action\.MAIN[\s\S]*?android\.intent\.category\.LAUNCHER[\s\S]*?</intent-filter>[\s\S]*?</activity>",
            RegexOptions.IgnoreCase);

        Match m = pattern.Match(xml);
        if (!m.Success)
        {
            pattern = new Regex(
                @"<activity\b[^>]*android:name\s*=\s*'com\.unity3d\.player\.UnityPlayerGameActivity'[^>]*>[\s\S]*?<intent-filter>[\s\S]*?android\.intent\.action\.MAIN[\s\S]*?android\.intent\.category\.LAUNCHER[\s\S]*?</intent-filter>[\s\S]*?</activity>",
                RegexOptions.IgnoreCase);
            m = pattern.Match(xml);
        }

        if (!m.Success)
            return;

        string block = m.Value;
        string newBlock = Regex.Replace(
            block,
            @"android:name\s*=\s*""com\.unity3d\.player\.UnityPlayerGameActivity""",
            "android:name=\"" + UnityPlayerActivity + "\"",
            RegexOptions.IgnoreCase);
        newBlock = Regex.Replace(
            newBlock,
            @"android:name\s*=\s*'com\.unity3d\.player\.UnityPlayerGameActivity'",
            "android:name=\"" + UnityPlayerActivity + "\"",
            RegexOptions.IgnoreCase);

        if (newBlock != block)
            File.WriteAllText(manifestPath, xml.Substring(0, m.Index) + newBlock + xml.Substring(m.Index + m.Length));
    }
}
#endif
