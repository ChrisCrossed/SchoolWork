using UnityEngine;
using System.Collections;

public class Cs_WallLogic : MonoBehaviour
{
    // WallLogic does two things: Ensures it is set as a 'Wall' layer, and contains functions to turn itself semi-transparent when told to (by camera raycasting)
    bool b_GoToTransparent;

    float f_TransparencyTimer = 1.0f;
    [SerializeField] float f_TimeToTransparent = .5f;
    [SerializeField] float f_LowestTransparencyPoint = 0.25f;

    // Use this for initialization
    void Start()
    {
        // Set the wall to be considered a 'wall'
        if(gameObject.layer != LayerMask.NameToLayer("Trigger"))
        {
            int i_LayerMask = LayerMask.NameToLayer("Wall");
            gameObject.layer = i_LayerMask;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // If we're either going transparent, or returning from transparent
        if ( (b_GoToTransparent && f_TransparencyTimer > f_LowestTransparencyPoint) ||
             (!b_GoToTransparent && f_TransparencyTimer < 1 ) )
        {

            // Lerp from current transparency point to required point
            if(b_GoToTransparent)
            {
                // Reduce the transparency by the time modifier
                f_TransparencyTimer -= (Time.deltaTime / f_TimeToTransparent);

                if (f_TransparencyTimer < f_LowestTransparencyPoint) f_TransparencyTimer = f_LowestTransparencyPoint;
            }
            else
            {
                // Increase the transparency point by the time modifier
                f_TransparencyTimer += (Time.deltaTime / f_TimeToTransparent);

                if (f_TransparencyTimer > 1) f_TransparencyTimer = 1;
            }

            SetMaterialsVisibility(f_TransparencyTimer);
        }
	}

    void SetMaterialsVisibility( float f_Transparency_ )
    {
        Material[] mat_CurrColor = gameObject.GetComponent<MeshRenderer>().materials;

        for(int i_ = 0; i_ < mat_CurrColor.Length; ++i_)
        {
            Color currColor = mat_CurrColor[i_].color;

            if(f_Transparency_ == 1.0f)
            {
                // print("Making Opaque: " + f_Transparency_);
                // mat_CurrColor[i_].SetFloat("_Mode", 1.0f);
                Material mat = GetComponentInChildren<MeshRenderer>().materials[i_];

                // Got guide: http://sassybot.com/blog/swapping-rendering-mode-in-unity-5-0/
                mat.SetFloat("_Mode", 0); // Sets the material to Opaque
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                mat.SetInt("_ZWrite", 1);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.DisableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = -1;
                
                // Set transparency for metallic objects only if they are already metallic
                if (mat.GetFloat("_Metallic") > (f_LowestTransparencyPoint - 0.01f))
                {
                    mat.SetFloat("_Metallic", f_Transparency_);
                }
            }
            else
            {
                // print("Making Transparent: " + currColor.a);
                // mat_CurrColor[i_].SetFloat("_Mode", 4.0f);
                Material mat = GetComponentInChildren<MeshRenderer>().materials[i_];

                // Got guide: http://sassybot.com/blog/swapping-rendering-mode-in-unity-5-0/
                mat.SetFloat("_Mode", 3); // Sets the material to Transparent
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.DisableKeyword("_ALPHABLEND_ON");
                mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;

                // Set transparency for metallic objects only if they are already metallic
                if (mat.GetFloat("_Metallic") > (f_LowestTransparencyPoint - 0.01f))
                {
                    mat.SetFloat("_Metallic", f_Transparency_);
                }
            }

            // GetComponent<MeshRenderer>().met
            currColor.a = f_Transparency_;
            mat_CurrColor[i_].color = currColor;

        }
    }

    public void SetVisibilityState( bool b_GoToTransparent_ )
    {
        b_GoToTransparent = b_GoToTransparent_;
    }
}
