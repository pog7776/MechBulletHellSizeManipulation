using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Kino;
using System;

public class PlayerController : MonoBehaviour
{

    private GameObject player;

    [Header("Movement Properties")]
    private Camera cam;
    private AnalogGlitch glitch;
    private Datamosh artifact;

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
    [SerializeField] private bool god = false;
    [SerializeField] private float maxHp = 100;
    [SerializeField] private float hp;
    public GameObject healthBar;
    [SerializeField] private GameObject HPMask;
    private float HPMaskRotation = 80;
    [SerializeField] private GameObject AbilityMask;
    private float AbilityMaskRotation = -80;
    public bool roundEnd;

    [SerializeField] private bool dead = false;
    [SerializeField] private GameObject deadText;
    private float fireTimer;
    private Vector3 shootDirection;

    [Header("Normal Gun")]
    public GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 20;
    [SerializeField] private float fireRate = 0.1f;

    [Header("ShotGun")]
    public GameObject shotgunPrefab;                     //Art Asset for the shotgun bullets

    [Header("Rocket")]
    public GameObject rocketPrefab;                     //Normal Rocket Asset
    public GameObject explosiveRocketPrefab;            //Explosive Rocket Asset
    public GameObject homingRocketPrefab;               //Homing Rocket Asset
    private Transform target;                           //Target for homing missle
    [SerializeField] private int maxRocketAmmo;         //How many rockets the player is allowed
    [SerializeField] private int rocketAmmo;            //How many rockets the player currently has
    [SerializeField] private float rocketReloading;     //How long between reload
    [SerializeField] private float rocketReloadTime;    //Reload time
    [SerializeField] private float rocketSpeed = 7;     //How fast the rocket is

    [Header("Abilities")]
    private GameObject abilitySelection;

    [Header("Blink")]
    [SerializeField] private bool blinkChosen;
    [SerializeField] private GameObject hologramPrefab;
    [SerializeField] private float blinkDelay = 0;//0.2f;
    [SerializeField] private GameObject trail;
    [SerializeField] private int blinkChargeMax = 10;
    [SerializeField] private int blinkCharges;
    private float blinkTimer = 2;
    private float blinkTime = 0;
    private bool isRecharging;

    [Header("Shield")]
    [SerializeField] private GameObject shieldObject;    //Art Asset for the Shield
    [SerializeField] private float shieldDuration;
    [SerializeField] private float shieldCharge;
    [SerializeField] private float shieldDrainRate = 0.5f;
    [SerializeField] private float shieldChargeRate = 0.5f;
    private bool shieldUp;                              //Check if shield is active
    [SerializeField] private bool shieldChosen;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        cam = Camera.main;
        glitch = cam.GetComponent<AnalogGlitch>();
        artifact = cam.GetComponent<Datamosh>();

        hp = maxHp;
        healthBar.transform.localScale = new Vector2((hp/maxHp), healthBar.transform.localScale.y);

        //which ability was slected
        abilitySelection = GameObject.FindGameObjectWithTag("AbilitySelection");
        shieldChosen = abilitySelection.GetComponent<AbilitySelection>().shieldSelected;
        blinkChosen = abilitySelection.GetComponent<AbilitySelection>().blinkSelected;
        //Destroy(abilitySelection);

        HPMask.transform.localRotation = new Quaternion(0, 0, HPMaskRotation, 0);        //set Resource bars to full
        AbilityMask.transform.localRotation = new Quaternion(0, 0, AbilityMaskRotation, 0);
        
