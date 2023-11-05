using UnityEngine;

namespace InventorySystem
{
    public class Pistol : Weapon
    {
        [SerializeField] private GameObject bullet;
        
        private static readonly int MouseClicked = Animator.StringToHash("MouseClicked");
        
        public override void Shoot()
        {
            if (!CanShoot) return;
            
            StartCoroutine(SetCooldown());
            Player.anim.SetTrigger(MouseClicked);

            var pTransform = Player.transform;
            var projGameObj = Instantiate(bullet, pTransform.position + (0.5f * Player.direction.normalized), pTransform.rotation);
            var bulletScript = projGameObj.GetComponent<BulletScript>();
            
            bulletScript.direction = Player.direction;
            bulletScript.speed = ProjSpeed;
            bulletScript.setDamage(Damage);
            
            Player.audioSource.PlayOneShot(shootSfx, 0.7f);
        }

        public Pistol(Player player)
        {
            weaponName = "Pistol";
            FireRate = 0.05f;

            ProjSpeed = 20f;
            Damage = 45f;

            Player = player;
        }
    }
}