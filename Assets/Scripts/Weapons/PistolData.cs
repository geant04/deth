using UnityEngine;

namespace Weapons
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Pistol")]
    public class PistolData : WeaponData
    {
        public override void Shoot(Transform entT, Vector3 entD)
        {
            var proj = Instantiate(projPrefab, entT.position + (0.5f * entD.normalized), entT.rotation);
            var bullet = proj.GetComponent<BulletScript>();
            
            bullet.direction = entD;
            bullet.speed = projSpeed;
            bullet.setDamage(damage);
        }
    }
}