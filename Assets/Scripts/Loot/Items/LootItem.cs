using UnityEngine;

public class LootItem : MonoBehaviour
{
    public Loot.LootType type;
    public Loot loot;

    float spawnTime;
    float timeToStartInteracting = 1;

    public void Init()
    {
        // try
        // {
        //     if ((loot as LootWeapon).name != null)
        //         type = Loot.LootType.Weapon;
        //     Debug.Log(loot.GetType());
        // }
        // catch
        // {
        //     try
        //     {
        //         if ((loot as LootTool).name != null)
        //             type = Loot.LootType.Tool;
        //         Debug.Log(loot.GetType());
        //     }
        //     catch
        //     {
        //         try
        //         {
        //             if ((loot as LootConsumable).name != null)
        //                 type = Loot.LootType.Consumable;
        //             Debug.Log(loot.GetType());
        //         }
        //         catch
        //         {
        //             try
        //             {
        //                 if ((loot as LootModifier).name != null)
        //                     type = Loot.LootType.Modifier;
        //                 Debug.Log(loot.GetType());
        //             }
        //             catch
        //             {
        //                 Debug.Log($"Unknown Loot Type: {loot.GetType()}");
        //                 return;
        //             }
        //         }
        //     }
        // }

        // WHAT THE FUCK YOU CAN DO THAT??!!!!!!
        if (loot is LootWeapon)
            type = Loot.LootType.Weapon;
        else if (loot is LootTool)
            type = Loot.LootType.Tool;
        else if (loot is LootConsumable)
            type = Loot.LootType.Consumable;
        else if (loot is LootModifier)
            type = Loot.LootType.Modifier;
        else
        {
            Debug.Log($"Unknown Loot Type: {loot.GetType()}");
            return;
        }


        switch (type)
        {
            case Loot.LootType.Consumable:
                GetComponent<SpriteRenderer>().sprite = (loot as LootConsumable).sprite;
                break;
            case Loot.LootType.Modifier:
                GetComponent<SpriteRenderer>().sprite = (loot as LootModifier).sprite;
                break;
            case Loot.LootType.Tool:
                GetComponent<SpriteRenderer>().sprite = (loot as LootTool).sprite;
                break;
            case Loot.LootType.Weapon:
                GetComponent<SpriteRenderer>().sprite = (loot as LootWeapon).sprite;
                break;
            default:
                break;
        }

        GetComponent<Rigidbody2D>().AddForce(Vector2.up * 7f, ForceMode2D.Impulse);
        spawnTime = Time.time;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (Time.time - spawnTime < timeToStartInteracting)
            return;

        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerInteractions>().SetLootUnder(this);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerInteractions>().ResetLootUnder(this);
        }
    }
}