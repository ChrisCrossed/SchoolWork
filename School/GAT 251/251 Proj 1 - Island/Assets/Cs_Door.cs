using UnityEngine;
using System.Collections;

enum Enum_MoveDirection
{
    Down,
    Right
}

public class Cs_Door : MonoBehaviour
{
    bool b_IsMoving;
    bool b_IsReset;
    Vector3 v3_StartingPos;
    float f_MoveDistance;
    float f_MoveSpeed = 2f;

    [SerializeField]
    Enum_MoveDirection moveDirection;

    [SerializeField]
    bool b_StartOpen;

    // Use this for initialization
    void Start ()
    {
        if(moveDirection == Enum_MoveDirection.Right)
        {
            f_MoveDistance  = gameObject.transform.lossyScale.y;
        }
        else
        {
            f_MoveDistance = gameObject.transform.lossyScale.y;
        }

        v3_StartingPos = gameObject.transform.position;

        if (b_StartOpen) MoveDoor();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(b_IsMoving)
        {
            if(AllowedToMove())
            {
                Vector3 v3_LerpPosition = gameObject.transform.right * (Time.deltaTime * f_MoveSpeed);

                gameObject.transform.position += v3_LerpPosition;
            }
        }
        else if(b_IsReset)
        {
            gameObject.transform.position = v3_StartingPos;

            b_IsReset = false;
        }
	}

    bool AllowedToMove()
    {
        if(Vector3.Distance(gameObject.transform.position, v3_StartingPos) >= f_MoveDistance)
        {
            b_IsMoving = false;

            return false;
        }

        return true;
    }

    [SerializeField] GameObject go_SFX;
    [SerializeField] AudioClip sfx_OpenDoor;
    [SerializeField] AudioClip sfx_CloseDoor;
    public void MoveDoor()
    {
        b_IsMoving = true;

        if(!b_StartOpen)
        {
            go_SFX.GetComponent<AudioSource>().PlayOneShot(sfx_OpenDoor);
        }
    }

    public void CloseDoor()
    {
        b_IsReset = true;

        go_SFX.GetComponent<AudioSource>().PlayOneShot(sfx_CloseDoor);
    }
}
