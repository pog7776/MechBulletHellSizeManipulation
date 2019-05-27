using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Kino;

public class PlayerController : MonoBehaviour
{

    private GameObject player;
    private Camera cam;
    private AnalogGlitch glitch;

    [Header("Movement Properties")]
    [SerializeField] private float speed;           //speed of player movement
    private float baseSpeed;                        //Original speed of player
    [SerializeField] private float maxFuel;         //Amount of fuel for speed up
    [SerializeField] private float fuel;            //Current fuel amount
    private bool isSpedUp;                          //Check if player is sped up

    [Header("Scale Properties")]
    [SerializeField] private float scaleSpeed = 0.1f;   //speed player changes size
    [SerializeField] private bool minimumSize = false;  //is player the minimum size
    [SerializeField] private bool maximumSize = false;  //is player the minimum size
    [SerializeField] public Vector3 size;               //current size
    [SerializeField] private float timeScale;           //current timeScale

    private Vector3 scaleVector;                        //modifier of scale

    [Header("Combat Properties")]
    [SerializeField] private float hp = 100;
    public GameObject healthBar;
    [SerializeField] private bool dead = false;
    public GameObject deadText;
    private float fireTimer;
    private Vector3 shootDirection;

    [Header("Normal Gun")]
    public GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 20;
    [SerializeField] private float fireRate = 0.1f;

    [Header("ShotGun")]
    public GameObject shotgunPrefab;                     //Art Asset for the shotgun bullets

    [Header("Rocket")]
    public GameObject rocketPrefab;                     //Art Asset for the Rocket
    [SerializeField] private int maxRocketAmmo;        //How many rockets the player is allowed
    [SerializeField] private int rocketAmmo;        //How many rockets the player currently has
    [SerializeField] private float rocketReload;    //How long between reload
    [SerializeField] private float rocketSpeed;     //How fast the rocket is

    [Header("Shield")]
    public GameObject shieldPrefab;                     //Art Asset for the Shield
    [SerializeField] private float shieldDuration;
    private bool shieldUp;                              //Check if shield is active

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        cam = Camera.main;
        glitch = cam.GetComponent<AnalogGlitch>();
        Debug.Log("Camera found: " + cam + cam.name);
        baseSpeed = speed;
        fuel = maxFuel;                 //Fuel Amount
        rocketAmmo = maxRocketAmmo;     //Rocket amount

        scaleVector.Set(scaleSpeed, scaleSpeed, scaleSpeed);
        size = player.transform.localScale;

