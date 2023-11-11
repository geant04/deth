using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyScript : MonoBehaviour
{
    private Vector3 _dir;
    private Vector3 _goal;
    private Rigidbody2D _rb;
    private GameObject _player;
    private UnityEngine.Rendering.Universal.Light2D _shadowLight;
    private Color _ogColor;

    private float _health = 100.0f;
    private float _speed = 6.5f;

    public Animator anim;
    private static readonly int Angry = Animator.StringToHash("Angry");

    // hitDamage
    private float _hitDamage;
    private float _deathSequence;
    private float _explodeTimer;

    private List<(int, int)> _path;
    private int _nx;
    private int _ny;

    // layerMask
    [FormerlySerializedAs("Ignore")] public LayerMask ignore;

    public AudioSource audioSource;
    public AudioClip beep;
    public AudioClip hit;
    public AudioClip boom;

    // coroutines
    private IEnumerator lightCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _player = GameObject.Find("Player");
        SetShadowLight();
        SetOGColor();
    }

    public void TakeDamage(float damage) {
        _health -= damage;
        _hitDamage = 0.1f;
        audioSource.PlayOneShot(hit, 0.4f);
    }

    public void SetShadowLight() {
        _shadowLight = this.transform.Find("ShadowCastLight").GetComponent<UnityEngine.Rendering.Universal.Light2D>();
    }

    public void SetOGColor() {
        _ogColor = GetComponent<SpriteRenderer>().color;
    }

    public void HurtSeqeunce() {
        GetComponent<SpriteRenderer>().color = _ogColor;
        if(_hitDamage != 0.0f) {
            _hitDamage = Mathf.Max(0.0f, _hitDamage - Time.deltaTime);

            Color dmg = Color.Lerp(_ogColor, Color.red, _hitDamage / 0.1f);
            GetComponent<SpriteRenderer>().color = dmg;
        }
    }

    public bool isDead() {
        return _health <= 0.0f;
    }

    public GameObject getPlayer() {
        return _player;
    }

    bool IsValidMove(int[][] maze, int y, int x, HashSet<(int, int)> visited)
    {
        return (y >= 0 && y < maze.Length) && (x >= 0 && x < maze[0].Length) && (maze[y][x] == 0) && !visited.Contains((y, x));
    }
    
    List<(int, int)> MazeSolver(int[][] maze, (int, int) start, (int, int) end) {
        // Directions for moving up, down, left, and right
        
        float n = GameManager.CurrentMap[0].Length;
        float m = GameManager.CurrentMap.Length;

        int[][] directions = { new[] { -1, 0 }, new[] { 1, 0 }, new[] { 0, -1 }, new[] { 0, 1 } };

        Queue<(int, int)> queue = new Queue<(int, int)>();
        HashSet<(int, int)> visited = new HashSet<(int, int)>();
        Dictionary<(int, int), (int, int)> previous = new Dictionary<(int, int), (int, int)>();

        queue.Enqueue(start);
        visited.Add(start);
        previous[start] = (0, 0);

        while (queue.Count > 0)
        {
            var (currY, currX) = queue.Dequeue();

            // If we've reached the destination
            if ((currY, currX) == end)
            {
                var path = new List<(int, int)>();
                while ((currY, currX) != start)
                {
                    //Instantiate(block, new Vector3(currX - (n / 2.0f), currY - (m / 2.0f), 0.0f), Quaternion.identity);
                    path.Add((currY, currX));
                    (currY, currX) = previous[(currY, currX)];
                }
                path.Add(start);
                path.Reverse();
                return path;
            }

            foreach (int[] direction in directions)
            {
                var newY = currY + direction[0];
                var newX = currX + direction[1];

                if (!IsValidMove(maze, newY, newX, visited)) continue;
                
                queue.Enqueue((newY, newX));
                visited.Add((newY, newX));
                previous[(newY, newX)] = (currY, currX);
            }
        }

        return null;
    }

    private void Sentry() {
        float n = GameManager.CurrentMap[0].Length;
        float m = GameManager.CurrentMap.Length;

        if(GameManager.Px >= 0 && GameManager.Px < n && GameManager.Py >= 0 && GameManager.Py < m) {
            int x = (int)GameManager.Px;
            int y = (int)GameManager.Py;

            Vector3 target = _player.transform.position - transform.position;

            if(target.magnitude < 7.0f || _path != null) {
                anim.SetBool(Angry, true);
                
                int ex = (int)(transform.position.x + (n / 2.0f));
                int ey = (int)(transform.position.y + (m / 2.0f));
                
                if(ex < 0 || ex >= n || ey < 0 || ey >= m) {
                    return;
                }

                if(_path != null && _path.Count > 0) {
                    if((_goal - transform.position).magnitude < 1.2f) {
                        audioSource.PlayOneShot(beep, 0.4f);

                        _path.RemoveAt(0);
                        _goal = _player.transform.position;
                        if(_path.Count != 0) {
                            (_ny, _nx) = _path[0];
                            _goal = new Vector3(_nx - (n / 2.0f), _ny - (m / 2.0f), 0.0f);
                        }
                        _dir = _goal - transform.position;
                    }
                } else {
                    _path = MazeSolver(GameManager.CurrentMap, (ey, ex), (y, x));

                    if (_path != null)
                    {
                        _path.RemoveAt(0);

                        _goal = _player.transform.position;
                        if(_path.Count != 0) {
                            (_ny, _nx) = _path[0];
                            _goal = new Vector3(_nx - (n / 2.0f), _ny - (m / 2.0f), 0.0f);
                        }
                        _dir = _goal - transform.position;
                    }
                }
            
                if(target.magnitude < 2.0f) {
                    _goal = _player.transform.position;
                    _dir = _goal - transform.position;

                    if(_explodeTimer < 0.0f) {
                        _health = 0.0f;
                        _player.GetComponent<PlayerScript>().TakeDamage(20.0f);
                        return;
                    }
                    if(_explodeTimer == 0.0f) {
                        _explodeTimer = 1.0f;
                    }
                    _explodeTimer -= Time.deltaTime;
                    transform.position += _dir.normalized * (_speed * Time.deltaTime);
                    return;
                }

                _explodeTimer = 0.0f;
            }
            
            transform.position += _dir.normalized * (_speed * Time.deltaTime);
        }
    }

    private IEnumerator FadeLight(float waitTime) {
        float t = 0.0f;
        float tColor = 0.4f;
        
        Destroy(GetComponent<Rigidbody2D>());
        Destroy(GetComponent<CircleCollider2D>());
        GameManager.EnemyList.Remove(gameObject);
        audioSource.PlayOneShot(boom, 0.2f);

        while(t < waitTime) {
            GetComponent<SpriteRenderer>().color = new Color(tColor, tColor, tColor, tColor);
            float amt = 1.0f - tColor;
            transform.localScale += new Vector3(amt * 0.05f, amt * 0.05f, 1.0f);

            t += Time.deltaTime;
            tColor -= Time.deltaTime;

            _shadowLight.intensity = Mathf.Lerp(1.0f, 0.0f, t / waitTime);

            yield return null;
        }
        transform.localScale = new Vector3(0, 0, 0);
        yield return new WaitForSeconds(0.4f);

        Destroy(gameObject);
    }

    public void CheckDeath() {
        if(isDead()) {
            if(lightCoroutine == null) {
                lightCoroutine = FadeLight(0.5f);
                StartCoroutine(lightCoroutine);
            }
            return;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (_player == null)
        {
            Destroy(gameObject);
            return;
        }
        
        HurtSeqeunce();
        CheckDeath();
        if(isDead()){
            return;
        }
        Sentry();
    }
}
