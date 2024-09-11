using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float Health;
    public GameObject[] Inventory;

    public float baseCritChance;
    public float baseMovementSpeed;
    public float baseHealth;
    public float baseDamage;
    public float baseKnockbackChance;
    public float baseWeaponSize;

    private List<ModifierItem> modifiers = new List<ModifierItem>();

    private PlayerModifier playerModifier;

    // Damage, MovementSpeed, Health, Crit, WeaponSize, Knockback

    // Start is called before the first frame update
    void Start()
    {
        playerModifier = GetComponent<PlayerModifier>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Upon picking up a modifier item
    // call PlayerModifier
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Loot Container"))
        {
            string item = other.gameObject.GetComponent<LootChest>().GetLoot();
            if (item != string.Empty)
            {
                //Debug.Log($"Collected {item}");
                GameObject lootPrefab = Resources.Load<GameObject>($"Loot/{item}");

                if (lootPrefab != null)
                {
                    // Destroy the chest and create the item.
                    Destroy(other.gameObject);
                    Instantiate(lootPrefab, transform.position, Quaternion.identity);
                }
            }
        }

        if (other.gameObject.CompareTag("Loot"))
        {
            if (other.gameObject.GetComponent<Item>() != null)
            {
                ModifierItem modifierItem = other.gameObject.GetComponent<Item>().GetModifier();
                Debug.Log(modifierItem);
                playerModifier.ApplyModifier(modifierItem);
                Destroy(other.gameObject);
            }
        }

    }

}
