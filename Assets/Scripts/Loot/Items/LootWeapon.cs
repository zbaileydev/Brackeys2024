using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Items/Weapon")]
public class LootWeapon : Loot
{
    [System.Serializable]
    public struct WeaponStats
    {
        public int damage;
        public float critChance;
        public float knockbackChance;
    }

    public new LootType Type { get => LootType.Weapon; }
    public new string name = " ";
    public Sprite sprite;
    public WeaponStats stats;
    public AnimationClip attackAnimation;

    // [HideInInspector]
    // public List<GameObject> damagedEnemies; //counts damaged enemies during the attack cycle so we wouldn't hit them again by walking back and forth

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.gameObject.CompareTag("Enemy") && !damagedEnemies.Contains(other.gameObject))
    //     {
    //         damagedEnemies.Add(other.gameObject);
    //         var enemy = other.gameObject.GetComponent<Enemy>();
    //         enemy.TakeDamage(damage);
    //     }
    // }

    // public void AttackCycleEnded()
    // {
    //     damagedEnemies.Clear();
    // }
}
