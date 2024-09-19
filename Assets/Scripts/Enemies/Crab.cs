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
    Vector3 forward = Vector3.right;
    Vector3 movementDir = Vector3.zero;
    bool grounded;

    private FMODUnity.StudioEventEmitter attackSoundEmitter;
    void Start()
    {
        Init();
        lastStuckCheckPos = transform.position;
        lastStuckCheckTime = 0;

        attackSoundEmitter = GetComponent<FMODUnity.StudioEventEmitter>();
         if (attackSoundEmitter == null)
        {
            Debug.LogError("StudioEventEmitter component not found on this GameObject.");
        }
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
            if ((target - transform.position).sqrMagnitude <= personalSpace * personalSpace)
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
        attackSoundEmitter.Play();
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

        // grounded = Physics2D.OverlapCircleAll(groundCheck.position, 0.1f, groundLayer).ToList().Any(x => x.gameObject != gameObject);
        // if (!grounded)
        //     return;

        movementDir = target.x < transform.position.x ? -forward : forward;
        transform.Translate(movementDir * movementSpeed * Time.deltaTime);
    }

    public override void Knockback(float force)
    {
        rb.AddForce((-movementDir + Vector3.up) * force, ForceMode2D.Impulse);
        Stun();
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

    void OnCollisionEnter2D(Collision2D other)
    {
        Vector3 normal;
        if (Vector3.Dot(Vector3.up, other.GetContact(0).normal) > 0.3)
            normal = other.GetContact(0).normal;
        else
            normal = Vector3.up;
        forward = Vector3.Cross(normal, Vector3.forward).normalized;
    }
}
