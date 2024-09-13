using System.Linq;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    bool modLock;
    Player player;

    public Transform containerCheck;
    public float containerCheckRadius;
    public System.Action OnLootEnter;
    public System.Action OnLootExit;
    public LootItem lootUnder;

    void Start()
    {
        player = GetComponent<Player>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

        }
        if (Input.GetMouseButtonDown(1))
        {
            if (GameManager.Instance.worldGenerator.DeleteTileAt(transform.position + Vector3.down))
                FMODUnity.RuntimeManager.PlayOneShot("event:/Objects/sfx_objects_shovel_dig");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (lootUnder != null)
            {
                if (lootUnder.loot is LootTool)
                {
                    GetComponent<PlayerInventory>().SwitchTool(new Tool(lootUnder.loot as LootTool));
                    Debug.Log($"picked up tool: {(lootUnder.loot as LootTool).name}");
                }
                else if (lootUnder.loot is LootWeapon)
                {
                    GetComponent<PlayerInventory>().SwitchWeapon(new Weapon(lootUnder.loot as LootWeapon));
                    Debug.Log($"picked up weapon: {(lootUnder.loot as LootWeapon).name}");
                }
                else if (lootUnder.loot is LootConsumable)
                {
                    GetComponent<PlayerInventory>().AddToInventory(new Consumable(lootUnder.loot as LootConsumable));
                    Debug.Log($"picked up consumable: {(lootUnder.loot as LootConsumable).name}");
                }
                else if (lootUnder.loot is LootModifier)
                {
                    GetComponent<Player>().ApplyModifier((lootUnder.loot as LootModifier).modifier);
                    Debug.Log($"picked up modifier: {(lootUnder.loot as LootModifier).name}");
                }
                else
                {
                    Debug.Log($"Unknown Loot Type: {lootUnder.loot.GetType()}");
                    return;
                }
                Destroy(lootUnder.gameObject);
            }
            else
                CheckContainers();
        }
        //         if (Input.GetMouseButtonDown(0))
        // {
        // }
    }

    public void SetLootUnder(LootItem loot)
    {
        OnLootEnter?.Invoke();
        lootUnder = loot;
    }
    public void ResetLootUnder(LootItem loot)
    {
        if (lootUnder == loot)
        {
            OnLootExit?.Invoke();
            lootUnder = null;
        }
    }

    void CheckContainers()
    {
        GameObject[] lootContainers = Physics2D.OverlapCircleAll(containerCheck.position, containerCheckRadius).ToList().FindAll(x => x.gameObject.CompareTag("Loot_container")).FindAll(x => !x.gameObject.GetComponent<ILootContainer>().Looted).Select(x => x.gameObject).ToArray();

        if (lootContainers.Length == 0)
            return;

        lootContainers[0].GetComponent<ILootContainer>().GetLoot();
        // if (item != null)
        // {
        //     Debug.Log($"Collected {item.Name}");
        //     GameObject lootPrefab = Resources.Load<GameObject>($"Loot/{item.Name}");

        //     if (lootPrefab != null)
        //     {
        //         Instantiate(lootPrefab, lootContainers[0].transform.position, Quaternion.identity);
        //         Destroy(lootContainers[0]);
        //     }
        // }
    }
    // if (other.gameObject.CompareTag("Loot Container"))
    //     {
    //         string item = other.gameObject.GetComponent<LootChest>().GetLoot();
    //         if (item != string.Empty)
    //         {
    //             //Debug.Log($"Collected {item}");
    //             GameObject lootPrefab = Resources.Load<GameObject>($"Loot/{item}");

    //             if (lootPrefab != null)
    //             {
    //                 // Destroy the chest and create the item.
    //                 Destroy(other.gameObject);
    //                 Instantiate(lootPrefab, transform.position, Quaternion.identity);
    //             }
    //         }
    //     }

    // Upon picking up a modifier item
    // call PlayerModifier
    void OnCollisionEnter2D(Collision2D other)
    {
        // if (other.gameObject.CompareTag("Loot"))
        // {
        //     if (other.gameObject.GetComponent<LootItem>().loot.Type == Loot.LootType.Modifier && !modLock)
        //     {
        //         modLock = true;
        //         ModifierItem modifierItem = other.gameObject.GetComponent<LootModifier>().modifier;
        //         Debug.Log(modifierItem);
        //         player.ApplyModifier(modifierItem);
        //         Destroy(other.gameObject);
        //         modLock = false;
        //     }
        // }

    }
}