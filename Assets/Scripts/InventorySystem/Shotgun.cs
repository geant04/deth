using UnityEngine;

namespace InventorySystem
{
    public class Shotgun : Weapon
    {
        [SerializeField] private GameObject pellet;

        private const int NumPellets = 4;
        private static readonly int MouseClicked = Animator.StringToHash("MouseClicked");

        public override void Shoot()
        {
            if (!CanShoot) return;

            StartCoroutine(SetCooldown());
            Player.anim.SetTrigger(MouseClicked);
            
            var pTransform = Player.transform;
            
            for (var i = -NumPellets + 1; i < NumPellets; i++) {
                var angle = i / (float) NumPellets * 10f;
                
                Vector2 rotatedVector = Quaternion.Euler(0, 0, angle) * Player.direction;
                rotatedVector.Normalize();

                var projGameObj = Instantiate(pellet, pTransform.position + (0.5f * Player.direction.normalized), pTransform.rotation);
                projGameObj.transform.localScale = new Vector3(0.175f, 0.175f, 0.0f);

                var pelletScript = projGameObj.GetComponent<BulletScript>();
                pelletScript.setDamage(Damage);
                pelletScript.direction = rotatedVector;
                pelletScript.speed = Damage + Random.Range(1, 5);
            }
            
            Player.audioSource.PlayOneShot(shootSfx, 0.4f);
        }
        
        public Shotgun(Player player)
        {
            weaponName = "Shotgun";
            FireRate = 0.65f;

            ProjSpeed = 16f;
            Damage = 15f;
            
            Player = player;
        }
    }
}