        Debug.Log("Camera found: " + cam + cam.name);
        baseSpeed = speed;
        fuel = maxFuel;                 //Fuel Amount
        rocketAmmo = maxRocketAmmo;     //Rocket amount
        shieldCharge = shieldDuration;  //shield charge
        blinkCharges = blinkChargeMax;  //set blink charges
        //blinkTime = blinkTimer;

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
        if(!dead){
            PrimaryFire();
            Movement();
            Size();
            //Rotation();
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, Rotation()));

            if(blinkChosen){
                Blink();
            }

            if(shieldChosen){
                //Shield Mechanic
                ShieldPower();
            }

            //Speedup Mechanic
            SpeedPower();
            SpedUp();
        }

        //  HP/Ability Bars----------------------

        HPMaskRotation = ((hp/maxHp)*100);  //set rotation angle
            HPMask.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, HPMaskRotation));   //rotate mask

        if(shieldChosen){
            AbilityMaskRotation = ((shieldCharge/shieldDuration)*100);  //set rotation angle
        }
        else if(blinkChosen){
            AbilityMaskRotation = (((float)blinkCharges/(float)blinkChargeMax)*100);  //set rotation angle
        }
        
            AbilityMask.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -AbilityMaskRotation-5));   //rotate mask
            Debug.Log(AbilityMaskRotation);
        //---------------------------------------

        //what to do between rounds -------------
        if(roundEnd){       
            hp = maxHp;
            roundEnd = false;
        }
        // --------------------------------------

        cam.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));    //keep camera upright

        if(Input.GetButtonDown("Reset")){
            ResetScene();
        }

        if(Input.GetButtonDown("Cancel")){
            Pause();
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
                //NormalRocket(FindMouse());
                //ExplosiveRocket(FindMouse());
                HomingRocket(FindMouse());
                rocketAmmo -= 1;
            }
        }
        //Reloading rocket
        if (rocketAmmo < maxRocketAmmo)
        {
            rocketReloading += Time.deltaTime;

            if (rocketReloading > rocketReloadTime)
            {
                rocketAmmo += 1;
                rocketReloading = 0;
                Debug.Log("Reloaded rocket");
            }
        }

    }

    private void Pause()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
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

    private float Rotation() {
		float angle = Mathf.Atan2(FindMouse().y, FindMouse().x) * Mathf.Rad2Deg;        //rotating the player
        angle-=90;
        return angle;
        //transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void Shoot(Vector3 target) {
        GameObject projectile = Instantiate(projectilePrefab, gameObject.transform.position, Quaternion.Euler(new Vector3(0, 0, Rotation())));  //spawn projectile angled towards mouse
        projectile.transform.localScale = new Vector3(size.x - 0.35f, size.x, size.z);
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();                                          //find projectile rigidbody

        float damage = projectile.GetComponent<PlayerProjectile>().damage;                                          //get damage of projectile
        damage = damage * size.x;                                                                                   //set damage value
        projectile.GetComponent<PlayerProjectile>().damage = damage;                                                //apply damage value

        projectileRb.velocity = new Vector2(target.x, target.y).normalized * projectileSpeed;       //fire towards target
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
            shotgunProjectileRb.velocity = new Vector2(target.x + i, target.y).normalized * projectileSpeed;       //fire towards target
        }
    }

    //Normal Rocket Launcher
    private void NormalRocket(Vector3 target)
    {
        GameObject projectile = Instantiate(rocketPrefab, gameObject.transform.position, Quaternion.identity);  //spawn projectile
        projectile.transform.localScale = size;
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        projectileRb.velocity = new Vector2(target.x, target.y).normalized * rocketSpeed;       //fire towards target
    }

    //Explosive Rocket
    private void ExplosiveRocket(Vector3 target)
    {
        GameObject projectile = Instantiate(explosiveRocketPrefab, gameObject.transform.position, Quaternion.identity);  //spawn projectile
        projectile.transform.localScale = size;
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        projectileRb.velocity = new Vector2(target.x, target.y).normalized * rocketSpeed;       //fire towards target
    }

    //Homing Rocket
    private void HomingRocket(Vector3 target)
    {
        GameObject projectile = Instantiate(homingRocketPrefab, gameObject.transform.position, Quaternion.Euler(new Vector3(0, 0, Rotation())));  //spawn projectile
        projectile.transform.localScale = size*3;
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        projectileRb.velocity = new Vector2(target.x, target.y).normalized * rocketSpeed;       //fire towards target
    }

    private void ShieldPower()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            if (!shieldUp && shieldCharge > 0)
            {
                shieldUp = true;
                //GameObject shield = Instantiate(shieldPrefab, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
                shieldObject.SetActive(true);
            }
        }
        if(Input.GetButtonUp("Fire2")){
            if (shieldUp)
            {
                shieldUp = false;
                //GameObject shield = Instantiate(shieldPrefab, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
                shieldObject.SetActive(false);
            }
        }

        if(!shieldUp && shieldCharge < shieldDuration){
            shieldCharge += shieldChargeRate;
        }

        if(shieldUp && shieldCharge > 0){
            shieldCharge -= shieldDrainRate;
        }
        
        if(shieldCharge <= 0){
            shieldUp = false;
            shieldObject.SetActive(false);
        }
    }

    private void Blink(){
        if(Input.GetButtonDown("Fire2") && blinkCharges > 0){
            Vector3 destination = new Vector3(FindMouse().x, FindMouse().y, 0);
            StartCoroutine(GlitchScreen(0.1f, 0.7f));
            StartCoroutine(BlinkDelay(blinkDelay, destination));
            blinkCharges--;
            blinkTime = blinkTimer;
        }

        //blink timer
        if(blinkTime > 0){
            blinkTime -= Time.fixedDeltaTime;
        }
        else if(!isRecharging){
            isRecharging = true;
            StartCoroutine(BlinkRecharge());
        }

    }

    private void die() {
        dead = true;
        //Destroy(gameObject);
        deadText.SetActive(true);
        Time.timeScale = 0;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        //StartCoroutine(Dead(0.5f));
    }

    private void ResetScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator Dead(float waitTime){       //when the player dies
        //artifact.Glitch();
        yield return new WaitForSeconds(0);
        Time.timeScale = 0;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;

    }

    private IEnumerator GlitchScreen(float waitTime, float amount){     //when the player blinks
        //glitch.scanLineJitter = amount;   //set screen glitch
        glitch.colorDrift = amount;         //set colour crazy
        yield return new WaitForSeconds(waitTime);
        //glitch.scanLineJitter = 0f;   //reset screen glitch
        glitch.colorDrift = 0f;       //reset colour crazy
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

    private IEnumerator BlinkDelay(float waitTime, Vector3 destination){      //PlayerBlink Power
        GameObject hologram = Instantiate(hologramPrefab, gameObject.transform.position, Quaternion.Euler(new Vector3(0, 0, Rotation())));  //spawn Hologram
        //TrailRenderer trail = hologram.transform.GetChild(0).gameObject.GetComponent<TrailRenderer>();
        //trail.widthCurve = new Vector2(size.x, size.x);
        hologram.transform.localScale = new Vector3(size.x/2, size.y/2, size.z/2);
        Vector3 previousLocation = gameObject.transform.position;

        yield return new WaitForSecondsRealtime(0.001f);
        hologram.transform.position = gameObject.transform.position + destination;

        yield return new WaitForSecondsRealtime(waitTime);
        //trail.SetActive(true);
        gameObject.transform.position = previousLocation + destination;

        yield return new WaitForSecondsRealtime(1f);
        //trail.SetActive(false);
        Destroy(hologram);
    }

    private IEnumerator BlinkRecharge(){      //PlayerBlink Recharge
        if(blinkCharges < blinkChargeMax && blinkTime <= 0){
            blinkCharges++;
            yield return new WaitForSecondsRealtime(0.5f);
            StartCoroutine(BlinkRecharge());
        }
        else{
            isRecharging = false;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag.Equals("Projectile") && !god) {    //collide with projectile
            if (hp > 0) {
                hp--;
                StartCoroutine(PlayerHit(0.1f));
            }
            else {
                die();
            }
            //healthBar.transform.localScale = new Vector2((hp/maxHp), healthBar.transform.localScale.y);
            //Debug.Log("Hit" + hp);
        }
    }
}
