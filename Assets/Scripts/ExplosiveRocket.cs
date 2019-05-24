using UnityEngine;

public class ExplosiveRocket : MonoBehaviour
{
    public GameObject shardPrefab;

    private float lifeTime = 4;
    private float damage = 1;

    void Update()
    {
        if (lifeTime > 0)
        {
            lifeTime -= Time.deltaTime;
        }
        else
        {
            SpawnShards();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Enemy"))
        {
            float hp = collision.GetComponent<EnemyController>().hp;
            hp -= damage;

            SpawnShards();

            Destroy(gameObject);
        }
    }

    //Spawns the explosive Strapnel
    private void SpawnShards()
    {
        GameObject[] shards = new GameObject[10];
        for (int i = 0; i < shards.Length; i++)
        {
            shards[i] = Instantiate(shardPrefab, gameObject.transform.position, Random.rotation);  //spawn projectile
            shards[i].transform.localScale = gameObject.transform.localScale;
            Rigidbody2D shotgunProjectileRb = shards[i].GetComponent<Rigidbody2D>();
            shotgunProjectileRb.AddForce(transform.forward * 20);
        }
    }
}
