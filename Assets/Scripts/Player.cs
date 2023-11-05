using System;
using System.Collections;
using InventorySystem;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private Inventory _inventory;
    
    private float health = 100.0f;
    private float speed = 6.0f;
    
    public Rigidbody2D rb;
    public Animator anim;
    
    public TMP_Text healthText;
    public GameObject currGunText;
    public GameObject currGunImage;
    public GameObject gameOver;

    private float hitDamage;
    private Color ogColor;
    
    private Coroutine dmgCoroutine;
    
    private const float Speed = 5.0f;
    public Vector3 direction = new(0, 0, 0);

    private float _pistolCooldown;
    private float _automaticCooldown;
    private float _shotgunCooldown;

    public AudioSource audioSource;
    public AudioClip hitSound;

    public Image overlay;
    private static readonly int MouseHeldDown = Animator.StringToHash("MouseHeldDown");

    private void RotatePlayer() {
        var charVector = Camera.main.WorldToScreenPoint(transform.position);
        direction = Input.mousePosition - charVector;
        
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }
    
    public void TakeDamage(float f) {
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

            var opacity = Mathf.Lerp(0.5f, 0f, ratio);
            overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, opacity);

            yield return null;
        }
    }

    public IEnumerator AnimateCurrGun(string gunName, Sprite sprite)
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
            
            var ratio = EaseOutExpo(elapsed / 0.5f);
            currGunText.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(initText, finText, ratio);
            currGunImage.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(initImage, finImage, ratio);
            
            yield return null;
        }
    }

    private static float EaseOutExpo(float num)
    {
        return num >= 1 ? 1 : (float) (1 - Math.Pow(2, -10 * num));
    }

    private void Start()
    {
        _inventory = new Inventory(this);
        
        healthText.text = "HP " + health;
        currGunText.GetComponent<TMP_Text>().text = _inventory.Curr.weaponName;
        currGunImage.GetComponent<Image>().sprite = _inventory.Curr.sprite;

        ogColor = GetComponent<SpriteRenderer>().color;
        
        StartCoroutine(AnimateCurrGun(_inventory.Curr.weaponName, _inventory.Curr.sprite));
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

            var dmg = Color.Lerp(ogColor, Color.red, hitDamage / 0.25f);
            GetComponent<SpriteRenderer>().color = dmg;
        }
        
        float h = 0;
        float v = 0;

        if(Input.GetKey(KeyCode.W)) {
            v = 1;
        } else if(Input.GetKey(KeyCode.S)) {
            v = -1;
        } else if(Input.GetKey(KeyCode.A)) {
            h = -1;
        } else if(Input.GetKey(KeyCode.D)) {
            h = 1;
        }

        RotatePlayer();

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
        {
            _inventory.ShootWeapon();
        } else if (Input.GetMouseButtonUp(0))
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
