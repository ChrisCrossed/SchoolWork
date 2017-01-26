using UnityEngine;
using System.Collections;

enum Enum_GateDirection
{
    Down,
    Right,
    Left,
    Up
}

public class Cs_GateScript : MonoBehaviour
{
    [SerializeField]
    Enum_GateDirection e_GateDirection = Enum_GateDirection.Right;

    [SerializeField]
    float f_MoveTime_Max = 1.0f;
    float f_MoveTime;

    [SerializeField]
    bool b_StartOpen;
    [SerializeField]
    bool b_IsObjectiveActive = true;

    Vector3 v3_FinalPos;
    Vector3 v3_StartPos;

    bool b_IsOpen;

    bool b_FirstFrame;

    // Use this for initialization
    void Start()
    {
        // Set start position
        v3_StartPos = gameObject.transform.position;

        // Set final position
        if (e_GateDirection == Enum_GateDirection.Right)
        {
            v3_FinalPos = gameObject.transform.position + (gameObject.transform.right * (gameObject.transform.lossyScale.x + 0.1f));
        }
        else if (e_GateDirection == Enum_GateDirection.Left)
        {
            v3_FinalPos = gameObject.transform.position + (-gameObject.transform.right * (gameObject.transform.lossyScale.x + 0.1f));
        }
        else if (e_GateDirection == Enum_GateDirection.Up)
        {
            v3_FinalPos = gameObject.transform.position + (gameObject.transform.up * (gameObject.transform.lossyScale.y + 0.1f));
        }
        else
        {
            v3_FinalPos = gameObject.transform.position + (-gameObject.transform.up * (gameObject.transform.lossyScale.y + 0.1f));
        }

        as_AudioSource = gameObject.GetComponent<AudioSource>();
        sfx_GarageOpen = Resources.Load("SFX_GarageOpen") as AudioClip;
        as_AudioSource.clip = sfx_GarageOpen;

        /*if (b_StartOpen) Set_DoorOpen(!b_IsOpen);
        b_PreviousSoundState = !b_IsOpen;*/
        if (b_StartOpen)
        {
            b_IsOpen = true;

            b_PreviousSoundState = !b_IsOpen;

            Set_DoorOpen(b_IsOpen);
        }
    }

    // Update is called once per frame
    bool b_PreviousDoorState;
    void Update()
    {
        PositionDoor();
        b_FirstFrame = true;
    }

    bool b_PreviousSoundState;
    AudioSource as_AudioSource;
    AudioClip sfx_GarageOpen;
    float f_EndClip = 8.25f;
    void PlaySFX()
    {
        if (b_IsOpen != b_PreviousSoundState)
        {
            // Play SFX
            if(as_AudioSource.isPlaying)
            {
                as_AudioSource.Stop();
            }
            as_AudioSource.loop = false;
            // as_AudioSource.clip = sfx_GarageOpen;
            as_AudioSource.Play();

            b_PreviousSoundState = b_IsOpen;
        }
    }

    void PositionDoor()
    {
        // Lerp between the door's current position and where it is going
        if (b_IsOpen != b_PreviousDoorState)
        {
            // If we're opening the door...
            if (b_IsOpen)
            {
                // Increment timer
                f_MoveTime += Time.deltaTime;

                // Clamp
                if (f_MoveTime > f_MoveTime_Max) f_MoveTime = f_MoveTime_Max;

                // Lerp position
                Vector3 v3_LerpPosition = Vector3.Lerp(v3_StartPos, v3_FinalPos, f_MoveTime / f_MoveTime_Max);

                // Set position
                gameObject.transform.position = v3_LerpPosition;

                // Set the new state
                if (f_MoveTime == f_MoveTime_Max)
                {
                    // as_AudioSource.time = f_EndClip;

                    as_AudioSource.Stop();

                    // b_PreviousDoorState = !b_PreviousDoorState;
                }
            }
            else // Closing door
            {
                // Decrement timer
                // If an objective door
                f_MoveTime -= Time.deltaTime * 3;

                // Clamp
                if (f_MoveTime < 0) f_MoveTime = 0;

                // Lerp percentage
                float f_LerpPercent = (f_MoveTime_Max - f_MoveTime) / f_MoveTime_Max;

                // Lerp position (from 0% to 100%)
                Vector3 v3_LerpPosition = Vector3.Lerp(v3_FinalPos, v3_StartPos, f_LerpPercent);

                // Set position
                gameObject.transform.position = v3_LerpPosition;

                // Set the new state
                if (f_MoveTime == 0)
                {
                    // as_AudioSource.time = f_EndClip;
                    as_AudioSource.Stop();

                    // b_PreviousDoorState = !b_PreviousDoorState;
                }

            }
        }
    }

    public void Set_DoorOpen(bool b_IsOpen_)
    {
        if (b_IsObjectiveActive)
        {
            b_IsOpen = b_IsOpen_;
            b_PreviousDoorState = !b_IsOpen_;

            // b_PreviousSoundState = !b_IsOpen;
        }

        if(b_FirstFrame)
        {
            PlaySFX();
        }
    }

    public bool Get_DoorOpen()
    {
        return b_IsOpen;
    }

    public void Set_ObjectiveActive(bool b_IsActive_)
    {
        b_IsObjectiveActive = b_IsActive_;
    }
}
