using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.UI;

public enum Gun
{
    Pistol = 0,
    Automatic = 1,
    Shotgun = 2
}

public class PlayerScript : MonoBehaviour
{
    private float health = 100.0f;
    private float speed = 6.0f;
    
    private Vector3 cVector;
    public GameObject bullet;
    public GameObject pellet;
    public Rigidbody2D rb;
    
    public TMP_Text healthText;

    public Animator anim;
    private static readonly int CurrGun = Animator.StringToHash("CurrGun");
    private static readonly int MouseHeldDown = Animator.StringToHash("MouseHeldDown");
    private static readonly int MouseClicked = Animator.StringToHash("MouseClicked");

    public GameObject currGunText;
    public GameObject currGunImage;
    
    public Sprite pistolSprite;
    public Sprite autoSprite;
    public Sprite shotgunSprite;

    public GameObject gameOver;

    private float hitDamage;
    private Color ogColor;
    
    private Coroutine gunCoroutine;
    private Coroutine dmgCoroutine;
    
    private const float Speed = 5.0f;
    private Gun _gun = Gun.Pistol;
    private Vector3 _cVector = new(0, 0, 0);

    private float _pistolCooldown;
    private float _automaticCooldown;
    private float _shotgunCooldown;

    public AudioSource audioSource;
    public AudioClip pistolSound;
    public AudioClip rifleSound;
    public AudioClip shotgunSound;
    public AudioClip hitSound;

    public Image overlay;

    private void RotatePlayer() {
        var charVector = Camera.main.WorldToScreenPoint(transform.position);
        _cVector = Input.mousePosition - charVector;
        
        var angle = Mathf.Atan2(_cVector.y, _cVector.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    private void ShootPistol()
    {
        if (_pistolCooldown != 0.0f) return;
        
        anim.SetTrigger(MouseClicked);
        _pistolCooldown = 0.05f;
        
        var projectile = Instantiate(bullet, transform.position + (0.5f * _cVector.normalized), transform.rotation);
        projectile.GetComponent<BulletScript>().direction = _cVector;
        projectile.GetComponent<BulletScript>().speed = 20.0f;
        projectile.GetComponent<BulletScript>().setDamage(45.0f);

        audioSource.PlayOneShot(pistolSound, 0.7f);
    }

    private void ShootAuto()
    {
        if (_automaticCooldown != 0.0f) return;
        
        anim.SetBool(MouseHeldDown, true);
        _automaticCooldown = 0.10f;
        
        var projectile = Instantiate(bullet, transform.position + (0.5f * _cVector.normalized), transform.rotation);
        projectile.GetComponent<BulletScript>().direction = _cVector;
        projectile.GetComponent<BulletScript>().speed = 20.0f;
        projectile.GetComponent<BulletScript>().setDamage(15.0f);
        
        audioSource.PlayOneShot(rifleSound, 0.7f);
    }

    private void ShootShotgun()
    {
        if (_shotgunCooldown != 0.0f) return;
        
        anim.SetTrigger(MouseClicked);
        _shotgunCooldown = 0.65f;
        var pellets = 4;
        
        for (var i = -pellets + 1; i < pellets; i++) {
            var angle = i / (float) pellets * 10.0f;
                
            Vector2 rotatedVector = Quaternion.Euler(0, 0, angle) * _cVector;
            rotatedVector.Normalize();

            var projectile = Instantiate(pellet, transform.position + (0.5f * _cVector.normalized), transform.rotation);
            projectile.transform.localScale = new Vector3(0.175f, 0.175f, 0.0f);
            projectile.GetComponent<BulletScript>().setDamage(15.0f);
            projectile.GetComponent<BulletScript>().direction = rotatedVector;
            projectile.GetComponent<BulletScript>().speed = 16.0f + Random.Range(1,5);
        }

        audioSource.PlayOneShot(shotgunSound, 0.4f);
    }
    
    public void takeDamage(float f) {
        health -= f;
        hitDamage = 0.25f;
        healthText.text = "HP " + health;
        
        audioSource.PlayOneShot(hitSound, 1f);

        if (dmgCoroutine != null)
        {
            StopCoroutine(dmgCoroutine);
        }
        StartCoroutine(AnimTakeDamage());
    }

    private IEnumerator AnimTakeDamage()
    {
        var elapsed = 0f;

        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            var ratio = elapsed / 0.5f;

            float opacity = Mathf.Lerp(0.5f, 0f, ratio);
            overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, opacity);

            yield return null;
        }
    }

