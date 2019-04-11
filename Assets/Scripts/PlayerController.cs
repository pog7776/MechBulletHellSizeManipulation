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
    [SerializeField] private Vector3 size;           //current size
    [SerializeField] private float timeScale;        //current timeScale

    private Vector3 scaleVector;                     //modifier of scale

    [Header("Combat Properties")]
    public GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 30;
    [SerializeField] private float fireRate = 0.5f;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        cam = Camera.main;
        Debug.Log("Camera found: " + cam + cam.name);

        scaleVector.Set(scaleSpeed, scaleSpeed, scaleSpeed);
        size = player.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        size = player.transform.localScale;
        Movement();
        Size();
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

        if(Input.GetAxisRaw("Mouse ScrollWheel") < 0) {     //enlarge player
            player.transform.localScale = size + scaleVector;

            //cam.transform.position.Set(player.transform.position.x, player.transform.position.y, cam.transform.position.z + scaleVector.z);  //please don't remove this, i want to do something with it  -jack

            cam.orthographicSize += scaleVector.x * 5;

            changeTime(size.x);     //modify timescale
        }
        else if(Input.GetAxisRaw("Mouse ScrollWheel") > 0 && !minimumSize) {    //shrink player
            player.transform.localScale = size - scaleVector;

            if (cam.orthographicSize - scaleVector.x * 5 > 0) {     //ensure camera size doesn't become a negative
                cam.orthographicSize -= scaleVector.x * 5;
            }

            changeTime(size.x);     //modify timescale
        }
    }

    private void changeTime(float timeShift) {
        if (timeShift > 0.05 && timeShift < 10) {       //maximum time change
            timeScale = timeShift;                      //to monitor current time scale
            Time.timeScale = timeScale;                 //set time scale
        }
    }

    private void Shoot(GameObject target) {
        GameObject projectile = Instantiate(projectilePrefab, gameObject.transform.position, Quaternion.identity);  //spawn projectile
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();                                          //find projectile rigidbody
        projectileRb.AddForce((target.transform.position - projectile.transform.position) * projectileSpeed);       //fire towards target
    }
}
