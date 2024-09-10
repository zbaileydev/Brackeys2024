using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.EditorTools;
using UnityEngine;

public class HUD : MonoBehaviour
{

    [Tooltip("Calm/Storm Timer")]
    public TMP_Text timer;
    [Tooltip("Player Health")]
    public TMP_Text health;
    [Tooltip("Primary weapon")]
    public TMP_Text weapon;
    [Tooltip("Tool")]
    public TMP_Text tool;

    // Converts float to string for the text display.
    public void UpdateTimerText(float newTime)
    {
        if (timer != null) 
        {
            int minutes = Mathf.FloorToInt(newTime / 60f);
            int seconds = Mathf.FloorToInt(newTime % 60f);
            timer.text = string.Format("{0:0}:{1:00}", minutes, seconds);
        }
    }

    public void UpdateHealthText(string newHealth)
    {
        if (health != null) health.text = newHealth;
    }

    // We might want to update an image next to the items.
    public void UpdateWeaponText(string newWeapon)
    {
        if (weapon != null) weapon.text = newWeapon;
    }

    public void UpdateToolText(string newTool)
    {
        if (tool != null) tool.text = newTool;
    }
}
