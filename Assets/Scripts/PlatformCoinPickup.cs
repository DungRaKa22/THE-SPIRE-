using UnityEngine;

namespace JumpDummy
{
    public sealed class PlatformCoinPickup : MonoBehaviour
    {
        private PlatformCoins owner;
        private int id;
        private Vector3 origin;
        private bool pickedUp;

        public void Initialize(PlatformCoins coins, int coinId)
        {
            owner = coins;
            id = coinId;
            origin = transform.position;
        }

        private void Update()
        {
            transform.position = origin + Vector3.up * (Mathf.Sin(Time.time * 3f + id) * 0.07f);
        }

        private void OnTriggerEnter2D(Collider2D other) => Collect(other);
        private void OnTriggerStay2D(Collider2D other) => Collect(other);

        private void Collect(Collider2D other)
        {
            if (pickedUp || other.GetComponentInParent<DummyController>() == null || owner == null) return;
            if (!owner.TryCollect(id)) return;
            pickedUp = true;
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
