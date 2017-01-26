using UnityEngine;
using System.Collections;

public class Cs_Exit : MonoBehaviour
{
    void OnTriggerEnter(Collider collision_)
    {
        if(collision_.gameObject.name == "Capsule")
        {
            GameObject.Find("Player").GetComponent<Cs_FPSController>().FadeToBlack();
        }
    }
}
