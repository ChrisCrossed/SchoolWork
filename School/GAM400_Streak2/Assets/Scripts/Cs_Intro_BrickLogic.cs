using UnityEngine;
using System.Collections;

/*********************************
 * 
 * Copyright DigiPen Institute of Technology 2016
 * 
 * Streak 2 by Christopher Christensen
 * 
 * *******************************/

public class Cs_Intro_BrickLogic : MonoBehaviour
{
    [SerializeField] bool b_IsMainMenu;
    [SerializeField] [Range(0, 3)] int i_BlockNumber;
    [SerializeField] AudioClip ac_SFX;
    float f_TimeToFinalPosition = .3f;

    float f_yPos_Final;
    float f_yPos_Start;
    float f_DelayBasedOnNumber = 1.0f;
    float f_PositionTimer;

    AudioSource as_AudioSource;

    [SerializeField] float f_BlockFadeTime = 1.0f;
    float f_MaxSceneTime; // References how long the whole scene runs
    float f_BlockFadeTimer; // Always increments on Delta Time. Evaluates when to begin & end fading.
    float f_BlockFadeTimer_Start; // Blocks all begin fading out at this point
    float f_BlockFadeTimer_End; // Blocks all end fading out at this point

    // Use this for initialization
    void Start ()
    {
        // Set the Audio Source
        if(transform.root.GetComponent<AudioSource>())
        {
            as_AudioSource = transform.root.GetComponent<AudioSource>();
        }
        Resources.Load("SFX_BlockDrop");

        #region Set the initial y Positions
        // Store current yPosition to lerp to
        f_yPos_Final = gameObject.transform.position.y;

        // Move object up above the screen
        Vector3 v3_HighPosition = gameObject.transform.position;
        v3_HighPosition.y += 750f;

        // Store the y starting position for lerp information
        f_yPos_Start = v3_HighPosition.y;

        // Set the initial starting position
        gameObject.transform.position = v3_HighPosition;

        #endregion

        #region Set delay based on which block this is
        if (i_BlockNumber == 1)
        {
            f_DelayBasedOnNumber += 0.3f;
        }
        else if (i_BlockNumber == 2)
        {
            f_DelayBasedOnNumber += 0.3f + 0.1f;
        }
        else if (i_BlockNumber == 3)
        {
            f_DelayBasedOnNumber += 0.3f + 0.1f + 0.3f;
        }

        if(b_IsMainMenu)
        {
            f_DelayBasedOnNumber /= 4;
        }
        #endregion

        #region Set time for when the blocks begin and end fading
        if(GameObject.Find("Canvas").GetComponent<Cs_IntroScreenLogic>())
        {
            f_MaxSceneTime = GameObject.Find("Canvas").GetComponent<Cs_IntroScreenLogic>().Get_SceneMaxTime();
            f_BlockFadeTimer_End = f_MaxSceneTime - 0.5f;
            f_BlockFadeTimer_Start = f_BlockFadeTimer_End - f_BlockFadeTime;
        }
        #endregion

        if(b_IsMainMenu)
        {
            f_TimeToFinalPosition = 2.0f;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        // Increment block fade time
        f_BlockFadeTimer += Time.deltaTime;

        #region Block positioning based on timer
        // While the block is delayed, don't move it. Count down the timer.
        if (f_DelayBasedOnNumber > 0)
        {
            f_DelayBasedOnNumber -= Time.deltaTime;
        }
        // Otherwise, the block begins to move to it's intended position
        else
        {
            // Continues to run only while the timer is less than the overall time
            if(f_PositionTimer < f_TimeToFinalPosition)
            {
                // Increase the timer for lerping
                f_PositionTimer += Time.deltaTime;
                
                Vector3 v3_NewPosition = gameObject.transform.position;

                if(f_PositionTimer < f_TimeToFinalPosition)
                {
                    float f_yPos_New = Mathf.Lerp(f_yPos_Start, f_yPos_Final, f_PositionTimer / f_TimeToFinalPosition);

                    v3_NewPosition.y = f_yPos_New;
                }
                else
                {
                    float f_yPos_New = f_yPos_Final;

                    v3_NewPosition.y = f_yPos_New;

                    // Play SFX
                    if(ac_SFX)
                    {
                        if(transform.root.GetComponent<AudioSource>())
                        {
                            as_AudioSource.PlayOneShot(ac_SFX);
                        }
                    }
                }

                gameObject.transform.position = v3_NewPosition;
            }
        }
        #endregion

        #region Evaluate block fading
        if(f_BlockFadeTimer > f_BlockFadeTimer_Start && f_BlockFadeTimer <= f_BlockFadeTimer_End)
        {
            // Runs from 100% to 0%
            float f_Perc = (f_BlockFadeTimer_End - f_BlockFadeTimer) / (f_BlockFadeTimer_End - f_BlockFadeTimer_Start);

            if (f_Perc > 1.0f) f_Perc = 1.0f;
            else if (f_Perc < 0.0f) f_Perc = 0.0f;

            if(f_Perc > 0.1f)
            {
                SetMaterialsVisibility(f_Perc);
            }
            else
            {
                gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
        }
        #endregion
    }

    void SetMaterialsVisibility(float f_Transparency_)
    {
        Material[] mat_CurrColor = gameObject.GetComponent<MeshRenderer>().materials;

        for (int i_ = 0; i_ < mat_CurrColor.Length; ++i_)
        {
            Color currColor = mat_CurrColor[i_].color;

            if (f_Transparency_ == 1.0f)
            {
                // print("Making Opaque: " + f_Transparency_);
                // mat_CurrColor[i_].SetFloat("_Mode", 1.0f);
                // Material mat = GetComponentInChildren<MeshRenderer>().materials[i_];
                Material mat = mat_CurrColor[i_];

                // Got guide: http://sassybot.com/blog/swapping-rendering-mode-in-unity-5-0/
                mat.SetFloat("_Mode", 0); // Sets the material to Opaque
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                mat.SetInt("_ZWrite", 1);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.DisableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = -1;

                // Set transparency for metallic objects only if they are already metallic
                //if (mat.GetFloat("_Metallic") > 0f)
                //{
                //    mat.SetFloat("_Metallic", f_Transparency_);
                //}

                gameObject.GetComponent<MeshRenderer>().materials[i_] = mat;
            }
            else
            {
                // print("Making Transparent: " + currColor.a);
                // mat_CurrColor[i_].SetFloat("_Mode", 4.0f);
                // Material mat = GetComponentInChildren<MeshRenderer>().materials[i_];
                Material mat = mat_CurrColor[i_];

                // Got guide: http://sassybot.com/blog/swapping-rendering-mode-in-unity-5-0/
                mat.SetFloat("_Mode", 3); // Sets the material to Transparent
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.DisableKeyword("_ALPHABLEND_ON");
                mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;

                // Set transparency for metallic objects only if they are already metallic
                //if (mat.GetFloat("_Metallic") > 0f)
                //{
                //    mat.SetFloat("_Metallic", f_Transparency_);
                //}

                gameObject.GetComponent<MeshRenderer>().materials[i_] = mat;
            }

            // GetComponent<MeshRenderer>().met
            currColor.a = f_Transparency_;
            mat_CurrColor[i_].color = currColor;
        }
    }
}
