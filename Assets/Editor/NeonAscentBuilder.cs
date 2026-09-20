using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace JumpDummy.Editor
{
    public static class NeonAscentBuilder
    {
        public const string ScenePath = "Assets/Scenes/NeonAscent.unity";
        private const string Art = "Assets/Art/Cyberpunk/ClimbProps/";
        private const string Prefabs = "Assets/Prefabs/Cyberpunk/ClimbProps/";
        private static Sprite square;
        private static readonly Color Cyan = new Color(0.15f, 0.8f, 0.92f);
        private static readonly Color Pink = new Color(0.8f, 0.16f, 0.48f);

        [MenuItem("JumpDummy/Open Neon Ascent", priority = -10)]
        public static void Open()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            if (!File.Exists(ScenePath)) Build();
            EditorSceneManager.OpenScene(ScenePath);
        }

        // Creates a separate authored level; the original rooftop scene stays available.
        public static void Build()
        {
            if (File.Exists(ScenePath)) throw new InvalidOperationException("Neon Ascent already exists; edit its scene directly.");
            Directory.CreateDirectory(Prefabs);
            AssetDatabase.Refresh();
            Sprite[] sprites = { Import("AirConditioner"), Import("NeonBillboard"), Import("Chimney") };
            square = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/PrototypeSquare.png");
            if (!AssetDatabase.CopyAsset(CyberpunkSceneBuilder.ScenePath, ScenePath)) throw new Exception("Could not copy source scene.");
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var roots = scene.GetRootGameObjects();
            foreach (var root in roots)
                if (root.name == "Rooftop Platforms" || root.name == "Parallax Skyline" || root.name == "Neon Signs and Vents" ||
                    root.name == "Rooftop Beacon" || root.name.Contains("steam vent")) Object.DestroyImmediate(root);
            var terrain = new GameObject("Neon Ascent Platforms").transform;
            var backdrop = new GameObject("Ascent Architecture").transform;
            var player = Object.FindAnyObjectByType<DummyController>();
            var material = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>("Assets/Physics/NoFriction.physicsMaterial2D");
            // Reuse the player's frictionless material if the project stores it elsewhere.
            if (material == null) material = player.GetComponent<BoxCollider2D>().sharedMaterial;
            Solid("Floor", terrain, new Vector2(0, -0.5f), new Vector2(18, 1), material);
            Solid("Left wall", terrain, new Vector2(-9.25f, 34), new Vector2(0.5f, 70), material);
            Solid("Right wall", terrain, new Vector2(9.25f, 34), new Vector2(0.5f, 70), material);
            float[] xs = { -5, -1.5f, 2, 5.5f, 1.5f, -2, -5.5f,
                -1, 1.5f, 5, 7, 3.5f, 0, -3.5f,
                -7, -2.5f, 0, 3.5f, 7, 3.5f, 0,
                -3.5f, -7, -2.5f, .5f, 3.5f, 7, 2.5f,
                0, -3.5f, -7, -2.5f, 0, 3.5f, -1 };
            string[] names = { "Condenser", "LED Sign", "Chimney" };
            float top = 1.4f;
            for (int i = 0; i < xs.Length; i++)
            {
                if (i > 0) top += new[] { 1.8f, 1.9f, 1.85f, 2f, 1.75f, 1.9f, 1.9f }[i % 7];
                int zone = i / 7;
                int kind = (i + zone) % 3;
                bool rest = i % 7 == 6;
                float width = rest ? 3.6f : kind == 2 ? 1.7f : kind == 1 ? 2.8f : 2.5f;
                if (i == 10 || i == 18 || i == 26) width = 2.1f;
                if (rest) kind = 0;
                var go = new GameObject($"{i + 1:00} {names[kind]}" + (rest ? " - Rest Roof" : ""));
                go.transform.SetParent(terrain, false);
                go.transform.position = new Vector3(xs[i], top, 0);
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = sprites[kind]; sr.sortingOrder = 5;
                sr.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
                float scale = width / sr.sprite.bounds.size.x;
                go.transform.localScale = Vector3.one * scale;
                var box = go.AddComponent<BoxCollider2D>();
                // Solid cap and body follow the visible silhouette; supports are decoration.
                float depth = sr.sprite.bounds.size.y * (kind == 1 ? 0.64f : kind == 0 ? 0.86f : 0.96f);
                box.size = new Vector2(sr.sprite.bounds.size.x, depth);
                box.offset = new Vector2(0, -depth / 2);
                box.sharedMaterial = material;
                if (i < 3) PrefabUtility.SaveAsPrefabAsset(go, Prefabs + names[kind].Replace(" ", "") + ".prefab");
                // Architectural attachments sit behind the collidable prop.
                float side = xs[i] < 0 ? -9 : 9;
                Bar("Cantilever brace", backdrop, new Vector2((side + xs[i]) / 2, top - 0.4f),
                    new Vector2(Mathf.Abs(side - xs[i]), 0.08f), new Color(0.10f, 0.18f, 0.23f), -3);
                if (rest)
                    Bar("Rest roof light", backdrop, new Vector2(xs[i], top - 1.6f), new Vector2(width, 0.035f), Cyan * 0.6f, -2);
            }
            BuildArchitecture(backdrop);
            var session = Object.FindAnyObjectByType<GameSession>();
            session.summitHeight = top - 0.05f;
            session.saveSlot = "NeonAscent.v1";
            player.transform.position = new Vector3(-7.2f, 0.65f, 0);
            var hud = Object.FindAnyObjectByType<CyberpunkPresentation>();
            hud.summitHeight = session.summitHeight;
            hud.skyline = null;
            hud.ascentLevel = true;
            hud.gameCamera.transform.position = new Vector3(0, 4.2f, -10);
            var beacon = new GameObject("Ascent Summit Beacon").transform;
            for (int i = 0; i < 3; i++)
                Bar("Uplink light", beacon, new Vector2(xs[xs.Length - 1] - 0.65f + i * 0.65f, top + 0.95f), new Vector2(0.045f, 1.6f), i == 1 ? Pink : Cyan, 4)
                    .gameObject.AddComponent<NeonPulse>().phase = i;
            EditorSceneManager.SaveScene(scene);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) }
                .Concat(EditorBuildSettings.scenes.Where(s => s.path != ScenePath)).ToArray();
            AssetDatabase.SaveAssets();
            Debug.Log($"NEON ASCENT BUILT: 35 platforms, 5 zones, summit {top:0.00}m, 7 coins per run.");
        }

        private static Sprite Import(string name)
        {
            string path = Art + name + ".png";
            var source = new Texture2D(2, 2);
            source.LoadImage(File.ReadAllBytes(path));
            var pixels = source.GetPixels32();
            int left = source.width, right = 0, bottom = source.height, top = 0;
            for (int y = 0; y < source.height; y++)
                for (int x = 0; x < source.width; x++)
                    if (pixels[y * source.width + x].a > 100)
                    { left = Math.Min(left, x); right = Math.Max(right, x); bottom = Math.Min(bottom, y); top = Math.Max(top, y); }
            if (left >= right) throw new Exception("Sprite is empty: " + name);
            Object.DestroyImmediate(source);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.maxTextureSize = 4096;
            importer.spritePixelsPerUnit = 400;
            importer.SaveAndReimport();
            var factory = new SpriteDataProviderFactories(); factory.Init();
            var provider = factory.GetSpriteEditorDataProviderFromObject(importer); provider.InitSpriteEditorDataProvider();
            var prior = provider.GetSpriteRects().FirstOrDefault(r => r.name == name);
            var rect = new SpriteRect { name = name, rect = new Rect(left, bottom, right - left + 1, top - bottom + 1),
                alignment = SpriteAlignment.Custom, pivot = new Vector2(0.5f, 1), spriteID = prior != null ? prior.spriteID : GUID.Generate() };
            provider.SetSpriteRects(new[] { rect });
            provider.GetDataProvider<ISpriteNameFileIdDataProvider>().SetNameFileIdPairs(new[] { new SpriteNameFileIdPair(name, rect.spriteID) });
            provider.Apply(); importer.SaveAndReimport();
            return AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().Single();
        }

        private static void BuildArchitecture(Transform parent)
        {
            var rng = new System.Random(84);
            Color[] skies = { new Color(.023f,.04f,.065f), new Color(.032f,.04f,.088f), new Color(.06f,.04f,.095f), new Color(.095f,.055f,.115f), new Color(.12f,.095f,.16f) };
            for (int zone = 0; zone < 5; zone++)
            {
                float baseY = zone * 13;
                Bar("Atmosphere " + zone, parent, new Vector2(0, baseY + (zone == 4 ? 12 : 6.5f)), new Vector2(40, zone == 4 ? 24 : 13.1f), skies[zone], -40);
                for (int j = 0; j < 8; j++)
                {
                    float x = -11 + j * 3.2f;
                    float h = 8 + (float)rng.NextDouble() * 7;
                    Bar("Distant tower", parent, new Vector2(x, baseY + h / 2), new Vector2(2.5f, h), skies[zone] * .65f, -30);
                    for (int row = 0; row < h; row++)
                        for (int col = 0; col < 3; col++)
                            if (rng.NextDouble() < .45)
                                Bar("Window", parent, new Vector2(x - .7f + col * .7f, baseY + row), new Vector2(.13f,.22f),
                                    (zone % 2 == 0 ? Cyan : Pink) * .22f, -29);
                }
                foreach (int side in new[] { -1, 1 })
                {
                    float inner = 7.6f + (float)rng.NextDouble() * .7f;
                    Bar("Facade", parent, new Vector2(side * (inner + 1.5f), baseY + 6.5f), new Vector2(3,13), new Color(.037f,.055f,.078f), -10);
                    Bar("Facade edge", parent, new Vector2(side * inner, baseY + 6.5f), new Vector2(.055f,12.8f), (zone % 2 == 0 ? Cyan : Pink) * .35f, -9);
                    for (int row = 0; row < 11; row++)
                    {
                        Bar("Masonry seam", parent, new Vector2(side * 8.8f, baseY + row * 1.2f), new Vector2(2,.025f), new Color(.095f,.13f,.16f), -8);
                        if (row % 3 == 0) Bar("Facade window", parent, new Vector2(side * 8.6f, baseY + row * 1.2f + .5f),
                            new Vector2(.32f,.65f), (zone % 2 == 0 ? Pink : Cyan) * .5f, -7);
                    }
                }
                // Layered central silhouettes echo the trunks and large masses in the references.
                float center = zone % 2 == 0 ? 1.5f : -1.5f;
                Bar("Service tower", parent, new Vector2(center,baseY + 5), new Vector2(2.6f,10), skies[zone] * .45f, -20);
                for (int k = 0; k < 4; k++)
                    Bar("Service riser", parent, new Vector2(center - 1 + k * .65f,baseY + 5), new Vector2(.04f,10), new Color(.08f,.115f,.15f), -19);
            }
        }

        private static void Solid(string name, Transform parent, Vector2 position, Vector2 size, PhysicsMaterial2D material)
        {
            var sr = Bar(name, parent, position, size, new Color(.055f,.09f,.13f), 0);
            sr.gameObject.AddComponent<BoxCollider2D>().sharedMaterial = material;
        }

        private static SpriteRenderer Bar(string name, Transform parent, Vector2 position, Vector2 size, Color color, int order)
        {
            var go = new GameObject(name); go.transform.SetParent(parent, false);
            go.transform.localPosition = position; go.transform.localScale = new Vector3(size.x, size.y, 1);
            var sr = go.AddComponent<SpriteRenderer>(); sr.sprite = square; sr.color = color; sr.sortingOrder = order;
            sr.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
            return sr;
        }
    }
}
