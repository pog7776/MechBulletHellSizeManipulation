using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    private GameObject projectile;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag.Equals("Projectile")){
            projectile = other.gameObject;
            //projectile.GetComponent<Rigidbody2D>().velocity = new Vector2(shootDirection.x, shootDirection.y).normalized * projectileSpeed;
            //to do- get velocity from projectile and change the vector 2 to a new direction away from the player
            //if you want i can do this later - jack
        }
    }
}
