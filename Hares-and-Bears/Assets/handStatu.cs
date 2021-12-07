using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class handStatu : MonoBehaviour
{
    public Text text;
    // Update is called once per frame
    void Update()
    {
        if (ManomotionManager.Instance == null)
        {
            text.text = "BUG";
            return;
        }

        GestureInfo gest = ManomotionManager.Instance.Hand_infos[0].hand_info.gesture_info;
        if (gest.mano_class == ManoClass.NO_HAND)
        {
            text.text = "No";
        } else if (gest.mano_class == ManoClass.PINCH_GESTURE)
        {
            text.text = "pinch";
        }

        return;

    }
}
