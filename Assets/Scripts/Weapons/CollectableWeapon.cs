using UnityEngine;

namespace Weapons
{
    public class CollectableWeapon : MonoBehaviour
    {
        public WeaponData weapon;
        
        public WeaponData CollectWeapon()
        {
            Destroy(gameObject);
            return weapon;
        }
    }
}