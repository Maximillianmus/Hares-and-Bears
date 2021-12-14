using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class handStatus : MonoBehaviour
{
    [SerializeField] private UnityEvent onBeginPalmOpenHand;
    [SerializeField] private UnityEvent onEndPalmOpenHand;

    private bool palmOpenHand = false;
    
    
    void Update()
    {
        if (ManomotionManager.Instance == null)
        {
            Debug.LogError("ManoMotionManager is not in the scene.");
            return;
        }

        GestureInfo gest = ManomotionManager.Instance.Hand_infos[0].hand_info.gesture_info;
        if (gest.mano_gesture_continuous == ManoGestureContinuous.OPEN_HAND_GESTURE && gest.hand_side == HandSide.Palmside)
        {
            if (!palmOpenHand)
            {
                onBeginPalmOpenHand.Invoke();
                palmOpenHand = true;
            }
        }
        else
        {
            if (palmOpenHand)
            {
                palmOpenHand = false;
                onEndPalmOpenHand.Invoke();
            }
        }

        return;

    }
}
