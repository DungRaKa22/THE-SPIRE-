using System;
using System.Reflection;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace JumpDummy.Editor
{
    // Batch smoke checks against the real controller and scene colliders.
    public static class JumpDummyPrototypeChecks
    {
        [MenuItem("JumpDummy/Run Prototype Checks")]
        public static void ValidateBatch()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) throw new InvalidOperationException("Stop Play Mode before running checks.");
            // Use a temporary copy so the current scene and unsaved edits remain intact.
            string testScenePath = "Assets/Scenes/JumpLab_Check_" + Guid.NewGuid().ToString("N") + ".unity";
            File.Copy(JumpDummyPrototypeBuilder.ScenePath, testScenePath, true);
            AssetDatabase.ImportAsset(testScenePath);
            var testScene = EditorSceneManager.OpenScene(testScenePath, OpenSceneMode.Additive);
            try
            {
            DummyController player = null;
            foreach (var root in testScene.GetRootGameObjects())
                if (root.TryGetComponent<DummyController>(out var found)) player = found;
            Require(player != null, "Scene contains a controller");
            Call("Awake");
            var body = player.GetComponent<Rigidbody2D>();

            float shortJump = Jump(1);
            float fullJump = Jump(60);
            Require(fullJump > shortJump + 5, "Holding jump produces greater vertical speed");
            Require(Mathf.Abs(fullJump - player.maximumJumpSpeed) < 0.01f, "Charge is capped at maximum speed");

            body.position = new Vector2(0, 18);
            body.linearVelocity = new Vector2(4, 3);
            Physics2D.SyncTransforms();
            int jumps = player.JumpCount;
            Set("move", -1f);
            Set("pressQueued", true);
            Set("releaseQueued", true);
            Call("FixedUpdate");
            Require(player.JumpCount == jumps, "No midair jump");
            Require(Mathf.Abs(body.linearVelocity.x - 4) < 0.01f, "No steering in the air");
            player.ResetRun();
            Require(player.JumpCount == 0 && !player.Charging && body.linearVelocity == Vector2.zero,
                "Restart clears movement, charge and jump count");
            Debug.Log("JUMPDUMMY CHECKS PASSED: ground detection, variable charge, charge cap, no air jump, no air steering, restart.");

            float Jump(int ticks)
            {
                player.ResetRun();
                body.position = new Vector2(-6, 0.56f);
                Set("ignoreGroundUntil", -1f);
                Set("move", 1f);
                Physics2D.SyncTransforms();
                Call("FixedUpdate");
                Require(player.Grounded, "Ground cast detects floor");
                Set("pressQueued", true);
                for (int i = 0; i < ticks; i++) Call("FixedUpdate");
                Require(player.Charging && Mathf.Abs(body.linearVelocity.x) < 0.01f, "Charging stops walking");
                Set("releaseQueued", true);
                Call("FixedUpdate");
                Require(player.JumpCount == 1 && !player.Grounded, "Release launches once");
                return body.linearVelocity.y;
            }

            void Call(string method) => typeof(DummyController).GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance).Invoke(player, null);
            void Set(string field, object value) => typeof(DummyController).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(player, value);
            }
            finally
            {
                EditorSceneManager.CloseScene(testScene, true);
                AssetDatabase.DeleteAsset(testScenePath);
            }
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new Exception("JumpDummy check failed: " + message);
        }
    }
}
