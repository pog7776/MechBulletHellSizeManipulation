using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyController : MonoBehaviour
{
    public GameObject projectilePrefab;
    public CircleCollider2D vision;
    public TextMeshPro fireText;

    [SerializeField] private float projectileSpeed = 30;
    [SerializeField] private float fireRate = 0.5f;

    private float fireTimer;
    private GameObject player;

    [HideInInspector] public float visionRadius;


    // Start is called before the first frame update
    void Start()
    {
        fireTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {

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
        projectileRb.AddForce((target.transform.position - projectile.transform.position) * projectileSpeed);       //fire towards target
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
