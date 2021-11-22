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
    
    private bool pressed = false;
    
    
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(spawning); //Connect spawning function with button
    }

    
    void FixedUpdate()
    {
    
    //create ray from camera to mouse position
	Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	RaycastHit hit;
	
    //only if ray hits plane and button was pressed
	if (Physics.Raycast(ray, out hit) && pressed)
	{
	
		//Debug.DrawLine(ray.origin, hit.point);
        
        
		//check animal condition
        //Spawn animal depending on what choice was set
        if (choices.captionText.text == "Rabbit")
        {
            Instantiate(Rabbit, hit.point, Quaternion.identity);
        }

        if (choices.captionText.text == "Fox")
        {
            Instantiate(Fox, hit.point, Quaternion.identity);
        }

        if (choices.captionText.text == "Bear")
        {
            Instantiate(Bear, hit.point, Quaternion.identity);
        }
	}
    
    else{
        //change button state back to inactive
        pressed ^= false;
    }
    }
    
    private void spawning()
    {
        Debug.Log("You have clicked the button!");
        Debug.Log(choices.captionText.text); //Get string of active choice from dropdown object
        pressed ^= false;
    
    }

}
