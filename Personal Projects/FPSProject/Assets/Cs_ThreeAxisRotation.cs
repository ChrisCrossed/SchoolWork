using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_ThreeAxisRotation : MonoBehaviour
{
    [SerializeField] bool RemoveFromParent = false;
    float f_TurnRate = 50f;

    Vector3 v3_Right = Vector3.right;
    Vector3 v3_Up = Vector3.up;

    Random rand;

    private void Start()
    {
        if (RemoveFromParent)
        {
            transform.SetParent(null);

            // Messing with values so it isn't standard
            v3_Right = Vector3.down;
            v3_Up = Vector3.forward;

            f_TurnRate /= 2f;
        }

        transform.rotation = Random.rotation;
    }
    
    // Update is called once per frame
    void Update ()
    {
        transform.Rotate(v3_Right, Time.deltaTime * f_TurnRate);
        transform.Rotate(v3_Up, Time.deltaTime * f_TurnRate);    }
}
