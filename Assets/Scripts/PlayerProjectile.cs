using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    private float lifeTime = 10;
    public float damage = 1;

    //[SerializeField] private float damage = 20;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (lifeTime > 0) {
            lifeTime -= Time.deltaTime;
        }
        else {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag.Equals("Enemy")) {    //collide with projectile
            if (collision.GetComponent<EnemyController>().hp > 0) {
                float hp = collision.GetComponent<EnemyController>().hp;
                collision.GetComponent<EnemyController>().hp = hp - damage;
            }
            //Debug.Log("Hit Enemy" + collision.GetComponent<EnemyController>().hp);
            Destroy(gameObject);
        }

        if (collision.gameObject.tag.Equals("ArenaWalls"))
        {
            Destroy(gameObject);
        }
    }
}
