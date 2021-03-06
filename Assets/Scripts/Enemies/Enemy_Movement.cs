﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{

private float turn;
[SerializeField] private float movementSpeed = 20;
private Rigidbody2D rb;
private float torque = 1;
private float turnTimer = 5;
private float turnTime;

private Vector3 thrustDirection;

[HideInInspector] public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = null;
        rb = GetComponent<Rigidbody2D>();
        turn = Random.Range(-10, 10);
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null){

            Vector3 pos = gameObject.transform.position;
            if(pos.x > 40 || pos.x < -40 || pos.y > 40 || pos.y < -40){
                EnemyMove(FindCentre());
            }
            
            turn = Random.Range(-10, 10);
            //Debug.Log(turn);

            if (turnTime > 0)
            {
                turnTime--; 
            }
            else{
                EnemyMoveRandom(turn);
                turnTime = turnTimer;
            }
        }
        else{
            EnemyMove(FindPlayer());
        }
    }

    private void EnemyMove(Vector3 target){
        rb.velocity = new Vector2(target.x, target.y).normalized * movementSpeed/8;
        //Debug.Log(gameObject + "move towards player");

        //rb.AddRelativeForce(transform.up*movementSpeed);
    }

    private void EnemyMoveRandom(float direction){
        //gameObject.transform.Rotate(direction, 0, 0, Space.Self);
        //rb.AddTorque(transform.up * torque * turn);
        rb.AddTorque(turn*torque);
        rb.AddRelativeForce(transform.up*movementSpeed);
    }

    private Vector3 FindPlayer() {
        //...setting thurst direction
        thrustDirection = player.transform.position;
        thrustDirection.z = 0.0f;
        if(Vector3.Distance(player.transform.position, gameObject.transform.position) > 5){
            thrustDirection = thrustDirection - transform.position;
        }
        else{
            thrustDirection = thrustDirection + transform.position;
        }
        
        return thrustDirection;
    }

    private Vector3 FindCentre() {
        //...setting thurst direction
        thrustDirection = new Vector3(0,0,0);
        thrustDirection.z = 0.0f;
        thrustDirection = thrustDirection - transform.position;
    
        return thrustDirection;
    }
}
