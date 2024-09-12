using UnityEngine;


public abstract class Enemy : MonoBehaviour
{
    [System.Serializable]
    public struct EnemyType
    {
        public string Name;
        public Enemy EnemyPrefab;
        public int DifficultyCost;
        public bool CanSpawn;
    }

    public enum EnemyState
    {
        wandering, chasing, stunned
    }

    [HideInInspector]
    public EnemyState state;
    public float wanderingSpeed;
    public float chasingSpeed;
    public float wanderingRadius;
    public float health;
    public float damage;
    public float detectionRadius;
    public float personalSpace;
    public float attackCooldown;
    public float stunnedTime;
    public GameObject graphics;
    // [SerializeField] protected float abilityCooldown;

    protected Transform playerTransform;
    protected Rigidbody2D rb;
    protected Vector3 target;
    protected float movementSpeed;
    protected bool atTarget = false;
    protected bool HasTarget { get => target != null; }
    protected bool Chasing { get => state == EnemyState.chasing; }
    protected bool Stunned { get => state == EnemyState.stunned; }
    protected bool Wandering { get => state == EnemyState.wandering; }
    protected float lastAttackTime = 0;
    protected float targetArrivalTime = 0;


    virtual public void Init()
    {
        // StartCoroutine(Process());
        target = PickTarget();
        playerTransform = GameManager.Instance.player.transform;
        rb = GetComponent<Rigidbody2D>();

        movementSpeed = wanderingSpeed;
    }

    // virtual public IEnumerator Process()
    // {
    //     while (true)
    //     {
    //         Move();
    //         yield return null;
    //     }
    // }

    public abstract Vector3 PickTarget();
    // In the FixedUpdate loop
    public abstract void Move();
    public abstract void Attack();
    public abstract void Stun();
    // public abstract void ActivateAbility();
    public virtual void Die()
    {
        Destroy(gameObject);
    }

    // In the Update loop
    public virtual void Loop()
    {
        DetectPlayer();

        if (!HasTarget)
            target = PickTarget();
    }

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) Die();
    }

    public virtual void DetectPlayer()
    {
        if (Stunned)
            return;

        if ((playerTransform.position - transform.position).sqrMagnitude <= detectionRadius * detectionRadius)
        {
            atTarget = false;
            target = playerTransform.position;
            state = EnemyState.chasing;
            movementSpeed = chasingSpeed;
        }
        else
        {
            state = EnemyState.wandering;
            movementSpeed = wanderingSpeed;
        }
    }

    // virtual public IEnumerator AbilityCycle()
    // {
    //     while (true)
    //     {
    //         yield return new WaitForSeconds(abilityCooldown);
    //         ActivateAbility();
    //     }
    // }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (HasTarget)
        {
            Gizmos.color = Chasing ? Color.red : Color.green;
            Gizmos.DrawLine(transform.position, target);
        }
    }
}

