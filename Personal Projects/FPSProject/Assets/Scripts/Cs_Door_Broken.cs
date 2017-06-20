using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_Door_Broken : Cs_Door
{
    [SerializeField] bool StartDoorBroken = true;
    Vector3 v3_StartPos;
    Vector3 v3_EndPos;

    private new void Start()
    {
        base.Start();

        DoorIsBroken = StartDoorBroken;

        v3_StartPos = gameObject.transform.position;

        v3_EndPos = v3_StartPos;
        v3_EndPos.y += 0.35f;
    }

    bool b_DoorIsBroken;
    public bool DoorIsBroken
    {
        set { b_DoorIsBroken = value; }
        get { return b_DoorIsBroken; }
    }

    float f_DoorMoveTimer;
    static float f_DoorMoveTimer_Max = 0.4f;
    [SerializeField] AnimationCurve ac_BrokenDoor;
    public new void Use_OpenDoor()
    {
        if(DoorIsBroken)
        {
            // Reset DoorMoveTimer
            f_DoorMoveTimer = 0f;

            // Change Layer (so player cannot interact
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
        else
        {
            base.Use_OpenDoor();
        }
    }

    void SetPosition( float f_Perc_ )
    {
        gameObject.transform.position = Vector3.LerpUnclamped( v3_StartPos, v3_EndPos, f_Perc_ );
    }

    private new void Update()
    {
        if(DoorIsBroken)
        {
            // If door timer is ready to move, move it
            if(f_DoorMoveTimer < f_DoorMoveTimer_Max)
            {
                f_DoorMoveTimer += Time.deltaTime;

                if (f_DoorMoveTimer > f_DoorMoveTimer_Max)
                {
                    f_DoorMoveTimer = f_DoorMoveTimer_Max;

                    gameObject.layer = LayerMask.NameToLayer("Use");
                }

                SetPosition( ac_BrokenDoor.Evaluate(f_DoorMoveTimer / f_DoorMoveTimer_Max) );
            }
        }
        else
        {
            // Run door code like normal
            base.Update();
        }
    }
}
