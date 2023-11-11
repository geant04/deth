using UnityEngine;

namespace Weapons
{
    public abstract class WeaponData : ScriptableObject
    {
        public string weaponName;
        public WeaponId weaponId;

        public float fireRate;
        public float projSpeed;
        public float damage;
    
        public AudioClip shootSfx;
        public Sprite sprite;
        public GameObject projPrefab;

        public abstract void Shoot(Transform entT, Vector3 entD);
    }
}