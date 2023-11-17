using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunnerScript : EnemyScript
{
    public GameObject projPrefab;
    private Vector3 dir;
    private Vector3 tdir;

    private Vector3 goal;
    private Rigidbody2D rb;
    private GameObject player;
    
    public AudioClip fire;

    float speed = 4.5f;

    private List<(int, int)> path;
    private int nx;
    private int ny;

    float n;
    float m;

    private IEnumerator ShootCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        SetShadowLight();
        SetOGColor();

        
        n = GameManager.CurrentMap[0].Length;
        m = GameManager.CurrentMap.Length;
    }

    private void Fire() {
        var proj = Instantiate(projPrefab, transform.position, transform.rotation);
        var bullet = proj.GetComponent<BulletScript>();

        bullet.setIsEnemy(true);
        bullet.setDamage(5.0f);
        bullet.direction = player.transform.position - transform.position;
        bullet.speed = 20.0f;
    }

    private IEnumerator Shoot() {
        Vector3 target = player.transform.position - transform.position;
        
        speed = 0.0f;
        tdir = dir;
        
        int ex = (int)(transform.position.x + (n / 2.0f));
        int ey = (int)(transform.position.y + (m / 2.0f));

        for(int i = 0; i < 5 && !isDead(); i++) {
            // if the ray thing still works, then i fire
            if(RayCheck(target, ex, ey)) {
                Fire();
                audioSource.PlayOneShot(fire, 0.2f);
            }
            yield return new WaitForSeconds(0.1f);
        }
        speed = 4.5f;
        dir = tdir;
        ShootCoroutine = null;
    }

    private bool RayCheck(Vector2 vec, int ex, int ey) {
        Vector2 ray = vec.normalized;

        for (float length = 0; length < vec.magnitude; length += 0.1f) {
            int px = (int)(transform.position.x + (n / 2.0f) + (ray.x * length));
            int py = (int)(transform.position.y + (m / 2.0f) + (ray.y * length));

            if(px < 0 || px >= n || py < 0 || py >= m) {
                return false;
            }

            if(GameManager.CurrentMap[py][px] == 1) {
                return false;
            }
        }
        
        return true;
    }

    private void AI() {
        if(GameManager.Px >= 0 && GameManager.Px < n && GameManager.Py >= 0 && GameManager.Py < m) {
            int x = (int)GameManager.Px;
            int y = (int)GameManager.Py;

            Vector3 target = player.transform.position - transform.position;
            
            if(target.magnitude < 7.0f || path != null) {
                //anim.SetBool(Angry, true);
                
                int ex = (int)(transform.position.x + (n / 2.0f));
                int ey = (int)(transform.position.y + (m / 2.0f));
                
                if(ex < 0 || ex >= n || ey < 0 || ey >= m) {
                    return;
                }
                
                if(ShootCoroutine != null) {
                    dir = player.transform.position - transform.position;
                    // we are shooting, so we don't need to do anything else
                } else if(RayCheck(target, ex, ey)) {
                    // player is within sight, we can just start blasting
                    ShootCoroutine = Shoot();
                    StartCoroutine(ShootCoroutine);
                }

                if(path != null && path.Count > 0) {
                    if((goal - transform.position).magnitude < 0.75f) {
                        //audioSource.PlayOneShot(beep, 0.4f);

                        path.RemoveAt(0);
                        goal = player.transform.position;
                        if(path.Count != 0) {
                            (ny, nx) = path[0];
                            // try an offset
                            goal = new Vector3(nx - (n / 2.0f) + 0.5f, ny - (m / 2.0f) + 0.5f, 0.0f);
                        }
                        dir = goal - transform.position;
                    }
                } else {
                    // inherited from the EnemyScript.cs
                    path = MazeSolver(GameManager.CurrentMap, (ey, ex), (y, x));

                    if (path != null)
                    {
                        path.RemoveAt(0);

                        goal = player.transform.position;
                        if(path.Count != 0) {
                            (ny, nx) = path[0];
                            goal = new Vector3(nx - (n / 2.0f) + 0.5f, ny - (m / 2.0f) + 0.5f, 0.0f);
                        }
                        dir = goal - transform.position;
                    }
                }
            }
            
            transform.position += dir.normalized * (speed * Time.deltaTime);
            
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }
    }

    // Update is called once per frame
    void Update()
    {
        HurtSeqeunce();
        CheckDeath();
        AI();
    }
}
