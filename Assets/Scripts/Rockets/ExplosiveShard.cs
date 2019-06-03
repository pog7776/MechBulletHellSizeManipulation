using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveShard : MonoBehaviour
{
    private float lifeTime = 10;
    public float damage = 1;
    public bool justSpawned = true;

    void Start()
    {
        Debug.Log("Boom1");
    }

    void Update()
    {
        StartCoroutine(CheckSpawn());

        if (lifeTime > 0)
        {
            lifeTime -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }

        Debug.Log("Boom2");
    }

    IEnumerator CheckSpawn()
    {
        yield return new WaitForSeconds(2f);
        justSpawned = false;
        Debug.Log("Active");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!justSpawned)
        {
            if (collision.gameObject.tag.Equals("Enemy"))
            {    //collide with projectile
                if (collision.GetComponent<EnemyController>().hp > 0)
                {
                    float hp = collision.GetComponent<EnemyController>().hp;
                    collision.GetComponent<EnemyController>().hp = hp - damage;
                }
                //Debug.Log("Hit Enemy" + collision.GetComponent<EnemyController>().hp);
                Destroy(gameObject);
            }
        }
    }
}
