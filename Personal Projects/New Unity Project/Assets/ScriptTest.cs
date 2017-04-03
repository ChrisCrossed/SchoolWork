using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    int i_ = 0;
    bool RecursiveNumber()
    {
        print(i_);

        ++i_;

        if (i_ >= 100) return true;
        else
        {
            RecursiveNumber();
        }

        return false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(RecursiveNumber())
        {
            print("Ended");
        }
	}
}
