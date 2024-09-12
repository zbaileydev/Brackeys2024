using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Crab : Enemy
{
    public Transform groundCheck;
    public LayerMask groundLayer;

    Vector3 lastStuckCheckPos;
    float lastStuckCheckTime;
    int stuck;
    Vector3 movementDir = Vector3.zero;
    bool grounded;

    void Start()
    {
        Init();
        lastStuckCheckPos = transform.position;
        lastStuckCheckTime = 0;
    }

    void Update()
    {
        Loop();
    }

    public override void Loop()
    {
        DetectPlayer();

        if (Wandering)
        {
            if (!atTarget && (Mathf.Abs(target.x - transform.position.x) <= 0.1f || stuck >= 3)) // ||GameManager.Instance.groundTilemap.GetTile(GameManager.Instance.groundTilemap.WorldToCell(transform.position + movementDir)) != null)
            {
                stuck = 0;
                atTarget = true;
                StartCoroutine(ArrivedAtTarget());
            }
        }
        else if (Chasing)
        {
            if (Mathf.Abs(target.x - transform.position.x) <= personalSpace)
            {
                atTarget = true;
                if (Time.time - lastAttackTime >= attackCooldown)
                {
                    Attack();
                    lastAttackTime = Time.time;
                }
            }
            else atTarget = false;
        }

        CheckStuck();
    }

    void CheckStuck()
    {
        if (Time.time - lastStuckCheckTime < 1.5f || atTarget)
            return;

        if ((lastStuckCheckPos - transform.position).sqrMagnitude <= 0.1)
        {
            stuck++;
            if (stuck < 3)
                rb.AddForce(Vector2.up * 40f, ForceMode2D.Impulse);
        }
        else
        {
            stuck = 0;
            lastStuckCheckPos = transform.position;
        }
        lastStuckCheckTime = Time.time;
    }

    IEnumerator ArrivedAtTarget()
    {
        yield return new WaitForSeconds(2);
        if (!Chasing)
        {
            target = PickTarget();
            atTarget = false;
        }
    }

    public override void Attack()
    {
        lastAttackTime = Time.time;
        Debug.Log("Attacked player!");
    }

    public override Vector3 PickTarget()
    {
        float randomPos = Random.Range(-wanderingRadius, wanderingRadius);

        return new Vector3(transform.position.x + randomPos, 0, 0);
    }

    void FixedUpdate()
    {
        Move();
    }

    public override void Move()
    {
        // animations here
        if (Stunned)
            return;
        if (atTarget)
            return;

        grounded = Physics2D.OverlapCircleAll(groundCheck.position, 0.1f, groundLayer).ToList().Any(x => x.gameObject != gameObject);
        // if (!grounded)
        //     return;

        movementDir = target.x < transform.position.x ? Vector3.left : Vector3.right;
        transform.Translate(movementDir * movementSpeed * Time.deltaTime);
    }

    public override void Stun()
    {
        state = EnemyState.stunned;
        Debug.Log("Stunned!", gameObject);
        StartCoroutine(WaitForStun());
    }

    IEnumerator WaitForStun()
    {
        yield return new WaitForSeconds(stunnedTime);
        state = EnemyState.wandering;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //Debug.Log("test1");
        //Debug.Log(other.gameObject.layer);
        // if (other.gameObject.layer == 6)
        // {
        //     Debug.Log("test2");
        //     currentXDirection *= -1;
        // }
    }

}