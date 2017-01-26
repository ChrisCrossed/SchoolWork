using UnityEngine;
using System.Collections;

/*********************************
 * 
 * Copyright DigiPen Institute of Technology 2016
 * 
 * Streak 2 by Christopher Christensen
 * 
 * *******************************/

public class Cs_CameraController : MonoBehaviour
{
	public void Init_CameraPosition( float f_BoardWidth_, float f_BoardHeight_, float f_BlockWidth_ )
    {
        Vector3 v3_CamPos = new Vector3();
        
        if (f_BoardHeight_ >= f_BoardWidth_)
        {
            v3_CamPos.x = (f_BoardWidth_ * f_BlockWidth_ / 2) - (f_BlockWidth_ / 2);
            v3_CamPos.y = (f_BoardHeight_ * f_BlockWidth_ / 2);
            v3_CamPos.z = (v3_CamPos.y * -2);
        }
        else
        {
            v3_CamPos.x = (f_BoardWidth_ * f_BlockWidth_ / 2) - (f_BlockWidth_ / 2);
            v3_CamPos.y = (f_BoardHeight_ * f_BlockWidth_ / 2) + (f_BoardWidth_ / 2);
            v3_CamPos.z = (v3_CamPos.y * -2);
        }

        gameObject.transform.position = v3_CamPos;
    }
}
