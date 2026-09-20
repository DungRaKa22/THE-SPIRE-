using UnityEngine;
using UnityEngine.InputSystem;

namespace JumpDummy
{
    public sealed class GameSession : MonoBehaviour
    {
        public string saveSlot = "";
        private string Prefix => string.IsNullOrEmpty(saveSlot) ? "JumpDummy.Run." : "JumpDummy." + saveSlot + ".";
        public static GameSession Instance { get; private set; }
        public DummyController player;
        public float summitHeight = 14.3f;
        public bool InputBlocked => Mode != SessionMode.Playing;
        public SessionMode Mode { get; private set; } = SessionMode.Title;
        public float ElapsedSeconds { get; private set; }
        public float PersonalBestSeconds { get; private set; }
        public int Score => coins != null ? coins.Score : 0;
        private PlatformCoins coins;

        [Header("Spawn Configuration")]
        [Tooltip("Khi choi trong Unity Editor, luon bat dau tai vi tri dat nhan vat trong Scene")]
        public bool ignoreSavedRunInEditor = true;

        public Vector2 DefaultSpawnPosition { get; private set; }
        public bool HasSavedRun
        {
            get
            {
#if UNITY_EDITOR
                if (ignoreSavedRunInEditor) return false;
#endif
                return PlayerPrefs.GetInt(Prefix + "Valid", 0) == 1;
            }
        }

        public string StatusMessage { get; private set; } = "";
        public Vector2 Checkpoint { get; private set; }

        private float stableGroundTime;
        private float savedHeight;
        private float nextSaveTime;
        private SessionMode modeBeforePause;

        public enum SessionMode { Title, Playing, Paused, Complete }

        private void Awake()
        {
            Instance = this;
            coins = GetComponent<PlatformCoins>();
            if (coins == null) coins = gameObject.AddComponent<PlatformCoins>();
            if (player == null) player = FindAnyObjectByType<DummyController>();
            DefaultSpawnPosition = player != null ? (Vector2)player.transform.position : new Vector2(-6, 0.65f);
            Checkpoint = DefaultSpawnPosition;
            PersonalBestSeconds = PlayerPrefs.GetFloat(Prefix + "BestTime", 0);
            Time.timeScale = 0;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            Time.timeScale = 1;
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null || player == null) return;
            if (Mode == SessionMode.Title)
            {
                if (keyboard.nKey.wasPressedThisFrame) StartNewRun();
                else if (keyboard.enterKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame)
                {
                    if (HasSavedRun) ContinueRun(); else StartNewRun();
                }
                return;
            }
            if (keyboard.escapeKey.wasPressedThisFrame || keyboard.pKey.wasPressedThisFrame)
            {
                if (Mode == SessionMode.Paused) Resume();
                else if (Mode == SessionMode.Playing) Pause();
                return;
            }
            if (Mode == SessionMode.Complete)
            {
                if (keyboard.enterKey.wasPressedThisFrame || keyboard.nKey.wasPressedThisFrame) StartNewRun();
                return;
            }
            if (Mode != SessionMode.Playing) return;
            ElapsedSeconds += Time.unscaledDeltaTime;
            TrackSafePosition();
            if (player.Grounded && player.transform.position.y - 0.55f >= summitHeight)
                CompleteRun();
        }

        private void TrackSafePosition()
        {
            if (!player.Grounded || Mathf.Abs(player.Velocity.x) > 0.08f)
            {
                stableGroundTime = 0;
                return;
            }
            stableGroundTime += Time.deltaTime;
            float height = player.transform.position.y;
            if (stableGroundTime < 0.3f || height <= savedHeight + 0.35f || Time.time < nextSaveTime) return;
            Checkpoint = player.transform.position;
            savedHeight = height;
            nextSaveTime = Time.time + 0.8f;
            SaveRun();
            StatusMessage = "SAFE POSITION SAVED";
            CancelInvoke(nameof(ClearStatus));
            Invoke(nameof(ClearStatus), 1.25f);
        }

        private void ClearStatus() => StatusMessage = "";

        public void StartNewRun()
        {
            PlayerPrefs.DeleteKey(Prefix + "Valid");
            PlayerPrefs.DeleteKey(Prefix + "X");
            PlayerPrefs.DeleteKey(Prefix + "Y");
            PlayerPrefs.DeleteKey(Prefix + "Time");
            PlayerPrefs.DeleteKey(Prefix + "Jumps");
            PlayerPrefs.Save();
            ElapsedSeconds = 0;
            coins.Begin(this, UnityEngine.Random.Range(1, int.MaxValue));
            savedHeight = 0;
            Checkpoint = DefaultSpawnPosition;
            if (player != null) player.SetRunState(Checkpoint, 0);
            Mode = SessionMode.Playing;
            Time.timeScale = 1;
            SaveRun();
        }

