using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    private GameObject projectile;
    private Vector3 thrustDirection;
    private GameObject owner;
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
            projectile.tag = "PlayerProjectile";
            //projectile.GetComponent<Rigidbody2D>().velocity = new Vector2(shootDirection.x, shootDirection.y).normalized * projectileSpeed;
            //to do- get velocity from projectile and change the vector 2 to a new direction away from the player
            //if you want i can do this later - jack
            ProjectileController controller = projectile.GetComponent<ProjectileController>();
            owner = controller.owner;
            FindEnemyDirection(owner);
            float projectileSpeed = controller.speed;
            Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
            projectileRb.velocity = new Vector2(thrustDirection.x, thrustDirection.y).normalized * projectileSpeed;
        }
    }

    private Vector3 FindEnemyDirection(GameObject target) {
        //...setting thurst direction
        if(projectile != null){
            thrustDirection = target.transform.position;
            thrustDirection.z = 0.0f;
            thrustDirection = thrustDirection - projectile.transform.position;
        }
        return thrustDirection;
    }
}
