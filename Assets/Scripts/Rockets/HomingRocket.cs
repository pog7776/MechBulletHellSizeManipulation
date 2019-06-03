using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingRocket : MonoBehaviour
{
    private Transform target;
    private Rigidbody2D rb;
    private Collider2D rocketCollider;
    private Collider2D detectionCollider;
    private Vector3 thrustDirection;

    private bool isHoming = false;

    [SerializeField] private float lifeTime = 10;
    [SerializeField] private float damage = 8;
    [SerializeField] private float speed;
    [SerializeField] private float rotateSpeed;


    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        detectionCollider = gameObject.GetComponent<CircleCollider2D>();
        rocketCollider = gameObject.GetComponent<CapsuleCollider2D>();
        rocketCollider.enabled = false;
    }

    void FixedUpdate()
    {
        //RocketFollow(isHoming);
        RocketFollowAlt(isHoming);
    }

    void Update()
    {
        if (lifeTime > 0){
            lifeTime -= Time.deltaTime;
        }
        else{
            Destroy(gameObject);
        }

        if(target){
            FindEnemyDirection();
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, Rotation()));
            if(Vector3.Distance(gameObject.transform.position, target.transform.position) < 0.5f){
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag.Equals("Enemy") && !rocketCollider.isActiveAndEnabled)
        {
            target = collision.gameObject.transform;
            FindEnemyDirection();
            isHoming = true;
            rocketCollider.enabled = true;
            detectionCollider.enabled = false;
        }

        if (collision.gameObject.tag.Equals("Enemy") && rocketCollider.isActiveAndEnabled)
        {
            float hp = collision.GetComponent<EnemyController>().hp;
            hp -= damage;

            //Destroy(gameObject);
        }
    }

    private void RocketFollow(bool isActive)
    {
        if (isActive)
        {
            Vector2 direction = (Vector2)target.position - rb.position;
            direction.Normalize();
            float rotateAmount = Vector3.Cross(direction, transform.up).z;
            rb.angularVelocity = -rotateAmount * rotateSpeed;
            rb.velocity = transform.up * speed;
        }
    }

    private void RocketFollowAlt(bool isActive)
    {
        if (isActive)
        {
            rb.velocity = new Vector2(thrustDirection.x, thrustDirection.y).normalized * speed;
        }
    }

    private Vector3 FindEnemyDirection() {
        //...setting thurst direction
        if(target != null){
            thrustDirection = target.position;
            thrustDirection.z = 0.0f;
            thrustDirection = thrustDirection - transform.position;
        }
        return thrustDirection;
    }

    private float Rotation() {
		float angle = Mathf.Atan2(target.transform.position.y, target.transform.position.x) * Mathf.Rad2Deg;        //rotating the rocket
        angle-=90;
        return angle;
    }
}
