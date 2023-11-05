using UnityEngine;

namespace InventorySystem
{
    public class Shotgun : Weapon
    {
        [SerializeField] private GameObject pellet;

        private const int NumPellets = 4;

        public override void Shoot(Transform entTransform, Vector3 entDirection, AudioSource audioSrc)
        {
            if (!CanShoot) return;

            StartCoroutine(SetCooldown());
            
            for (var i = -NumPellets + 1; i < NumPellets; i++) {
                var angle = i / (float) NumPellets * 10f;
                
                Vector2 rotatedVector = Quaternion.Euler(0, 0, angle) * entDirection;
                rotatedVector.Normalize();

                var projGameObj = Instantiate(pellet, entTransform.position + (0.5f * entDirection.normalized), entTransform.rotation);
                projGameObj.transform.localScale = new Vector3(0.175f, 0.175f, 0.0f);

                var pelletScript = projGameObj.GetComponent<BulletScript>();
                pelletScript.setDamage(Damage);
                pelletScript.direction = rotatedVector;
                pelletScript.speed = Damage + Random.Range(1, 5);
            }
            
            audioSrc.PlayOneShot(shootSfx, 0.3f);
        }

        private void Start()
        {
            WeaponName = "Shotgun";
            WeaponId = WeaponId.Shotgun;
            
            CanShoot = true;
            FireRate = 0.65f;
            
            ProjSpeed = 16f;
            Damage = 15f;
        }
    }
}