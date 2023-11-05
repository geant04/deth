using UnityEngine;

namespace InventorySystem
{
    public class AssaultRifle : Weapon
    {
        [SerializeField] private GameObject bullet;

        public override void Shoot(Transform entTransform, Vector3 entDirection, AudioSource audioSrc)
        {
            if (!CanShoot) return;

            StartCoroutine(SetCooldown());
            
            var projGameObj = Instantiate(bullet, entTransform.position + (0.5f * entDirection.normalized), entTransform.rotation);
            var bulletScript = projGameObj.GetComponent<BulletScript>();

            bulletScript.direction = entDirection;
            bulletScript.speed = ProjSpeed;
            bulletScript.setDamage(Damage);
            
            audioSrc.PlayOneShot(shootSfx, 0.7f);
        }

        private void Start()
        {
            WeaponName = "Assault Rifle";
            WeaponId = WeaponId.AssaultRifle;

            CanShoot = true;
            FireRate = 0.1f;
            
            ProjSpeed = 20f;
            Damage = 15f;
        }
    }
}