    private IEnumerator AnimateCurrGun(string gunName, Sprite sprite)
    {
        currGunText.GetComponent<TMP_Text>().text = gunName;
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
            
            var ratio = easeOutExpo(elapsed / 0.5f);
            currGunText.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(initText, finText, ratio);
            currGunImage.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(initImage, finImage, ratio);
            
            yield return null;
        }
    }

    private float easeOutExpo(float num)
    {
        return num >= 1 ? 1 : (float) (1 - Math.Pow(2, -10 * num));
    }

    private void Start()
    {
        healthText.text = "HP " + health;
        currGunText.GetComponent<TMP_Text>().text = "Pistol";
        currGunImage.GetComponent<Image>().sprite = pistolSprite;

        ogColor = GetComponent<SpriteRenderer>().color;
        
        StartCoroutine(AnimateCurrGun("Pistol", pistolSprite));
    }

    private void Update()
    {
        if (health <= 0.0f)
        {
            gameOver.SetActive(true);
            
            Destroy(gameObject);
            return;
        }
        
        if (hitDamage != 0f)
        {
            hitDamage = Mathf.Max(0.0f, hitDamage - Time.deltaTime);

            Color dmg = Color.Lerp(ogColor, Color.red, hitDamage / 0.25f);
            GetComponent<SpriteRenderer>().color = dmg;
        }
        
        float h = 0;
        float v = 0;

        if(Input.GetKey(KeyCode.W)) {
            v = 1;
        }
        if(Input.GetKey(KeyCode.S)) {
            v = -1;
        }
        if(Input.GetKey(KeyCode.A)) {
            h = -1;
        }
        if(Input.GetKey(KeyCode.D)) {
            h = 1;
        }

        if (Input.GetKey(KeyCode.Alpha1))
        {
            if (_gun != Gun.Pistol)
            {
                _gun = Gun.Pistol;
                anim.SetInteger(CurrGun, 0);
                
                if (gunCoroutine != null)
                {
                    StopCoroutine(gunCoroutine);
                }
                gunCoroutine = StartCoroutine(AnimateCurrGun("Pistol", pistolSprite));
            }
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            if (_gun != Gun.Automatic)
            {
                _gun = Gun.Automatic;
                anim.SetInteger(CurrGun, 1);
                
                if (gunCoroutine != null)
                {
                    StopCoroutine(gunCoroutine);
                }
                gunCoroutine = StartCoroutine(AnimateCurrGun("Assault Rifle", autoSprite));
            }
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            if (_gun != Gun.Shotgun)
            {
                _gun = Gun.Shotgun;
                anim.SetInteger(CurrGun, 2);

                if (gunCoroutine != null)
                {
                    StopCoroutine(gunCoroutine);
                }
                gunCoroutine = StartCoroutine(AnimateCurrGun("Shotgun", shotgunSprite));
            }
        }

        RotatePlayer();

        if (Input.GetMouseButtonDown(0))
        {
            switch (_gun)
            {
                case Gun.Pistol:
                    ShootPistol();
                    break;
                case Gun.Shotgun:
                    ShootShotgun();
                    break;
                case Gun.Automatic:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (_gun == Gun.Automatic)
            {
                ShootAuto();
            }
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            anim.SetBool(MouseHeldDown, false);
        }

        var change = new Vector2(h, v).normalized;
        if(change.magnitude > 0.0f) {
            float n = GameManager.CurrentMap[0].Length;
            float m = GameManager.CurrentMap.Length;

            // Unity raycasting sucks ass
            GameManager.Px = (transform.position.x + transform.localScale.x / 2.0f + (n / 2.0f));
            GameManager.Py = (transform.position.y + transform.localScale.y / 2.0f + (m / 2.0f));
        }

        rb.velocity = change * Speed;

        if(_pistolCooldown > 0.0f) {
            _pistolCooldown = Mathf.Max(_pistolCooldown - Time.deltaTime, 0.0f);
        }
        if(_automaticCooldown > 0.0f) {
            _automaticCooldown = Mathf.Max(_automaticCooldown - Time.deltaTime, 0.0f);
        }
        if(_shotgunCooldown > 0.0f) {
            _shotgunCooldown = Mathf.Max(_shotgunCooldown - Time.deltaTime, 0.0f);
        }
    }
}
