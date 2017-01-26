using UnityEngine;
using System.Collections;

public class Cs_BossWallTrigger : MonoBehaviour
{
    bool b_Active = false;
    bool b_Enabled;
    bool b_EndGame;

    float f_IsPenetrableTimer;
    float f_StartPos;

    public GameObject go_Wall;

	// Use this for initialization
	void Start ()
    {
        f_StartPos = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(!b_EndGame)
        {
            if (b_Enabled)
            {
                f_IsPenetrableTimer += Time.deltaTime;
            }

            if(f_IsPenetrableTimer >= 3)
            {
                if(go_Wall.transform.position.y < 10)
                {
                    Vector3 newPos = go_Wall.transform.position;
                    newPos.y += Time.deltaTime * 5;
                    if (newPos.y > 10) newPos.y = 10;
                    go_Wall.transform.position = newPos;
                }

                if(f_IsPenetrableTimer >= 15)
                {
                    b_Enabled = false;
                    f_IsPenetrableTimer = 0;
                }
            }
            else
            {
                if (go_Wall.transform.position.y > f_StartPos)
                {
                    Vector3 newPos = go_Wall.transform.position;
                    newPos.y -= Time.deltaTime * 3;
                    if (newPos.y < f_StartPos) newPos.y = f_StartPos;
                    go_Wall.transform.position = newPos;
                }
            }
        }
        else
        {
            if (go_Wall.transform.position.y < 10)
            {
                Vector3 newPos = go_Wall.transform.position;
                newPos.y += Time.deltaTime * 5;
                if (newPos.y > 10) newPos.y = 10;
                go_Wall.transform.position = newPos;
            }
        }
	}

    public void EndGame()
    {
        b_EndGame = true;
    }

    public void SetState(bool b_State_)
    {
        b_Active = b_State_;

        // Since it's the first time, raise the gate immediately
        f_IsPenetrableTimer = 3.0f;

        b_Enabled = true;
    }

    void OnTriggerEnter(Collider collider_)
    {
        if(b_Active)
        {
            if(collider_.tag == "Laser")
            {
                b_Enabled = true;
            }
        }
    }
}
