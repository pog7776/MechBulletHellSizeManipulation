using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundController : MonoBehaviour
{

    private Rigidbody2D rb;
    private float rotate;
    private GameObject[] enemies;
    private GameObject player;
    private PlayerController pc;

    [SerializeField] private float rotationSpeed = 1;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject vorplex;

    private int waveDifficulty = 5;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerController>();
        rb = vorplex.GetComponent<Rigidbody2D>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    // Update is called once per frame
    void Update()
    {
        //rb.AddTorque(10);
        vorplex.transform.Rotate(new Vector3(0, 0, rotate-rotationSpeed));

        if(checkSpawn()){
            EndRound(enemyPrefab);
        }
    }

    private bool checkSpawn(){
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if(enemies.Length <= 0){
            return true;
        }
        else{
            return false;
        }
    }

    private void EndRound(GameObject enemyType){
        int spawnCount = waveDifficulty;
        while(spawnCount > 0){
            GameObject enemy = Instantiate(enemyType, gameObject.transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));  //spawn enemy
            spawnCount--;
        }
        waveDifficulty += 5;
        pc.roundEnd = true;
    }
}
