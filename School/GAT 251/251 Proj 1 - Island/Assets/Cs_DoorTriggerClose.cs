using UnityEngine;
using System.Collections;

public class Cs_DoorTriggerClose : MonoBehaviour
{
    [SerializeField]
    GameObject go_DoorToClose;

    void OnTriggerEnter( Collider collider_ )
    {
        if(collider_.gameObject.tag == "Player")
        {
            if(go_DoorToClose != null)
            {
                go_DoorToClose.GetComponent<Cs_Door>().CloseDoor();

                gameObject.SetActive(false);
            }
            else
            {
                print("No Door Close Trigger to Call!");
            }
        }
    }
}
