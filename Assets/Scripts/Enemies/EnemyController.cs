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

    [SerializeField] public float hp = 20;
    [SerializeField] private bool dead = false;
    [SerializeField] private float projectileSpeed = 10;
    [SerializeField] private float fireRate = 0.5f;
    private Vector3 shootDirection;

    private float fireTimer;
    private GameObject player;

    [HideInInspector] public float visionRadius;


    // Start is called before the first frame update
    void Start()
    {
        fireTimer = 0;
        vision = visionObject.GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

        if(hp <= 0) {
            die();
        }

        vision.radius = visionRadius;

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
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();                                          //find projectile rigidbody
        shootDirection = target.transform.position - projectile.transform.position;
        //projectileRb.AddForce((target.transform.position - projectile.transform.position) * projectileSpeed);       //fire towards target
        projectileRb.velocity = new Vector2(shootDirection.x, shootDirection.y).normalized * projectileSpeed;       //fire towards target
    }

    private void die() {
        dead = true;
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag.Equals("Player")) {    //find and set the player
            Debug.Log("Enemy Found Player" + collision);
            player = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.tag.Equals("Player")) {    //stop shooting when player is far enough away
            Debug.Log("Enemy Lost Player" + collision);
            player = null;
        }
    }
}
