using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JumpDummy.Editor
{
    [InitializeOnLoad]
    public static class CyberpunkVerticalSliceFinisher
    {
        private const string ScenePath = "Assets/Scenes/CyberpunkRooftops.unity";
        private const string VersionKey = "JumpDummy.VerticalSliceVersion";
        private const int Version = 2;

        static CyberpunkVerticalSliceFinisher() { EditorApplication.delayCall += UpgradeIfNeeded; }

        [MenuItem("JumpDummy/Finish Vertical Slice")]
        public static void FinishFromMenu()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            Upgrade(true);
        }

        private static void UpgradeIfNeeded()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling) return;
            if (SceneManager.GetActiveScene().path == NeonAscentBuilder.ScenePath) return;
            if (EditorPrefs.GetInt(VersionKey, 0) >= Version && SceneHasSession()) return;
            Upgrade(false);
        }

        private static bool SceneHasSession()
        {
            if (SceneManager.GetActiveScene().path != ScenePath) return false;
            var fb = Object.FindAnyObjectByType<CyberFeedback>();
            return Object.FindAnyObjectByType<GameSession>() != null && fb != null && fb.bgmClip != null;
        }

        private static void Upgrade(bool openAfter)
        {
            if (!File.Exists(ScenePath)) return;
            Scene previous = SceneManager.GetActiveScene();
            bool alreadyOpen = previous.path == ScenePath;
            bool empty = string.IsNullOrEmpty(previous.path) && !previous.isDirty;
            Scene scene = alreadyOpen ? previous : EditorSceneManager.OpenScene(ScenePath, empty ? OpenSceneMode.Single : OpenSceneMode.Additive);
            bool changed = false;
            var player = Object.FindAnyObjectByType<DummyController>();
            var presentation = Object.FindAnyObjectByType<CyberpunkPresentation>();
            var session = Object.FindAnyObjectByType<GameSession>();
            if (session == null)
            {
                var go = new GameObject("Game Session");
                session = go.AddComponent<GameSession>();
                session.player = player;
                changed = true;
            }
            var feedback = Object.FindAnyObjectByType<CyberFeedback>();
            if (feedback == null)
            {
                var go = new GameObject("Cyber Feedback");
                feedback = go.AddComponent<CyberFeedback>();
                feedback.player = player;
                changed = true;
            }
            if (feedback != null)
            {
                var jump = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Sounds/jump.mp3");
                var bump = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Sounds/bump.mp3");
                var land = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Sounds/land.mp3");
                var fall = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Sounds/fall.mp3");
                var bgm = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Music/bgm_cyberpunk.mp3");

                if (feedback.jumpClip != jump || feedback.bumpClip != bump || feedback.landClip != land || feedback.fallClip != fall || feedback.bgmClip != bgm)
                {
                    feedback.jumpClip = jump;
                    feedback.bumpClip = bump;
                    feedback.landClip = land;
                    feedback.fallClip = fall;
                    feedback.bgmClip = bgm;
                    changed = true;
                }
            }
            if (presentation != null && presentation.session != session)
            {
                presentation.session = session;
                changed = true;
            }
            if (changed) EditorSceneManager.SaveScene(scene);
            EditorPrefs.SetInt(VersionKey, Version);
            if (!alreadyOpen && previous.IsValid() && previous.isLoaded)
            {
                SceneManager.SetActiveScene(previous);
                EditorSceneManager.CloseScene(scene, true);
            }
            else SceneManager.SetActiveScene(scene);
            AssetDatabase.SaveAssets();
            if (openAfter && !alreadyOpen) EditorSceneManager.OpenScene(ScenePath);
            Debug.Log("JUMPDUMMY VERTICAL SLICE READY: title/continue, save, pause, finish, feedback and stats installed.");
        }
    }
}
