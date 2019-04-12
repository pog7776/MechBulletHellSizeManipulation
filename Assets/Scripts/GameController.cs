using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private GameObject[] enemies;
    private GameObject player;

    private float radius;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        radius = player.GetComponent<PlayerController>().size.x;      //get player size
    }

    // Update is called once per frame
    void Update()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        updateEnemyVision();
    }

    private void updateEnemyVision() {
        radius = player.GetComponent<PlayerController>().size.x;      //get player size

        foreach(GameObject enemy in enemies) {
            enemy.GetComponent<EnemyController>().visionRadius = radius * 10;    //for each enemy in the scene, change it's vision radius according to the players size
        }
    }
}
