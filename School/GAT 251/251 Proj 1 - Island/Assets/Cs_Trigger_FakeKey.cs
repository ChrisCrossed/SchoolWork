using UnityEngine;
using System.Collections;

public class Cs_Trigger_FakeKey : MonoBehaviour
{
    [SerializeField]
    GameObject go_FakeKey;

    void OnTriggerEnter(Collider collider_)
    {
        if (collider_.gameObject.tag == "Player")
        {
            if(go_FakeKey != null)
            {
                go_FakeKey.GetComponent<Cs_KeyLogic>().DestroyKey();
            }

            Destroy(gameObject);
        }
    }
}
