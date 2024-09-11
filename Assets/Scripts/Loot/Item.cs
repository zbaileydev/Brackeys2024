using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    
    public ModifierType modifierType;
    public float modifierValue;

    private ModifierItem item;

    void Start() {
        item = new ModifierItem(modifierType, modifierValue);
    }

    public ModifierItem GetModifier()
    {
        Debug.Log("getting item.");
        Debug.Log(item);
        return item;
    }
    
}
