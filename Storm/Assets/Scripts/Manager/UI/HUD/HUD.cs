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
    public TMP_Text item1;
    [Tooltip("Tool")]
    public TMP_Text item2;

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

    public void UpdateItem1Text(string newItem1)
    {
        if (item1 != null) item1.text = newItem1;
    }

    public void UpdateItem2Text(string newItem2)
    {
        if (item2 != null) item2.text = newItem2;
    }
}
