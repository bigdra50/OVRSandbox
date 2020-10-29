using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;

public class ApplicationBuild : MonoBehaviour
{
    static string[] ScenePaths => EditorBuildSettings.scenes.Select(scene => scene.path).ToArray();
    private static string _outputPath = "Build";
    private static string _outputFileName = "app";

    [MenuItem("Oculus/SetupQuest")]
    public static void SetupQuest()
    {
        EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ASTC;
        PlayerSettings.colorSpace = ColorSpace.Linear;
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] {GraphicsDeviceType.OpenGLES3, GraphicsDeviceType.Vulkan});
        PlayerSettings.openGLRequireES31 = true;
        PlayerSettings.openGLRequireES31AEP = true;
        PlayerSettings.openGLRequireES32 = false;
        PlayerSettings.MTRendering = true;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);   // ビルド時間よりパフォーマンス優先ならIL2CPP
        PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Android, ApiCompatibilityLevel.NET_4_6);
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23;

        if (!Application.isBatchMode)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        }
    }

    public static void SetupStandAlone()
    {
        
        if (!Application.isBatchMode)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
        }
    }

    [MenuItem("Build/Quest")]
    public static void BuildQuest()
    {
        SetupQuest();
        Build(BuildTarget.Android, BuildOptions.CompressWithLz4);
    }

    [MenuItem("Build/StandAlone")]
    public static void BuildStandAlone()
    {
        SetupStandAlone();
        Build(BuildTarget.StandaloneWindows, BuildOptions.None);
    }

    private static void Build(BuildTarget buildTarget, BuildOptions buildOptions)
    {
        
        var outputPath = GetOutputPath(buildTarget);
        var report = BuildPipeline.BuildPlayer(ScenePaths, outputPath, buildTarget,
            buildOptions);
        var summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Build Success : {outputPath}");
        }
        else
        {
            Debug.Assert(false, $"Build Error");
        }
    }

    private static string GetOutputPath(BuildTarget buildTarget)
    {
        switch (buildTarget)
        {
            case BuildTarget.Android:
                return $"{_outputPath}/Quest/{_outputFileName}.apk";
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return $"{_outputPath}/StandAlone/{_outputFileName}.exe";
            default:
                throw new ArgumentException($"Build Target {buildTarget} is invalid");
        }
    }
}