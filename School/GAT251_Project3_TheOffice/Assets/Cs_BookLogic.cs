using UnityEngine;
using System.Collections;

public class Cs_BookLogic : MonoBehaviour
{
    bool b_IsEnabled;
    bool b_IsTransparent;
    [SerializeField] bool b_IsMainBook;

	// Use this for initialization
	void Start ()
    {
        BookEnabled(false);
	}

    public bool IsMainBook()
    {
        return b_IsMainBook;
    }

    public bool ArrowState
    {
        set { transform.Find("RotArrow").GetComponent<Cs_RotArrow>().IsEnabled = value; }
        get { return transform.Find("RotArrow").GetComponent<Cs_RotArrow>().IsEnabled; }
    }

    public bool Get_Enabled()
    {
        return b_IsEnabled;
    }

    public void BookEnabled( bool b_IsActive_, bool b_IsTransparent_ = false )
    {
        b_IsEnabled = b_IsActive_;
        b_IsTransparent = b_IsTransparent_;

        if(b_IsEnabled)
        {
            if (b_IsTransparent) Set_Transparent(0.25f);
            else Set_Opaque();

            gameObject.GetComponent<MeshRenderer>().enabled = true;
            gameObject.GetComponent<BoxCollider>().enabled = true;

            if( !b_IsMainBook && !b_IsTransparent )
            {
                // Not the main book, but has been activated. Set the Objective state.
                transform.root.gameObject.GetComponent<Cs_Objective>().Set_State = Enum_ObjectiveState.Completed;
            }
        }
        else
        {
            Set_Transparent(0.25f);
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.GetComponent<BoxCollider>().enabled = false;

            ArrowState = false;
        }
    }

    void Set_Transparent(float f_Transparency_ = 0.25f)
    {
        Material[] mat_CurrColor = gameObject.GetComponent<MeshRenderer>().materials;

        for (int i_ = 0; i_ < mat_CurrColor.Length; ++i_)
        {
            Color currColor = mat_CurrColor[i_].color;
            Material mat = GetComponentInChildren<MeshRenderer>().materials[i_];

            // Got guide: http://sassybot.com/blog/swapping-rendering-mode-in-unity-5-0/
            mat.SetFloat("_Mode", 3); // Sets the material to Transparent
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.DisableKeyword("_ALPHABLEND_ON");
            mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;

            currColor.a = f_Transparency_;
            mat_CurrColor[i_].color = currColor;
        }
    }
    void Set_Opaque()
    {
        Material[] mat_CurrColor = gameObject.GetComponent<MeshRenderer>().materials;

        for (int i_ = 0; i_ < mat_CurrColor.Length; ++i_)
        {
            Color currColor = mat_CurrColor[i_].color;
            Material mat = GetComponentInChildren<MeshRenderer>().materials[i_];

            // Got guide: http://sassybot.com/blog/swapping-rendering-mode-in-unity-5-0/
            mat.SetFloat("_Mode", 0); // Sets the material to Opaque
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            mat.SetInt("_ZWrite", 1);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.DisableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = -1;

            currColor.a = 1f;
            mat_CurrColor[i_].color = currColor;
        }
    }

    // Update is called once per frame
    void Update ()
    {
	
	}
}
