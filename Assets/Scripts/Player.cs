using System.Collections;
using InventorySystem;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private float _health = 100.0f;
    private const float Speed = 5.0f;
    
    public Vector3 direction = new(0, 0, 0);

    private Inventory _inventory;
    private SpriteRenderer _spriteRenderer;
    
    public Rigidbody2D rb;
    public Animator anim;
    
    public TMP_Text healthText;
    public GameObject gameOver;
    public Image overlay;
    
    private Coroutine _dmgCoroutine;
    
    public AudioSource audioSource;
    public AudioClip hitSound;
    
    private static readonly int MouseHeldDown = Animator.StringToHash("MouseHeldDown");
    private static readonly int MouseClicked = Animator.StringToHash("MouseClicked");

    private void RotatePlayer() {
        var charVector = Camera.main!.WorldToScreenPoint(transform.position);
        direction = Input.mousePosition - charVector;
        
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }
    
    public void TakeDamage(float f) {
        _health -= f;
        healthText.text = "HP " + _health;
        audioSource.PlayOneShot(hitSound, 1f);

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
            _spriteRenderer.color = color;

            yield return null;
        }

        // just in case it doesn't fully return back to normal
        _spriteRenderer.color = Color.white;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0f);
    }
    
    private void Start()
    {
        _inventory = gameObject.GetComponent<Inventory>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        healthText.text = "HP " + _health;
    }

    private void Update()
    {
        if (_health <= 0.0f)
        {
            gameOver.SetActive(true);
            Destroy(gameObject);
            
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
            _inventory.ShootCurrWeapon();
            anim.SetTrigger(MouseClicked);
        } else if (Input.GetMouseButton(0) && _inventory.Curr().WeaponId == WeaponId.AssaultRifle)
        {
            _inventory.ShootCurrWeapon();
            anim.SetBool(MouseHeldDown, true);
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
    }
}
