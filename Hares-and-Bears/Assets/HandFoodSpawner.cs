using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class HandFoodSpawner : MonoBehaviour
{
    [SerializeField] private GameObject handPrefab;
    [SerializeField] private Camera arCam;
    private GameObject hand = null;
    private GameObject food = null;

    public GameObject Hand => hand;

    public GameObject Food => food;


    public void OnSpawnHand()
    {
        if (this.hand != null) return;
        var hand = ManomotionManager.Instance.Hand_infos[0].hand_info;
        var positionHand = hand.tracking_info.palm_center;
        var spawnPos = ManoUtils.Instance.CalculateNewPosition(positionHand,
            hand.tracking_info.depth_estimation);
        this.hand = Instantiate(handPrefab, spawnPos, Quaternion.identity);
        food = GameObject.FindGameObjectWithTag("HandFood");
    }

    public void DespawnHand()
    {
        if (hand == null) return;
        
        Destroy(hand);
        hand = null;
        food = null;
    }

    public void EatFood()
    {
        food = null;
    }

    public void Update()
    {
        if (this.hand == null) return;

        var hand = ManomotionManager.Instance.Hand_infos[0].hand_info;
        var positionHand = hand.tracking_info.palm_center;
        var newPos = ManoUtils.Instance.CalculateNewPosition(positionHand,
            hand.tracking_info.depth_estimation);
        this.hand.transform.position = newPos;
        var bodyPosHand = arCam.transform.position - new Vector3(0, (arCam.transform.position.y - newPos.y), 0);
        var bodyToHand = bodyPosHand - newPos;
        this.hand.transform.rotation = Quaternion.LookRotation(bodyToHand);
        this.hand.transform.Rotate(0, 180, 0);

    }
}
