using UnityEngine;

namespace JumpDummy
{
    public sealed class CyberFeedback : MonoBehaviour
    {
        public DummyController player;

        [Header("Audio Clips")]
        public AudioClip jumpClip;
        public AudioClip bumpClip;
        public AudioClip landClip;
        public AudioClip fallClip;
        public AudioClip bgmClip;

        [Header("Audio Volume")]
        [Range(0f, 1f)] public float masterVolume = 0.2f;
        [Range(0f, 5f)] public float sfxVolume = 2f;
        [Range(0f, 2f)] public float bgmVolume = 2f;

        public float EffectiveSfxVolume => masterVolume * sfxVolume;
        public float EffectiveBgmVolume => masterVolume * bgmVolume;

        private ParticleSystem particles;
        private AudioSource sfxSource;
        private AudioSource bgmSource;

        private void Awake()
        {
            if (player == null) player = FindAnyObjectByType<DummyController>();
            particles = CreateParticles();

            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.volume = 1f;

            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.playOnAwake = false;
            bgmSource.loop = true;
            bgmSource.volume = EffectiveBgmVolume;

            LoadClipsIfMissing();
        }

        private void Start()
        {
            StartBgm();
        }

        public void LoadClipsIfMissing()
        {
            if (jumpClip == null) jumpClip = Resources.Load<AudioClip>("Sounds/jump");
            if (bumpClip == null) bumpClip = Resources.Load<AudioClip>("Sounds/bump");
            if (landClip == null) landClip = Resources.Load<AudioClip>("Sounds/land");
            if (fallClip == null) fallClip = Resources.Load<AudioClip>("Sounds/fall");
            if (bgmClip == null) bgmClip = Resources.Load<AudioClip>("Music/bgm_cyberpunk");
        }

        private void StartBgm()
        {
            if (bgmClip != null)
            {
                bgmSource.clip = bgmClip;
                bgmSource.volume = bgmVolume;
                if (!bgmSource.isPlaying) bgmSource.Play();
            }
        }

        private void OnEnable()
        {
            if (player == null) return;
            player.Jumped += OnJump;
            player.WallBounced += OnBump;
            player.CeilingBumped += OnBump;
            player.Landed += OnLand;
            player.Fell += OnFall;
        }

        private void OnDisable()
        {
            if (player == null) return;
            player.Jumped -= OnJump;
            player.WallBounced -= OnBump;
            player.CeilingBumped -= OnBump;
            player.Landed -= OnLand;
            player.Fell -= OnFall;
        }

        private void Update()
        {
            UpdateBgmVolume();
        }

        private void UpdateBgmVolume()
        {
            if (bgmSource == null || bgmClip == null) return;
            if (!bgmSource.isPlaying)
            {
                bgmSource.clip = bgmClip;
                bgmSource.Play();
            }

            float targetVolume = EffectiveBgmVolume;
            if (GameSession.Instance != null)
            {
                if (GameSession.Instance.Mode == GameSession.SessionMode.Paused)
                    targetVolume = EffectiveBgmVolume * 0.3f;
            }
            bgmSource.volume = Mathf.MoveTowards(bgmSource.volume, targetVolume, Time.unscaledDeltaTime * 2f);
        }

        private void OnJump()
        {
            Burst(new Color(1f, 0.16f, 0.65f), 12, 1.2f);
            if (jumpClip != null) sfxSource.PlayOneShot(jumpClip, EffectiveSfxVolume);
            
            else PlayTone(280, 0.11f);
        }

        private void OnBump()
        {
            Burst(new Color(0.25f, 0.9f, 1f), 10, 1.4f);
            if (bumpClip != null) sfxSource.PlayOneShot(bumpClip, EffectiveSfxVolume);
            else PlayTone(180, 0.08f);
        }

        private void OnLand()
        {
            Burst(new Color(0.25f, 0.9f, 1f), 8, 0.8f);
            if (landClip != null) sfxSource.PlayOneShot(landClip, EffectiveSfxVolume * 0.9f);
        }

        private void OnFall()
        {
            if (fallClip != null) sfxSource.PlayOneShot(fallClip, EffectiveSfxVolume);
        }

        private void Burst(Color color, int count, float speed)
        {
            if (player == null || particles == null) return;
            particles.transform.position = player.transform.position + Vector3.down * 0.48f;
            var main = particles.main;
            main.startColor = color;
            main.startSpeed = speed;
            particles.Emit(count);
        }

        private void PlayTone(float frequency, float duration)
        {
            const int rate = 22050;
            int samples = Mathf.CeilToInt(rate * duration);
            var data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float envelope = 1f - i / (float)samples;
                data[i] = Mathf.Sin(2 * Mathf.PI * frequency * i / rate) * envelope * 0.35f;
            }
            var clip = AudioClip.Create("Cyber cue", samples, 1, rate, false);
            clip.SetData(data, 0);
            sfxSource.PlayOneShot(clip, EffectiveSfxVolume * 0.5f);
            Destroy(clip, duration + 0.1f);
        }

        private ParticleSystem CreateParticles()
        {
            var go = new GameObject("Impact particles");
            go.transform.SetParent(transform, false);
            var system = go.AddComponent<ParticleSystem>();
            var main = system.main;
            main.playOnAwake = false;
            main.loop = false;
            main.startLifetime = 0.3f;
            main.startSize = 0.055f;
            main.gravityModifier = 0.5f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            var emission = system.emission;
            emission.enabled = false;
            var shape = system.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 55;
            shape.rotation = new Vector3(-90, 0, 0);
            var renderer = system.GetComponent<ParticleSystemRenderer>();
            renderer.sortingOrder = 30;
            renderer.material = new Material(Shader.Find("Sprites/Default"));
            return system;
        }
    }
}
