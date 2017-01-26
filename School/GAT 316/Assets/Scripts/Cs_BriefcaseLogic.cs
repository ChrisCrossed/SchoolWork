using UnityEngine;
using System.Collections;

public class Cs_BriefcaseLogic : MonoBehaviour
{
    float f_yPos_Initial;
    float f_SinTimer;
    bool b_PickedUp;
    bool b_Disabled;

	// Use this for initialization
	void Start ()
    {
        f_yPos_Initial = gameObject.transform.position.y;
	}

    float f_SpinSpeed = 90f;
    void Spin()
    {
        float f_CurrRot = gameObject.transform.eulerAngles.y;
        f_CurrRot += Time.deltaTime * f_SpinSpeed;
        if (f_CurrRot >= 360f) f_CurrRot = 0f;
        gameObject.transform.eulerAngles = new Vector3(0, f_CurrRot, 0);
    }

    float f_BobSpeed = 2f;
    float f_BobDistance = 0.5f;
    void BobUpDown()
    {
        f_SinTimer += Time.deltaTime * f_BobSpeed;

        Vector3 v3_Pos = gameObject.transform.position;
        v3_Pos.y = Mathf.Sin(f_SinTimer) * f_BobDistance + f_yPos_Initial;
        gameObject.transform.position = v3_Pos;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(!b_Disabled)
        {
            Spin();
            BobUpDown();

	        if(b_PickedUp)
            {
                // Reduce the Alpha
                Material[] mat_Alpha = gameObject.GetComponent<MeshRenderer>().materials;
                for (int i_ = 0; i_ < mat_Alpha.Length; ++i_)
                {
                    Color clr_Alpha = mat_Alpha[i_].color;
                    clr_Alpha.a -= Time.deltaTime * 3;
                    if (clr_Alpha.a < 0.05f)
                    {
                        clr_Alpha.a = 0f;

                        gameObject.GetComponent<MeshRenderer>().enabled = false;

                        // Disables the model from doing any more actions
                        b_Disabled = true;
                    }
                    mat_Alpha[i_].color = clr_Alpha;
                }
            }
        }
    }

    public void Set_PickedUp()
    {
        b_PickedUp = true;

        #region Ensure the material is Transparent

        Material[] mat_List = gameObject.GetComponent<MeshRenderer>().materials;

        for(int i_ = 0; i_ < mat_List.Length; ++ i_)
        {
            // Got guide: http://sassybot.com/blog/swapping-rendering-mode-in-unity-5-0/
            mat_List[i_].SetFloat("_Mode", 3); // Sets the material to Transparent
            mat_List[i_].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat_List[i_].SetInt("_ZWrite", 0);
            mat_List[i_].DisableKeyword("_ALPHATEST_ON");
            mat_List[i_].DisableKeyword("_ALPHABLEND_ON");
            mat_List[i_].EnableKeyword("_ALPHAPREMULTIPLY_ON");
            mat_List[i_].renderQueue = 3000;
        }

        #endregion
    }
}
