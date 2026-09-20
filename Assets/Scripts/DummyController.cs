using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace JumpDummy
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public sealed class DummyController : MonoBehaviour
    {
        [Header("Movement")]
        [Min(0)] public float walkSpeed = 3f;
        [Min(0.05f)] public float chargeDuration = 0.85f;
        public float minimumJumpSpeed = 4f;
        public float maximumJumpSpeed = 13f;
        public float horizontalJumpSpeed = 6f;
        [Range(0, 1)] public float wallBounceRetention = 0.8f;
        public float gravityScale = 3f;

        public bool Grounded { get; private set; }
        public bool Charging { get; private set; }
        public float Charge01 => Mathf.Clamp01(chargeTime / chargeDuration);
        public int JumpCount { get; private set; }
        public Vector2 Velocity => body != null ? body.linearVelocity : Vector2.zero;
        public int Facing { get; private set; } = 1;
        public float LastWallBounceTime { get; private set; } = -1000;
        // Shared marker for every impulse that is not a normal jump (wall bounce, electric shock,
        // bounce rail). The run visual reads it for its Bounce state, so external kicks must go
        // through ApplyImpulse instead of writing linearVelocity directly.
        public float LastImpulseTime { get; private set; } = -1000;
        // Surface the player was standing on during the previous FixedUpdate. Lets the
        // controller ride moving platforms: see CheckGrounded for why the absolute
        // linearVelocity test cannot decide groundedness on an ascending piston.
        public IMovingSurface CurrentSurface { get; set; }
        public int ResetVersion { get; private set; }
        public event Action Jumped;
        public event Action WallBounced;
        public event Action CeilingBumped;
        public event Action Landed;
        public event Action Fell;
        public string State => Charging ? "CHARGING" : Grounded ? "READY" : body.linearVelocity.y > 0 ? "RISING" : "FALLING";

        private Rigidbody2D body;
        private BoxCollider2D shape;
        private readonly RaycastHit2D[] groundHits = new RaycastHit2D[8];
        private ContactFilter2D groundFilter;
        private Vector2 spawn;
        private Vector2 incomingVelocity;
        private float move;
        private float chargeTime;
        private float ignoreGroundUntil;
        private int jumpDirection;
        private bool pressQueued;
        private bool releaseQueued;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            shape = GetComponent<BoxCollider2D>();
            body.gravityScale = gravityScale;
            body.freezeRotation = true;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            groundFilter = new ContactFilter2D { useTriggers = false };
            spawn = body.position;
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;
            if (GameSession.Instance != null && GameSession.Instance.InputBlocked)
            {
                move = 0;
                pressQueued = releaseQueued = false;
                CancelCharge();
                return;
            }
            move = (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed ? 1 : 0)
                 - (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed ? 1 : 0);
            if (Grounded && move != 0) Facing = (int)move;
            pressQueued |= keyboard.spaceKey.wasPressedThisFrame;
            releaseQueued |= keyboard.spaceKey.wasReleasedThisFrame;
            if (keyboard.rKey.wasPressedThisFrame)
            {
                if (GameSession.Instance != null) GameSession.Instance.StartNewRun();
                else ResetRun();
            }
        }

        private void FixedUpdate()
        {
            CheckGrounded();

            if (!Grounded) CancelCharge();
            if (pressQueued && Grounded)
            {
                Charging = true;
                chargeTime = 0;
                jumpDirection = (int)move;
            }

            if (Grounded)
            {
                body.linearVelocity = new Vector2(Charging ? 0 : move * walkSpeed, body.linearVelocity.y);
                if (Charging)
                {
                    // Direction can be chosen while charging; it is locked at takeoff.
                    jumpDirection = (int)move;
                    chargeTime = Mathf.Min(chargeTime + Time.fixedDeltaTime, chargeDuration);
                    if (releaseQueued)
                    {
                        float strength = Charge01;
                        body.linearVelocity = new Vector2(jumpDirection * horizontalJumpSpeed * Mathf.Lerp(0.4f, 1f, strength),
                            Mathf.Lerp(minimumJumpSpeed, maximumJumpSpeed, strength));
                        ignoreGroundUntil = Time.time + 0.1f;
                        Grounded = false;
                        CurrentSurface = null;
                        JumpCount++;
                        Jumped?.Invoke();
                        CancelCharge();
                    }
                }
            }
            pressQueued = releaseQueued = false;
            incomingVelocity = body.linearVelocity;
            if (body.position.y < -5)
            {
                Fell?.Invoke();
                if (GameSession.Instance != null) GameSession.Instance.RespawnAfterFall();
                else ResetRun();
            }
        }

        // Grounded means "not separating from what is under the feet", measured RELATIVE to the
        // surface. An ascending piston carries the player at +3 m/s, so the absolute body
        // linearVelocity is +3 and the old `y <= 0.1f` test failed every frame -> CancelCharge()
        // every frame -> charging impossible for the whole Steam sector (Docs/TheSpire-Level-
        // Sector2.md, risk #1).
        private void CheckGrounded()
        {
            bool wasGrounded = Grounded;
            Grounded = false;
            CurrentSurface = null;
            if (Time.time >= ignoreGroundUntil)
            {
                int count = shape.Cast(Vector2.down, groundFilter, groundHits, 0.045f);
                // Fastest upward surface under the feet, from THIS frame's cast.
                float maxUpward = 0f;
                for (int i = 0; i < count; i++)
                {
                    if (groundHits[i].normal.y <= 0.7f) continue;
                    var surface = groundHits[i].collider != null
                        ? groundHits[i].collider.GetComponentInParent<IMovingSurface>()
                        : null;
                    if (surface != null) maxUpward = Mathf.Max(maxUpward, surface.SurfaceVelocity.y);
                }
                if (body.linearVelocity.y - maxUpward <= 0.1f)
                {
                    for (int i = 0; i < count; i++)
                        if (groundHits[i].normal.y > 0.7f)
                        {
                            Grounded = true;
                            CurrentSurface = groundHits[i].collider != null
                                ? groundHits[i].collider.GetComponentInParent<IMovingSurface>()
                                : null;
                        }
                }
            }

            if (Grounded && !wasGrounded && incomingVelocity.y <= -0.5f)
            {
                Landed?.Invoke();
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (Grounded) return;
            Vector2 velocity = body.linearVelocity;
            for (int i = 0; i < collision.contactCount; i++)
            {
                Vector2 normal = collision.GetContact(i).normal;
                if (Mathf.Abs(normal.x) > 0.7f && incomingVelocity.x * normal.x < -0.1f)
                {
                    velocity.x = -incomingVelocity.x * wallBounceRetention;
                    LastWallBounceTime = Time.time;
                    NoteImpulse();
                    WallBounced?.Invoke();
                }
                if (normal.y < -0.7f && incomingVelocity.y > 0)
                {
                    // Rising into a ceiling kills only the jump's own upward motion. The piston
                    // carried case is designed out instead (3.2 m clearance rule, Docs/
                    // TheSpire-Level-Sector2.md risk #3), so no relative-velocity handling here.
                    velocity.y = Mathf.Min(0, velocity.y);
                }
            }
            body.linearVelocity = velocity;
        }

        private void OnApplicationFocus(bool focused)
        {
            if (focused) return;
            CancelCharge();
            pressQueued = releaseQueued = false;
            move = 0;
        }

        private void CancelCharge() { Charging = false; chargeTime = 0; }

        private void NoteImpulse() { LastImpulseTime = Time.time; }

        // External kick (electric platform, bounce rail). Call this from FixedUpdate, never from a
        // collision callback: OnCollisionEnter2D rewrites linearVelocity from the player's own state
        // and would erase the kick. ignoreGroundUntil also keeps the next FixedUpdate from treating
        // the player as still standing on whatever threw them.
        public void ApplyImpulse(Vector2 velocity)
        {
            body.linearVelocity = velocity;
            incomingVelocity = velocity;
            Grounded = false;
            CurrentSurface = null;
            ignoreGroundUntil = Time.time + 0.1f;
            CancelCharge();
            NoteImpulse();
        }

        public void ResetRun()
        {
            SetRunState(spawn, 0);
            ResetVersion++;
            LastWallBounceTime = -1000;
        }

        public void SetRunState(Vector2 position, int jumps)
        {
            body.position = position;
            body.linearVelocity = Vector2.zero;
            incomingVelocity = Vector2.zero;
            Grounded = false;
            CurrentSurface = null;
            LastImpulseTime = -1000;
            ignoreGroundUntil = Time.time + 0.1f;
            JumpCount = Mathf.Max(0, jumps);
            pressQueued = releaseQueued = false;
            CancelCharge();
        }
    }
}
