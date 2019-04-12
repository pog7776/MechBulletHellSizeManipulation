using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private GameObject player;
    private Camera cam;

    [Header("Movement Properties")]
    [SerializeField] private float speed = 8;           //speed of player movement

    [Header("Scale Properties")]
    [SerializeField] private float scaleSpeed = 0.1f;   //speed player changes size
    [SerializeField] private bool minimumSize = false;  //is player the minimum size
    [SerializeField] public Vector3 size;               //current size
    [SerializeField] private float timeScale;           //current timeScale

    private Vector3 scaleVector;                        //modifier of scale

    [Header("Combat Properties")]
    public GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 20;
    [SerializeField] private float fireRate = 0.1f;
    [SerializeField] private float hp = 100;
    public GameObject healthBar;
    [SerializeField] private bool dead = false;

    private float fireTimer;
    private Vector3 shootDirection;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        cam = Camera.main;
        Debug.Log("Camera found: " + cam + cam.name);

        scaleVector.Set(scaleSpeed, scaleSpeed, scaleSpeed);
        size = player.transform.localScale;

        fireTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        size = player.transform.localScale;
        Movement();
        Size();

        if (Input.GetButton("Fire1")) {
            if (fireTimer > 0) {
                fireTimer -= Time.deltaTime;
            }
            else {
                Shoot(FindMouse());
                fireTimer = fireRate;
            }
        }
    }

    private void Movement() {

        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");

        float xVelocity = xInput * speed * Time.deltaTime;
        float yVelocity = yInput * speed * Time.deltaTime;

        if (xVelocity != 0 || yVelocity != 0) {
            transform.position += new Vector3(xVelocity, yVelocity, 0);
        }

        Vector3 position = transform.position;
        transform.position = position;
    }

    private void Size() {
        if(size.x - scaleVector.x <= 0) {       //check if player can shrink anymore
            minimumSize = true;
        }
        else {
            minimumSize = false;
        }

        if(Input.GetAxisRaw("Mouse ScrollWheel") < 0) {                         //enlarge player
            player.transform.localScale = size + scaleVector;

            //cam.transform.position.Set(player.transform.position.x, player.transform.position.y, cam.transform.position.z + scaleVector.z);  //please don't remove this, i want to do something with it  -jack

            cam.orthographicSize += scaleVector.x * 5;                          //modify camera
            changeTime(size.x);                                                 //modify timescale
        }
        else if(Input.GetAxisRaw("Mouse ScrollWheel") > 0 && !minimumSize) {    //shrink player
            player.transform.localScale = size - scaleVector;

            if (cam.orthographicSize - scaleVector.x * 5 > 0) {     //ensure camera size doesn't become a negative
                cam.orthographicSize -= scaleVector.x * 5;          //modify camera
            }

            changeTime(size.x);                                     //modify timescale
        }
    }

    private void changeTime(float timeShift) {
        if (timeShift > 0.05 && timeShift < 10) {       //maximum time change
            timeScale = timeShift;                      //to monitor current time scale
            Time.timeScale = timeScale;                 //set time scale
        }
    }

    private Vector3 FindMouse() {
        //...setting shoot direction
        shootDirection = Input.mousePosition;
        shootDirection.z = 0.0f;
        shootDirection = Camera.main.ScreenToWorldPoint(shootDirection);
        shootDirection = shootDirection - transform.position;
        return shootDirection;
    }

    private void Shoot(Vector3 target) {
        GameObject projectile = Instantiate(projectilePrefab, gameObject.transform.position, Quaternion.identity);  //spawn projectile
        projectile.transform.localScale = size;
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();                                          //find projectile rigidbody
        projectileRb.velocity = new Vector2(shootDirection.x, shootDirection.y).normalized * projectileSpeed;       //fire towards target
    }

    private void die() {
        dead = true;
        Destroy(gameObject);
    }


    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag.Equals("Projectile")) {    //collide with projectile
            if (hp > 0) {
                hp--;
            }
            else {
                die();
            }
            healthBar.transform.localScale = new Vector2(hp, healthBar.transform.localScale.y);
            //Debug.Log("Hit" + hp);
        }
    }
}
