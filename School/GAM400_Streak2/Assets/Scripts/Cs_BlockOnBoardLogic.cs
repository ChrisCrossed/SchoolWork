using UnityEngine;
using System.Collections;

/*********************************
 * 
 * Copyright DigiPen Institute of Technology 2016
 * 
 * Streak 2 by Christopher Christensen
 * 
 * *******************************/

public class Cs_BlockOnBoardLogic : MonoBehaviour
{
    float f_LerpTimer_Horiz;
    float f_LerpTimer_Vert;
    float f_LerpTimer_Max = 0.25f;

    float f_yPos;
    float f_xPos;

    float f_BlockScale;
    float f_InitialScale;
    int i_BoardWidth;

    // Variables - Block Gets Killed
    bool b_IsDead;
    float f_TransparencyTimer = 0.5f;
    [SerializeField] float f_TimeToTransparent = .5f;
    [SerializeField] float f_LowestTransparencyPoint = 0.05f;

    // Variables - Block Gets Scored
    bool b_IsScored;
    bool b_GameOver;
    float f_Timer_SinWave; // Incremental timer
    static float f_Timer_SinWave_Max = 5; // Max timer
    float f_Timer_SinWave_Speed = 5; // Multiples against Delta Time to speed/slow the rate of movement
    float f_Timer_SinWave_MaxDist = 1; // Multiplies against the f_Timer_SinWave when applied to the Z position

    // Use this for initialization
    void Start ()
    {
        // Init_BlockModel(5, 5, 3, 20);
	}

    public void Init_BlockModel( int i_xPos_, int i_yPos_, float f_BlockScale_, float f_InitialScale_, int i_BoardWidth_ )
    {
        f_xPos = i_xPos_;
        f_yPos = i_yPos_;

        f_BlockScale = f_BlockScale_;
        f_InitialScale = f_InitialScale_;
        i_BoardWidth = i_BoardWidth_;

        gameObject.transform.position = new Vector3( f_xPos * f_BlockScale, f_yPos * f_BlockScale, 0 );

        gameObject.transform.localScale = new Vector3( f_InitialScale, f_InitialScale, f_InitialScale );
    }

