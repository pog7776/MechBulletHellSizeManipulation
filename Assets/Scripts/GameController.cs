using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour
{
    private GameObject[] enemies;
    private GameObject player;
    private GameObject ui;
    private PlayerController pc;

    private float playerSize;
    private float radius;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        player = GameObject.FindGameObjectWithTag("Player");
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        ui = GameObject.FindGameObjectWithTag("UI");
        
        pc = player.GetComponent<PlayerController>();

        playerSize = pc.size.x;    //get player size
        radius = playerSize;                                            //get player size for radius size
    }

    // Update is called once per frame
    void Update()
    {
        playerSize = pc.size.x;    //get player size
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        updateEnemyVision();
    }

    private void updateEnemyVision() {
        radius = playerSize;      //get player size

        foreach(GameObject enemy in enemies) {
            //enemy.GetComponent<CircleCollider2D>().radius;        //use later to set starting radius of each enemy and apply player size to it
            
            if(radius < 1.75) {
                enemy.GetComponent<EnemyController>().visionRadius = 5.75f;                      //minimum vision radius
            }
            else {
                enemy.GetComponent<EnemyController>().visionRadius = Mathf.Exp(radius);          //for each enemy in the scene, change its vision according to the players size exponentially 
            }
        }
    }
}
