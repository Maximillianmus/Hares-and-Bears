using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rabbit : Animal
{

    override public void Act(AreaScanResult asr)
    {
        // Check nearby area for food, water, other animals

        // If no predator near
        if (asr.closestPredator == null)
        {
            // Animal wants to eat
            if (hunger <= comfortableHungerLevel * maxHunger && asr.closestFood != null)
            {
                // If close enough, eat!
                if (Vector3.Distance(asr.closestFood.transform.position, transform.position) <= eatRange)
                {
                    Destroy(asr.closestFood);
                    hunger = maxHunger;
                    agent.SetDestination(transform.position);
                }
                // Go to foodsource
                else
                {
                    agent.SetDestination(asr.closestFood.transform.position);
                }

            }
            // Animal wants to drink
            else if (thirst <= comfortableThirstLevel * maxThirst && asr.closestWater != null)
            {
                // If close enough, drink!
                if (Vector3.Distance(asr.closestWater.transform.position, transform.position) <= eatRange)
                {
                    thirst = maxThirst;
                    agent.SetDestination(transform.position);
                }
                // Go to watersource
                else
                {
                    agent.SetDestination(asr.closestWater.transform.position);
                }
            }
            // Look for Mate
            else if (asr.closestMate != null)
            {
                // TODO
            }
            // If not found anything interesting, go explore
            else
            {
                if (!exploring)
                {
                    StartCoroutine("Explore");
                }

            }
        }
        // If predator is near
        else
        {
            // TODO Flee!
        }
    }

}
