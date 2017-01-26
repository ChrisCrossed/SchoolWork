using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Cs_PlayerController : MonoBehaviour
{
    bool b_IsGameOver;
    float f_GameOverTimer;

    GameObject go_Camera;
    [SerializeField] GameObject go_RaycastObj_Use;
    GameObject go_UseObject;
    GameObject ui_Reticle;

    float f_Speed_Curr;
    float f_Speed_Max = 3f;
    float f_Speed_Acceleration = 15f;

    float f_TimeToMaxSpeed = 4f;

    [SerializeField] float f_JumpHeight = 1.0f;
    float f_yPos_Ground;
    float f_yPos_Jump;

    bool b_HasBook;
    GameObject go_Book;

	// Use this for initialization
	void Start ()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        go_Camera = gameObject.transform.Find("Main Camera").gameObject;
        ui_Reticle = GameObject.Find("UI_Raycast").gameObject;

        f_yPos_Ground = gameObject.transform.position.y;
        f_yPos_Jump = f_yPos_Ground + f_JumpHeight;

        go_Book = transform.Find("Main Camera").Find("BookPos").Find("Book").gameObject;
        BookState = false;
	}

    float f_AngleVert;
    float f_AngleHoriz;
    void MouseInput()
    {
        // Horizontal Mouse Input
        f_AngleHoriz = Input.GetAxis("Mouse X");

        // Rotate player object for AngleHoriz
        Vector3 v3_PlayerRot = gameObject.transform.eulerAngles;
        v3_PlayerRot.y += f_AngleHoriz;
        gameObject.transform.eulerAngles = v3_PlayerRot;

        // Vertical Mouse Input
        f_AngleVert -= Input.GetAxis("Mouse Y");
        f_AngleVert = Mathf.Clamp(f_AngleVert, -80f, 80f);

        // Rotate Camera object for AngleVert
        Vector3 v3_CamRot = go_Camera.transform.eulerAngles;
        v3_CamRot.x = f_AngleVert;
        go_Camera.transform.eulerAngles = v3_CamRot;
    }

    void PlayerInput()
    {
        // Movement Vector
        Vector3 v3_InputVector = new Vector3();

        float f_SpeedTemp = f_Speed_Max;
        
        // Sprint. Move fast.
        if(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift))
        {
            f_SpeedTemp *= 2f;
        }

        if( Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) )
        {
            v3_InputVector.z = 1;
        }
        else if( Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W) )
        {
            v3_InputVector.z = -1;
        }

        if( Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) )
        {
            v3_InputVector.x = -1;
        }
        else if( Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) )
        {
            v3_InputVector.x = 1;
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            Use_InteractObject();
        }

        if(Input.GetKey(KeyCode.Space))
        {
            Set_Jump( true );

            // Forcibly resets the player's velocity, slowing/stopping them
            v3_InputVector = new Vector3();
        }
        else
        {
            Set_Jump( false );
        }

        if( v3_InputVector != new Vector3())
        {
            ManageSpeed( true );

            v3_InputVector.Normalize();
        }
        else
        {
            ManageSpeed( false );
        }

        Vector3 v3_PrevVelocity = gameObject.GetComponent<Rigidbody>().velocity;

        Vector3 v3_FinalVelocity = gameObject.transform.rotation * v3_InputVector;

        Vector3 v3_NewVelocity = Vector3.Lerp(v3_PrevVelocity, v3_FinalVelocity * f_SpeedTemp, Time.deltaTime * f_Speed_Acceleration);

        gameObject.GetComponent<Rigidbody>().velocity = v3_NewVelocity;
    }

    void ManageSpeed( bool b_IncreaseSpeed_ = false )
    {
        if(b_IncreaseSpeed_)
        {
            f_Speed_Curr += Time.deltaTime * f_Speed_Acceleration;

            if (f_Speed_Curr > f_Speed_Max) f_Speed_Curr = f_Speed_Max;
        }
        else
        {
            f_Speed_Curr -= Time.deltaTime * f_Speed_Acceleration;

            if (f_Speed_Curr < 0 ) f_Speed_Curr = 0f;
        }
    }

    public bool BookState
    {
        set
        {
            b_HasBook = value;

            if(b_HasBook)
            {
                go_Book.GetComponent<MeshRenderer>().enabled = true;

                if(GameObject.Find("Book_Desk_1").GetComponent<Cs_BookLogic>().Get_Enabled())
                {
                    GameObject.Find("Book_Desk_1").GetComponent<Cs_BookLogic>().ArrowState = true;
                }
                else if (GameObject.Find("Book_Desk_2").GetComponent<Cs_BookLogic>().Get_Enabled())
                {
                    GameObject.Find("Book_Desk_2").GetComponent<Cs_BookLogic>().ArrowState = true;
                }
                else if(GameObject.Find("Book_Desk_3").GetComponent<Cs_BookLogic>().Get_Enabled())
                {
                    GameObject.Find("Book_Desk_3").GetComponent<Cs_BookLogic>().ArrowState = true;
                }
            }
            else
            {
                go_Book.GetComponent<MeshRenderer>().enabled = false;
            }
        }
        get { return b_HasBook; }
    }

    public bool GameOverState
    {
        set { b_IsGameOver = value; }
        get { return b_IsGameOver; }
    }

    #region Jump abilities
    float f_LerpTimer;
    bool b_PrevJumpState;
    void Set_Jump( bool b_IsJumping_ )
    {
        Vector3 v3_CurrPos = gameObject.transform.position;

        if(b_PrevJumpState != b_IsJumping_)
        {
            // f_LerpTimer += Time.deltaTime;
            f_LerpTimer = 0.0f;
        }
        else
        {
            f_LerpTimer += Time.deltaTime / 4;
        }

        if( b_IsJumping_ )
        {
            if(v3_CurrPos.y < f_yPos_Jump)
            {
                v3_CurrPos.y += f_LerpTimer;
                if (v3_CurrPos.y > f_yPos_Jump) v3_CurrPos.y = f_yPos_Jump;
            }
        }
        else
        {
            if(v3_CurrPos.y > f_yPos_Ground)
            {
                v3_CurrPos.y -= f_LerpTimer;
                if (v3_CurrPos.y < f_yPos_Ground) v3_CurrPos.y = f_yPos_Ground;
            }
        }

        if(v3_CurrPos != gameObject.transform.position)
        {
            gameObject.transform.position = v3_CurrPos;
        }

        b_PrevJumpState = b_IsJumping_;
    }
    #endregion

    void RaycastThroughCanvas()
    {
        RaycastHit hit;
        int i_LayerMask = LayerMask.GetMask("Wall", "Boss", "Interact", "Default");

        Physics.Raycast(go_Camera.transform.position, go_Camera.transform.forward, out hit, 5f, i_LayerMask);

        if(hit.collider)
        {
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Interact"))
            {
                go_UseObject = hit.collider.gameObject;

                // Change color of reticle
                ui_Reticle.GetComponent<Image>().color = new Color(1, 0, 0);
            }
            else
            {
                // Reset color of reticle
                ui_Reticle.GetComponent<Image>().color = new Color(1, 1, 1);

                // Disable the ability to 'use' an object
                go_UseObject = null;
            }
        }
        else
        {
            // Reset color of reticle
            ui_Reticle.GetComponent<Image>().color = new Color(1, 1, 1);

            // Disable the ability to 'use' an object
            go_UseObject = null;
        }
    }

    void Use_InteractObject()
    {
        if(go_UseObject != null)
        {
            // Check the object's scripts to see what type of object it is
            if(go_UseObject.GetComponent<Cs_Phone>())
            {
                go_UseObject.GetComponent<Cs_Phone>().Use();
            }
            else if(go_UseObject.GetComponent<Cs_KeyboardLogic_Key>())
            {
                go_UseObject.GetComponent<Cs_KeyboardLogic_Key>().Use();
            }
            else if(go_UseObject.GetComponent<Cs_RadioLogic>())
            {
                go_UseObject.GetComponent<Cs_RadioLogic>().Use();
            }
            else if(go_UseObject.GetComponent<Cs_BookLogic>())
            {
                // Disable only the main desk book when we do not have a book
                if(!BookState)
                {
                    if( go_UseObject.GetComponent<Cs_BookLogic>().IsMainBook() )
                    {
                        go_UseObject.GetComponent<Cs_BookLogic>().BookEnabled(false);

                        BookState = true;
                    }
                }
                // If this desk book is not the main and we have a book, place it and remove the model from our hands
                else
                {
                    if( !go_UseObject.GetComponent<Cs_BookLogic>().IsMainBook() )
                    {
                        go_UseObject.GetComponent<Cs_BookLogic>().BookEnabled(true, false);

                        BookState = false;
                    }
                }
            }
            else if(go_UseObject.GetComponent<Cs_Objective>())
            {
                go_UseObject.GetComponent<Cs_Objective>().Use();
            }

        }
    }

    // Update is called once per frame
    void Update ()
    {
        // Quit to Menu
        if (Input.GetKeyDown(KeyCode.Escape)) SceneManager.LoadScene(1);

        if (!GameOverState)
        {
            PlayerInput();

            RaycastThroughCanvas();

            if(Input.GetKeyDown(KeyCode.L))
            {
                BookState = !BookState;
            }
        }
    }

    bool b_MusicPlayedYet;
    void LateUpdate()
    {
        if (!GameOverState)
        {
            MouseInput();

            // Fade In the screen if need-be
            Color clr_CurrAlpha = GameObject.Find("FadeInOut").GetComponent<Image>().color;
            clr_CurrAlpha.a -= Time.deltaTime / 4f;
            if (clr_CurrAlpha.a < 0.0f) clr_CurrAlpha.a = 0.0f;
            GameObject.Find("FadeInOut").GetComponent<Image>().color = clr_CurrAlpha;
        }
        else
        {
            GameObject.Find("FaxMachine").GetComponent<AudioSource>().volume = 0f;

            f_GameOverTimer += Time.deltaTime;

            gameObject.GetComponent<Rigidbody>().velocity = new Vector3();

            Color clr_CurrAlpha = GameObject.Find("FadeInOut").GetComponent<Image>().color;
            clr_CurrAlpha.a += Time.deltaTime / 4f;
            if (clr_CurrAlpha.a > 0.75f)
            {
                clr_CurrAlpha.r += Time.deltaTime / 2f;

                Color clr_HammerAlpha = GameObject.Find("Img_HammerSickle").GetComponent<Image>().color;
                clr_HammerAlpha.a += Time.deltaTime / 2;
                GameObject.Find("Img_HammerSickle").GetComponent<Image>().color = clr_HammerAlpha;

                if (!b_MusicPlayedYet)
                {
                    gameObject.GetComponent<AudioSource>().PlayOneShot(Resources.Load("mus_Anthem") as AudioClip);
                    b_MusicPlayedYet = true;
                }
            }

            GameObject.Find("FadeInOut").GetComponent<Image>().color = clr_CurrAlpha;

            if (f_GameOverTimer > 30f) GameObject.Find("GameOverText").GetComponent<Text>().enabled = true;

            if (f_GameOverTimer > 45f) GameObject.Find("GameOverText").GetComponent<Text>().text = "Press Escape.";
            if (f_GameOverTimer > 60f) GameObject.Find("GameOverText").GetComponent<Text>().text = "OK. We'll just sit here.";
            if (f_GameOverTimer > 75f) GameObject.Find("GameOverText").GetComponent<Text>().text = "All Glory to the Motherland.";
            if (f_GameOverTimer > 75f) GameObject.Find("GameOverText").GetComponent<Text>().text = "All Glory to the Motherland. Comrade.";
        }
    }
}
