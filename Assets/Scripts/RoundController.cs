using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RoundController : MonoBehaviour
{

    private Rigidbody2D rb;
    private float rotate;
    private GameObject[] enemies;
    private GameObject player;
    private PlayerController pc;

    [SerializeField] private float rotationSpeed = 1;
    [SerializeField] private GameObject enemyPrefab1;
    [SerializeField] private GameObject enemyPrefab2;
    [SerializeField] private GameObject enemyPrefab3;
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private GameObject vorplex;
    [SerializeField] private float roundDelay = 5;
    private float delay;

    [SerializeField] private Text roundNumber;

    [SerializeField]private int waveDifficulty = 5;
    private int spawnLeft = 0;
    private int waveNo = 0;
    [SerializeField]private bool bossRound;
    [SerializeField]private int bossRoundCount = 1;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerController>();
        rb = vorplex.GetComponent<Rigidbody2D>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        delay = roundDelay;
    }

    // Update is called once per frame
    void Update()
    {
        //rb.AddTorque(10);
        vorplex.transform.Rotate(new Vector3(0, 0, rotate-rotationSpeed));

        if(waveDifficulty == 20 * bossRoundCount){
            bossRound = true;
        }
        else{
            bossRound = false;
        }

        if(checkSpawn()){
            pc.roundEnd = true;
            spawnLeft = waveDifficulty;
            delay -= Time.fixedDeltaTime;
            if(delay <= 0){
                if(!bossRound){
                    NewRound();
                }
                else if(bossRound){
                    Boss(1);
                    NormalEnemy(1);
                    BlueEnemy(1);
                    YellowEnemy(1);
                    bossRoundCount++;
                }                
            }
        }
        roundNumber.text = waveNo.ToString();
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

    private void NewRound(){
        while(spawnLeft > 0){
            if(spawnLeft > 0){NormalEnemy(1);}
            if(spawnLeft > 0){BlueEnemy(1);}
            if(spawnLeft > 0){YellowEnemy(1);}
        }

        waveDifficulty += 5;
        waveNo++;
    }

    private void NormalEnemy(int quantity){
        int spawnCount = quantity;
        while(spawnCount > 0){
            GameObject enemy = Instantiate(enemyPrefab1, gameObject.transform.position, Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 359))));  //spawn enemy
            //spawnCount--;
            spawnCount--;
            spawnLeft-= enemy.GetComponent<EnemyController>().threatValue;
        }
    }

    private void BlueEnemy(int quantity){
        int spawnCount = quantity;
        while(spawnCount > 0){
            GameObject enemy = Instantiate(enemyPrefab2, gameObject.transform.position, Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 359))));  //spawn enemy
            //spawnCount--;
            spawnCount--;
            spawnLeft-= enemy.GetComponent<EnemyController>().threatValue;
        }
    }

    private void YellowEnemy(int quantity){
        int spawnCount = quantity;
        while(spawnCount > 0){
            GameObject enemy = Instantiate(enemyPrefab3, gameObject.transform.position, Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 359))));  //spawn enemy
            //spawnCount--;
            spawnCount--;
            spawnLeft-= enemy.GetComponent<EnemyController>().threatValue;
        }
    }

    private void Boss(int quantity){
        int spawnCount = quantity;
        while(spawnCount > 0){
            GameObject enemy = Instantiate(bossPrefab, gameObject.transform.position, Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 359))));  //spawn enemy
            spawnCount--;
            spawnLeft-= enemy.GetComponent<EnemyController>().threatValue;
        }
    }
}
