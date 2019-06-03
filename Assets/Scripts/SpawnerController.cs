using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{

    private Rigidbody2D rb;
    private float rotate;
    [SerializeField] private float rotationSpeed = 1;
    private GameObject[] enemies;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject vorplex;

    // Start is called before the first frame update
    void Start()
    {
        rb = vorplex.GetComponent<Rigidbody2D>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    // Update is called once per frame
    void Update()
    {
        //rb.AddTorque(10);
        vorplex.transform.Rotate(new Vector3(0, 0, rotate-rotationSpeed));

        if(checkSpawn()){
            SpawnEnemy(enemyPrefab);
        }
    }

    private bool checkSpawn(){
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if(enemies.Length < 20){
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SpawnEnemy(GameObject enemyType){
        GameObject enemy = Instantiate(enemyType, gameObject.transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));  //spawn enemy
    }
}
