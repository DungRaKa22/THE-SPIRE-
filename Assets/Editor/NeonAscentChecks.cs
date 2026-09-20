using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JumpDummy.Editor
{
    public static class NeonAscentChecks
    {
        public static void RunBatch()
        {
            if (!Application.isBatchMode) throw new Exception("Run in an isolated batch project to protect saved progress.");
            if (!File.Exists(NeonAscentBuilder.ScenePath)) NeonAscentBuilder.Build();
            else EditorSceneManager.OpenScene(NeonAscentBuilder.ScenePath);
            PlayerSettings.companyName = "JumpDummyChecks";
            PlayerSettings.productName = "IsolatedAscentValidation";
            SessionState.SetBool("AscentCheck", true);
            EditorApplication.EnterPlaymode();
        }

        [InitializeOnLoadMethod]
        private static void Setup()
        {
            EditorApplication.playModeStateChanged += state =>
            {
                if (state == PlayModeStateChange.EnteredPlayMode && SessionState.GetBool("AscentCheck", false))
                    EditorApplication.update += Check;
            };
        }

        private static void Check()
        {
            EditorApplication.update -= Check;
            SessionState.SetBool("AscentCheck", false);
            try
            {
                var player = Object.FindAnyObjectByType<DummyController>();
                var body = player.GetComponent<Rigidbody2D>();
                var shape = player.GetComponent<BoxCollider2D>();
                var session = Object.FindAnyObjectByType<GameSession>();
                var feedback = Object.FindAnyObjectByType<CyberFeedback>();
                if (feedback != null) feedback.enabled = false;
                session.StartNewRun();
                player.enabled = false;
                Physics2D.simulationMode = SimulationMode2D.Script;
                Physics2D.SyncTransforms();
                var blocks = Object.FindObjectsByType<BoxCollider2D>().Where(b => b != shape && !b.isTrigger).ToArray();
                var route = blocks.Where(b => int.TryParse(b.name.Split(' ')[0], out _)).OrderBy(b => b.name).ToArray();
                Require(route.Length == 35, "35 authored platforms");
                Require(Object.FindObjectsByType<PlatformCoinPickup>().Length == 7, "7 coins: one per five platforms");
                Require(session.summitHeight > 64, "Summit moved above 64m");
                Require(Object.FindObjectsByType<SpriteRenderer>().All(s => s.sprite != null), "All sprite references resolve");
                Directory.CreateDirectory("Docs");
                Capture(player);
                var report = new List<string>();
                var floor = blocks.Single(b => b.name == "Floor");
                BoxCollider2D previous = floor;
                foreach (var target in route)
                {
                    bool found = false;
                    Bounds origin = previous.bounds, goal = target.bounds;
                    // Sample legal takeoff positions and charge strengths, then replay with Unity physics.
                    for (int start = 0; start <= 20 && !found; start++)
                    {
                        float x = Mathf.Lerp(origin.min.x + .45f, origin.max.x - .45f, start / 20f);
                        var pos = new Vector2(x, origin.max.y + shape.size.y / 2 + .012f);
                        for (int q = 0; q <= 40 && !found; q++)
                        {
                            float strength = q / 40f;
                            int direction = goal.center.x >= x ? 1 : -1;
                            var velocity = new Vector2(direction * player.horizontalJumpSpeed * Mathf.Lerp(.4f, 1f, strength),
                                Mathf.Lerp(player.minimumJumpSpeed, player.maximumJumpSpeed, strength));
                            if (!Predict(pos, velocity, shape.size, previous, target, blocks, player.gravityScale)) continue;
                            body.position = pos; body.linearVelocity = velocity;
                            Physics2D.SyncTransforms();
                            for (int tick = 0; tick < 85; tick++)
                            {
                                Physics2D.Simulate(.02f);
                                var p = body.position;
                                if (Mathf.Abs(p.y - shape.size.y / 2 - goal.max.y) < .06f &&
                                    p.x > goal.min.x + .3f && p.x < goal.max.x - .3f && Mathf.Abs(body.linearVelocity.y) < .1f)
                                { found = true; break; }
                                if (p.y < origin.max.y - 1) break;
                            }
                            if (found) report.Add($"PASS {target.name}: start x={x:0.00}, charge={strength:0.000}, top={goal.max.y:0.00}");
                        }
                    }
                    Require(found, "Reachable with real physics: " + target.name);
                    previous = target;
                }
                // Keep the coin rule and save integration covered in the extended scene.
                session.StartNewRun();
                var coins = session.GetComponent<PlatformCoins>();
                Require(coins.TryCollect(0) && session.Score == 10, "Coin adds score");
                session.ContinueRun();
                Require(session.Score == 10 && Object.FindObjectsByType<PlatformCoinPickup>().Length == 6, "Saved coin stays collected");
                session.StartNewRun();
                Require(session.Score == 0 && Object.FindObjectsByType<PlatformCoinPickup>().Length == 7, "Restart resets coins");
                Directory.CreateDirectory("Docs");
                File.WriteAllLines("Docs/NeonAscentValidation.txt", report.Concat(new[] { "PASS: 35 physics-tested route links, 7 coins, save/continue, restart, assets, summit." }));
                Capture(player);
                Debug.Log("NEON ASCENT CHECKS PASSED");
                EditorApplication.Exit(0);
            }
            catch (Exception e) { Debug.LogException(e); EditorApplication.Exit(1); }
        }

        private static bool Predict(Vector2 pos, Vector2 v, Vector2 size, BoxCollider2D origin, BoxCollider2D target, BoxCollider2D[] blocks, float gravity)
        {
            // Reject any side/ceiling contact, even when it might be salvageable by a wall bounce.
            var half = size / 2;
            for (int tick = 0; tick < 70; tick++)
            {
                v.y += Physics2D.gravity.y * gravity * .02f;
                Vector2 next = pos + v * .02f;
                var goal = target.bounds;
                if (v.y < 0 && pos.y - half.y >= goal.max.y && next.y - half.y <= goal.max.y &&
                    next.x > goal.min.x + half.x + .06f && next.x < goal.max.x - half.x - .06f) return true;
                foreach (var b in blocks)
                {
                    if (b == origin && tick < 2) continue;
                    Bounds bound = b.bounds;
                    if (next.x + half.x > bound.min.x && next.x - half.x < bound.max.x &&
                        next.y + half.y > bound.min.y && next.y - half.y < bound.max.y) return false;
                }
                pos = next;
            }
            return false;
        }

        private static void Capture(DummyController player)
        {
            var hud = Object.FindAnyObjectByType<CyberpunkPresentation>();
            hud.enabled = false;
            if (hud.chargeFill != null) hud.chargeFill.gameObject.SetActive(false);
            var camera = hud.gameCamera;
            var rt = new RenderTexture(1200, 900, 24);
            camera.targetTexture = rt;
            camera.orthographicSize = 7.35f;
            camera.aspect = 4f / 3;
            player.GetComponent<Rigidbody2D>().position = new Vector2(-7.2f,.65f);
            Physics2D.SyncTransforms();
            for (int zone = 0; zone < 5; zone++)
            {
                camera.transform.position = new Vector3(0, zone == 4 ? 60 : 5.9f + zone * 13, -10);
                camera.Render();
                RenderTexture.active = rt;
                var texture = new Texture2D(1200,900,TextureFormat.RGB24,false);
                texture.ReadPixels(new Rect(0,0,1200,900),0,0); texture.Apply();
                File.WriteAllBytes($"Docs/NeonAscent-{zone + 1}.png",texture.EncodeToPNG());
                Object.DestroyImmediate(texture);
            }
            RenderTexture.active = null; camera.targetTexture = null; rt.Release(); Object.DestroyImmediate(rt);
        }

        private static void Require(bool ok, string message)
        {
            if (!ok) throw new Exception(message);
            Debug.Log("PASS: " + message);
        }
    }
}
