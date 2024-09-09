using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : Enemy
{
    [SerializeField] private float jumpCooldown;
    [SerializeField] protected float jumpPower;
    [SerializeField] Transform groundPoint;
    private float jumpTimer = 0;

    override public void Move()
    {
        if(jumpTimer>0) jumpTimer-=Time.deltaTime;
        Vector2 direction = playerTransform.position - transform.position;
        if(Physics2D.Raycast(groundPoint.position,direction.x > 0 ? Vector2.right : Vector2.left,1,1 << 6))
        {
            Debug.Log("test");
            if(jumpTimer <= 0 && rb.velocity.y == 0)
            {
                Debug.Log("test2");
                Jump();
                jumpTimer = jumpCooldown;
            }
            return;
        }
        rb.velocity = new Vector2(direction.x > 0 ? movementSpeed : -movementSpeed,rb.velocity.y);
    }
    void Jump()
    {
        rb.AddForce(Vector2.up*jumpPower, ForceMode2D.Impulse);
    }
}
