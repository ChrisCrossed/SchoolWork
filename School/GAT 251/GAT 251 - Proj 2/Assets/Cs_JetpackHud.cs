using UnityEngine;
using System.Collections;

public class Cs_JetpackHud : MonoBehaviour
{
    float f_xPos_Init;
    float f_yPos_Init;
    float f_Width_Init;
    float f_Height_Init;

    // Use this for initialization
    void Start ()
    {
        f_xPos_Init = gameObject.transform.position.x;
        f_yPos_Init = gameObject.transform.position.y;
        
        f_Width_Init = gameObject.GetComponent<RectTransform>().rect.width;
        f_Height_Init = gameObject.GetComponent<RectTransform>().rect.height;
    }

    float f_Width;
    float f_xPos;
    public void Set_HUDPercentage( float f_Percent_ )
    {
        f_Width = f_Width_Init * f_Percent_;
        
        f_xPos = 25 + (f_Width / 2);
        
        Vector2 v2_CurrPos = gameObject.transform.position;
        v2_CurrPos.x = f_xPos;
        gameObject.transform.position = v2_CurrPos;

        Vector3 v3_CurrScale = gameObject.transform.localScale;
        v3_CurrScale.x = f_Percent_;
        gameObject.transform.localScale = v3_CurrScale;
    }
}
