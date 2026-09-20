using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace JumpDummy.Editor
{
    [InitializeOnLoad]
    public static class CyberpunkSceneBuilder
    {
        public const string ScenePath = "Assets/Scenes/CyberpunkRooftops.unity";
        private const string Art = "Assets/Art/Cyberpunk/";
        private const string AnimationFolder = "Assets/Animations/CyberRunner/";
        private const string Prefabs = "Assets/Prefabs/Cyberpunk/";

        static CyberpunkSceneBuilder() { EditorApplication.delayCall += CreateIfMissing; }

        private static void CreateIfMissing()
        {
            if (File.Exists(ScenePath) || EditorApplication.isPlayingOrWillChangePlaymode) return;
            Scene active = SceneManager.GetActiveScene();
            if (string.IsNullOrEmpty(active.path) && active.isDirty) return;
            Build();
        }

        [MenuItem("JumpDummy/Open Cyberpunk Rooftops", priority = 0)]
        public static void Open()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            if (!File.Exists(ScenePath)) Build();
            EditorSceneManager.OpenScene(ScenePath);
        }

        public static void Build()
        {
            if (File.Exists(ScenePath)) return;
            Directory.CreateDirectory(AnimationFolder);
            Directory.CreateDirectory(Prefabs);
            AssetDatabase.Refresh();
            Sprite[] runner = Slice(Art + "RunnerAtlas.png", 4, true);
            Sprite[] props = Slice(Art + "EnvironmentAtlas.png", 2, false);
            AnimatorController animation = MakeAnimations(runner);
            Scene previous = SceneManager.GetActiveScene();
            File.Copy(JumpDummyPrototypeBuilder.ScenePath, ScenePath);
            AssetDatabase.ImportAsset(ScenePath);
            bool empty = string.IsNullOrEmpty(previous.path) && !previous.isDirty;
            Scene scene = EditorSceneManager.OpenScene(ScenePath, empty ? OpenSceneMode.Single : OpenSceneMode.Additive);
            SceneManager.SetActiveScene(scene);
            try
            {
                var roots = scene.GetRootGameObjects();
                var player = roots.Select(x => x.GetComponent<DummyController>()).First(x => x != null);
                var oldHud = roots.Select(x => x.GetComponent<PrototypePresentation>()).First(x => x != null);
                Camera camera = oldHud.gameCamera;
                Transform meter = oldHud.chargeFill;
                Object.DestroyImmediate(player.transform.Find("Body").gameObject);
                Object.DestroyImmediate(player.transform.Find("Visor").gameObject);
                var avatar = Draw("Cyber Runner", player.transform, runner[0], new Vector2(0, -0.55f), 1, 20);
                var animator = avatar.gameObject.AddComponent<Animator>();
                animator.runtimeAnimatorController = animation;
                var visual = player.gameObject.AddComponent<CyberRunnerVisual>();
                visual.player = player;
                visual.animator = animator;
                visual.spriteRenderer = avatar;
                PrefabUtility.SaveAsPrefabAsset(player.gameObject, Prefabs + "CyberRunner.prefab");

                Sprite square = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/PrototypeSquare.png");
                var terrain = roots.First(x => x.name == "Test Geometry").transform;
                terrain.name = "Rooftop Platforms";
                var decorations = new GameObject("Neon Signs and Vents").transform;
                foreach (Transform block in terrain.Cast<Transform>().ToArray())
                {
                    if (block.name.EndsWith(" rim")) { Object.DestroyImmediate(block.gameObject); continue; }
                    var box = block.GetComponent<BoxCollider2D>();
                    if (box == null) continue;
                    if (block.name == "Floor" || block.name.Contains("wall"))
                    {
                        block.GetComponent<SpriteRenderer>().color = new Color(0.055f, 0.095f, 0.16f);
                        if (block.name == "Floor")
                        {
                            Bar("Street edge", null, square, new Vector2(0, 0.01f), new Vector2(18, 0.06f), Cyan, 2);
                            for (int i = 0; i < 18; i++)
                                Bar("Street panel", null, square, new Vector2(-8.5f + i, -0.35f), new Vector2(0.92f, 0.55f), new Color(0.08f, 0.13f, 0.22f), 1);
                        }
                        else
                        {
                            float x = block.position.x > 0 ? 8.98f : -8.98f;
                            Bar("Tower edge neon", decorations, square, new Vector2(x, 8), new Vector2(0.035f, 18), Cyan * 0.65f, 1);
                            for (int j = 0; j < 18; j++)
                                Bar("Wall rib", decorations, square, new Vector2(block.position.x, j), new Vector2(0.5f, 0.045f), new Color(0.16f, 0.22f, 0.31f), 2);
                        }
                        continue;
                    }
                    Object.DestroyImmediate(block.GetComponent<SpriteRenderer>());
                    bool caution = block.name.StartsWith("03") || block.name.StartsWith("05") || block.name.Contains("ceiling");
                    Sprite sprite = props[caution ? 1 : 0];
                    float width = block.lossyScale.x;
                    float top = block.position.y + 0.2f;
                    // Only the top slab collides; support brackets remain decorative.
                    box.size = new Vector2(1, 0.5f);
                    box.offset = new Vector2(0, 0.25f);
                    SpriteRenderer art = Draw("Platform artwork", null, sprite, new Vector2(block.position.x, top), width / sprite.bounds.size.x, 4);
                    art.transform.SetParent(block, true);
                    if (block.name.StartsWith("01")) PrefabUtility.SaveAsPrefabAsset(block.gameObject, Prefabs + "NeonPlatform.prefab");
                    if (block.name.StartsWith("03")) PrefabUtility.SaveAsPrefabAsset(block.gameObject, Prefabs + "CautionPlatform.prefab");
                }
                for (int i = 0; i < 5; i++)
                {
                    float x = i % 2 == 0 ? 7.8f : -7.8f;
                    var sign = Draw("Neon sign " + i, decorations, props[3], new Vector2(x, 2 + i * 3), 1.6f / props[3].bounds.size.y, 0);
                    sign.gameObject.AddComponent<NeonPulse>().phase = i * 1.7f;
                    if (i == 0) PrefabUtility.SaveAsPrefabAsset(sign.gameObject, Prefabs + "NeonSign.prefab");
                }
                var vent = Draw("Street steam vent", decorations, props[2], new Vector2(7, 0), 2.5f / props[2].bounds.size.y, 3);
                PrefabUtility.SaveAsPrefabAsset(vent.gameObject, Prefabs + "SteamVent.prefab");
                Draw("Upper steam vent", decorations, props[2], new Vector2(-7.5f, 10.3f), 1.6f / props[2].bounds.size.y, 3);
                Transform skyline = MakeSkyline(square);
                var finish = new GameObject("Rooftop Beacon").transform;
                for (int i = 0; i < 3; i++)
                {
                    var beam = Bar("Beacon", finish, square, new Vector2(-0.1f + i * 0.65f, 15.4f), new Vector2(0.055f, 1.6f), i == 1 ? Pink : Cyan, 5);
                    beam.gameObject.AddComponent<NeonPulse>().phase = i;
                }
                camera.backgroundColor = new Color(0.022f, 0.029f, 0.065f);
                camera.orthographicSize = 5.8f;
                var hudObject = oldHud.gameObject;
                Object.DestroyImmediate(oldHud);
                hudObject.name = "Cyberpunk Camera and HUD";
                var hud = hudObject.AddComponent<CyberpunkPresentation>();
                hud.player = player; hud.gameCamera = camera; hud.chargeFill = meter; hud.skyline = skyline;
                var session = new GameObject("Game Session").AddComponent<GameSession>();
                session.player = player;
                hud.session = session;
                var feedback = new GameObject("Cyber Feedback").AddComponent<CyberFeedback>();
                feedback.player = player;
                feedback.jumpClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Sounds/jump.mp3");
                feedback.bumpClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Sounds/bump.mp3");
                feedback.landClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Sounds/land.mp3");
                feedback.fallClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Sounds/fall.mp3");
                feedback.bgmClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Music/bgm_cyberpunk.mp3");
                EditorSceneManager.SaveScene(scene);
                var scenes = EditorBuildSettings.scenes.Where(x => x.path != ScenePath).ToList();
                scenes.Insert(0, new EditorBuildSettingsScene(ScenePath, true));
                EditorBuildSettings.scenes = scenes.ToArray();
                AssetDatabase.SaveAssets();
                Validate(scene, runner, props, animation);
                Debug.Log("CYBERPUNK READY: JumpDummy > Open Cyberpunk Rooftops");
            }
            finally
            {
                if (previous.IsValid() && previous.isLoaded)
                {
                    SceneManager.SetActiveScene(previous);
                    EditorSceneManager.CloseScene(scene, true);
                }
            }
        }

        private static readonly Color Cyan = new Color(0.15f, 0.85f, 1f);
        private static readonly Color Pink = new Color(0.85f, 0.12f, 0.55f);

        private static SpriteRenderer Draw(string name, Transform parent, Sprite sprite, Vector2 position, float scale, int order)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;
            go.transform.localScale = Vector3.one * scale;
            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = order;
            renderer.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
            return renderer;
        }

        private static SpriteRenderer Bar(string name, Transform parent, Sprite square, Vector2 position, Vector2 size, Color color, int order)
        {
            var renderer = Draw(name, parent, square, position, 1, order);
            renderer.transform.localScale = new Vector3(size.x, size.y, 1);
            renderer.color = color;
            return renderer;
        }

        private static Transform MakeSkyline(Sprite square)
        {
            var skyline = new GameObject("Parallax Skyline").transform;
            var random = new System.Random(42);
            for (int i = 0; i < 15; i++)
            {
                float x = -12 + i * 1.7f;
                float height = 10 + (float)random.NextDouble() * 16;
                Bar("Distant tower", skyline, square, new Vector2(x, height / 2 - 3), new Vector2(1.35f, height),
                    i % 2 == 0 ? new Color(0.035f, 0.055f, 0.105f) : new Color(0.046f, 0.06f, 0.13f), -30);
                for (int row = 0; row < height; row++)
                    for (int col = 0; col < 3; col++)
                    {
                        if (random.NextDouble() < 0.5) continue;
                        Color color = random.NextDouble() < 0.7 ? new Color(0.09f, 0.24f, 0.33f) : new Color(0.25f, 0.07f, 0.23f);
                        Bar("Window", skyline, square, new Vector2(x - 0.4f + col * 0.4f, row - 2.5f), new Vector2(0.12f, 0.22f), color, -29);
                    }
            }
            return skyline;
        }

        private static Sprite[] Slice(string path, int columns, bool character)
        {
            AssetDatabase.ImportAsset(path);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.maxTextureSize = 2048;
            importer.spritePixelsPerUnit = character ? 205 : 200;
            importer.SaveAndReimport();
            var source = new Texture2D(2, 2);
            source.LoadImage(File.ReadAllBytes(path));
            Color32[] pixels = source.GetPixels32();
            var factories = new SpriteDataProviderFactories();
            factories.Init();
            var provider = factories.GetSpriteEditorDataProviderFromObject(importer);
            provider.InitSpriteEditorDataProvider();
            var existing = provider.GetSpriteRects().ToDictionary(x => x.name, x => x.spriteID);
            var rects = new SpriteRect[columns * columns];
            for (int row = 0; row < columns; row++)
            for (int col = 0; col < columns; col++)
            {
                int index = row * columns + col;
                int left = col * source.width / columns, right = (col + 1) * source.width / columns;
                int bottom = source.height - (row + 1) * source.height / columns, top = source.height - row * source.height / columns;
                int minX = right, minY = top, maxX = left, maxY = bottom;
                for (int y = bottom; y < top; y++)
                for (int x = left; x < right; x++)
                    if (pixels[y * source.width + x].a > 80)
                    { minX = Mathf.Min(x, minX); minY = Mathf.Min(y, minY); maxX = Mathf.Max(x, maxX); maxY = Mathf.Max(y, maxY); }
                if (minX > maxX) throw new InvalidOperationException("Empty atlas frame " + index);
                string name = (character ? "Runner_" : "Prop_") + index.ToString("00");
                Rect rect = character ? new Rect(left, bottom, right - left, top - bottom) : new Rect(minX, minY, maxX - minX + 1, maxY - minY + 1);
                Vector2 pivot = character ? new Vector2(0.55f, (minY - bottom) / rect.height)
                    : index < 2 ? new Vector2(0.5f, 1) : new Vector2(0.5f, 0);
                rects[index] = new SpriteRect { name = name, rect = rect, alignment = SpriteAlignment.Custom,
                    pivot = pivot, spriteID = existing.TryGetValue(name, out var id) ? id : GUID.Generate() };
            }
            Object.DestroyImmediate(source);
            provider.SetSpriteRects(rects);
            provider.GetDataProvider<ISpriteNameFileIdDataProvider>().SetNameFileIdPairs(rects.Select(x => new SpriteNameFileIdPair(x.name, x.spriteID)));
            provider.Apply();
            importer.SaveAndReimport();
            return AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().OrderBy(x => x.name).ToArray();
        }

        private static AnimatorController MakeAnimations(Sprite[] sprites)
        {
            string path = AnimationFolder + "CyberRunner.controller";
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
            if (controller != null) return controller;
            controller = AnimatorController.CreateAnimatorControllerAtPath(path);
            Add("Idle", new[] { 0, 1, 2, 3 }, 5, true);
            Add("Run", new[] { 4, 5, 6, 7 }, 10, true);
            Add("Charge", new[] { 8 }, 6, false);
            Add("ChargeDeep", new[] { 9 }, 6, false);
            Add("Jump", new[] { 10 }, 6, false);
            Add("Fall", new[] { 11 }, 6, false);
            Add("Land", new[] { 12, 13 }, 12, false);
            Add("Bounce", new[] { 14 }, 6, false);
            return controller;

            void Add(string name, int[] frames, float fps, bool loop)
            {
                var clip = new AnimationClip { frameRate = fps, name = name };
                var keys = new ObjectReferenceKeyframe[frames.Length + 1];
                for (int i = 0; i < frames.Length; i++) keys[i] = new ObjectReferenceKeyframe { time = i / fps, value = sprites[frames[i]] };
                keys[frames.Length] = new ObjectReferenceKeyframe { time = frames.Length / fps, value = sprites[frames[frames.Length - 1]] };
                AnimationUtility.SetObjectReferenceCurve(clip, new EditorCurveBinding { path = "", type = typeof(SpriteRenderer), propertyName = "m_Sprite" }, keys);
                var settings = AnimationUtility.GetAnimationClipSettings(clip);
                settings.loopTime = loop;
                AnimationUtility.SetAnimationClipSettings(clip, settings);
                AssetDatabase.CreateAsset(clip, AnimationFolder + name + ".anim");
                controller.AddMotion(clip);
            }
        }

        private static void Validate(Scene scene, Sprite[] runner, Sprite[] props, AnimatorController animation)
        {
            if (runner.Length != 16 || props.Length != 4 || animation.animationClips.Length != 8)
                throw new InvalidOperationException("Cyberpunk atlas or animation count is incorrect.");
            foreach (var root in scene.GetRootGameObjects())
                foreach (var renderer in root.GetComponentsInChildren<SpriteRenderer>(true))
                    if (renderer.sprite == null) throw new InvalidOperationException("Missing sprite: " + renderer.name);
            Directory.CreateDirectory("Docs");
            File.WriteAllText("Docs/CyberpunkValidation.txt", "PASS: 16 runner sprites, 4 environment sprites, 8 animation clips, all scene SpriteRenderer references valid.\n");
        }
    }
}
