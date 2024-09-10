using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cycle : MonoBehaviour
{

    public float calmTime = 30f;
    public float stormTime = 90f;

    private bool isCalmPhase = true;
    private float currentTime = 0;
    
    void Start()
    {
        StartCoroutine(CountdownCycle());
    }
    
    IEnumerator CountdownCycle()
    {
        while (true)
        {
            if (isCalmPhase)
            {
                yield return StartCoroutine(Countdown(calmTime));
                isCalmPhase = false;
            }
            else
            {
                yield return StartCoroutine(Countdown(stormTime));
                isCalmPhase = true;
            }
        }
    }

    IEnumerator Countdown(float time)
    {
        currentTime = time;

        while (currentTime > 0)
        {
            yield return new WaitForSeconds(1f);
            currentTime--;
        }
    }

    public float GetTimer()
    {
        return currentTime;
    }

   
}
