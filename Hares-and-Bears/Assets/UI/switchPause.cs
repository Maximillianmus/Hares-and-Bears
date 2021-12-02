using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class switchPause : MonoBehaviour
{
    [SerializeField] private Sprite pause;

    [SerializeField] private Sprite play;

    private bool paused = false;

    public void Switch()
    {
        paused = !paused;
        if (paused)
        {
            gameObject.GetComponent<Image>().sprite = play;
        }
        else
        {
            gameObject.GetComponent<Image>().sprite = pause;
        }
    }
}
