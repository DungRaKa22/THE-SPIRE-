using UnityEngine;

namespace JumpDummy
{
    public sealed class CyberpunkPresentation : MonoBehaviour
    {
        public DummyController player;
        public Camera gameCamera;
        public Transform chargeFill;
        public Transform skyline;
        public float summitHeight = 14.3f;
        public GameSession session;
        public bool ascentLevel;
        private static readonly string[] Zones = { "01 / CONDENSER ALLEY", "02 / NEON MARKET", "03 / SERVICE SHAFT", "04 / CHIMNEY DISTRICT", "05 / SKYLINE UPLINK" };
        private GUIStyle title, small, number;
        private int resetVersion;
        private bool reachedRoof;
        private float bestHeight;
        private readonly Color cyan = new Color(0.2f, 0.92f, 1f);
        private readonly Color pink = new Color(1f, 0.2f, 0.66f);

        private void LateUpdate()
        {
            if (player == null || gameCamera == null) return;
            if (session == null) session = GameSession.Instance;
            gameCamera.orthographicSize = Mathf.Max(5.8f, 9.8f / gameCamera.aspect);
            float y = Mathf.Max(gameCamera.orthographicSize - 1.6f, player.transform.position.y + 1.5f);
            gameCamera.transform.position = new Vector3(0, y, -10);
            if (skyline != null) skyline.localPosition = new Vector3(0, (y - 4.2f) * 0.35f, 0);
            if (chargeFill != null)
            {
                chargeFill.gameObject.SetActive(player.Charging);
                chargeFill.localPosition = new Vector3(-0.45f + player.Charge01 * 0.45f, 1.1f, 0);
                chargeFill.localScale = new Vector3(Mathf.Max(0.001f, player.Charge01 * 0.9f), 0.06f, 1);
            }
            if (resetVersion != player.ResetVersion)
            {
                resetVersion = player.ResetVersion;
                reachedRoof = false;
                bestHeight = 0;
            }
            bestHeight = Mathf.Max(bestHeight, player.transform.position.y - 0.55f);
            if (player.Grounded && player.transform.position.y - 0.55f >= summitHeight - 0.08f)
                reachedRoof = true;
        }

        private void OnGUI()
        {
            if (player == null) return;
            if (title == null)
            {
                title = new GUIStyle(GUI.skin.label) { fontSize = 26, fontStyle = FontStyle.Bold };
                title.normal.textColor = Color.white;
                small = new GUIStyle(GUI.skin.label) { fontSize = 13 };
                small.normal.textColor = new Color(0.63f, 0.76f, 0.86f);
                number = new GUIStyle(title) { fontSize = 20, alignment = TextAnchor.MiddleRight };
            }
            var oldMatrix = GUI.matrix;
            var oldColor = GUI.color;
            float scale = Mathf.Min(Screen.width / 1000f, Screen.height / 650f);
            float w = Screen.width / scale, h = Screen.height / scale;
            GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1));
            Panel(new Rect(20, 20, 278, 90), new Color(0.025f, 0.04f, 0.085f, 0.93f));
            Panel(new Rect(20, 20, 3, 90), cyan);
            GUI.Label(new Rect(38, 29, 260, 32), "JUMP / DUMMY", title);
            GUI.Label(new Rect(39, 65, 260, 24), ascentLevel ? Zones[Mathf.Clamp(Mathf.FloorToInt((player.transform.position.y - 0.55f) / 13), 0, 4)] : "01  /  NEON ROOFTOPS", small);
            GUI.Label(new Rect(320, 30, 220, 36), $"SCORE  {(session != null ? session.Score : 0):000}", title);
            Panel(new Rect(w - 213, 20, 193, 90), new Color(0.025f, 0.04f, 0.085f, 0.93f));
            GUI.Label(new Rect(w - 198, 30, 160, 25), session != null ? GameSession.FormatTime(session.ElapsedSeconds) : $"{Mathf.Max(0, player.transform.position.y - 0.55f):0.0} m", number);
            GUI.Label(new Rect(w - 195, 66, 175, 22), $"JUMPS {player.JumpCount:000}   /   HEIGHT {bestHeight:0.0}", small);
            Panel(new Rect(20, h - 64, w - 40, 44), new Color(0.025f, 0.04f, 0.085f, 0.93f));
            GUI.Label(new Rect(36, h - 55, w - 80, 28), "A / D  MOVE + AIM      HOLD SPACE  CHARGE      RELEASE  JUMP      R  RESTART", small);
            if (player.Charging)
            {
                Panel(new Rect(w / 2 - 90, h - 87, 180, 5), new Color(0.12f, 0.16f, 0.25f));
                Panel(new Rect(w / 2 - 90, h - 87, 180 * player.Charge01, 5), player.Charge01 > 0.95f ? pink : cyan);
            }
            if (session != null && !string.IsNullOrEmpty(session.StatusMessage))
                GUI.Label(new Rect(w / 2 - 120, 86, 240, 28), session.StatusMessage, small);
            if (session != null && session.Mode == GameSession.SessionMode.Title)
            {
                Panel(new Rect(w / 2 - 235, h / 2 - 90, 470, 180), new Color(0.025f, 0.04f, 0.085f, 0.97f));
                GUI.Label(new Rect(w / 2 - 200, h / 2 - 65, 400, 40), ascentLevel ? "NEON ASCENT / 65 M" : "NEON ROOFTOPS", title);
                string action = session.HasSavedRun ? "ENTER / SPACE   CONTINUE\nN   NEW RUN" : "ENTER / SPACE   START RUN";
                GUI.Label(new Rect(w / 2 - 200, h / 2 - 12, 400, 65), action, small);
            }
            else if (session != null && session.Mode == GameSession.SessionMode.Paused)
            {
                Panel(new Rect(w / 2 - 190, h / 2 - 70, 380, 140), new Color(0.025f, 0.04f, 0.085f, 0.97f));
                GUI.Label(new Rect(w / 2 - 150, h / 2 - 45, 300, 40), "SIGNAL PAUSED", title);
                GUI.Label(new Rect(w / 2 - 150, h / 2 + 5, 300, 28), "ESC / P   RESUME", small);
            }
            else if (session != null && session.Mode == GameSession.SessionMode.Complete)
            {
                Panel(new Rect(w / 2 - 210, h / 2 - 90, 420, 180), new Color(0.025f, 0.04f, 0.085f, 0.97f));
                GUI.Label(new Rect(w / 2 - 170, h / 2 - 65, 340, 40), "ROOFTOP REACHED", title);
                GUI.Label(new Rect(w / 2 - 170, h / 2 - 10, 340, 60), $"TIME {GameSession.FormatTime(session.ElapsedSeconds)}\nBEST {GameSession.FormatTime(session.PersonalBestSeconds)}    /    ENTER NEW RUN", small);
            }
            else if (reachedRoof)
                GUI.Label(new Rect(w / 2 - 110, 125, 220, 30), "UPLINK READY", small);
            GUI.color = oldColor;
            GUI.matrix = oldMatrix;
        }

        private static void Panel(Rect rect, Color color)
        {
            GUI.color = color;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = Color.white;
        }
    }
}
