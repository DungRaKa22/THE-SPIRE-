using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace JumpDummy.Editor
{
    public static class CyberpunkPlayChecks
    {
        private static Keyboard keyboard, previousKeyboard;
        private static DummyController player;
        private static Rigidbody2D body;
        private static Animator animator;
        private static GameSession session;
        private static int stage;
        private static double start;

        [MenuItem("JumpDummy/Run Cyberpunk Play Checks")]
        public static void Run()
        {
            if (!EditorApplication.isPlaying || keyboard != null)
                throw new InvalidOperationException("Enter Play Mode in Cyberpunk Rooftops first.");
            player = UnityEngine.Object.FindAnyObjectByType<DummyController>();
            var visual = player != null ? player.GetComponent<CyberRunnerVisual>() : null;
            if (visual == null) throw new InvalidOperationException("Open Cyberpunk Rooftops first.");
            animator = visual.animator;
            body = player.GetComponent<Rigidbody2D>();
            session = UnityEngine.Object.FindAnyObjectByType<GameSession>();
            Require(session != null, "Game session exists");
            var feedback = UnityEngine.Object.FindAnyObjectByType<CyberFeedback>();
            Require(feedback != null, "Cyber feedback exists");
            feedback.LoadClipsIfMissing();
            Require(feedback.jumpClip != null && feedback.bumpClip != null && feedback.landClip != null && feedback.fallClip != null && feedback.bgmClip != null, "Jump King audio SFX and BGM clips loaded");
            session.StartNewRun();
            previousKeyboard = Keyboard.current;
            keyboard = InputSystem.AddDevice<Keyboard>("CyberpunkTestKeyboard");
            player.ResetRun();
            // Start clear of the first platform side so this phase isolates takeoff.
            body.position = new Vector2(-7.5f, 0.65f);
            stage = 0;
            start = EditorApplication.timeSinceStartup;
            EditorApplication.update += Tick;
            EditorApplication.playModeStateChanged += OnPlayModeChange;
        }

        private static void Tick()
        {
            if (!EditorApplication.isPlaying || player == null) { Finish("CANCELLED"); return; }
            double elapsed = EditorApplication.timeSinceStartup - start;
            try
            {
                switch (stage)
                {
                    case 0:
                        if (elapsed < 0.6) return;
                        Require(player.Grounded && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"), "Idle and grounded");
                        Keys(Key.D); Next(); break;
                    case 1:
                        if (elapsed < 0.2) return;
                        Require(animator.GetCurrentAnimatorStateInfo(0).IsName("Run"), "Run animation");
                        Keys(Key.D, Key.Space); Next(); break;
                    case 2:
                        if (elapsed < 0.7) return;
                        Require(player.Charging && animator.GetCurrentAnimatorStateInfo(0).IsName("ChargeDeep"), "Charge animation");
                        Require(Mathf.Abs(body.linearVelocity.x) < 0.01f, "Stationary charge");
                        Keys(Key.D); Next(); break;
                    case 3:
                        if (elapsed < 0.12) return;
                        Require(player.JumpCount == 1 && body.linearVelocity.y > 0 && animator.GetCurrentAnimatorStateInfo(0).IsName("Jump"), "Launch animation");
                        Keys(Key.A); Next(); break;
                    case 4:
                        if (elapsed < 0.3) return;
                        Require(body.linearVelocity.x > 0, "No midair steering");
                        Keys(); Next(); break;
                    case 5:
                        if (elapsed < 0.08) return;
                        Require(!player.Grounded && body.linearVelocity.y < 0 && animator.GetCurrentAnimatorStateInfo(0).IsName("Fall"), "Fall animation");
                        Next(); break;
                    case 6:
                        if (elapsed < 0.9) return;
                        Require(player.Grounded, "Lands on platform or floor");
                        body.position = new Vector2(8.45f, 5.2f);
                        body.linearVelocity = new Vector2(6, 0);
                        Next(); break;
                    case 7:
                        if (elapsed < 0.12) return;
                        Require(body.linearVelocity.x < 0 && Time.time - player.LastWallBounceTime < 0.25f, "Wall bounce");
                        body.position = new Vector2(6.6f, 0.7f);
                        body.linearVelocity = new Vector2(0, 13);
                        Next(); break;
                    case 8:
                        if (elapsed < 0.2) return;
                        Require(body.linearVelocity.y <= 0, "Ceiling stops ascent");
                        player.ResetRun(); Next(); break;
                    case 9:
                        if (elapsed < 0.5) return;
                        ScreenCapture.CaptureScreenshot("Docs/CyberpunkGameplay.png");
                        session.Pause();
                        Require(session.Mode == GameSession.SessionMode.Paused && Mathf.Approximately(Time.timeScale, 0), "Pause");
                        session.Resume();
                        Require(session.Mode == GameSession.SessionMode.Playing && Mathf.Approximately(Time.timeScale, 1), "Resume");
                        session.RespawnAfterFall();
                        Require(Vector2.Distance(body.position, session.Checkpoint) < 0.01f, "Checkpoint respawn");
                        Finish("PASS: title session, Jump King SFX (jump/bump/land/fall) and cyberpunk BGM verified; idle, run, charge, jump, fall animations; charge stops walking; no midair steering; landing; wall bounce; ceiling impact; pause/resume; checkpoint respawn. Play Mode with synthetic keyboard input.");
                        break;
                }
            }
            catch (Exception error) { Finish("FAIL: " + error.Message); Debug.LogException(error); }
        }

        private static void Require(bool valid, string message) { if (!valid) throw new Exception(message); }
        private static void Keys(params Key[] keys) => InputSystem.QueueStateEvent(keyboard, new KeyboardState(keys));
        private static void Next() { stage++; start = EditorApplication.timeSinceStartup; }
        private static void OnPlayModeChange(PlayModeStateChange state) { if (state == PlayModeStateChange.ExitingPlayMode) Finish("CANCELLED"); }
        private static void Finish(string result)
        {
            EditorApplication.update -= Tick;
            EditorApplication.playModeStateChanged -= OnPlayModeChange;
            if (keyboard != null) InputSystem.RemoveDevice(keyboard);
            keyboard = null;
            if (previousKeyboard != null && previousKeyboard.added) previousKeyboard.MakeCurrent();
            Directory.CreateDirectory("Docs");
            File.WriteAllText("Docs/CyberpunkPlayValidation.txt", result + "\n");
            Debug.Log("Cyberpunk Play checks: " + result);
        }
    }
}
