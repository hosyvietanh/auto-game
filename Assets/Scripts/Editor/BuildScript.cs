using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace BattleCity.EditorTools
{
    /// <summary>
    /// Produces a WebGL build of the game for static hosting (GitHub Pages).
    /// Run via: Unity -batchmode -quit -projectPath . -executeMethod BattleCity.EditorTools.BuildScript.BuildWebGL
    /// (see scripts/build-web.sh). Output goes to Build/WebGL/ (gitignored).
    ///
    /// Compression is DISABLED on purpose: Unity's default Brotli/Gzip output relies on the
    /// web server sending a matching Content-Encoding header, which GitHub Pages (and most
    /// plain static hosts) do NOT do — the loader then fails. Uncompressed output just works
    /// everywhere. The build is small, so the size cost is negligible.
    /// </summary>
    public static class BuildScript
    {
        const string ScenePath = "Assets/Scenes/Game.unity";
        const string OutputDir = "Build/WebGL";

        [MenuItem("Tools/BattleCity/Build WebGL")]
        public static void BuildWebGL()
        {
            // Make WebGL the active target so BuildPlayer doesn't have to switch mid-build.
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebGL)
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);

            // Static-host-friendly output (see class summary).
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
            PlayerSettings.WebGL.decompressionFallback = false;
            // Served from a project subpath (username.github.io/auto-game/), so keep asset
            // URLs relative rather than rooted at "/".
            PlayerSettings.runInBackground = true;

            string outputPath = Path.Combine(Directory.GetCurrentDirectory(), OutputDir);
            Directory.CreateDirectory(outputPath);

            var options = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = outputPath,
                target = BuildTarget.WebGL,
                targetGroup = BuildTargetGroup.WebGL,
                options = BuildOptions.None,
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"WebGL build succeeded: {summary.totalSize} bytes -> {OutputDir}");
            }
            else
            {
                Debug.LogError($"WebGL build FAILED: {summary.result} ({summary.totalErrors} errors)");
                // Non-zero exit so scripts/build-web.sh (and CI) can detect failure.
                EditorApplication.Exit(1);
            }
        }
    }
}