    // Update is called once per frame
    float f_SpinTimer;
	void Update ()
    {
        if(!(b_IsDead || b_IsScored))
        {
            #region Update X & Y position
        if (gameObject.transform.position != new Vector3(f_xPos * f_BlockScale, f_yPos * f_BlockScale, 0))
        {
            float f_xPos_Temp;
            float f_yPos_Temp;

            // Lerp to current X
            if(f_LerpTimer_Horiz < f_LerpTimer_Max)
            {
                f_LerpTimer_Horiz += Time.deltaTime;

                if ( f_LerpTimer_Horiz > f_LerpTimer_Max )
                {
                    f_LerpTimer_Horiz = f_LerpTimer_Max;
                }
            }
            
            f_xPos_Temp = Mathf.Lerp(gameObject.transform.position.x, f_xPos * f_BlockScale, f_LerpTimer_Horiz / f_LerpTimer_Max);

            // Lerp to current Y
            if(f_LerpTimer_Vert < f_LerpTimer_Max)
            {
                f_LerpTimer_Vert += Time.deltaTime;

                if(f_LerpTimer_Vert > f_LerpTimer_Max)
                {
                    f_LerpTimer_Vert = f_LerpTimer_Max;
                }
            }

            f_yPos_Temp = Mathf.Lerp(gameObject.transform.position.y, f_yPos * f_BlockScale, f_LerpTimer_Vert / f_LerpTimer_Max);

            gameObject.transform.position = new Vector3(f_xPos_Temp, f_yPos_Temp, 0);
        }
        #endregion
        }

        #region Fade Out (When Destroyed)
        if(b_IsDead)
        {
            f_TransparencyTimer -= (Time.deltaTime / f_TimeToTransparent);

            if (f_TransparencyTimer < f_LowestTransparencyPoint) f_TransparencyTimer = f_LowestTransparencyPoint;

            SetMaterialsVisibility(f_TransparencyTimer);

            if(f_TransparencyTimer ==  f_LowestTransparencyPoint)
            {
                GameObject.Destroy(gameObject);
            }
        }
        #endregion

        if(b_IsScored)
        {
            float f_zPos;
            Vector3 v3_CurrPos = gameObject.transform.position;

            // Between 0 & 1, position the block toward, then away from the player
            f_Timer_SinWave += (Time.deltaTime * f_Timer_SinWave_Speed);

            if(f_Timer_SinWave < Mathf.PI)
            {
                f_zPos = Mathf.Sin(f_Timer_SinWave) * -f_Timer_SinWave_MaxDist;
            }
            else
            {
                // Continue moving the z Position backward rather than loop through the Sine Wave
                f_zPos = (f_Timer_SinWave - Mathf.PI) * f_Timer_SinWave_MaxDist;

                // After the amount of time has passed, fade out the block
                if(f_Timer_SinWave > f_Timer_SinWave_Max)
                {
                    // Set the block to 'die' so it fades out
                    b_IsDead = true;
                }
            }

            // Multiplying by -1 since the block needs to head toward the player before regressing
            v3_CurrPos.z = f_zPos;

            // Set final position
            gameObject.transform.position = v3_CurrPos;

            // Rotate the block
            if(f_SpinTimer < 3)
            {
                f_SpinTimer += Time.deltaTime * 3;
            }
            Vector3 v3_CurrRot = gameObject.transform.eulerAngles;
            v3_CurrRot.z -= Time.deltaTime * 360 * f_SpinTimer;
            gameObject.transform.eulerAngles = v3_CurrRot;
        }

        // Scale Increase
        if(gameObject.transform.localScale.x < 1.0f)
        {
            Vector3 v3_NewScale = gameObject.transform.localScale;

            v3_NewScale.x += Time.deltaTime * 1.0f;
            v3_NewScale.y += Time.deltaTime * 1.0f;
            v3_NewScale.z += Time.deltaTime * 1.0f;

            if(v3_NewScale.x > f_BlockScale)
            {
                v3_NewScale.x = 1.0f;
                v3_NewScale.y = 1.0f;
                v3_NewScale.z = 1.0f;
            }

            gameObject.transform.localScale = v3_NewScale;
        }
    }

    public void Set_MoveLeft()
    {
        // Decrement X position by 1
        if(f_xPos - 1 >= 0)
        {
            f_xPos -= 1;
        }

        // Reset LerpTimer_Horiz
        f_LerpTimer_Horiz = 0f;
    }

    public void Set_MoveRight()
    {
        // Increment X position by 1
        if( f_xPos + 1< i_BoardWidth)
        {
            f_xPos += 1;
        }

        // Reset LerpTimer_Horiz
        f_LerpTimer_Horiz = 0.0f;
    }

    public void Set_MoveDown()
    {
        // Decrement Y position by 1
        if(f_yPos - 1 >= 0)
        {
            f_yPos -= 1;
        }

        // Reset LerpTimer_Vert
        f_LerpTimer_Vert = 0.0f;
    }

    public void Set_MoveUp()
    {
        // Increment Y position by 1
        if (f_yPos + 1 >= 0)
        {
            f_yPos += 1;
        }

        // Reset LerpTimer_Vert
        f_LerpTimer_Vert = 0.0f;
    }

    public void Set_MoveDownToPos( int f_yPos_ )
    {
        // Get current Y position
        int i_Temp = (int)(f_yPos - f_yPos_);

        // Loop
        for(int i_ = 0; i_ < i_Temp; ++i_)
        {
            Set_MoveDown();
        }
    }

    public void Set_DeleteBlock()
    {
        // Set state to 'dead'
        b_IsDead = true;
    }

    public void Set_ScoreBlock()
    {
        b_IsScored = true;
    }

    public void Set_GameOverBlock()
    {
        b_GameOver = true;
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
}
