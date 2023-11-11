using UnityEngine;

namespace Weapons
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Shotgun")]
    public class ShotgunData : WeaponData
    {
        public int numProj;
        
        public override void Shoot(Transform entT, Vector3 entD)
        {
            for (var i = -numProj + 1; i < numProj; i++)
            {
                var angle = (i / (float) numProj) * 10f;

                Vector2 rotated = Quaternion.Euler(0, 0, angle) * entD;
                rotated.Normalize();
                
                var proj = Instantiate(projPrefab, entT.position + (0.5f * entD.normalized), entT.rotation);
                proj.transform.localScale = new Vector3(0.175f, 0.175f, 0.0f);
                
                var pellet = proj.GetComponent<BulletScript>();
                pellet.setDamage(damage);
                pellet.direction = rotated;
                pellet.speed = projSpeed + Random.Range(1, 5);
            }
        }
    }
}