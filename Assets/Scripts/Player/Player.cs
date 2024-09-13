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
    private PlayerInventory playerInventory;
    private bool modLock;
    private bool overWeapon;

    private Weapon weaponHover;

    // Damage, MovementSpeed, Health, Crit, WeaponSize, Knockback

    // Start is called before the first frame update
    void Start()
    {
        playerModifier = GetComponent<PlayerModifier>();
        playerInventory = GetComponent<PlayerInventory>();
        modLock = false;
        overWeapon = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && overWeapon)
        {
            PickUpWeapon();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            playerInventory.weaponAnimation.Play();
        }

        if (Health <= 0)
        {
            // pause game, tell menu manager to turn on the game over screen
            MenuManager.Instance.GameOver();
        }
    }

    void PickUpWeapon()
    {
        /*
        Using the weapon we found from our collision
        switch to it and set its stats to our modifiers.
        Then reset base weapon size and destroy the physical item.
        */
        playerInventory.SwitchToWeapon(weaponHover);
        playerInventory.currentWeapon.UpdateKnockback(baseKnockbackChance);
        playerInventory.currentWeapon.UpdateCrit(baseCritChance);
        baseWeaponSize = 0;
        overWeapon = false;
        Destroy(weaponHover.gameObject);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        /*
        If we have a loot container we open it and instantiate the loot.
        If we have loot we pick it up and update our modifiers.
        If we have a weapon we save it until the player presses E.
        */
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
            if (other.gameObject.GetComponent<Item>() != null && !modLock)
            {
                modLock = true;
                ModifierItem modifierItem = other.gameObject.GetComponent<Item>().GetModifier();
                Debug.Log(modifierItem);
                playerModifier.ApplyModifier(modifierItem);
                Destroy(other.gameObject);
                modLock = false;
            }
        }

        if (other.gameObject.CompareTag("Weapon"))
        {
            overWeapon = true;
            weaponHover = other.gameObject.GetComponent<Weapon>();
        }
    }

    void OnCollisionExit2D(Collision2D other) {
        if (other.gameObject.CompareTag("Weapon"))
        {
            overWeapon = false;
            weaponHover = null;
        }
    }

}
