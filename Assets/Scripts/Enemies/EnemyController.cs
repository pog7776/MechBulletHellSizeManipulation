using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyController : MonoBehaviour
{
    public GameObject projectilePrefab;
    public CircleCollider2D vision;
    public TextMeshPro fireText;
    public float projectileVelocity;
    public GameObject visionObject;
    private GameObject gameControllerObj;
    private ScoreController scoreController;

    [SerializeField] private GameObject enemySprite;
    [SerializeField] public float hp = 20;
    private float previousHp;
    [SerializeField] private float hitTime = 0.2f;
    [SerializeField] private Color normal;
    [SerializeField] private Color hit;
    [SerializeField] private bool dead = false;
    [SerializeField] private float projectileSpeed = 10;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private int value;     //point value for killing enemy
    [SerializeField] public int threatValue;
    private Vector3 shootDirection;

    private float fireTimer;
    private GameObject player;

    [HideInInspector] public float visionRadius;


    // Start is called before the first frame update
    void Start()
    {
        fireTimer = 0;
        vision = visionObject.GetComponent<CircleCollider2D>();
        gameControllerObj = GameObject.Find("GameController");
        scoreController = gameControllerObj.GetComponent<ScoreController>();
        previousHp = hp;
    }

    // Update is called once per frame
    void Update()
    {
        enemySprite.transform.localPosition = new Vector3(0,0,0);

        if(hp < previousHp){
            StartCoroutine(HitColour());
        }

        if(hp <= 0)
        {
            Die();
        }

        vision.radius = visionRadius*1.5f;

        if (player != null) {
            if (fireTimer > 0) {
                fireTimer -= Time.deltaTime;
                fireText.text = fireTimer.ToString();
            }
            else {
                Shoot(player);
                fireTimer = fireRate; 
            }
        }
    }

    private void Shoot(GameObject target) {
        GameObject projectile = Instantiate(projectilePrefab, gameObject.transform.position, Quaternion.identity);  //spawn projectile
        ProjectileController controller = projectile.GetComponent<ProjectileController>();
        controller.owner = gameObject;
        controller.speed = projectileSpeed;
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();                                          //find projectile rigidbody
        shootDirection = target.transform.position - projectile.transform.position;
        //projectileRb.AddForce((target.transform.position - projectile.transform.position) * projectileSpeed);       //fire towards target
        projectileRb.velocity = new Vector2(shootDirection.x, shootDirection.y).normalized * projectileSpeed;       //fire towards target
    }

    private void Die() {
        dead = true;
        scoreController.GetComponent<ScoreController>().AddScore(value);
        Destroy(gameObject);
        Debug.Log("Enemy Died" + gameObject.name);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag.Equals("Player")) {    //find and set the player
            //Debug.Log("Enemy Found Player" + collision);
            player = collision.gameObject;
            gameObject.GetComponent<Enemy_Movement>().player = player;
        }

        if(collision.gameObject.tag.Equals("Arena Walls"))
        {

        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.tag.Equals("Player")) {    //stop shooting when player is far enough away
            Debug.Log("Enemy Lost Player" + collision);
            player = null;
            gameObject.GetComponent<Enemy_Movement>().player = player;
        }
    }

    private IEnumerator HitColour(){
        GetComponentInChildren<SpriteRenderer>().color = hit;
        yield return new WaitForSeconds(hitTime);
        GetComponentInChildren<SpriteRenderer>().color = normal;
    }
}
