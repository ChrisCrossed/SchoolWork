using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cs_Button_Map : MonoBehaviour
{
    [SerializeField] Enum_MapList e_MapType;
    Cs_OverlaySystem overlaySystem;

    [SerializeField] AnimationCurve ac_Lerp;
    [SerializeField] AnimationCurve ac_Scale;

    Image img_Picked;
    Image img_Banned;

    Vector3 v3_StartPos;

	// Use this for initialization
	void Start ()
    {
        overlaySystem = GameObject.Find("Canvas").GetComponent<Cs_OverlaySystem>();

        v3_StartPos = gameObject.transform.position;

        img_Picked = transform.Find("Img_Picked").GetComponent<Image>();
        img_Banned = transform.Find("Img_Banned").GetComponent<Image>();
    }

    public void ClickButton()
    {
        // Disable the mouse cursor input
        GameObject.Find("Canvas").GetComponent<GraphicRaycaster>().enabled = false;

        // Tell the overlay this button was pressed
        overlaySystem.MapClicked( gameObject );
    }

    public Enum_MapList MapType
    {
        get { return e_MapType; }
    }

    bool b_Banned;
    public bool Set_MapState
    {
        set
        {
            b_Banned = value;

            if(b_Banned)
            {
                gameObject.transform.SetParent( GameObject.Find("Ban Positions").transform );
            }
        }
    }

    bool b_IsActivated;
    float f_MoveTimer;
    RectTransform FinalPosition;
    Transform FinalParent;
    public void GoToPosition( RectTransform pos_, Transform finalParent_ )
    {
        // Set transform to "MovingObjects" so it renders over everything else
        gameObject.transform.SetParent(GameObject.Find("Moving Objects").transform);

        // Set the final parent object to go to
        FinalParent = finalParent_;

        // Set position to move to
        FinalPosition = pos_;

        // Activate object for lerping
        b_IsActivated = true;

        // If map is chosen, keep the button state as 'Normal' for visability purposes
        if(!b_Banned) gameObject.GetComponent<Button>().enabled = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
		// If button was pressed, then begin lerping to the final position
        if( b_IsActivated )
        {
            overlaySystem.GameClock( true );

            // Increment timer over the course of 2 seconds
            f_MoveTimer += Time.deltaTime / 2.0f;
            
            // Lerp the button's position based on the Animation Curve
            gameObject.transform.position = Vector3.LerpUnclamped( v3_StartPos, FinalPosition.transform.position, ac_Lerp.Evaluate(f_MoveTimer) );

            // Scale the button's size based on the other Animation curve
            Vector3 v3_Scale = gameObject.transform.localScale;
            v3_Scale.x = Mathf.LerpUnclamped( 1.0f, 1.25f, ac_Scale.Evaluate(f_MoveTimer) );
            v3_Scale.y = Mathf.LerpUnclamped( 1.0f, 1.25f, ac_Scale.Evaluate(f_MoveTimer) );
            gameObject.transform.localScale = v3_Scale;

            // Increase the alpha of the appropriate overlay
            Color clr_CurrAlpha;
            if (b_Banned) clr_CurrAlpha = img_Banned.color; else clr_CurrAlpha = img_Picked.color;
            clr_CurrAlpha.a = f_MoveTimer * 2.0f;
            if (clr_CurrAlpha.a > 1.0f) clr_CurrAlpha.a = 1.0f;
            if (b_Banned)
            {
                clr_CurrAlpha.a = f_MoveTimer / 2f;
                img_Banned.color = clr_CurrAlpha;
            }
            else img_Picked.color = clr_CurrAlpha;

            // End
            if (f_MoveTimer >= 1.0f)
            {
                // Return object to proper parent
                gameObject.transform.SetParent( FinalParent );

                // Finalize object position
                gameObject.transform.position = FinalPosition.transform.position;

                // Finalize object scale
                gameObject.transform.localScale = new Vector3(1, 1, 1);

                // Enable the mouse cursor input
                GameObject.Find("Canvas").GetComponent<GraphicRaycaster>().enabled = true;

                // Disable button
                b_IsActivated = false;
            }
        }
	}
}
