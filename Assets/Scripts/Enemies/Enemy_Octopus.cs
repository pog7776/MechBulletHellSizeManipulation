using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Octopus : MonoBehaviour
{

private float turn;
private float movementSpeed = 20;
private Rigidbody2D rb;
private float torque = 1;
private float turnTimer = 5;
private float turnTime;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        turn = Random.Range(-10, 10);
    }

    // Update is called once per frame
    void Update()
    {
        turn = Random.Range(-10, 10);
        Debug.Log(turn);

        if (turnTime > 0)
        {
            turnTime--; 
        }
        else{
            BossMove(turn);
            turnTime = turnTimer;
            //BossMove(0);
        }
    }

    private void BossMove(float direction){
        //gameObject.transform.Rotate(direction, 0, 0, Space.Self);
        //rb.AddTorque(transform.up * torque * turn);
        rb.AddTorque(turn*torque);
        rb.AddRelativeForce(transform.up*movementSpeed);
    }
}
