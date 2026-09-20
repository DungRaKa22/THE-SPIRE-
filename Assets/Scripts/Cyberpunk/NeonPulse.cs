using UnityEngine;

namespace JumpDummy
{
    public sealed class NeonPulse : MonoBehaviour
    {
        public float speed = 1.8f;
        [Range(0, 0.5f)] public float amount = 0.15f;
        public float phase;
        private SpriteRenderer target;
        private Color baseColor;
        private void Awake() { target = GetComponent<SpriteRenderer>(); baseColor = target.color; }
        private void Update()
        {
            Color color = baseColor;
            color.a *= 1 - amount + amount * Mathf.Sin(Time.time * speed + phase);
            target.color = color;
        }
    }
}