        fireTimer = 0;
        if(!dead){
            Time.timeScale = 1;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        size = player.transform.localScale;
        PrimaryFire();
        Movement();
        Size();
        Rotation();

        cam.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));    //keep camera upright

        if(Input.GetButtonDown("Reset")){
            ResetScene();
        }

        if(Input.GetButtonDown("Fire2")){
            Blink();
        }


        //Shotgun Mechanic
        /*
        if (Input.GetButton("Fire1"))
        {
            if (fireTimer > 0)
            {
                fireTimer -= Time.deltaTime;
            }
            else
            {
                ShotgunShoot(FindMouse());
                fireTimer = fireRate;
            }
        }*/

        //Rocket Mechanic
        if (Input.GetButtonDown("Rocket"))
        {
            if(rocketAmmo > 0)
            {
                RocketShoot(FindMouse());
                rocketAmmo-= 1;
            }
        }
        //Reloading rocket
        if (rocketAmmo < maxRocketAmmo)
        {
            rocketReload += Time.deltaTime;

            if (rocketReload > 10)
            {
                rocketAmmo += 1;
                rocketReload = 0;
                Debug.Log("Reloaded rocket");
            }
        }

        //Shield Mechanic
        ShieldPower();

        //Speedup Mechanic
        SpeedPower();
        SpedUp();
    }

    private void PrimaryFire(){
         if (Input.GetButton("Fire1")) {
            if (fireTimer > 0) {
                fireTimer -= Time.unscaledDeltaTime;
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
        if(size.x - scaleVector.x <= 0.05) {       //check if player can shrink anymore
            minimumSize = true;
        }
        else {
            minimumSize = false;
        }

        if(size.x + scaleVector.x >= 2) {       //check if player can grow anymore
            maximumSize = true;
        }
        else {
            maximumSize = false;
        }

        if(Input.GetAxisRaw("Mouse ScrollWheel") < 0 && !maximumSize  && !dead) {                       //enlarge player
            player.transform.localScale = size + scaleVector;
            cam.orthographicSize += scaleVector.x * 5;               //modify camera
            changeTime(size.x);                                      //modify timescale
            //speed -= size.x*3;
            speed = baseSpeed-size.x*5;
        }
        else if(Input.GetAxisRaw("Mouse ScrollWheel") > 0 && !minimumSize && !dead) {   //shrink player
            player.transform.localScale = size - scaleVector;

            if (cam.orthographicSize - scaleVector.x * 5 > 0) {     //ensure camera size doesn't become a negative
                cam.orthographicSize -= scaleVector.x * 5;          //modify camera
            }

            changeTime(size.x);                                     //modify timescale
            //speed += size.x*3;
            speed = baseSpeed+size.x*5;
        }
    }

    private void changeTime(float timeShift) {
        if (timeShift > 0.05 && timeShift < 10) {       //maximum time change
            timeScale = timeShift;                      //to monitor current time scale
            Time.timeScale = timeScale;                 //set time scale
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
    }

    private void SpeedPower()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
            isSpedUp = true;

        if (Input.GetKeyUp(KeyCode.LeftShift))
            isSpedUp = false;

        if(isSpedUp)
        {
            fuel -= Time.deltaTime;
            if(fuel < 0)
            {
                fuel = 0;
                isSpedUp = false;
            }
        }
        else if(fuel < maxFuel)
        {
            fuel += Time.deltaTime;
        }
    }

    private void SpedUp()
    {
        if (isSpedUp)
        {
            speed = baseSpeed + size.x * 7;
            cam.GetComponent<Kino.Motion>().enabled = true;
        }
        else
        {
            speed = baseSpeed + size.x;
            cam.GetComponent<Kino.Motion>().enabled = false;
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

    private void Rotation() {
		float angle = Mathf.Atan2(FindMouse().y, FindMouse().x) * Mathf.Rad2Deg;        //rotating the player
        angle-=90;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void Shoot(Vector3 target) {
        GameObject projectile = Instantiate(projectilePrefab, gameObject.transform.position, Quaternion.identity);  //spawn projectile
        projectile.transform.localScale = size;
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();                                          //find projectile rigidbody

        float damage = projectile.GetComponent<PlayerProjectile>().damage;                                          //get damage of projectile
        damage = damage * size.x;                                                                                   //set damage value
        projectile.GetComponent<PlayerProjectile>().damage = damage;                                                //apply damage value

        projectileRb.velocity = new Vector2(shootDirection.x, shootDirection.y).normalized * projectileSpeed;       //fire towards target
    }

    //Shotgun Weapon
    private void ShotgunShoot(Vector3 target)
    {
        GameObject[] shotgunProjectile = new GameObject[3];
        for (int i = 0; i < shotgunProjectile.Length; i++)
        {
            shotgunProjectile[i] = Instantiate(shotgunPrefab, gameObject.transform.position, Quaternion.identity);  //spawn projectile
            shotgunProjectile[i].transform.localScale = size/2;
            Rigidbody2D shotgunProjectileRb = shotgunProjectile[i].GetComponent<Rigidbody2D>();                                          //find projectile rigidbody
            shotgunProjectileRb.velocity = new Vector2(shootDirection.x + i, shootDirection.y).normalized * projectileSpeed;       //fire towards target
        }
    }

    //Normal Rocket Launcher
    private void RocketShoot(Vector3 target)
    {
        GameObject projectile = Instantiate(rocketPrefab, gameObject.transform.position, Quaternion.identity);  //spawn projectile
        projectile.transform.localScale = size;
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        projectileRb.velocity = new Vector2(shootDirection.x, shootDirection.y).normalized * rocketSpeed;       //fire towards target
    }

    private void ShieldPower()
    {
        if (Input.GetButton("Fire2"))
        {
            if (!shieldUp)
            {
                shieldUp = true;
                GameObject shield = Instantiate(shieldPrefab, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);

            }
        }
    }

    private void Blink(){
        Vector3 destination = new Vector3(FindMouse().x, FindMouse().y, 0);
        StartCoroutine(GlitchScreen(0.1f, 0.7f));
        gameObject.transform.position += destination;
    }

    private void die() {
        dead = true;
        //Destroy(gameObject);
        Time.timeScale = 0;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        deadText.SetActive(true);
    }

    private void ResetScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator GlitchScreen(float waitTime, float amount){     //when the player blinks
        glitch.scanLineJitter = amount;   //set screen glitch
        yield return new WaitForSeconds(waitTime);
        glitch.scanLineJitter = 0f;   //reset screen glitch
    }

    private IEnumerator PlayerHit(float waitTime){      //if the player is hit
        glitch.scanLineJitter = 0.3f;   //set screen glitch
        glitch.horizontalShake = 0.3f;
        glitch.colorDrift = 0.3f;
        yield return new WaitForSeconds(waitTime);
        glitch.scanLineJitter = 0f;   //reset screen glitch
        glitch.horizontalShake = 0f;
        glitch.colorDrift = 0f;
    }


    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag.Equals("Projectile")) {    //collide with projectile
            if (hp > 0) {
                hp--;
                StartCoroutine(PlayerHit(0.1f));
            }
            else {
                die();
            }
            healthBar.transform.localScale = new Vector2(hp/200, healthBar.transform.localScale.y);
            //Debug.Log("Hit" + hp);
        }
    }
}
