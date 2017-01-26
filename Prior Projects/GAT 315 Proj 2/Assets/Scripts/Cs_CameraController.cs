using UnityEngine;
using System.Collections;
using XInputDotNetPure; // Controller input

public class Cs_CameraController : MonoBehaviour
{
    public GameObject go_CamReference;
    Vector3 newPos;
    Quaternion newRot;

    bool b_CameraLockedToPlayer = false;
    float f_CamMoveTimer = 0.5f;

    GamePadState state;
    GamePadState prevState;
    public PlayerIndex playerIndex = PlayerIndex.One;

    // Raycast Objects
    public GameObject rayCastObj;

    // Used for creating linear equation
    float scale_;
    float yInt_;

    // Use this for initialization
    void Start ()
    {
        newPos = go_CamReference.transform.position;

        CreateLinearEquation(30, 30, 50, 80);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(b_CameraLockedToPlayer)
        {
            if(go_CamReference)
            {
                // Increases the speed at which the camera will slide to new locations
                if (f_CamMoveTimer < 1.0f) f_CamMoveTimer += Time.deltaTime; else f_CamMoveTimer = 1.0f;

                RotateCameraPosition();

                // Lerp to the cam reference's position
                newPos = Vector3.Lerp(gameObject.transform.position, go_CamReference.transform.position, f_CamMoveTimer);

                // Slerp to the cam reference's rotation
                newRot = Quaternion.Slerp(gameObject.transform.rotation, go_CamReference.transform.rotation, f_CamMoveTimer / 2);
                
                // Set new information
                gameObject.transform.position = newPos;
                gameObject.transform.rotation = newRot;

                // Raycast in-front of player. Changes FOV and Timespeed
                CameraRaycast();
            }
        }
	}

    void CameraRaycast()
    {
        Ray ray = new Ray(rayCastObj.transform.position, rayCastObj.transform.forward);
        Debug.DrawLine(rayCastObj.transform.position, rayCastObj.transform.position + (rayCastObj.transform.forward * 50f), Color.red);
        RaycastHit hit;

        // If we are within 50 units of something in front of us, slow down the deltatime of the game to accomodate
        if (Physics.Raycast(ray, out hit, 50f))
        {
            // Slope equation: Y = 2(X) - 20; (x 50 = y 80, x 30 = y 40)
            float y_;
            if (hit.distance > 30f) y_ = scale_ * (hit.distance) - yInt_;
            else y_ = 40;

            // Pull in the camera FOV based on distance
            gameObject.GetComponent<Camera>().fieldOfView = Mathf.Lerp(gameObject.GetComponent<Camera>().fieldOfView, y_, 0.1f);

            // Slow down the speed of the game
            Time.timeScale = 0.50f;
            GameObject.Find("LevelManager").GetComponent<AudioSource>().pitch = 1.15f;
        }
        else // Otherwise, pull back the camera FOV
        {
            // Lerp out the Camera FOV
            gameObject.GetComponent<Camera>().fieldOfView = Mathf.Lerp(gameObject.GetComponent<Camera>().fieldOfView, 100f, 0.1f);

            // Lerp the speed of the game back toward normal
            if(Time.timeScale <= 1.0f)
            {
                Time.timeScale = Mathf.Lerp(Time.timeScale, 1.0f, 0.1f);
            }

            GameObject.Find("LevelManager").GetComponent<AudioSource>().pitch = 1.0f;
        }
    }

    void CreateLinearEquation(int x1_, int y1_, int x2_, int y2_)
    {
        scale_ = (y2_ - y1_) / (x2_ - x1_);
        yInt_ = -(scale_ * x1_) + y1_;

        print("Scale: " + scale_);
        print("yInt: " + yInt_);
    }

    

    void RotateCameraPosition()
    {
        prevState = state;
        state = GamePad.GetState(playerIndex);

        // Get the xPos of the controller left analog
        float xPos = state.ThumbSticks.Left.X;

        // Move the camRef to a new xPos based on the left analog
        Vector3 newPos = go_CamReference.transform.localPosition;
        newPos.x = Mathf.Lerp(go_CamReference.transform.localPosition.x, xPos * -5f, 0.01f);

        // Move the camRef to a new yPos based on the left analog
        newPos.y = Mathf.Lerp(go_CamReference.transform.localPosition.y, (Mathf.Abs(xPos) * -0.5f) + 2.55f, 0.01f); // 2.55f is the starting yPos of the camRef

        go_CamReference.transform.localPosition = newPos;
    }

    public void SetCameraLock(bool b_IsLockedToPlayer_)
    {
        b_CameraLockedToPlayer = b_IsLockedToPlayer_;
        f_CamMoveTimer = 0.0f;
    }
}