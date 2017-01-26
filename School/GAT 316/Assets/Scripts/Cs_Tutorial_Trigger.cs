using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Cs_Tutorial_Trigger : MonoBehaviour
{
    [SerializeField] GameObject go_Image_Visual;
    [SerializeField] GameObject go_Model_FadeInOut;
    [SerializeField] GameObject go_Model_FadeOutOnly;
    [SerializeField] float f_TimeUntilFadeOut = 5.0f;

    float f_Timer;
    bool b_Deactivated;

	// Use this for initialization
	void Start ()
    {
        // Start invisible
	    if(go_Image_Visual)
        {
            if(go_Image_Visual.GetComponent<Image>())
            {
                Color clr_CurrAlpha = go_Image_Visual.GetComponent<Image>().color;
                clr_CurrAlpha.a = 0.0f;
                go_Image_Visual.GetComponent<Image>().color = clr_CurrAlpha;
            }
            else
            {
                print("Tutorial Trigger " + gameObject.name + " has an improper Image Visual");
            }
        }

        // Start invisible
        if(go_Model_FadeInOut)
        {
            if (go_Model_FadeInOut.GetComponent<MeshRenderer>())
            {
                Material[] mat_CurrAlpha = go_Model_FadeInOut.GetComponent<MeshRenderer>().materials;
                for (int i_ = 0; i_ < mat_CurrAlpha.Length; ++i_)
                {
                    // Declare material as invisible
                    // Got guide: http://sassybot.com/blog/swapping-rendering-mode-in-unity-5-0/
                    mat_CurrAlpha[i_].SetFloat("_Mode", 3); // Sets the material to Transparent
                    mat_CurrAlpha[i_].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat_CurrAlpha[i_].SetInt("_ZWrite", 0);
                    mat_CurrAlpha[i_].DisableKeyword("_ALPHATEST_ON");
                    mat_CurrAlpha[i_].DisableKeyword("_ALPHABLEND_ON");
                    mat_CurrAlpha[i_].EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat_CurrAlpha[i_].renderQueue = 3000;

                    Color clr_CurrAlpha = mat_CurrAlpha[i_].color;
                    clr_CurrAlpha.a = 0f;
                    mat_CurrAlpha[i_].color = clr_CurrAlpha;
                }

                // Turn off the Mesh Renderer for now
                go_Model_FadeInOut.GetComponent<MeshRenderer>().enabled = false;
            }
            else
            {
                print("Tutorial Trigger " + gameObject.name + " has an improper Model Fade In");
            }
        }

        // Start at full transparency
        if(go_Model_FadeOutOnly)
        {
            if(go_Model_FadeOutOnly.GetComponent<MeshRenderer>())
            {
                Material[] mat_CurrAlpha = go_Model_FadeOutOnly.GetComponent<MeshRenderer>().materials;
                for(int i_ = 0; i_ < mat_CurrAlpha.Length; ++i_)
                {
                    // Declare material as opaque
                    // Got guide: http://sassybot.com/blog/swapping-rendering-mode-in-unity-5-0/
                    mat_CurrAlpha[i_].SetFloat("_Mode", 0); // Sets the material to Opaque
                    mat_CurrAlpha[i_].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    mat_CurrAlpha[i_].SetInt("_ZWrite", 1);
                    mat_CurrAlpha[i_].DisableKeyword("_ALPHATEST_ON");
                    mat_CurrAlpha[i_].DisableKeyword("_ALPHABLEND_ON");
                    mat_CurrAlpha[i_].DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat_CurrAlpha[i_].renderQueue = -1;

                    Color clr_CurrAlpha = mat_CurrAlpha[i_].color;
                    clr_CurrAlpha.a = 1f;
                    mat_CurrAlpha[i_].color = clr_CurrAlpha;
                }
            }
            else
            {
                print("Tutorial Trigger " + gameObject.name + " has an improper Model Fade Out");
            }
        }
	}

    // Update is called once per frame
    float f_LerpTimer;
	void Update ()
    {
        // Activation check. Ensures the script is not run unless allowed to
	    if(f_Timer > 0f && !b_Deactivated)
        {
            f_Timer += Time.deltaTime;

            #region Increase/Decrease Lerp Timer
            // If there is any object to fade in, increment or decrement the timer
            if(go_Image_Visual || go_Model_FadeInOut || go_Model_FadeOutOnly)
            {
                // While under the threshhold, increment the timer
                if(f_Timer < f_TimeUntilFadeOut)
                {
                    f_LerpTimer += Time.deltaTime;

                    // Cap
                    if (f_LerpTimer > 1.0f) f_LerpTimer = 1.0f;
                }
                // While above the threshhold, decrement the timer
                else
                {
                    f_LerpTimer -= Time.deltaTime;

                    // Cap
                    if ( f_LerpTimer < 0.0f ) f_LerpTimer = 0.0f;
                }
            }
            #endregion

            #region Image Visual Fade In/Out
            // Lerp in the Visual & FadeInOut objects while under the threshhold
            if (go_Image_Visual)
            {
                if (go_Image_Visual.GetComponent<Image>())
                {
                    Color clr_CurrAlpha = go_Image_Visual.GetComponent<Image>().color;
                    clr_CurrAlpha.a = f_LerpTimer;
                    go_Image_Visual.GetComponent<Image>().color = clr_CurrAlpha;
                }
            }
            #endregion

            #region Model 'Fade In/Out'
            if (go_Model_FadeInOut)
            {
                if(go_Model_FadeInOut.GetComponent<MeshRenderer>())
                {
                    // Search through all of the materials in the game object. Set to fade in or out.
                    Material[] mat_CurrAlpha = go_Model_FadeInOut.GetComponent<MeshRenderer>().materials;
                    for(int i_ = 0; i_ < mat_CurrAlpha.Length; ++i_)
                    {
                        Color clr_CurrAlpha = mat_CurrAlpha[i_].color;
                        clr_CurrAlpha.a = f_LerpTimer;

                        #region Opaque vs. Transparent
                        // Declare material as Opaque
                        if (clr_CurrAlpha.a == 1.0f)
                        {
                            // Got guide: http://sassybot.com/blog/swapping-rendering-mode-in-unity-5-0/
                            mat_CurrAlpha[i_].SetFloat("_Mode", 3); // Sets the material to Transparent
                            mat_CurrAlpha[i_].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                            mat_CurrAlpha[i_].SetInt("_ZWrite", 0);
                            mat_CurrAlpha[i_].DisableKeyword("_ALPHATEST_ON");
                            mat_CurrAlpha[i_].DisableKeyword("_ALPHABLEND_ON");
                            mat_CurrAlpha[i_].EnableKeyword("_ALPHAPREMULTIPLY_ON");
                            mat_CurrAlpha[i_].renderQueue = 3000;
                        }
                        // Declare material as Transparent
                        else
                        {
                            // Declare material as invisible
                            // Got guide: http://sassybot.com/blog/swapping-rendering-mode-in-unity-5-0/
                            mat_CurrAlpha[i_].SetFloat("_Mode", 3); // Sets the material to Transparent
                            mat_CurrAlpha[i_].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                            mat_CurrAlpha[i_].SetInt("_ZWrite", 0);
                            mat_CurrAlpha[i_].DisableKeyword("_ALPHATEST_ON");
                            mat_CurrAlpha[i_].DisableKeyword("_ALPHABLEND_ON");
                            mat_CurrAlpha[i_].EnableKeyword("_ALPHAPREMULTIPLY_ON");
                            mat_CurrAlpha[i_].renderQueue = 3000;
                        }
                        #endregion

                        // If we're fading out && the lerp timer is nearly 0f...
                        if(f_Timer > f_TimeUntilFadeOut && f_LerpTimer < 0.05f)
                        {
                            // ... Turn off the Mesh Renderer
                            go_Model_FadeInOut.GetComponent<MeshRenderer>().enabled = false;
                        }

                        mat_CurrAlpha[i_].color = clr_CurrAlpha;
                    }

                    #region Enable or Disable the Mesh Renderer when appropriate
                    if (f_LerpTimer > 0.05f)
                    {
                        go_Model_FadeInOut.GetComponent<MeshRenderer>().enabled = true;
                    }
                    else
                    {
                        go_Model_FadeInOut.GetComponent<MeshRenderer>().enabled = false;
                    }
                    #endregion
                }
            }
            #endregion

            #region Mode 'Fade Out Only'
            // Only fade out the 'Fade Out Only' model while above the threshhold
            if (f_Timer > f_TimeUntilFadeOut)
            {
                if(go_Model_FadeOutOnly)
                {
                    // Search through the 'Fade Out Only' model and fade out the model
                    if(go_Model_FadeOutOnly.GetComponent<MeshRenderer>())
                    {
                        Material[] mat_CurrMat = go_Model_FadeOutOnly.GetComponent<MeshRenderer>().materials;
                        for(int i_ = 0; i_ < mat_CurrMat.Length; ++i_)
                        {
                            Color clr_CurrAlpha = mat_CurrMat[i_].color;
                            clr_CurrAlpha.a = f_LerpTimer;

                            #region Opaque vs. Transparent
                            // Declare material as Opaque
                            if (clr_CurrAlpha.a == 1.0f)
                            {
                                // Got guide: http://sassybot.com/blog/swapping-rendering-mode-in-unity-5-0/
                                mat_CurrMat[i_].SetFloat("_Mode", 3); // Sets the material to Transparent
                                mat_CurrMat[i_].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                                mat_CurrMat[i_].SetInt("_ZWrite", 0);
                                mat_CurrMat[i_].DisableKeyword("_ALPHATEST_ON");
                                mat_CurrMat[i_].DisableKeyword("_ALPHABLEND_ON");
                                mat_CurrMat[i_].EnableKeyword("_ALPHAPREMULTIPLY_ON");
                                mat_CurrMat[i_].renderQueue = 3000;
                            }
                            // Declare material as Transparent
                            else
                            {
                                // Declare material as invisible
                                // Got guide: http://sassybot.com/blog/swapping-rendering-mode-in-unity-5-0/
                                mat_CurrMat[i_].SetFloat("_Mode", 3); // Sets the material to Transparent
                                mat_CurrMat[i_].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                                mat_CurrMat[i_].SetInt("_ZWrite", 0);
                                mat_CurrMat[i_].DisableKeyword("_ALPHATEST_ON");
                                mat_CurrMat[i_].DisableKeyword("_ALPHABLEND_ON");
                                mat_CurrMat[i_].EnableKeyword("_ALPHAPREMULTIPLY_ON");
                                mat_CurrMat[i_].renderQueue = 3000;
                            }
                            #endregion

                            // If we're fading out && the lerp timer is nearly 0f...
                            if (f_Timer > f_TimeUntilFadeOut && f_LerpTimer < 0.05f)
                            {
                                // ... Turn off the Mesh Renderer
                                go_Model_FadeOutOnly.GetComponent<MeshRenderer>().enabled = false;
                            }

                            mat_CurrMat[i_].color = clr_CurrAlpha;
                        }
                    }
                }
            }
            #endregion

            // If we're out of time
            if (f_Timer > f_TimeUntilFadeOut && f_LerpTimer == 0.0f)
            {
                b_Deactivated = true;
            }
        }
	}

    bool b_IsPlayer;
    void OnTriggerEnter( Collider collider_ )
    {
        if(collider_.transform.root.gameObject.name == "Player")
        {
            RaycastHit hit;
            int i_LayerMask = LayerMask.GetMask("Player", "Wall");
            Vector3 v3_VectorToPlayer = collider_.transform.root.gameObject.transform.position - gameObject.transform.position;

            if(Physics.Raycast(gameObject.transform.position, v3_VectorToPlayer, out hit, float.PositiveInfinity, i_LayerMask))
            {
                if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    b_IsPlayer = true;

                    f_Timer += Time.deltaTime;
                }
            }

        }
    }

    void OnTriggerExit( Collider collider_ )
    {
        if (collider_.transform.root.gameObject.name == "Player" )
        {
            if(b_IsPlayer)
            {
                // If the timer is under the time to fade out, jump the timer forward a bit
                if ( f_Timer < f_TimeUntilFadeOut - 0.5f )
                {
                    f_Timer = f_TimeUntilFadeOut - 0.5f;
                }

                b_IsPlayer = false;
            }
        }
    }
}
