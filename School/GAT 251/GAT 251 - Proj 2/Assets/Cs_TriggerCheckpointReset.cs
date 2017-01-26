using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Cs_TriggerCheckpointReset : MonoBehaviour
{
    [SerializeField] int i_CheckpointPos;
    [SerializeField] bool b_Enabled = true;

    public void Set_Active( bool b_IsActive_ )
    {
        b_Enabled = b_IsActive_;
    }
    
    void OnTriggerStay( Collider collider_ )
    {
        if(b_Enabled)
        {
            if(collider_.gameObject.tag == "Player")
            {
                if(i_CheckpointPos == 0)
                {
                    // Reset level (TODO: Fix with fade)
                    SceneManager.LoadScene(2);
                }
                else
                {
                    // Move player to position based on i_CheckpointPos
                    if(i_CheckpointPos == 1)
                    {
                        GameObject go_NewPos = GameObject.Find("Checkpoint_1");

                        collider_.gameObject.GetComponent<Cs_SkiingPlayerController>().Set_ResetVelocity( go_NewPos );
                    }
                    else if(i_CheckpointPos == 2)
                    {
                        GameObject go_NewPos = GameObject.Find("Camera_Render_1");

                        collider_.gameObject.GetComponent<Cs_SkiingPlayerController>().Set_ResetVelocity( go_NewPos );
                    }
                    else if (i_CheckpointPos == 3)
                    {
                        GameObject go_NewPos = GameObject.Find("RespawnPoint_3");

                        collider_.gameObject.GetComponent<Cs_SkiingPlayerController>().Set_ResetVelocity(go_NewPos);
                    }
                }
            }
        }
    }
}
