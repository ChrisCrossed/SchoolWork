using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

enum MouseState
{
    Off, // Default position. Set to Off the 1st frame after 'Released'
    Pressed, // Mouse is pressed
    Released, // Mouse is Released
    Held, // If Mouse is still held after 0.3f, becomes Held
    ScrollUp, // Scroll Wheel Up was performed
    ScrollDown // Scroll Wheel Down was performed
}

public class Cs_CameraLogic : MonoBehaviour
{
    MouseState mouseState;
    MouseState prevState;
    float f_MouseTimer;
    const float MAX_MOUSE_HELD = 1.0f;
    bool b_Left;
    bool b_Right;
    bool b_Forward;
    bool b_Backward;

    bool b_GameRunning;
    public GameObject go_Canvas;

    bool b_Camera_AttachedToMain;
    public GameObject Cam_Regular;
    public GameObject Cam_TopDown;
    GameObject LevelController;

    // Mouse scroll objects
    int i_MouseScrollPos;

    GameObject go_GridObjectList;
    PurchaseObjects nextTowerObject;

    AudioSource audioSource;
    public AudioClip sfx_MenuSelect;
    public AudioClip sfx_PlaceTerrain;
    public AudioClip sfx_DestroyObject;
    public AudioClip sfx_ChangePaintColor;

    // Use this for initialization
    void Start ()
    {
        mouseState = MouseState.Off;
        f_MouseTimer = 0f;
        b_GameRunning = true;
        SetPauseMenu(false);
        b_Camera_AttachedToMain = true;
        i_MouseScrollPos = 4;
        go_GridObjectList = GameObject.Find("GridObject List");
        LevelController = GameObject.Find("LevelController");
        nextTowerObject = PurchaseObjects.None;
    }

