using UnityEngine;

namespace JumpDummy
{
    public sealed class CyberRunnerVisual : MonoBehaviour
    {
        public DummyController player;
        public Animator animator;
        public SpriteRenderer spriteRenderer;
        private bool wasGrounded;
        private float landingUntil;
        private int resetVersion;
        private string currentState;

        private void Update()
        {
            if (player == null || animator == null) return;
            if (resetVersion != player.ResetVersion)
            {
                resetVersion = player.ResetVersion;
                wasGrounded = false;
                landingUntil = 0;
            }
            if (player.Grounded && !wasGrounded) landingUntil = Time.time + 0.16f;
            wasGrounded = player.Grounded;
            string state;
            if (player.Charging) state = player.Charge01 > 0.55f ? "ChargeDeep" : "Charge";
            else if (!player.Grounded)
                // Any non-jump impulse (wall bounce, electric shock, bounce rail) shares one marker.
                state = Time.time - player.LastImpulseTime < 0.16f ? "Bounce" : player.Velocity.y > 0 ? "Jump" : "Fall";
            else if (Mathf.Abs(player.Velocity.x) > 0.1f) state = "Run";
            else state = Time.time < landingUntil ? "Land" : "Idle";

            int direction = player.Grounded || Mathf.Abs(player.Velocity.x) < 0.1f
                ? player.Facing : (int)Mathf.Sign(player.Velocity.x);
            spriteRenderer.flipX = direction < 0;
            if (state == currentState) return;
            animator.Play(state, 0, 0);
            currentState = state;
        }
    }
}
