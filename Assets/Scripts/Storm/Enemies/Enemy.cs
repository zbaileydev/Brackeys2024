using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class EnemyType
{
    public string Name;
    public Enemy EnemyPrefab;
    public int DifficultyCost;
    public bool CanSpawn = true;
}

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected float movementSpeed;
    [SerializeField] protected float abilityCooldown;
    [SerializeField] protected Rigidbody2D rb; 
    [SerializeField] protected Transform playerTransform;
    Transform target;

    private void Awake() // for testings
    {
        Init();    
    }

    virtual public void Init()
    {
        Activate();
    }

    public void Activate()
    {
        StartCoroutine(Process());
    }

    virtual public IEnumerator Process()
    {
        while(true)
        {
            Move();
            yield return null;
        }
    }
    virtual public void Move()
    {

    }

    virtual public void ActivateAbility()
    {
        
    }

    virtual public IEnumerator AbilityCycle()
    {
        while(true)
        {
            yield return new WaitForSeconds(abilityCooldown);
            ActivateAbility();
        }
    }
}

