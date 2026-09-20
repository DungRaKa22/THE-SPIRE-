using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace JumpDummy.Editor
{
    public static class JumpDummyBuild
    {
        [MenuItem("JumpDummy/Build Windows Playtest")]
        public static void BuildWindows()
        {
            Directory.CreateDirectory("Builds/Windows");
            PlayerSettings.productName = "JumpDummy";
            PlayerSettings.companyName = "JumpDummy Studio";
            PlayerSettings.defaultScreenWidth = 1280;
            PlayerSettings.defaultScreenHeight = 720;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.resizableWindow = true;
            var options = new BuildPlayerOptions
            {
                scenes = new[] { File.Exists(NeonAscentBuilder.ScenePath) ? NeonAscentBuilder.ScenePath : CyberpunkSceneBuilder.ScenePath },
                locationPathName = "Builds/Windows/JumpDummy.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.Development
            };
            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
                throw new BuildFailedException("JumpDummy build failed: " + report.summary.result);
            File.WriteAllText("Docs/BuildValidation.txt",
                $"PASS: Windows x64 development build, {report.summary.totalSize} bytes, {report.summary.totalTime}.\n");
            Debug.Log("JUMPDUMMY BUILD READY: Builds/Windows/JumpDummy.exe");
        }

        [MenuItem("JumpDummy/Clear Saved Run Progress", priority = 100)]
        public static void ClearSavedRun()
        {
            var session = Object.FindAnyObjectByType<GameSession>();
            if (session != null) { session.ClearSavedProgress(); return; }
            PlayerPrefs.DeleteKey("JumpDummy.Run.Valid");
            PlayerPrefs.DeleteKey("JumpDummy.Run.X");
            PlayerPrefs.DeleteKey("JumpDummy.Run.Y");
            PlayerPrefs.DeleteKey("JumpDummy.Run.Time");
            PlayerPrefs.DeleteKey("JumpDummy.Run.Jumps");
            PlayerPrefs.Save();
            Debug.Log("JumpDummy: Đã xóa dữ liệu lưu tiến độ. Nhân vật sẽ xuất phát tại vị trí đặt trong Scene.");
        }
    }
}
