using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private float speed = 5.0f;
    Vector3 cVector;
    public GameObject bullet;
    public Rigidbody2D rb;
    
    private int gunType = 1;

    // pistolCoolDown
    private float pistolCoolDown = 0.0f;

    // shotgunCoolDown

    // automaticCoolDown
    private float automaticCoolDown = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        cVector = new Vector3(0, 0, 0);
    }

    // shotgun, weapon
    // pistol weapon
    // rifle weapon

    void rotatePlayer() {
        Vector3 charVector = Camera.main.WorldToScreenPoint(transform.position);
        cVector = Input.mousePosition - charVector;
        float angle = Mathf.Atan2(cVector.y, cVector.x) * Mathf.Rad2Deg;
 
        //transform.position = charVector;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    void shootPistol() {
        if(pistolCoolDown == 0.0f) {
            // mouseCoolDown
            pistolCoolDown = 0.05f;
            GameObject projectile = Instantiate(bullet, transform.position, transform.rotation);
            projectile.GetComponent<BulletScript>().direction = cVector;
            projectile.GetComponent<BulletScript>().speed = 12.0f;
        }
    }

    void shootAuto() {
        if(automaticCoolDown == 0.0f) {
            // mouseCoolDown
            automaticCoolDown = 0.10f;
            GameObject projectile = Instantiate(bullet, transform.position, transform.rotation);
            projectile.GetComponent<BulletScript>().direction = cVector;
            projectile.GetComponent<BulletScript>().speed = 12.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float h = 0;
        float v = 0;
        
        //Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //mouseWorldPos.z = 0f;

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

        rotatePlayer();
        if(Input.GetMouseButtonDown(0) && gunType == 0) {
            shootPistol();
        }
        if(gunType == 1 && Input.GetMouseButton(0)) {
            shootAuto();
        }

        Vector2 change = new Vector2(h, v).normalized;
        rb.velocity = change * speed;

        if(pistolCoolDown > 0.0f) {
            pistolCoolDown = Mathf.Max(pistolCoolDown - Time.deltaTime, 0.0f);
        }
        if(automaticCoolDown > 0.0f) {
            automaticCoolDown = Mathf.Max(automaticCoolDown - Time.deltaTime, 0.0f);
        }
    }
}