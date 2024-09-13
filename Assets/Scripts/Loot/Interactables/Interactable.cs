// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;

// public class Interactable : MonoBehaviour
// {
//     [Header("Loot Given")]
//     [SerializeField] List<Weapon> weapons = new List<Weapon>();
//     [SerializeField] List<Tool> tools = new List<Tool>();
//     [SerializeField] List<Consumable> consumables = new List<Consumable>();
//     PlayerInventory playerInventory;
//     public void PickUp()
//     {
//         playerInventory = FindObjectOfType<PlayerInventory>();

//         if (weapons.Count != 0)
//         {
//             //Switch the current weapon to the last weapon in the list (in case there's more than one weapon in the list.)
//             playerInventory.SwitchToWeapon(weapons[^1]);
//         }

//         if (tools.Count != 0)
//         {
//             //Switch the current tool to the last tool in the list (in case there's more than one tool in the list.)
//             playerInventory.SwitchToTool(tools[^1]);
//         }

//         if (consumables.Count != 0)
//         {
//             foreach (Consumable consumable in consumables)
//             {
//                 playerInventory.AddToInventory(consumable);
//             }
//         }        
//     }
// }
