using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class Cs_GeneratorLogic : MonoBehaviour
{
    bool b_PlayerInCollider;

    bool b_ObjectiveActive = true;

    [SerializeField] GameObject go_Objective;
    GameObject go_XButton;
    float f_ButtonYPos;

    // Controller Input
    GamePadState state;
    GamePadState prevState;
    public PlayerIndex playerOne = PlayerIndex.One;

    AudioSource as_AudioSource;
    AudioClip ac_LevelPull;

    bool b_DoorState = false;

    void Start()
    {
        // Set Audio Source Info
        as_AudioSource = gameObject.GetComponent<AudioSource>();
        ac_LevelPull = Resources.Load("Lever_Pull") as AudioClip;
        as_AudioSource.clip = ac_LevelPull;

        // Set Button Information
        go_XButton = transform.Find("Mdl_Button_X").gameObject;
        f_ButtonYPos = go_XButton.transform.position.y;
    }

    float f_ButtonTimer;
	// Update is called once per frame
	void Update ()
    {
        if(go_Objective.GetComponent<Cs_GateScript>())
        {
            b_DoorState = go_Objective.GetComponent<Cs_GateScript>().Get_DoorOpen();
        }

        // Bounce the X Button Model
        f_ButtonTimer += Time.deltaTime * 3f / 4f;
        Vector3 v3_ButtonPos = go_XButton.transform.position;
        v3_ButtonPos.y = Mathf.Sin(f_ButtonTimer * 2f) + f_ButtonYPos;
        go_XButton.transform.position = v3_ButtonPos;

	    if(b_PlayerInCollider)
        {
            prevState = state;
            state = GamePad.GetState(playerOne);

            if (state.Buttons.X == ButtonState.Pressed && prevState.Buttons.X == ButtonState.Released)
            {
                Set_ObjectiveState(true);

                if (go_Objective != null)
                {
                    if(go_Objective.GetComponent<Cs_GateScript>())
                    {
                        go_Objective.GetComponent<Cs_GateScript>().Set_ObjectiveActive(true);

                        go_Objective.GetComponent<Cs_GateScript>().Set_DoorOpen(true);

                        // Player was caught, turn off the Objective Window setting
                        GameObject.Find("Canvas").GetComponent<Cs_ObjectiveWindow>().Set_DeactivateGenerator = true;

                        as_AudioSource.Play();
                    }
                }
            }

            // Player in collider.
            Material[] mat_ButtonColors = go_XButton.GetComponent<MeshRenderer>().materials;
            for(int i_ = 0; i_ < mat_ButtonColors.Length; ++i_)
            {
                Color clr_CurrAlpha = mat_ButtonColors[i_].color;

                if(!b_DoorState) // If the objective isn't activated, fade in the button
                {
                    clr_CurrAlpha.a += Time.deltaTime;

                    #region Only if the Alpha is 1.0f, then make it opaque. Otherwise, it should still be semi-transparent
                    if (clr_CurrAlpha.a >= 1.0f)
                    {
                        clr_CurrAlpha.a = 1.0f;

                        // Got guide: http://sassybot.com/blog/swapping-rendering-mode-in-unity-5-0/
                        mat_ButtonColors[i_].SetFloat("_Mode", 0); // Sets the material to Opaque
                        mat_ButtonColors[i_].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                        mat_ButtonColors[i_].SetInt("_ZWrite", 1);
                        mat_ButtonColors[i_].DisableKeyword("_ALPHATEST_ON");
                        mat_ButtonColors[i_].DisableKeyword("_ALPHABLEND_ON");
                        mat_ButtonColors[i_].DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        mat_ButtonColors[i_].renderQueue = -1;

                        // Set transparency for metallic objects only if they are already metallic
                        //if (mat_ButtonColors[i_].GetFloat("_Metallic") > 0.01f)
                        //{
                        //    mat_ButtonColors[i_].SetFloat("_Metallic", 0.01f);
                        //}
                    }
                    #endregion

                    if(clr_CurrAlpha.a > 0.01f) go_XButton.GetComponent<MeshRenderer>().enabled = true;

                }
                else // Else, if the objective isn't activated, fade out the button
                {
                    clr_CurrAlpha.a -= Time.deltaTime;

                    if (clr_CurrAlpha.a < 0.01)
                    {
                        clr_CurrAlpha.a = 0.0f;

                        go_XButton.GetComponent<MeshRenderer>().enabled = false;
                    }
                    else
                    {
                        go_XButton.GetComponent<MeshRenderer>().enabled = true;
                    }
                    
                    #region Ensure the material is Transparent
                    // Got guide: http://sassybot.com/blog/swapping-rendering-mode-in-unity-5-0/
                    mat_ButtonColors[i_].SetFloat("_Mode", 3); // Sets the material to Transparent
                    mat_ButtonColors[i_].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat_ButtonColors[i_].SetInt("_ZWrite", 0);
                    mat_ButtonColors[i_].DisableKeyword("_ALPHATEST_ON");
                    mat_ButtonColors[i_].DisableKeyword("_ALPHABLEND_ON");
                    mat_ButtonColors[i_].EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat_ButtonColors[i_].renderQueue = 3000;
                    #endregion

                }

                mat_ButtonColors[i_].color = clr_CurrAlpha;
            }
        }
        else // Player NOT in collider. Fade out the button
        {
            Material[] mat_ButtonColors = go_XButton.GetComponent<MeshRenderer>().materials;
            for (int i_ = 0; i_ < mat_ButtonColors.Length; ++i_)
            {
                Color clr_CurrAlpha = mat_ButtonColors[i_].color;

                clr_CurrAlpha.a -= Time.deltaTime;
                if (clr_CurrAlpha.a < 0.01)
                {
                    clr_CurrAlpha.a = 0.0f;

                    // Since we're at 0f, turn off the model
                    go_XButton.GetComponent<MeshRenderer>().enabled = false;
                }

                mat_ButtonColors[i_].color = clr_CurrAlpha;

                #region Ensure the material is Transparent
                // Got guide: http://sassybot.com/blog/swapping-rendering-mode-in-unity-5-0/
                mat_ButtonColors[i_].SetFloat("_Mode", 3); // Sets the material to Transparent
                mat_ButtonColors[i_].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat_ButtonColors[i_].SetInt("_ZWrite", 0);
                mat_ButtonColors[i_].DisableKeyword("_ALPHATEST_ON");
                mat_ButtonColors[i_].DisableKeyword("_ALPHABLEND_ON");
                mat_ButtonColors[i_].EnableKeyword("_ALPHAPREMULTIPLY_ON");
                mat_ButtonColors[i_].renderQueue = 3000;
                #endregion
            }
        }
    }

    public void Set_ObjectiveState( bool b_ObjectiveActive_)
    {
        b_ObjectiveActive = b_ObjectiveActive_;
    }

    bool Get_ObjectiveState()
    {
        return b_ObjectiveActive;
    }

    void OnTriggerEnter( Collider collider_ )
    {
        if(collider_.transform.root.gameObject.name == "Player")
        {
            // Raycast to the player model
            RaycastHit hit;
            int i_LayerMask = LayerMask.GetMask("Player", "Wall");
            Vector3 v3_Vector = collider_.transform.root.gameObject.transform.position - gameObject.transform.position;

            Debug.DrawRay(gameObject.transform.position, v3_Vector, Color.red, 5.0f);

            if (Physics.Raycast(gameObject.transform.position, v3_Vector, out hit, float.PositiveInfinity, i_LayerMask))
            {
                print("Touching: " + hit.collider.name);

                if( hit.collider.gameObject.layer == LayerMask.NameToLayer("Player") )
                {
                    b_PlayerInCollider = true;
                }
            }
        }
    }

    void OnTriggerExit(Collider collider_)
    {
        if (collider_.transform.root.gameObject.name == "Player")
        {
            b_PlayerInCollider = false;
        }
    }
}
