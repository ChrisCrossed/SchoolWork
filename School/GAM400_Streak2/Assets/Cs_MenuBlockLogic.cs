using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_MenuBlockLogic : MonoBehaviour
{
    // Use this for initialization
	void Start ()
    {
        Init_Position();
	}

    void MoveBlock()
    {
        Vector3 v3_CurrPos = gameObject.transform.localPosition;

        v3_CurrPos.y -= Time.fixedDeltaTime * 10f;

        if (v3_CurrPos.y < -110f) ResetHeight();
        else
        {
            gameObject.transform.localPosition = v3_CurrPos;
        }

        Vector3 v3_CurrRot = gameObject.transform.eulerAngles;
        v3_CurrRot.z += 30f * Time.fixedDeltaTime;
        gameObject.transform.eulerAngles = v3_CurrRot;
    }

    void Init_Position()
    {
        Vector3 v3_Pos = gameObject.transform.localPosition;

        v3_Pos.y = Random.Range(-115f, 115f);
        v3_Pos.x = Random.Range(-115f, 115f);
        v3_Pos.z = Random.Range(120f, 160f);

        gameObject.transform.localPosition = v3_Pos;
    }

    void ResetHeight()
    {
        Vector3 v3_Pos = gameObject.transform.localPosition;

        v3_Pos.y = 110f;
        v3_Pos.x = Random.Range(-115f, 115f);
        v3_Pos.z = Random.Range(120f, 160f);

        gameObject.transform.localPosition = v3_Pos;
    }

    bool b_IsPaused;
    public bool PauseState
    {
        set { b_IsPaused = value; }
        get { return b_IsPaused; }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if( !b_IsPaused ) MoveBlock();
    }
}
