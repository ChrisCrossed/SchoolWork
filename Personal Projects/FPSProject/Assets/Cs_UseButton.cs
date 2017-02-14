using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_UseButton : MonoBehaviour
{
    // Object Connection
    [SerializeField] GameObject ConnectedObject;
    [SerializeField] bool UseOnce;

	// Use this for initialization
	void Start ()
    {
		if(ConnectedObject == new GameObject() || ConnectedObject == null )
        {
            print(gameObject.name + " is missing a Connected Object!");
        }
	}

    bool b_Active;
    public bool ButtonState
    {
        get { return b_Active; }
        set
        {
            if(value == true || !UseOnce )
            {
                b_Active = value;
            }
        }
    }

    public void Use_Button()
    {
        ButtonState = !ButtonState;
    }

	
	// Update is called once per frame
	void Update ()
    {


		if(b_Active)
        {

        }
        else
        {

        }
	}
}
