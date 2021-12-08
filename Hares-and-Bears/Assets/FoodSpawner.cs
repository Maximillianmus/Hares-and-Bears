using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] private GameObject foodPrefab;
    [SerializeField] private Camera arCam;
    private GameObject food = null;
    
    public void OnSpawnFood()
    {
        if (food != null) return;
        var positionHand = ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info.palm_center;
        var spawnPos = ManoUtils.Instance.CalculateNewPosition(positionHand,
            ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info.depth_estimation);
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
        var positionHand = ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info.palm_center;
        var newPos = ManoUtils.Instance.CalculateNewPosition(positionHand,
            ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info.depth_estimation);
        food.transform.position = newPos;

    }
}
