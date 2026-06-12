using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;

namespace Lobotom.Build.Editor
{
    public static class BuildCommand
    {
        public static void BuildWindows64()  
        {
            var enabledScenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();

            if (enabledScenes.Length == 0)
            {
                throw new BuildFailedException("No enabled scenes were found in Build Settings.");
            }

            const string buildFolder = "build/StandaloneWindows64";
            const string executableName = "Lobotom.exe";

            Directory.CreateDirectory(buildFolder);

            var options = new BuildPlayerOptions
            {
                scenes = enabledScenes,
                locationPathName = Path.Combine(buildFolder, executableName),
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                throw new BuildFailedException($"Build failed with result: {report.summary.result}");
            }
        }
    }
}