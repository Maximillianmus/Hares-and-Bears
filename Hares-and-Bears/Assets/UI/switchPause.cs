using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class switchPause : MonoBehaviour
{
    [SerializeField] private Sprite pause;

    [SerializeField] private Sprite play;
    [SerializeField] private Image pauseMulti;
    [SerializeField] private Text textMulti;

    private bool paused = false;

    public void Switch()
    {
        paused = !paused;
        pauseMulti.enabled = paused;
        textMulti.enabled = !paused;
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
