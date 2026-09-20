using UnityEngine;

namespace JumpDummy
{
    public sealed class PrototypePresentation : MonoBehaviour
    {
        public DummyController player;
        public Camera gameCamera;
        public Transform chargeFill;
        private GUIStyle titleStyle;
        private GUIStyle textStyle;

        private void LateUpdate()
        {
            if (player == null || gameCamera == null) return;
            float targetY = Mathf.Max(4.3f, player.transform.position.y + 1.6f);
            gameCamera.transform.position = new Vector3(0, targetY, -10);
            if (chargeFill != null)
            {
                chargeFill.gameObject.SetActive(player.Charging);
                chargeFill.localScale = new Vector3(0.9f * player.Charge01, 0.09f, 1);
                chargeFill.localPosition = new Vector3(-0.45f + 0.45f * player.Charge01, 0.86f, 0);
            }
        }

        private void OnGUI()
        {
            if (player == null) return;
            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 27, fontStyle = FontStyle.Bold };
                titleStyle.normal.textColor = new Color(0.35f, 0.92f, 0.8f);
                textStyle = new GUIStyle(GUI.skin.label) { fontSize = 16, wordWrap = true };
                textStyle.normal.textColor = Color.white;
            }
            float scale = Mathf.Min(Screen.width / 960f, Screen.height / 600f);
            Matrix4x4 previous = GUI.matrix;
            GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1));
            GUI.Box(new Rect(16, 14, 440, 122), GUIContent.none);
            GUI.Label(new Rect(30, 23, 410, 38), "JUMP DUMMY / JUMP LAB", titleStyle);
            GUI.Label(new Rect(30, 64, 410, 62), "A / D : move & aim    |    Hold SPACE : charge\nRelease SPACE : jump    |    R : restart", textStyle);
            GUI.Label(new Rect(24, Screen.height / scale - 42, 850, 30),
                $"{player.State}     /     JUMPS {player.JumpCount:000}     /     HEIGHT {Mathf.Max(0, player.transform.position.y - 0.6f):0.0} m", textStyle);
            GUI.matrix = previous;
        }
    }
}
