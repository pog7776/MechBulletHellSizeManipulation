using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingRocket : MonoBehaviour
{
    private Transform target;
    private Rigidbody2D rb;
    private Collider2D rocketCollider;
    private Collider2D detectionCollider;

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
        RocketFollow(isHoming);
    }

    void Update()
    {
        if (lifeTime > 0)
        {
            lifeTime -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag.Equals("Enemy") && !rocketCollider.isActiveAndEnabled)
        {
            target = collision.gameObject.transform;
            isHoming = true;
            rocketCollider.enabled = true;
            detectionCollider.enabled = false;
        }

        if (collision.gameObject.tag.Equals("Enemy") && rocketCollider.isActiveAndEnabled)
        {
            float hp = collision.GetComponent<EnemyController>().hp;
            hp -= damage;

            Destroy(gameObject);
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
}
