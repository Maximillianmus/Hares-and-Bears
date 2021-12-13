using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] private GameObject foodPrefab;
    [SerializeField] private Camera arCam;
    private GameObject food = null;
    
    
    public void OnSpawnFood()
    {
        if (food != null) return;
        var hand = ManomotionManager.Instance.Hand_infos[0].hand_info;
        var positionHand = hand.tracking_info.palm_center;
        var spawnPos = ManoUtils.Instance.CalculateNewPosition(positionHand,
            hand.tracking_info.depth_estimation);
        food = Instantiate(foodPrefab, spawnPos, Quaternion.identity);
    }

    public void DespawnFood()
    {
        if (food == null) return;
        
        Destroy(food);
        food = null;
    }

    public void Update()
    {
        if (food == null) return;

        var hand = ManomotionManager.Instance.Hand_infos[0].hand_info;
        var positionHand = hand.tracking_info.palm_center;
        var newPos = ManoUtils.Instance.CalculateNewPosition(positionHand,
            hand.tracking_info.depth_estimation);
        food.transform.position = newPos;
        var bodyPosHand = arCam.transform.position - new Vector3(0, (arCam.transform.position.y - newPos.y), 0);
        var bodyToHand = bodyPosHand - newPos;
        food.transform.rotation = Quaternion.LookRotation(bodyToHand);
        food.transform.Rotate(0, 180, 0);

    }
}
