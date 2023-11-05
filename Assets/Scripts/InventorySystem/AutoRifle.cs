using UnityEngine;

namespace InventorySystem
{
    public class AutoRifle : Weapon
    {
        [SerializeField] private GameObject bullet;
        
        private static readonly int MouseHeldDown = Animator.StringToHash("MouseHeldDown");

        public override void Shoot()
        {
            if (!CanShoot) return;

            StartCoroutine(SetCooldown());
            Player.anim.SetTrigger(MouseHeldDown);
            
            var pTransform = Player.transform;
            var projGameObj = Instantiate(bullet, pTransform.position + (0.5f * Player.direction.normalized), pTransform.rotation);
            var bulletScript = projGameObj.GetComponent<BulletScript>();
            
            bulletScript.direction = Player.direction;
            bulletScript.speed = ProjSpeed;
            bulletScript.setDamage(Damage);
            
            Player.audioSource.PlayOneShot(shootSfx, 0.7f);
        }

        public AutoRifle(Player player)
        {
            weaponName = "Assault Rifle";
            FireRate = 0.1f;

            ProjSpeed = 20f;
            Damage = 15f;
            
            Player = player;
        }
    }
}