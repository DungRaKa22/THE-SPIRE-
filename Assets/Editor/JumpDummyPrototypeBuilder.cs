using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JumpDummy.Editor
{
    [InitializeOnLoad]
    public static class JumpDummyPrototypeBuilder
    {
        public const string ScenePath = "Assets/Scenes/JumpLab.unity";
        private const string SpritePath = "Assets/Art/PrototypeSquare.png";

        static JumpDummyPrototypeBuilder()
        {
            EditorApplication.delayCall += CreateIfMissing;
        }

        private static void CreateIfMissing()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling) return;
            Scene active = SceneManager.GetActiveScene();
            if (string.IsNullOrEmpty(active.path) && active.isDirty) return;
            if (!File.Exists(ScenePath)) Build();
        }

        [MenuItem("JumpDummy/Open Jump Lab")]
        public static void OpenLab()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            if (!File.Exists(ScenePath)) Build();
            EditorSceneManager.OpenScene(ScenePath);
        }

        public static void Build()
        {
            Directory.CreateDirectory("Assets/Art");
            if (!File.Exists(SpritePath))
            {
                var texture = new Texture2D(8, 8);
                var pixels = new Color[64];
                for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
                texture.SetPixels(pixels);
                texture.Apply();
                File.WriteAllBytes(SpritePath, texture.EncodeToPNG());
                Object.DestroyImmediate(texture);
                AssetDatabase.ImportAsset(SpritePath);
            }
            var importer = (TextureImporter)AssetImporter.GetAtPath(SpritePath);
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 8;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(SpritePath);

            Scene previousScene = SceneManager.GetActiveScene();
            bool replaceEmptyScene = string.IsNullOrEmpty(previousScene.path) && !previousScene.isDirty;
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,
                replaceEmptyScene ? NewSceneMode.Single : NewSceneMode.Additive);
            SceneManager.SetActiveScene(scene);
            var material = new PhysicsMaterial2D("JumpDummy Frictionless") { friction = 0, bounciness = 0 };
            const string materialPath = "Assets/Art/PrototypePhysics.physicsMaterial2D";
            if (File.Exists(materialPath))
            {
                Object.DestroyImmediate(material);
                material = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(materialPath);
            }
            else AssetDatabase.CreateAsset(material, materialPath);

            var terrain = new GameObject("Test Geometry").transform;
            Color platformColor = new Color(0.22f, 0.39f, 0.47f);
            AddBlock("Floor", new Vector2(0, -0.5f), new Vector2(18, 1));
            AddBlock("Left wall - bounce test", new Vector2(-9.25f, 8), new Vector2(0.5f, 18));
            AddBlock("Right wall - bounce test", new Vector2(9.25f, 8), new Vector2(0.5f, 18));
            AddBlock("01 Short hop", new Vector2(-3.1f, 0.9f), new Vector2(2.8f, 0.4f));
            AddBlock("02 Medium hop", new Vector2(0.3f, 2.3f), new Vector2(2.4f, 0.4f));
            AddBlock("03 Long hop", new Vector2(4.4f, 4.1f), new Vector2(2.3f, 0.4f));
            AddBlock("04 Return jump", new Vector2(0.5f, 6.1f), new Vector2(2.1f, 0.4f));
            AddBlock("05 Narrow landing", new Vector2(-3.3f, 8.1f), new Vector2(1.4f, 0.4f));
            AddBlock("06 Wall approach", new Vector2(-7f, 10.1f), new Vector2(2, 0.4f));
            AddBlock("07 Upper landing", new Vector2(-3.5f, 12.1f), new Vector2(2.2f, 0.4f));
            AddBlock("08 Summit", new Vector2(0.6f, 14.1f), new Vector2(3, 0.4f));
            AddBlock("Low ceiling - head impact test", new Vector2(6.6f, 2.1f), new Vector2(2.3f, 0.4f));

            var dummy = new GameObject("Dummy");
            dummy.transform.position = new Vector3(-6, 0.65f, 0);
            var collider = dummy.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.7f, 1.1f);
            collider.sharedMaterial = material;
            dummy.AddComponent<Rigidbody2D>();
            var controller = dummy.AddComponent<DummyController>();
            Rect("Body", dummy.transform, Vector2.zero, new Vector2(0.7f, 1.1f), new Color(1, 0.72f, 0.28f), 3);
            Rect("Visor", dummy.transform, new Vector2(0, 0.22f), new Vector2(0.5f, 0.16f), new Color(0.09f, 0.14f, 0.2f), 4);
            Transform fill = Rect("Charge meter", dummy.transform, new Vector2(0, 0.86f), new Vector2(0.9f, 0.09f), new Color(0.35f, 0.92f, 0.8f), 5);

            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 6;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.055f, 0.08f, 0.13f);
            camera.transform.position = new Vector3(0, 4.3f, -10);
            cameraObject.AddComponent<AudioListener>();
            var presentation = new GameObject("Prototype HUD and Camera").AddComponent<PrototypePresentation>();
            presentation.player = controller;
            presentation.gameCamera = camera;
            presentation.chargeFill = fill;

            EditorSceneManager.SaveScene(scene, ScenePath);
            if (previousScene.IsValid() && previousScene.isLoaded)
            {
                SceneManager.SetActiveScene(previousScene);
                EditorSceneManager.CloseScene(scene, true);
            }
            Debug.Log("JumpDummy: JumpLab is ready. Open it using JumpDummy > Open Jump Lab.");

            void AddBlock(string name, Vector2 position, Vector2 size)
            {
                Transform block = Rect(name, terrain, position, size, platformColor, 0);
                var box = block.gameObject.AddComponent<BoxCollider2D>();
                box.sharedMaterial = material;
                Rect(name + " rim", terrain, position + new Vector2(0, size.y / 2 - 0.035f), new Vector2(size.x, 0.07f), new Color(0.35f, 0.92f, 0.8f), 1);
            }

            Transform Rect(string name, Transform parent, Vector2 position, Vector2 size, Color color, int order)
            {
                var obj = new GameObject(name);
                obj.transform.SetParent(parent, false);
                obj.transform.localPosition = position;
                obj.transform.localScale = new Vector3(size.x, size.y, 1);
                var renderer = obj.AddComponent<SpriteRenderer>();
                renderer.sprite = sprite;
                renderer.color = color;
                renderer.sortingOrder = order;
                renderer.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
                return obj.transform;
            }
        }
    }
}
