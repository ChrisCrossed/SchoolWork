using UnityEngine;
using System.Collections;

public class Cs_WarningTileLogic : MonoBehaviour
{
    [SerializeField] float f_FurthestDistance = 5f;
    [SerializeField] float f_ClosestDistance = 2f;
    GameObject go_Player;

    // Use this for initialization
    void Start ()
    {
        go_Player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update ()
    {
        float f_Distance = Vector3.Distance(go_Player.transform.position, gameObject.transform.position);
        Color clr_CurrAlpha = gameObject.GetComponent<Renderer>().material.GetColor("_TintColor");

        // If we're closer than the minimum, begin fading in the object
        if ( f_Distance < f_FurthestDistance )
        {
            // Default to no visibility
            float f_Perc = 0.0f;

            // If we're not close enough to have full visibility, find the percent.
            if(f_Distance > f_ClosestDistance)
            {
                // Scales the alpha based on distance between 0f & 0.5f
                float f_Scale = f_FurthestDistance - f_ClosestDistance;
                f_Perc = (1 - (f_Distance - f_ClosestDistance) / (f_FurthestDistance - f_ClosestDistance)) / 2;
            }
            else
            {
                // Caps at 0.5f
                f_Perc = 0.5f;
            }

            // Sets the alpha
            clr_CurrAlpha.a = f_Perc;
        }
        // Not close enough. Set the alpha to 0.0f;
        else
        {
            clr_CurrAlpha.a = 0f;
        }

        // Set the new color.
        gameObject.GetComponent<Renderer>().material.SetColor("_TintColor", clr_CurrAlpha);
    }
}
