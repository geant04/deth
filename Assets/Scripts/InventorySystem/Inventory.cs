using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Util;

namespace InventorySystem
{
    public class Inventory : MonoBehaviour
    {
        private Player _player;
        
        private Coroutine _gunCoroutine;
        private Weapon[] _weapons;
        private int _slotIdx;
        
        private static readonly int CurrGun = Animator.StringToHash("CurrGun");
        
        public GameObject currGunText;
        public GameObject currGunImage;

        public GameObject pistolPrefab;
        public GameObject assaultRiflePrefab;
        public GameObject shotgunPrefab;

        public Weapon Curr()
        {
            return _weapons[_slotIdx];
        }

        public void ShootCurrWeapon()
        {
            Curr().Shoot(_player.transform, _player.direction, _player.audioSource);
        }
        
        private IEnumerator AnimSwitchGun(string weaponName, Sprite sprite)
        {
            currGunText.GetComponent<TMP_Text>().text = weaponName;
            currGunImage.GetComponent<Image>().sprite = sprite;

            var xText = currGunText.GetComponent<RectTransform>().anchoredPosition.x;
            var xImage = currGunImage.GetComponent<RectTransform>().anchoredPosition.x;
        
            var initText = new Vector2(xText, -200);
            var initImage = new Vector2(xImage, -250);
        
            var finText = new Vector2(xText, 116);
            var finImage = new Vector2(xImage, 217);

            var elapsed = 0f;

            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
            
                var ratio = Easing.EaseOutExpo(elapsed / 0.5f);
                currGunText.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(initText, finText, ratio);
                currGunImage.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(initImage, finImage, ratio);
            
                yield return null;
            }
        }

        private void Start()
        {
            _slotIdx = 1;
            _weapons = new Weapon[3];
            _player = gameObject.GetComponent<Player>();
            
            var pistol = Instantiate(pistolPrefab, _player.transform);
            var assaultRifle = Instantiate(assaultRiflePrefab, _player.transform);
            var shotgun = Instantiate(shotgunPrefab, _player.transform);

            // in the future the player may not start with all weapons
            _weapons[0] = pistol.GetComponent<Pistol>();
            _weapons[1] = assaultRifle.GetComponent<AssaultRifle>();
            _weapons[2] = shotgun.GetComponent<Shotgun>();
            
            Debug.Log(Curr());
            Debug.Log(pistol.GetComponent<Weapon>().WeaponName);
            Debug.Log((int) Curr().WeaponId);

            _player.anim.SetInteger(CurrGun, (int) Curr().WeaponId);
            StartCoroutine(AnimSwitchGun(Curr().WeaponName, Curr().sprite));
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Alpha1))
            {
                _slotIdx = 0;
                _player.anim.SetInteger(CurrGun, (int) Curr().WeaponId);

                if (_gunCoroutine != null) StopCoroutine(_gunCoroutine);
                _gunCoroutine = StartCoroutine(AnimSwitchGun(Curr().WeaponName, Curr().sprite));
            } else if (Input.GetKey(KeyCode.Alpha2))
            {
                _slotIdx = 1;
                _player.anim.SetInteger(CurrGun, (int) Curr().WeaponId);
                
                if (_gunCoroutine != null) StopCoroutine(_gunCoroutine);
                _gunCoroutine = StartCoroutine(AnimSwitchGun(Curr().WeaponName, Curr().sprite));
            } else if (Input.GetKey(KeyCode.Alpha3))
            {
                _slotIdx = 2;
                _player.anim.SetInteger(CurrGun, (int) Curr().WeaponId);
                
                if (_gunCoroutine != null) StopCoroutine(_gunCoroutine);
                _gunCoroutine = StartCoroutine(AnimSwitchGun(Curr().WeaponName, Curr().sprite));
            }
        }
    }
}