        public void ContinueRun()
        {
            coins.Begin(this, PlayerPrefs.GetInt(Prefix + "CoinSeed", 1), PlayerPrefs.GetString(Prefix + "Coins", ""));
            ElapsedSeconds = PlayerPrefs.GetFloat(Prefix + "Time", 0);
            Checkpoint = new Vector2(
                PlayerPrefs.GetFloat(Prefix + "X", DefaultSpawnPosition.x),
                PlayerPrefs.GetFloat(Prefix + "Y", DefaultSpawnPosition.y)
            );
            savedHeight = Checkpoint.y;
            if (player != null) player.SetRunState(Checkpoint, PlayerPrefs.GetInt(Prefix + "Jumps", 0));
            Mode = SessionMode.Playing;
            Time.timeScale = 1;
        }

        [ContextMenu("Clear Saved Progress")]
        public void ClearSavedProgress()
        {
            PlayerPrefs.DeleteKey(Prefix + "CoinSeed");
            PlayerPrefs.DeleteKey(Prefix + "Coins");
            PlayerPrefs.DeleteKey(Prefix + "Valid");
            PlayerPrefs.DeleteKey(Prefix + "X");
            PlayerPrefs.DeleteKey(Prefix + "Y");
            PlayerPrefs.DeleteKey(Prefix + "Time");
            PlayerPrefs.DeleteKey(Prefix + "Jumps");
            PlayerPrefs.Save();
            Debug.Log("JumpDummy: Đã xóa tiến độ lưu.");
        }

        public void RespawnAfterFall()
        {
            player.SetRunState(Checkpoint, player.JumpCount);
            StatusMessage = "SIGNAL RESTORED";
            CancelInvoke(nameof(ClearStatus));
            Invoke(nameof(ClearStatus), 1.1f);
        }

        public void Pause()
        {
            modeBeforePause = Mode;
            SaveRun();
            Mode = SessionMode.Paused;
            Time.timeScale = 0;
        }

        public void Resume()
        {
            Mode = modeBeforePause == SessionMode.Complete ? SessionMode.Complete : SessionMode.Playing;
            Time.timeScale = Mode == SessionMode.Playing ? 1 : 0;
        }

        private void CompleteRun()
        {
            Mode = SessionMode.Complete;
            Time.timeScale = 0;
            PlayerPrefs.DeleteKey(Prefix + "Valid");
            if (PersonalBestSeconds <= 0 || ElapsedSeconds < PersonalBestSeconds)
            {
                PersonalBestSeconds = ElapsedSeconds;
                PlayerPrefs.SetFloat(Prefix + "BestTime", PersonalBestSeconds);
            }
            PlayerPrefs.Save();
        }

        private void SaveRun()
        {
            if (Mode != SessionMode.Playing) return;
            PlayerPrefs.SetInt(Prefix + "Valid", 1);
            PlayerPrefs.SetFloat(Prefix + "X", Checkpoint.x);
            PlayerPrefs.SetFloat(Prefix + "Y", Checkpoint.y);
            PlayerPrefs.SetFloat(Prefix + "Time", ElapsedSeconds);
            PlayerPrefs.SetInt(Prefix + "Jumps", player.JumpCount);
            PlayerPrefs.SetInt(Prefix + "CoinSeed", coins.Seed);
            PlayerPrefs.SetString(Prefix + "Coins", coins.CollectedIds);
            PlayerPrefs.Save();
        }

        public void SaveCoinProgress()
        {
            SaveRun();
            StatusMessage = "+10 POINTS";
            CancelInvoke(nameof(ClearStatus));
            Invoke(nameof(ClearStatus), 1.1f);
        }

        private void OnApplicationPause(bool paused) { if (paused && Mode == SessionMode.Playing) SaveRun(); }
        private void OnApplicationQuit() { if (Mode == SessionMode.Playing) SaveRun(); }

        public static string FormatTime(float seconds)
        {
            int total = Mathf.Max(0, Mathf.FloorToInt(seconds));
            return $"{total / 60:00}:{total % 60:00}.{Mathf.FloorToInt(seconds * 10) % 10}";
        }
    }
}