    void SetMouseState()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0.0f)
        {
            // If the Mouse scroll axis is > 0, the mouse scroll wheel is 'Up', otherwise it is down.
            if (Input.GetAxis("Mouse ScrollWheel") > 0.0f) mouseState = MouseState.ScrollUp; else mouseState = MouseState.ScrollDown;
            return;
        }

        // Search for an excuse for the mouse to be considered off
        if(Input.GetMouseButtonUp(0) || mouseState == MouseState.Released)
        {
            // If the mouse was pressed last frame, release it now. Otherwise, shut it off.
            if (mouseState == MouseState.Held || mouseState == MouseState.Pressed) { mouseState = MouseState.Released; f_MouseTimer = 0f; return; }
            else { mouseState = MouseState.Off; return; }
        }

        if(mouseState == MouseState.Pressed || mouseState == MouseState.Held)
        {
            f_MouseTimer += Time.deltaTime;

            // If the button has been held less than 0.3f, then it's pressed. Otherwise, it's held down.
            if (f_MouseTimer < MAX_MOUSE_HELD) { mouseState = MouseState.Pressed; return; }
            else { mouseState = MouseState.Held; return; }
        }

        // Mouse pressed
        if (Input.GetMouseButtonDown(0))
        {
            mouseState = MouseState.Pressed;
            return;
        }

        // Default
        mouseState = MouseState.Off;
    }

    void MoveCamera()
    {
        if(b_Camera_AttachedToMain)
        {
            if (Input.GetKeyDown(KeyCode.A)) b_Left = true;
            if (Input.GetKeyDown(KeyCode.W)) b_Forward = true;
            if (Input.GetKeyDown(KeyCode.D)) b_Right = true;
            if (Input.GetKeyDown(KeyCode.S)) b_Backward = true;

            if (Input.GetKeyUp(KeyCode.A)) b_Left = false;
            if (Input.GetKeyUp(KeyCode.W)) b_Forward = false;
            if (Input.GetKeyUp(KeyCode.D)) b_Right = false;
            if (Input.GetKeyUp(KeyCode.S)) b_Backward = false;

            var newPos = Cam_Regular.transform.position;

            int i_MoveSpeed = 2;

            if (Input.GetKey(KeyCode.LeftShift)) i_MoveSpeed = 6;

            if (b_Left)
            {
                // > -3
                /*
                if (Cam_Regular.transform.position.x > -3f)
                {
                    newPos.x -= Time.deltaTime * i_MoveSpeed;
                }
                */
                if(GameObject.Find("DeadCenter").transform.position.x > -3f)
                {
                    GameObject.Find("DeadCenter").transform.position -= GameObject.Find("DeadCenter").transform.right * i_MoveSpeed * Time.deltaTime;
                    newPos -= GameObject.Find("DeadCenter").transform.right * i_MoveSpeed * Time.deltaTime;
                }
            }
            if(b_Right)
            {
                /*
                if (Cam_Regular.transform.position.x < 3f)
                {
                    newPos.x += Time.deltaTime * i_MoveSpeed;
                }
                */
                if (GameObject.Find("DeadCenter").transform.position.x < 3f)
                {
                    GameObject.Find("DeadCenter").transform.position += GameObject.Find("DeadCenter").transform.right * i_MoveSpeed * Time.deltaTime;
                    newPos += GameObject.Find("DeadCenter").transform.right * i_MoveSpeed * Time.deltaTime;
                }
            }

            if (b_Backward)
            {
                // > -3
                /*
                if (Cam_Regular.transform.position.z > -9f)
                {
                    newPos.z -= Time.deltaTime * i_MoveSpeed;
                }
                */
                if (GameObject.Find("DeadCenter").transform.position.x > -3f)
                {
                    GameObject.Find("DeadCenter").transform.position -= GameObject.Find("DeadCenter").transform.forward * i_MoveSpeed * Time.deltaTime;
                    newPos -= GameObject.Find("DeadCenter").transform.forward * i_MoveSpeed * Time.deltaTime;
                }
            }
            if (b_Forward)
            {
                /*
                if (Cam_Regular.transform.position.z < 0f)
                {
                    newPos.z += Time.deltaTime * i_MoveSpeed;
                }
                */
                if (GameObject.Find("DeadCenter").transform.position.x < 3f)
                {
                    GameObject.Find("DeadCenter").transform.position += GameObject.Find("DeadCenter").transform.forward * i_MoveSpeed * Time.deltaTime;
                    newPos += GameObject.Find("DeadCenter").transform.forward * i_MoveSpeed * Time.deltaTime;
                }
            }

            Cam_Regular.transform.position = newPos;

            // Move camera in/out when scrolling the mouse
            if(mouseState == MouseState.ScrollUp)
            {
                // Work with a counter for how far in/out the camera can move
                if(i_MouseScrollPos > 2)
                {
                    Cam_Regular.transform.position += Cam_Regular.transform.forward * 1.0f;
                    --i_MouseScrollPos;
                }
            }
            else if(mouseState == MouseState.ScrollDown)
            {
                // Work with a counter for how far in/out the camera can move
                if (i_MouseScrollPos < 11)
                {
                    Cam_Regular.transform.position -= Cam_Regular.transform.forward * 1.0f;
                    ++i_MouseScrollPos;
                }
            }

            
            if(Input.GetMouseButton(1))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                mouseState = MouseState.Off;

                float MouseX = Input.GetAxis("Mouse X");
                Vector3 newRot = GameObject.Find("DeadCenter").transform.eulerAngles;
                newRot.y += MouseX;
                GameObject.Find("DeadCenter").transform.eulerAngles = newRot;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    public void SetNextTowerObject_Wall()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.PlayOneShot(sfx_MenuSelect);

        if (GameObject.Find("Toggle_Wall").GetComponent<Toggle>().isOn) nextTowerObject = PurchaseObjects.Wall;
        else nextTowerObject = PurchaseObjects.None;
    }

    public void SetNextTowerObject_Tree()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.PlayOneShot(sfx_MenuSelect);

        if (GameObject.Find("Toggle_Tree").GetComponent<Toggle>().isOn) nextTowerObject = PurchaseObjects.Tree;
        else nextTowerObject = PurchaseObjects.None;
    }

    public void SetNextTowerObject_Bush()
    {
        audioSource = gameObject.GetComponent<AudioSource>(); 
        audioSource.PlayOneShot(sfx_MenuSelect);

        if (GameObject.Find("Toggle_Bush").GetComponent<Toggle>().isOn) nextTowerObject = PurchaseObjects.Bush;
        else nextTowerObject = PurchaseObjects.None;
    }

    public void SetNextTowerObject_Halfwall()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.PlayOneShot(sfx_MenuSelect);

        if(GameObject.Find("Toggle_Halfwall").GetComponent<Toggle>().isOn) nextTowerObject = PurchaseObjects.Halfwall;
        else nextTowerObject = PurchaseObjects.None;
    }

    public void SetNextTowerObject_Halfwall_90()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.PlayOneShot(sfx_MenuSelect);

        if(GameObject.Find("Toggle_Halfwall_90").GetComponent<Toggle>().isOn) nextTowerObject = PurchaseObjects.Halfwall_90;
        else nextTowerObject = PurchaseObjects.None;
    }

    public void SetNextTowerObject_Corner()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.PlayOneShot(sfx_MenuSelect);

        if (GameObject.Find("Toggle_Corner").GetComponent<Toggle>().isOn) nextTowerObject = PurchaseObjects.Corner;
        else nextTowerObject = PurchaseObjects.None;
    }

    void DisableOtherButtons(PurchaseObjects buttonType_)
    {
        GameObject.Find("Toggle_Bush").GetComponent<Toggle>().isOn = false;
        GameObject.Find("Toggle_Corner").GetComponent<Toggle>().isOn = false;
        GameObject.Find("Toggle_Halfwall").GetComponent<Toggle>().isOn = false;
        GameObject.Find("Toggle_Halfwall_90").GetComponent<Toggle>().isOn = false;
        GameObject.Find("Toggle_Wall").GetComponent<Toggle>().isOn = false;
        GameObject.Find("Toggle_Tree").GetComponent<Toggle>().isOn = false;
    }

    public void PlaySFX(bool b_True = true)
    {
        if(b_True)
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            audioSource.PlayOneShot(sfx_ChangePaintColor);
        }
        else
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            audioSource.PlayOneShot(sfx_DestroyObject);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        // Update mouse information
        SetMouseState();
        MoveCamera();

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            b_GameRunning = false;
            SetPauseMenu(!b_GameRunning);
        }

        if (Input.GetKeyDown(KeyCode.Space)) b_Camera_AttachedToMain = !b_Camera_AttachedToMain;

        Vector3 newPos;
        Quaternion newRot;

        if (b_Camera_AttachedToMain)
        {
            // Lerp to the cam reference's position
            newPos = Vector3.Lerp(gameObject.transform.position, Cam_Regular.transform.position, 0.1f);

            // Slerp to the cam reference's rotation
            newRot = Quaternion.Slerp(gameObject.transform.rotation, Cam_Regular.transform.rotation, 0.1f);
            
            // Set new information
            gameObject.transform.position = newPos;
            gameObject.transform.rotation = newRot;
        }
        else
        {
            // Lerp to the cam reference's position
            newPos = Vector3.Lerp(gameObject.transform.position, Cam_TopDown.transform.position, 0.1f);

            // Slerp to the cam reference's rotation
            newRot = Quaternion.Slerp(gameObject.transform.rotation, Cam_TopDown.transform.rotation, 0.1f);

            // Set new information
            gameObject.transform.position = newPos;
            gameObject.transform.rotation = newRot;
        }
        
        if(b_GameRunning)
        {
            if(mouseState == MouseState.Pressed && prevState != MouseState.Pressed)
            {
                RaycastHit hit;
                Ray ray = gameObject.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    Transform objectHit = hit.transform;

                    // Check to see if the object, when clicked, has an appropriate collider. If it does, attempt to create a wall.
                    if(objectHit.GetComponent<Cs_GridObjectLogic>())
                    {
                        // If the player clicks to want a wall
                        if(objectHit.GetComponent<Cs_GridObjectLogic>().Get_GridObjectState() == GridObjectState.On)
                        {
                            // We can, so buy a wall
                            // If NOT 'none'
                            if(nextTowerObject != PurchaseObjects.None)
                            {
                                audioSource.PlayOneShot(sfx_PlaceTerrain);

                                objectHit.GetComponent<Cs_GridObjectLogic>().Set_GridObjectType(nextTowerObject);
                            }
                        }
                        else if(objectHit.GetComponent<Cs_GridObjectLogic>().Get_GridObjectState() == GridObjectState.Active)
                        {
                            objectHit.GetComponent<Cs_GridObjectLogic>().ToggleGameObjects();
                        }
                        

                        // go_GridObjectList.GetComponent<Cs_GridLogic>().IncrementNumberOfTowers();
                    }
                }
            }

            // Store the previous state
            prevState = mouseState;
        }
    }

    public void SetPauseMenu(bool b_IsPaused_)
    {
        if (b_IsPaused_) Time.timeScale = 0; else Time.timeScale = 1;

        go_Canvas.SetActive(b_IsPaused_);

        b_GameRunning = !b_IsPaused_;
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    public void HowToPlay()
    {
        SceneManager.LoadScene("Menu_1");
        SceneManager.UnloadScene("Level");
    }
}
