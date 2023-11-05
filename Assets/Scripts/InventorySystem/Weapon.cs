using System.Collections;
using UnityEngine;

namespace InventorySystem
{
    public abstract class Weapon : MonoBehaviour
    {
        public AudioClip shootSfx;
        public Sprite sprite;
        public string weaponName;
        
        protected float FireRate { get; set; }
        protected bool CanShoot { get; private set; } = true;
        
        protected float ProjSpeed { get; set; }
        protected float Damage { get; set; }

        protected Player Player { get; set; }
        
        protected IEnumerator SetCooldown()
        {
            CanShoot = false;
            yield return new WaitForSeconds(FireRate);
            CanShoot = true;
        }
        
        public abstract void Shoot();
    }
}