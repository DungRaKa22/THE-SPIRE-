using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace JumpDummy.Editor
{
    // Guards the two silent failures found while designing Electric Age
    // (Docs/TheSpire-Level-Sector3.md section 5):
    //   1. DummyController.OnCollisionEnter2D returns early while grounded, so a platform that
    //      waits for a collision event never shocks a player who is already standing on it.
    //   2. Only wall bounces used to feed the run visual's Bounce state, so an electric shock
    //      played no animation. Both now go through one shared impulse marker.
    public static class ElectricPlatformChecks
    {
        private static Keyboard keyboard, previousKeyboard;
        private static DummyController player;
        private static Rigidbody2D body;
        private static ElectricPlatform platform;
        private static BoxCollider2D platformShape;
        private static GameObject fixture;
        private static bool sawWarning;
        private static int stage;
        private static double start;

        [MenuItem("JumpDummy/Run Electric Platform Checks")]
        public static void Run()
        {
            if (!EditorApplication.isPlaying || keyboard != null)
                throw new InvalidOperationException("Enter Play Mode first.");
            player = UnityEngine.Object.FindAnyObjectByType<DummyController>();
            if (player == null) throw new InvalidOperationException("Enter Play Mode in a scene that has the player first.");
            var session = UnityEngine.Object.FindAnyObjectByType<GameSession>();
            Require(session != null, "Game session exists");
            body = player.GetComponent<Rigidbody2D>();
            var playerShape = player.GetComponent<BoxCollider2D>();
            Require(body != null && playerShape != null, "Player has a body and a box collider");
            session.StartNewRun();
            Require(!session.InputBlocked, "Run started, input accepted");

            Build(playerShape);
            previousKeyboard = Keyboard.current;
            keyboard = InputSystem.AddDevice<Keyboard>("ElectricCheckKeyboard");
            sawWarning = false;
            stage = 0;
            start = EditorApplication.timeSinceStartup;
            EditorApplication.update += Tick;
            EditorApplication.playModeStateChanged += OnPlayModeChange;
        }

        // Builds the fixture far below any level geometry but above the -5 fall-out line.
        private static void Build(BoxCollider2D playerShape)
        {
            const float fixtureTop = -3.6f;
            float delta = fixtureTop - playerShape.bounds.min.y;
            body.position += new Vector2(-body.position.x, delta);
            body.linearVelocity = Vector2.zero;

            fixture = new GameObject("Electric Check Fixture");

            var ground = new GameObject("Electric Check Ground");
            ground.transform.SetParent(fixture.transform, false);
            ground.transform.position = new Vector2(0f, fixtureTop - 1.1f);
            ground.AddComponent<BoxCollider2D>().size = new Vector2(14f, 1f);

            var rig = new GameObject("Electric Check Circuit");
            rig.transform.SetParent(fixture.transform, false);
            var circuit = rig.AddComponent<ElectricCircuit>();
            circuit.period = 3.4f;
            circuit.safeDuration = 1.9f;
            circuit.warnDuration = 0.5f;

            var deck = new GameObject("Electric Check Platform");
            deck.transform.SetParent(rig.transform, false);
            deck.transform.position = new Vector2(0f, fixtureTop - 0.2f);
            platformShape = deck.AddComponent<BoxCollider2D>();
            platformShape.size = new Vector2(3f, 0.4f);
            platform = deck.AddComponent<ElectricPlatform>();
            platform.circuit = circuit;
            platform.phaseOffset = circuit.OffsetForImpendingLive(Time.time, 1.5f);
            Physics2D.SyncTransforms();
        }

        private static void Tick()
        {
            if (!EditorApplication.isPlaying || player == null) { Finish("CANCELLED"); return; }
            double elapsed = EditorApplication.timeSinceStartup - start;
            try
            {
                if (platform != null && platform.State == ElectricState.Warning) sawWarning = true;
                switch (stage)
                {
                    case 0:
                        if (elapsed < 0.25) return;
                        Require(player.Grounded, "Player rests on the electric platform while SAFE");
                        Require(platform.State == ElectricState.Safe, "Platform starts in SAFE");
                        Keys(Key.D); Next(); break;
                    case 1:
                        if (elapsed < 0.2) return;
                        Require(platform.State != ElectricState.Live, "Still SAFE or WARNING while walking across");
                        Keys(Key.Space); Next(); break;
                    case 2:
                        if (elapsed < 0.3) return;
                        Require(player.Charging, "Charges on the electric platform while it is not LIVE");
                        Require(Mathf.Abs(body.linearVelocity.x) < 0.01f, "Charge stops walking on the platform");
                        Next(); break;
                    case 3:
                        if (platform.ShockCount == 0 && elapsed < 3.5) return;
                        Require(platform.ShockCount >= 1, "LIVE platform shocked a player who was already standing on it");
                        Require(Time.time - player.LastImpulseTime < 0.5f, "Shock registered on the shared impulse marker");
                        Require(body.linearVelocity.y > 5f, "Shock throws the player upward");
                        Require(body.linearVelocity.x < -0.5f, "Shock throws the player back against the entry direction");
                        Require(!player.Charging && !player.Grounded, "Shock cancels the charge and frees the player");
                        Require(platformShape.enabled && !platformShape.isTrigger && platform.enabled,
                            "Platform keeps a solid, enabled collider while LIVE");
                        Next(); break;
                    case 4:
                        if (elapsed < 0.5) return;
                        Require(sawWarning, "WARNING state telegraphs the shock before it fires");
                        player.ResetRun();
                        Finish("PASS: electric circuit clock drives SAFE/WARNING/LIVE; a player already standing on a platform is shocked by polling in FixedUpdate (not by a collision event); the shock uses the shared impulse marker read by the run visual; it throws the player up and back, cancels the charge, and never disables the platform collider. Play Mode with synthetic keyboard input.");
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
            if (fixture != null) UnityEngine.Object.Destroy(fixture);
            fixture = null;
            platform = null;
            Directory.CreateDirectory("Docs");
            File.WriteAllText("Docs/TheSpireElectricValidation.txt", result + "\n");
            Debug.Log("Electric platform checks: " + result);
        }
    }
}
