using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crab : Enemy
{
    private int currentXDirection;
    public override void Init()
    {
        base.Init();
        currentXDirection = transform.position.x < playerTransform.position.x ? 1 : -1;
    }
    override public void Move()
    {
        rb.velocity = new Vector2(currentXDirection*movementSpeed,rb.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        //Debug.Log("test1");
        //Debug.Log(other.gameObject.layer);
        if(other.gameObject.layer == 6)
        {
            Debug.Log("test2");
            currentXDirection *= -1;
        }
    }
}