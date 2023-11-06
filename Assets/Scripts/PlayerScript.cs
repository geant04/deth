using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Weapons;

public class PlayerScript : MonoBehaviour
{
    private float _health = 100.0f;
    private float _speed = 5.0f;
    private Vector3 _direction = new(0, 0, 0);
    private bool _canShoot;

    public Inventory Inventory { get; private set; }
    public Animator Anim { get; private set; }
    
    private SpriteRenderer _spriteRend;
    private Rigidbody2D _rb;
    private AudioSource _audioSrc;
    
    public TMP_Text healthText;
    public GameObject gameOver;
    public Image overlay;
    public AudioClip hitSound;
    public GameObject startingWeapon;
    
    private Coroutine _dmgCoroutine;
    
    private static readonly int MouseHeldDown = Animator.StringToHash("MouseHeldDown");
    private static readonly int MouseClicked = Animator.StringToHash("MouseClicked");
    private static readonly int CurrGun = Animator.StringToHash("CurrGun");

    private void RotatePlayer() {
        var charVector = Camera.main.WorldToScreenPoint(transform.position);
        _direction = Input.mousePosition - charVector;
        
        var angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }
    
    public void TakeDamage(float f) {
        _health -= f;
        healthText.text = "HP " + _health;
        _audioSrc.PlayOneShot(hitSound, 1f);

        if (_dmgCoroutine != null) StopCoroutine(_dmgCoroutine);
        StartCoroutine(AnimTakeDamage());
    }

    private IEnumerator AnimTakeDamage()
    {
        var elapsed = 0f;
        const float duration = 0.4f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            var ratio = elapsed / duration;

            var opacity = Mathf.Lerp(0.5f, 0f, ratio);
            overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, opacity);
            
            var color = Color.Lerp(Color.red, Color.white, ratio);
            _spriteRend.color = color;

            yield return null;
        }

        // just in case it doesn't fully return back to normal
        _spriteRend.color = Color.white;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0f);
    }

    private IEnumerator ShootCooldown(float duration)
    {
        _canShoot = false;
        yield return new WaitForSeconds(duration);
        _canShoot = true;
    }

    private void ShootWeapon(bool heldDown)
    {
        if (!_canShoot) return;
        
        var curr = Inventory.Curr();

        if (curr == null) return;
        
        // only auto weapons can shoot when held down
        if (heldDown && curr.weaponId != WeaponId.AssaultRifle) return;
        
        // at this point either `heldDown` is false or current weapon is an auto
        curr.Shoot(transform, _direction);
        
        StartCoroutine(ShootCooldown(curr.fireRate));
        Anim.SetInteger(CurrGun, (int) curr.weaponId);
        
        if (heldDown)
        {
            Anim.SetBool(MouseHeldDown, true);
        }
        else
        {
            Anim.SetTrigger(MouseClicked);
        }
        
        _audioSrc.PlayOneShot(curr.shootSfx, 0.7f);
    }
    
    private void Awake()
    {
        Inventory = GetComponent<Inventory>();
        _spriteRend = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        _audioSrc = GetComponent<AudioSource>();

        var starting = Instantiate(startingWeapon);
        Inventory.AddWeapon(starting.GetComponent<CollectableWeapon>());

        _canShoot = true;
        healthText.text = "HP " + _health;
    }

    private void Update()
    {
        if (_health <= 0.0f)
        {
            gameOver.SetActive(true);
            return;
        }
        
        float h = 0;
        float v = 0;

        if (Input.GetKey(KeyCode.W))
        {
            v = 1;
        }
        
        if (Input.GetKey(KeyCode.S))
        {
            v = -1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            h = -1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            h = 1;
        }

        RotatePlayer();

        if (Input.GetMouseButtonDown(0))
        {
            ShootWeapon(false);
        }
        
        if (Input.GetMouseButton(0))
        {
            ShootWeapon(true);
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            Anim.SetBool(MouseHeldDown, false);
        }

        var change = new Vector2(h, v).normalized;
        if(change.magnitude > 0.0f) {
            float n = GameManager.CurrentMap[0].Length;
            float m = GameManager.CurrentMap.Length;
            
            var playerPosition = transform;
            var position = playerPosition.position;
            var localScale = playerPosition.localScale;
            
            // Unity raycasting sucks ass
            GameManager.Px = (position.x + localScale.x / 2.0f + (n / 2.0f));
            GameManager.Py = (position.y + localScale.y / 2.0f + (m / 2.0f));
        }

        _rb.velocity = change * _speed;
    }
}
