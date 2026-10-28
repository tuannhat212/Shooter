using System.Collections.Generic;
using UnityEngine;

namespace ArenaSurvivor.Pool
{
    public class BulletPool : MonoBehaviour
    {
        public static BulletPool Instance { get; private set; }

        [Header("Pool Setup")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private int initialPoolSize = 35;

        private readonly Queue<GameObject> poolQueue = new Queue<GameObject>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            InitializePool();
        }

        private void InitializePool()
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                GameObject bullet = Instantiate(bulletPrefab, transform);
                bullet.SetActive(false);
                poolQueue.Enqueue(bullet);
            }
        }

        public GameObject GetBullet(Vector3 position, Quaternion rotation)
        {
            GameObject bullet = (poolQueue.Count > 0) ? poolQueue.Dequeue() : Instantiate(bulletPrefab, transform);

            bullet.transform.position = position;
            bullet.transform.rotation = rotation;
            bullet.SetActive(true);
            return bullet;
        }

        public void ReturnBullet(GameObject bullet)
        {
            if (bullet == null) return;

            bullet.SetActive(false);
            bullet.transform.SetParent(transform);

            if (!poolQueue.Contains(bullet))
            {
                poolQueue.Enqueue(bullet);
            }
        }
    }
}
