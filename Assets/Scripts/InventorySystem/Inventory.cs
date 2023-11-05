using UnityEngine;

namespace InventorySystem
{
    public class Inventory : MonoBehaviour
    {
        private Weapon[] _weapons;
        private int _curr;
        private Player _player;
        
        private Coroutine _gunCoroutine;
        
        private static readonly int CurrGun = Animator.StringToHash("CurrGun");
        
        public Weapon Curr => _weapons[_curr];

        public void ShootWeapon()
        {
            Curr.Shoot();
        }

        public Inventory(Player player)
        {
            _player = player;
            
            _weapons = new Weapon[] {
                new Pistol(player),
                new AutoRifle(player),
                new Shotgun(player)
            };
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Alpha1))
            {
                _curr = 0;
                _player.anim.SetInteger(CurrGun, 0);

                var curr = Curr;
                
                if (_gunCoroutine != null) StopCoroutine(_gunCoroutine);
                _gunCoroutine = StartCoroutine(_player.AnimateCurrGun(curr.weaponName, curr.sprite));
            } else if (Input.GetKey(KeyCode.Alpha2))
            {
                _curr = 1;
                _player.anim.SetInteger(CurrGun, 1);
                
                var curr = Curr;
                
                if (_gunCoroutine != null) StopCoroutine(_gunCoroutine);
                _gunCoroutine = StartCoroutine(_player.AnimateCurrGun(curr.weaponName, curr.sprite));
            } else if (Input.GetKey(KeyCode.Alpha3))
            {
                _curr = 2;
                _player.anim.SetInteger(CurrGun, 2);
                
                var curr = Curr;
                
                if (_gunCoroutine != null) StopCoroutine(_gunCoroutine);
                _gunCoroutine = StartCoroutine(_player.AnimateCurrGun(curr.weaponName, curr.sprite));
            }
        }
    }
}