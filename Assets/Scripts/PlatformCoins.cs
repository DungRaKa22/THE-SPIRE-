using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JumpDummy
{
    // Numbered platform names define the route; floors, walls and ceilings are excluded.
    public sealed class PlatformCoins : MonoBehaviour
    {
        public const int PointsPerCoin = 10;
        public int Score => collected.Count * PointsPerCoin;
        public int Seed { get; private set; }
        public string CollectedIds => string.Join(",", collected.OrderBy(x => x));
        private readonly HashSet<int> collected = new HashSet<int>();
        private int groupCount;
        private GameSession session;
        private Transform container;
        private Sprite sprite;
        private Texture2D texture;

        public void Begin(GameSession owner, int seed, string savedIds = "")
        {
            session = owner;
            Seed = seed;
            collected.Clear();
            foreach (string value in savedIds.Split(','))
                if (int.TryParse(value, out int id) && id >= 0) collected.Add(id);
            if (container != null)
            {
                container.gameObject.SetActive(false);
                Destroy(container.gameObject);
            }
            container = new GameObject("Run Coins").transform;
            container.SetParent(transform, false);
            Physics2D.SyncTransforms();
            var platforms = FindObjectsByType<BoxCollider2D>()
                .Where(p => p.gameObject.scene == gameObject.scene && p.enabled && !p.isTrigger &&
                    int.TryParse(p.name.Split(' ')[0], out _))
                .OrderBy(p => int.Parse(p.name.Split(' ')[0]))
                .ThenBy(p => p.bounds.center.y).ThenBy(p => p.bounds.center.x).ToArray();
            int groups = platforms.Length / 5;
            groupCount = groups;
            collected.RemoveWhere(id => id >= groups);
            var random = new System.Random(seed);
            EnsureSprite();
            for (int group = 0; group < groups; group++)
            {
                var platform = platforms[group * 5 + random.Next(5)];
                // Always consume randomness, including for coins already collected.
                float offset = (float)random.NextDouble();
                if (collected.Contains(group)) continue;
                Bounds bounds = platform.bounds;
                float margin = Mathf.Min(0.3f, bounds.size.x / 2);
                Vector3 position = new Vector3(Mathf.Lerp(bounds.min.x + margin, bounds.max.x - margin, offset), bounds.max.y + 0.5f, 0);
                var coin = new GameObject("Coin " + group);
                coin.transform.SetParent(container, false);
                coin.transform.position = position;
                var renderer = coin.AddComponent<SpriteRenderer>();
                renderer.sprite = sprite;
                renderer.sortingOrder = 25;
                var trigger = coin.AddComponent<CircleCollider2D>();
                trigger.radius = 0.23f;
                trigger.isTrigger = true;
                coin.AddComponent<PlatformCoinPickup>().Initialize(this, group);
            }
        }

        public bool TryCollect(int id)
        {
            if (session == null || session.InputBlocked || id < 0 || id >= groupCount || !collected.Add(id)) return false;
            session.SaveCoinProgress();
            return true;
        }

        private void EnsureSprite()
        {
            if (sprite != null) return;
            texture = new Texture2D(32, 32, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
            var pixels = new Color[32 * 32];
            for (int y = 0; y < 32; y++)
                for (int x = 0; x < 32; x++)
                {
                    float radius = Vector2.Distance(new Vector2(x, y), new Vector2(15.5f, 15.5f));
                    Color color = Color.clear;
                    if (radius < 15) color = radius > 12 ? new Color(1f, 0.62f, 0.05f) : new Color(1f, 0.87f, 0.22f);
                    if (radius < 10 && x >= 14 && x <= 17) color = new Color(1f, 1f, 0.78f);
                    pixels[y * 32 + x] = color;
                }
            texture.SetPixels(pixels);
            texture.Apply();
            sprite = Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 64);
        }

        private void OnDestroy()
        {
            if (sprite != null) Destroy(sprite);
            if (texture != null) Destroy(texture);
        }
    }
}
