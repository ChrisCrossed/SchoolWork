using UnityEngine;
using System.Collections;

public class Cs_LimoTrigger : MonoBehaviour
{
    bool b_MeshRendererOn;
    GameObject go_Player;
    Vector3 v3_PlayerPosition;
    Vector3 v3_LimoPosition;

    bool b_IsDone;

	// Use this for initialization
	void Start ()
    {
        Color clr_MeshColor = gameObject.GetComponent<MeshRenderer>().material.color;
        clr_MeshColor.a = 0.0f;
        gameObject.GetComponent<MeshRenderer>().material.color = clr_MeshColor;

        go_Player = GameObject.Find("Player");

        // Disable trigger for now
        gameObject.GetComponent<BoxCollider>().enabled = false;
	}

    // Update is called once per frame
    float f_PositionalLerpTimer;
	void Update ()
    {
        if(!b_IsDone)
        {
            #region Lerp Mesh Renderer
            Color clr_MeshColor = gameObject.GetComponent<MeshRenderer>().material.color;

            if (b_MeshRendererOn)
            {
                // Increase the color's alpha
                if (clr_MeshColor.a < 0.75f)
                {
                    clr_MeshColor.a += Time.deltaTime;

                    if (clr_MeshColor.a > 0.75f)
                    {
                        clr_MeshColor.a = 0.75f;

                        gameObject.GetComponent<BoxCollider>().enabled = true;
                    }
                }
            }
            else
            {
                if (clr_MeshColor.a > 0f)
                {
                    clr_MeshColor.a -= Time.deltaTime;

                    if (clr_MeshColor.a < 0.05f)
                    {
                        clr_MeshColor.a = 0.0f;
                    }
                }
            }
        
            gameObject.GetComponent<MeshRenderer>().material.color = clr_MeshColor;

            if(clr_MeshColor.a > 0f)
            {
                gameObject.GetComponent<MeshRenderer>().enabled = true;
            }
            else
            {
                gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
            #endregion

            if(go_Player.GetComponent<Cs_PlayerController>().GameOver)
            {
                f_PositionalLerpTimer += Time.deltaTime;

                // Turn off the trigger mesh renderer
                Set_MeshRenderer = false;

                if (f_PositionalLerpTimer > 1.0f)
                {
                    f_PositionalLerpTimer = 1.0f;

                    // Turn off the player's model
                    go_Player.transform.Find("Model").transform.Find("Capsule").GetComponent<MeshRenderer>().enabled = false;
                    go_Player.GetComponent<LineRenderer>().enabled = false;

                    // Detatch camera from player so it remains stationary
                    GameObject go_CameraChild = go_Player.transform.Find("Camera_Player").gameObject;
                    go_CameraChild.transform.SetParent(GameObject.Find("Road").transform);

                    // Tell Limo to move out of screen
                    transform.root.GetComponent<Cs_LimoLogic>().Set_LimoGoal();

                    // Fade to black
                    go_Player.GetComponent<Cs_PlayerController>().Set_FadeState( true );

                    b_IsDone = true;
                }

                go_Player.transform.position = Vector3.Lerp(v3_PlayerPosition, v3_LimoPosition, f_PositionalLerpTimer);
            }
        }
    }

    public bool Set_MeshRenderer
    {
        set
        {
            b_MeshRendererOn = value;
        }
    }

    void OnTriggerEnter( Collider collider_ )
    {
        GameObject go_Collider = collider_.transform.root.gameObject;

        if( go_Collider.tag == "Player" )
        {
            // Tell player controller to enter limo
            go_Collider.GetComponent<Cs_PlayerController>().GameOver = true;
            v3_PlayerPosition = go_Collider.transform.position;

            // Store the X position of the Limo, and the Y/Z of the collider
            v3_LimoPosition = GameObject.Find("Limo_Center").transform.position;
        }
    }
}
