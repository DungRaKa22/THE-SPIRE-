using System;
using UnityEngine;

namespace JumpDummy
{
    // Electric platform: it never moves and never loses its collider. Only the state changes, so a
    // LIVE platform is still solid - it just throws whoever is standing on it.
    //
    // The shock is applied by polling in FixedUpdate, not by a collision callback: DummyController
    // .OnCollisionEnter2D starts with "if (Grounded) return", so a player already standing on the
    // platform produces no new collision event and a callback-driven shock would never fire.
    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class ElectricPlatform : MonoBehaviour
    {
        public ElectricCircuit circuit;

        [Tooltip("Seconds added to the circuit clock for this platform's phase.")]
        public float phaseOffset;

        [Header("Shock")]
        [Min(0f)] public float backVelocity = 1.5f;
        [Min(0f)] public float liftVelocity = 7.5f;
        [Min(0.05f)] public float pollInterval = 0.2f;

        [Header("Visual")]
        public SpriteRenderer target;
        public Color safeColor = new Color(0.62f, 0.72f, 0.8f);
        public Color warnColor = new Color(1f, 0.85f, 0.25f);
        public Color liveColor = new Color(0.45f, 0.98f, 1f);

        public ElectricState State { get; private set; } = ElectricState.Safe;
        public int ShockCount { get; private set; }
        public float SecondsUntilLive => circuit != null ? circuit.TimeUntilLive(Time.time, phaseOffset) : 0f;
        public event Action<ElectricState> StateChanged;
        public event Action<DummyController> Shocked;

        private BoxCollider2D shape;
        private readonly Collider2D[] overlap = new Collider2D[8];
        private ContactFilter2D filter;
        private float nextPoll;

        private void Awake()
        {
            shape = GetComponent<BoxCollider2D>();
            if (circuit == null) circuit = GetComponentInParent<ElectricCircuit>();
            filter = new ContactFilter2D { useTriggers = false };
        }

        private void Update()
        {
            if (circuit == null) return;
            var next = circuit.StateAt(Time.time, phaseOffset);
            if (next != State)
            {
                State = next;
                StateChanged?.Invoke(State);
            }
            Paint();
        }

        private void FixedUpdate()
        {
            if (circuit == null) return;
            if (circuit.StateAt(Time.time, phaseOffset) != ElectricState.Live) return;
            if (Time.time < nextPoll) return;
            nextPoll = Time.time + pollInterval;
            Shock(FindWalker());
        }

        // Whoever is resting on the top face. Uses the ground contact rule of the controller
        // (Grounding is itself a downward cast that ignores triggers), so trigger-only volumes
        // would never be standable in the first place.
        private DummyController FindWalker()
        {
            Bounds bounds = shape.bounds;
            Vector2 center = new Vector2(bounds.center.x, bounds.max.y);
            Vector2 size = new Vector2(Mathf.Max(0.05f, bounds.size.x - 0.02f), 0.16f);
            int count = Physics2D.OverlapBox(center, size, 0f, filter, overlap);
            for (int i = 0; i < count; i++)
            {
                Collider2D hit = overlap[i];
                if (hit == null) continue;
                Rigidbody2D body = hit.attachedRigidbody;
                var walker = body != null ? body.GetComponent<DummyController>() : null;
                if (walker == null || !walker.Grounded) continue;
                if (walker.transform.position.y < bounds.max.y - 0.05f) continue;
                return walker;
            }
            return null;
        }

        private void Shock(DummyController walker)
        {
            if (walker == null) return;
            Rigidbody2D body = walker.GetComponent<Rigidbody2D>();
            if (body == null) return;
            // Throw back the way the player arrived; while charging there is no horizontal velocity,
            // so fall back to the facing they locked in.
            int direction = Mathf.Abs(body.linearVelocity.x) >= 0.5f
                ? (int)Mathf.Sign(body.linearVelocity.x)
                : walker.Facing;
            walker.ApplyImpulse(new Vector2(-direction * backVelocity, liftVelocity));
            ShockCount++;
            Shocked?.Invoke(walker);
        }

        private void Paint()
        {
            if (target == null) return;
            switch (State)
            {
                case ElectricState.Warning:
                    // 4 Hz blink: readable without audio, and still solid to stand on.
                    target.color = Mathf.Repeat(Time.time * 4f, 1f) < 0.5f ? warnColor : safeColor;
                    break;
                case ElectricState.Live:
                    target.color = liveColor;
                    break;
                default:
                    target.color = safeColor;
                    break;
            }
        }

        private void OnDrawGizmosSelected()
        {
            var box = GetComponent<BoxCollider2D>();
            if (box == null) return;
            Bounds bounds = box.bounds;
            Gizmos.color = new Color(0.45f, 0.98f, 1f, 0.35f);
            Gizmos.DrawWireCube(new Vector3(bounds.center.x, bounds.max.y, 0f),
                new Vector3(Mathf.Max(0.05f, bounds.size.x - 0.02f), 0.16f, 0f));
        }
    }
}
