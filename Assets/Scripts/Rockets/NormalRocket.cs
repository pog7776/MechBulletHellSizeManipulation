using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalRocket : MonoBehaviour
{
    [SerializeField] private float lifeTime = 4;
    [SerializeField] private float damage = 10;

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
        if (collision.gameObject.tag.Equals("Enemy"))
        {
            float hp = collision.GetComponent<EnemyController>().hp;
            hp -= damage;

            Destroy(gameObject);
        }
    }
}
