using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] float inGameTimesteps;

    // How long a timStep is in seconds
    public float timeStepDuration = 0.1f;

    private float[] multipliers = {0.25f, 0.5f, 1.0f, 2.0f, 4.0f, 8.0f, 16.0f};
    private int currentMultiplier = 2;

    public float GetMultiplier()
    {
        return multipliers[currentMultiplier];
    } 

    public bool paused = false;

    public delegate void OnTimeAdvanceHandler();
    public static event OnTimeAdvanceHandler onTimeAdvance;

    float advanceTimer;

    // Start is called before the first frame update
    void Start()
    {
        inGameTimesteps = 0f;
        advanceTimer = timeStepDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if(!paused)
        {
            advanceTimer -= Time.deltaTime * GetMultiplier();

            if(advanceTimer <= 0)
            {
                // Reset timer
                advanceTimer = timeStepDuration;

                // update total inGameTime
                inGameTimesteps += 1;
                

                // Calls all functions that are "bound to onTimeAdvance
                onTimeAdvance?.Invoke();
            }

        }
    }

    public void Next()
    {
        currentMultiplier = Math.Min(currentMultiplier + 1, multipliers.Length - 1);
    }

    public void Previous()
    {
        currentMultiplier = Math.Max(0, currentMultiplier - 1);
    }

    public void SwitchPause()
    {
        paused = !paused;
    }
}