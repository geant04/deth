using System.Collections;
using UnityEngine;

namespace InventorySystem
{
    public abstract class Weapon : MonoBehaviour
    {
        [SerializeField] public AudioClip shootSfx;
        [SerializeField] public Sprite sprite;
        
        public string WeaponName { get; protected set; }
        public WeaponId WeaponId { get; protected set; }
        public bool CanShoot { get; set; }
        
        protected float FireRate { get; set; }
        protected float ProjSpeed { get; set; }
        protected float Damage { get; set; }

        protected IEnumerator SetCooldown()
        {
            CanShoot = false;
            yield return new WaitForSeconds(FireRate);
            CanShoot = true;
        }
        
        public abstract void Shoot(Transform entTransform, Vector3 entDirection, AudioSource audioSrc);
    }
}