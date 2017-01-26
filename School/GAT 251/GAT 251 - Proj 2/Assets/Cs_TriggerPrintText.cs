using UnityEngine;
using System.Collections;

public class Cs_TriggerPrintText : MonoBehaviour
{
    [SerializeField] string s_Text;

    float f_Timer = 30f;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(f_Timer < 30f)
        {
            f_Timer += Time.deltaTime;

            if (f_Timer > 30f) f_Timer = 30.0f;
        }
	}

    void OnTriggerEnter( Collider collider_ )
    {
        if(collider_.gameObject.tag == "Player")
        {
            if(f_Timer == 30.0f)
            {
                collider_.gameObject.GetComponent<Cs_TextHint>().Set_TextHint(s_Text);

                f_Timer = 0.0f;
            }
        }
    }
}
