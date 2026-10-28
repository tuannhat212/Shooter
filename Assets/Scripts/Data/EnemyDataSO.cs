using UnityEngine;

namespace ArenaSurvivor.Data
{
    [CreateAssetMenu(fileName = "NewEnemyData", menuName = "ArenaSurvivor/Enemy Data")]
    public class EnemyDataSO : ScriptableObject
    {
        [Header("Identity")]
        public string enemyName = "Zombie";

        [Header("Stats")]
        public float maxHp = 40f;
        public float moveSpeed = 3f;
        public float detectRange = 6f;
        public float attackDamage = 10f;
        public int rewardScore = 100;

        [Header("Appearance")]
        public Sprite bodySprite;
        public Color bodyColor = Color.white;
        public Vector3 visualScale = Vector3.one;
    }
}
