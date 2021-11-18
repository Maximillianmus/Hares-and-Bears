using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] float inGameTimesteps;

    // How long a timStep is in seconds
    public float timeStepDuration = 0.1f;
    
    public float fastForwardMultiplier = 5f;

    public bool paused = false;
    public bool fastForward = false;

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
            if (fastForward)
                advanceTimer -= Time.deltaTime * fastForwardMultiplier;
            else
                advanceTimer -= Time.deltaTime;

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
}