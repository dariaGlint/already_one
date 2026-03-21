using UnityEditor;
using UnityEngine;

public class WebGLBuilder
{
    public static void Build()
    {
        var scenes = new[] { "Assets/Scenes/SampleScene.unity" };
        var report = BuildPipeline.BuildPlayer(scenes, "Build/WebGL", BuildTarget.WebGL, BuildOptions.None);

        if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.LogError("WebGL build failed: " + report.summary.totalErrors + " errors");
            EditorApplication.Exit(1);
        }
        else
        {
            Debug.Log("WebGL build succeeded");
        }
    }
}
