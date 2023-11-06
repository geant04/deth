using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Weapons;
using TMPro;
using Util;

public class Inventory : MonoBehaviour
{
    public Image slot1Select;
    public Image slot2Select;
    public Image slot3Select;
    
    public Image slot1Weapon;
    public Image slot2Weapon;
    public Image slot3Weapon;

    public TMP_Text currText;
    public RectTransform currTextRect;
    public Image currImage;
    public RectTransform currImageRect;
    private Coroutine _gunCoroutine;

    private readonly Color _selected = Color.white;
    private readonly Color _deselected = new(0f, 0f, 0f, 0.6862745f);

    private PlayerScript _player;
    private static readonly int CurrGun = Animator.StringToHash("CurrGun");

    private WeaponData[] _weaponSlots = new WeaponData[3];
    private int _currSlotIdx;

    public WeaponData Curr()
    {
        return _weaponSlots[_currSlotIdx];
    }

    public void AddWeapon(CollectableWeapon newWeapon)
    {
        var weapon = newWeapon.CollectWeapon();
        
        if (_weaponSlots[0] == null)
        {
            _weaponSlots[0] = weapon;
        } else if (_weaponSlots[1] == null)
        {
            _weaponSlots[1] = weapon;
        } else if (_weaponSlots[2] == null)
        {
            _weaponSlots[2] = weapon;
        }
        else
        {
            // otherwise, replace currently selected weapon
            _weaponSlots[_currSlotIdx] = weapon;
        }
        
        // finally, update UI
        if (_gunCoroutine != null) StopCoroutine(_gunCoroutine);
        _gunCoroutine = StartCoroutine(AnimSwitchToCurr());
        
        SetWeaponImage();
    }
    
    private Image IdxToSlot(int idx)
    {
        var index = idx == -1 ? _currSlotIdx : idx;
        
        return index switch
        {
            0 => slot1Select,
            1 => slot2Select,
            2 => slot3Select,
            _ => null
        };
    }

    private static IEnumerator AnimFade(Graphic slot, Color begin, Color end)
    {
        const float duration = 0.1f;
        var elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            var ratio = elapsed / duration;
            var color = Color.Lerp(begin, end, ratio);
            slot.color = color;

            yield return null;
        }

        slot.color = end;
    }
    
    private IEnumerator AnimSwitchToCurr()
    {
        var curr = Curr();
        
        currText.text = curr ? curr.weaponName : "Nothing";

        if (curr == null)
        {
            currImage.color = Color.clear;
        }
        else
        {
            currImage.color = Color.white;
            currImage.sprite = curr.sprite;
        }
        
        if (_player != null) _player.Anim.SetInteger(CurrGun, curr ? (int) curr.weaponId : 0);
        
        var xText = currTextRect.anchoredPosition.x;
        var xImage = currImageRect.anchoredPosition.x;
        
        var initText = new Vector2(xText, -200);
        var initImage = new Vector2(xImage, -250);
        
        var finText = new Vector2(xText, 116);
        var finImage = new Vector2(xImage, 217);

        var elapsed = 0f;
        const float duration = 0.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            
            var ratio = Easing.EaseOutExpo(elapsed / duration);
            currTextRect.anchoredPosition = Vector2.Lerp(initText, finText, ratio);
            currImageRect.anchoredPosition = Vector2.Lerp(initImage, finImage, ratio);
            
            yield return null;
        }
        
        currTextRect.anchoredPosition = finText;
        currImageRect.anchoredPosition = finImage;
    }

    private void SetWeaponImage()
    {
        if (_weaponSlots[0] != null)
        {
            slot1Weapon.color = Color.white;
            slot1Weapon.sprite = _weaponSlots[0].sprite;
        }
        else
        {
            slot1Weapon.color = Color.clear;
        }
        
        if (_weaponSlots[1] != null)
        {
            slot2Weapon.color = Color.white;
            slot2Weapon.sprite = _weaponSlots[1].sprite;
        }
        else
        {
            slot2Weapon.color = Color.clear;
        }
        
        if (_weaponSlots[2] != null)
        {
            slot3Weapon.color = Color.white;
            slot3Weapon.sprite = _weaponSlots[2].sprite;
        }
        else
        {
            slot3Weapon.color = Color.clear;
        }
    }

    private void Awake()
    {
        _player = GetComponent<PlayerScript>();
    }

    private void Start()
    {
        _currSlotIdx = 0;
        
        StartCoroutine(AnimFade(IdxToSlot(-1), _deselected, _selected));
        
        if (_gunCoroutine != null) StopCoroutine(_gunCoroutine);
        _gunCoroutine = StartCoroutine(AnimSwitchToCurr());
        
        SetWeaponImage();
    }
    
    private void Update()
    {
        var prevSlot = -1;
        
        if (Input.GetKey(KeyCode.Alpha1))
        {
            prevSlot = _currSlotIdx;
            _currSlotIdx = 0;
        } else if (Input.GetKey(KeyCode.Alpha2))
        {
            prevSlot = _currSlotIdx;
            _currSlotIdx = 1;
        } else if (Input.GetKey(KeyCode.Alpha3))
        {
            prevSlot = _currSlotIdx;
            _currSlotIdx = 2;
        }
        
        // if player changed to a different slot
        if (prevSlot != -1 && prevSlot != _currSlotIdx)
        {
            StartCoroutine(AnimFade(IdxToSlot(-1), _deselected, _selected));
            StartCoroutine(AnimFade(IdxToSlot(prevSlot), _selected, _deselected));
            
            if (_gunCoroutine != null) StopCoroutine(_gunCoroutine);
            _gunCoroutine = StartCoroutine(AnimSwitchToCurr());
        }
    }
}