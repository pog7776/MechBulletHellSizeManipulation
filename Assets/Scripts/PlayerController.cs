using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{

    private GameObject player;
    private Camera cam;

    [Header("Movement Properties")]
    [SerializeField] private float speed = 15;           //speed of player movement
    [SerializeField] private float doubleSpeed;    //speed up for speedup mechanic
    private float maxFuel = 40;         //How much fuel we want to give the speedup
    [SerializeField] private float fuel;           //Usable Resource for SpeedUp Mechanic
    private bool isSpedUp = false;
    private float baseSpeed;

    [Header("Scale Properties")]
    [SerializeField] private float scaleSpeed = 0.1f;   //speed player changes size
    [SerializeField] private bool minimumSize = false;  //is player the minimum size
    [SerializeField] private bool maximumSize = false;  //is player the minimum size
    [SerializeField] public Vector3 size;               //current size
    [SerializeField] private float timeScale;           //current timeScale

    private Vector3 scaleVector;                        //modifier of scale

    [Header("Combat Properties")]
    public GameObject projectilePrefab;
    //public GameObject projectile2Prefab;
    [SerializeField] private float projectileSpeed = 20;
    [SerializeField] private float fireRate = 0.1f;
    //[SerializeField] private float rocketRate = 0.1f;
    [SerializeField] private float hp = 100;
    public GameObject healthBar;
    [SerializeField] private bool dead = false;
    public GameObject deadText;

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
        if(!dead){
            Time.timeScale = 1;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }

        doubleSpeed = speed * 2;        //for speed up mechanic to get variable speed
        fuel = maxFuel;                 //Giving fuel for player
        baseSpeed = speed;              //Record original speed
    }

    // Update is called once per frame
    void Update()
    {
        size = player.transform.localScale;
        Movement();
        Size();

        //transform.LookAt(FindMouse());

        //changeTime(size.x); 

        if (Input.GetButton("Fire1")) {
            if (fireTimer > 0) {
                fireTimer -= Time.deltaTime;
            }
            else {
                Shoot(FindMouse());
                fireTimer = fireRate;
            }
        }

        //Shoot shotgun type gun
        /*
        if (Input.GetButton("Fire2"))
        {
            if (fireTimer > 0)
            {
                fireTimer -= Time.deltaTime;
            }
            else
            {
                Shoot(FindMouse());
                fireTimer = fireRate;
            }
        }*/

        if (Input.GetButtonDown("Reset")){
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        SpeedUp();

        if (speed < 0) {
            speed = 1;
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
        if(size.x - scaleVector.x <= 0.05) {       //check if player can shrink anymore
            minimumSize = true;
        }
        else {
            minimumSize = false;
        }

        if(size.x + scaleVector.x >= 2) {       //check if player can shrink anymore
            maximumSize = true;
        }
        else {
            maximumSize = false;
        }

        if(Input.GetAxisRaw("Mouse ScrollWheel") < 0 && !maximumSize  && !dead) {                       //enlarge player
            player.transform.localScale = size + scaleVector;
            cam.orthographicSize += scaleVector.x * 5;               //modify camera
            changeTime(size.x);                                      //modify timescale
            speed -= size.x*3;
        }
        else if(Input.GetAxisRaw("Mouse ScrollWheel") > 0 && !minimumSize && !dead) {   //shrink player
            player.transform.localScale = size - scaleVector;

            if (cam.orthographicSize - scaleVector.x * 5 > 0) {     //ensure camera size doesn't become a negative
                cam.orthographicSize -= scaleVector.x * 5;          //modify camera
            }

            changeTime(size.x);                                     //modify timescale
            speed += size.x*3;
        }
    }

    private void changeTime(float timeShift) {
        if (timeShift > 0.05 && timeShift < 10) {       //maximum time change
            timeScale = timeShift;                      //to monitor current time scale
            Time.timeScale = timeScale;                 //set time scale
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
    }

    private void SpeedUp()                              //Speed up Mechanic
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            SetSpeed(true);
            Debug.Log("Shift Down");
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            SetSpeed(false);
            Debug.Log("Shift Up");
        }
        if (isSpedUp)
        {
            fuel -= Time.deltaTime;
            if (fuel < 0)
            {
                fuel = 0;
                SetSpeed(false);
            }
            else if (fuel < maxFuel)
            {
                fuel += Time.deltaTime;
            }
        }
    }

    private void SetSpeed(bool isSpedUp)                          //Check if player is speeding up
    {
        if (isSpedUp) {
            speed = doubleSpeed;
        }
        else if (!isSpedUp)
        {
            speed = baseSpeed;
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


    /*Shotgun
    private void Shoot3(Vector3 target)
    {
        GameObject projectile = Instantiate(projectilePrefab, gameObject.transform.position, Quaternion.identity);  //spawn projectile
        projectile.transform.localScale = size;
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();                                          //find projectile rigidbody
        projectileRb.velocity = new Vector2(shootDirection.x, shootDirection.y).normalized * projectileSpeed;       //fire towards target

        GameObject projectile2 = Instantiate(projectilePrefab, gameObject.transform.position, Quaternion.identity);  //spawn projectile
        projectile.transform.localScale = size;
        Rigidbody2D projectile2Rb = projectile.GetComponent<Rigidbody2D>();                                          //find projectile rigidbody
        projectileRb.velocity = new Vector2(shootDirection.x - 3, shootDirection.y).normalized * projectileSpeed;       //fire towards target

        GameObject projectile3 = Instantiate(projectilePrefab, gameObject.transform.position, Quaternion.identity);  //spawn projectile
        projectile.transform.localScale = size;
        Rigidbody2D projectile3Rb = projectile.GetComponent<Rigidbody2D>();                                          //find projectile rigidbody
        projectileRb.velocity = new Vector2(shootDirection.x + 3, shootDirection.y).normalized * projectileSpeed;       //fire towards target
    }*/

    private void die() {
        dead = true;
        //Destroy(gameObject);
        Time.timeScale = 0;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        deadText.SetActive(true);
    }


    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag.Equals("Projectile")) {    //collide with projectile
            if (hp > 0) {
                hp--;
            }
            else {
                die();
            }
            healthBar.transform.localScale = new Vector2(hp/200, healthBar.transform.localScale.y);
            //Debug.Log("Hit" + hp);
        }
    }
}
