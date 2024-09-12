using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] int damage;
    public AnimationClip animationClip;
    public List<GameObject> damagedEnemies; //counts damaged enemies during the attack cycle so we wouldn't hit them again by walking back and forth 
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.CompareTag("Enemy") && !damagedEnemies.Contains(other.gameObject))
        {
            damagedEnemies.Add(other.gameObject);
            var enemy = other.gameObject.GetComponent<Enemy>();
            enemy.TakeDamage(damage);
        }
    }

    public void AttackCycleEnded()
    {
        damagedEnemies.Clear();
    }
}
