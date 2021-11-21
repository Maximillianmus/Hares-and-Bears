using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnOnPress : MonoBehaviour
{
    public GameObject Rabbit;
    public GameObject Bear;
    public GameObject Fox;
    public Button yourButton;
    public Dropdown choices;
    
    
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(spawning);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void spawning()
    {
        Debug.Log("You have clicked the button!");
        Debug.Log(choices.captionText.text);

        if (choices.captionText.text == "Rabbit")
        {
            Instantiate(Rabbit);
        }

        if (choices.captionText.text == "Fox")
        {
            Instantiate(Fox);
        }

        if (choices.captionText.text == "Bear")
        {
            Instantiate(Bear);
        }
    }

}
