using UnityEngine;
using System.Collections;

public class Cs_TriggerAbilityEnable : MonoBehaviour
{
    [SerializeField] Enum_Tutorial e_Tut_;

    void OnTriggerEnter( Collider collider_ )
    {
        if(collider_.gameObject.tag == "Player")
        {
            collider_.gameObject.GetComponent<Cs_SkiingPlayerController>().Set_TutorialState( e_Tut_ );

            GameObject.Destroy(gameObject);
        }
    }
}
