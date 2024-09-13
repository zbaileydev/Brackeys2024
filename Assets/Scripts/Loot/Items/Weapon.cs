using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] float critChance;
     [SerializeField] float critMultiplier;
    [SerializeField] float knockbackChance;
    public AnimationClip animationClip;
    public List<GameObject> damagedEnemies; //counts damaged enemies during the attack cycle so we wouldn't hit them again by walking back and forth 
    
    private void Start()
    {
        critChance = 0;
        knockbackChance = 0;
        critMultiplier = 2;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.CompareTag("Enemy") && !damagedEnemies.Contains(other.gameObject))
        {
            Debug.Log("Enemy hit!");
            damagedEnemies.Add(other.gameObject);
            var enemy = other.gameObject.GetComponent<Enemy>();

            bool critHit = Random.Range(1,100) < critChance;
            if (critHit) 
            {
                enemy.TakeDamage((int)(damage * critMultiplier));
            } 
            else
            {
                enemy.TakeDamage(damage);
            }
            AttackCycleEnded();
            
        }
    }

    public void AttackCycleEnded()
    {
        damagedEnemies.Clear();
    }

    public void UpdateKnockback(float newChance)
    {
        knockbackChance = newChance;
    }

    public void UpdateCrit(float newChance)
    {
        critChance = newChance;
    }
    
    
    public void UpdateSize(float baseWeaponSize)
    {
        transform.localScale = new Vector3(baseWeaponSize, baseWeaponSize, baseWeaponSize);
    }
}
