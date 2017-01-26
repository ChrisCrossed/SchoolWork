using UnityEngine;
using System.Collections;

public class cs_TriggerAudioClip : MonoBehaviour
{
    [SerializeField] int i_Clip;

    bool b_ActivateTriggerSoon;

    [SerializeField] GameObject go_TriggerToActivate;

    float f_Timer;
    void Update()
    {
        if(b_ActivateTriggerSoon)
        {
            f_Timer += Time.deltaTime;

            if (f_Timer > 7.0f)
            {
                if(go_TriggerToActivate != null)
                {
                    if(go_TriggerToActivate.GetComponent<Cs_TriggerCheckpointReset>())
                    {
                        go_TriggerToActivate.GetComponent<Cs_TriggerCheckpointReset>().Set_Active(true);

                    }
                }
            }
        }
    }
    
    void OnTriggerEnter( Collider collider_ )
    {
        if(collider_.gameObject.tag == "Player")
        {
            collider_.gameObject.GetComponent<Cs_SkiingPlayerController>().PlayAudioClip(i_Clip);

            if (go_TriggerToActivate != null)
            {
                if (go_TriggerToActivate.GetComponent<Cs_TriggerCheckpointReset>())
                {
                    b_ActivateTriggerSoon = true;
                }
            }
            else
            {
                GameObject.Destroy(gameObject);
            }
        }
    }
}
