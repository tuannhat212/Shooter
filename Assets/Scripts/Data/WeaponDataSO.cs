using UnityEngine;

namespace ArenaSurvivor.Data
{
    [CreateAssetMenu(fileName = "NewWeaponData", menuName = "ScriptableObject/Weapon Data")]
    public class WeaponDataSO : ScriptableObject
    {
        [Header("Combat Stats")]
        public float damage = 25f;
        public float fireRate = 5f;
        public float bulletSpeed = 16f;
        public float bulletLifeTime = 2f;
    }
}
