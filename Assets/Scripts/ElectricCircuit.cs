using UnityEngine;

namespace JumpDummy
{
    public enum ElectricState
    {
        Safe,
        Warning,
        Live
    }

    // One clock per circuit; platforms only declare an offset. Phase differences decide the safe
    // windows in Docs/TheSpire-Level-Sector3.md section 3-4, so every platform must read the same
    // clock. A per-object accumulator drifts by a frame and silently invalidates those windows.
    public sealed class ElectricCircuit : MonoBehaviour
    {
        [Min(0.2f)] public float period = 6f;
        [Min(0f)] public float safeDuration = 4f;
        [Min(0.5f)] public float warnDuration = 0.8f;
        public float circuitOffset;

        public float LiveStart => safeDuration + warnDuration;
        public float LiveDuration => Mathf.Max(0f, period - LiveStart);

        private void OnValidate()
        {
            period = Mathf.Max(0.2f, period);
            safeDuration = Mathf.Clamp(safeDuration, 0f, Mathf.Max(0f, period - 0.05f));
        }

        public ElectricState StateAt(float time, float platformOffset)
        {
            float u = Mathf.Repeat(time + circuitOffset + platformOffset, period);
            if (u < safeDuration) return ElectricState.Safe;
            if (u < LiveStart) return ElectricState.Warning;
            return ElectricState.Live;
        }

        // Seconds until the platform reaches the start of its LIVE window; 0 while it is already live.
        public float TimeUntilLive(float time, float platformOffset)
        {
            float u = Mathf.Repeat(time + circuitOffset + platformOffset, period);
            return u < LiveStart ? LiveStart - u : 0f;
        }

        // Seconds of SAFE left. Negative once the platform is warning or live.
        public float SafeRemaining(float time, float platformOffset)
        {
            float u = Mathf.Repeat(time + circuitOffset + platformOffset, period);
            return safeDuration - u;
        }

        // Offset that leaves the platform `secondsUntilLive` before its LIVE window at `time`.
        // Used by builders, validators and checks to schedule a phase deterministically.
        public float OffsetForImpendingLive(float time, float secondsUntilLive)
        {
            float wanted = Mathf.Repeat(LiveStart - secondsUntilLive, period);
            return Mathf.Repeat(wanted - time - circuitOffset, period);
        }

        // Difficulty presets from Docs/TheSpire-Level-Sector3.md section 2.1.
        [ContextMenu("Preset E-A (introduce)")]
        public void ApplyPresetEA() { Set(6f, 4f, 0.8f); }

        [ContextMenu("Preset E-B (body of sector)")]
        public void ApplyPresetEB() { Set(4.6f, 2.8f, 0.6f); }

        [ContextMenu("Preset E-C (climax)")]
        public void ApplyPresetEC() { Set(3.4f, 1.9f, 0.5f); }

        public void Set(float period, float safe, float warn)
        {
            this.period = Mathf.Max(0.2f, period);
            safeDuration = safe;
            warnDuration = Mathf.Max(0.5f, warn);
        }
    }
}
