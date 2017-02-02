using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.InteropServices;

enum Enum_IconTypes
{
    FirstBlood,
    Dragon,
    Tower,
    Inhib,
    Baron
}
enum Enum_IconOwner { None, Left, Right }

public class Icon
{
    public GameObject go_Icon;
    public bool b_IsActive;
    public bool b_CanMove;
    public int i_UIPos;
    public bool b_IsTeamLeft;
    public float currPos_X;
    public float startPos_X;
    public float finalPos_X;
}

public class Cs_SystemManager : MonoBehaviour
{
    TournamentInfo tourneyInfo;
    public string TournamentName;
    public string TeamName_Left;
    public string TeamName_Right;
    int TeamScore_Left;
    int TeamScore_Right;
    public bool SmallText;
    public bool NoIcon;
    public Sprite TournamentLogo;

    GameObject Nametag_Left;
    GameObject Nametag_Right;

    Icon Icon_FirstBlood = new Icon();
    Icon Icon_Dragon = new Icon();
    Icon Icon_Tower = new Icon();
    Icon Icon_Baron = new Icon();
    Icon Icon_Inhib = new Icon();
    Enum_IconOwner IO_FirstBlood = Enum_IconOwner.None;
    Enum_IconOwner IO_Dragon = Enum_IconOwner.None;
    Enum_IconOwner IO_Tower = Enum_IconOwner.None;
    Enum_IconOwner IO_Inhib = Enum_IconOwner.None;
    Enum_IconOwner IO_Baron = Enum_IconOwner.None;
    bool keyPressed_Dragon;
    bool keyPressed_Tower;
    bool keyPressed_FirstBlood;
    bool keyPressed_Inhib;
    bool keyPressed_Baron;

    int i_TeamIcons_Left = 0;
    int i_TeamIcons_Right = 0;

    float f_IconStopTimer_FirstBlood = 0;
    float f_IconStopTimer_Dragon = 0;
    float f_IconStopTimer_Tower = 0;
    float f_IconStopTimer_Inhib = 0;
    float f_IconStopTimer_Baron = 0;

    float f_TimeSinceLastButtonPress;

    [DllImport("User32.dll")]
    extern static uint GetRawInputDeviceList(IntPtr pRawInputDeviceList, ref uint uiNumDevices, uint cbSize);

    // Use this for initialization
    void Start ()
    {
        GameObject.Find("GREEN_SCREEN").SetActive(true);
        // SetTransparentBackground(true);
        
    }

    public void InitializeGame(TournamentInfo tourneyInfo_)
    {
        tourneyInfo = tourneyInfo_;

        TeamName_Left = tourneyInfo_.s_BluTeam;
        TeamName_Right = tourneyInfo_.s_RedTeam;
        TeamScore_Left = tourneyInfo_.i_BluScore;
        TeamScore_Right = tourneyInfo_.i_RedScore;
        TournamentName = tourneyInfo_.s_EventName;
        SmallText = !tourneyInfo_.b_EventTwoLines;
        NoIcon = !tourneyInfo_.b_ShowLogo;

        print(TournamentName);

        InitializeIcons();
        InitializeTeamNames();
        SetTournamentLogo();
    }

    void InitializeTeamNames()
    {
        GameObject.Find("Nametag_Left_Text").GetComponent<TextMesh>().text = TeamName_Left;
        GameObject.Find("Nametag_Right_Text").GetComponent<TextMesh>().text = TeamName_Right;
        GameObject.Find("Nametag_Left_Score").GetComponent<TextMesh>().text = TeamScore_Left.ToString();
        GameObject.Find("Nametag_Right_Score").GetComponent<TextMesh>().text = TeamScore_Right.ToString();
    }

    void SetTransparentBackground(bool b_IsEnabled)
    {
        GameObject.Find("GREEN_SCREEN").GetComponent<Cs_TransparentWindow>().enabled = b_IsEnabled;

        if(!b_IsEnabled) GameObject.Find("GREEN_SCREEN").GetComponent<Cs_TransparentWindow>().EndTransparentWindow();
        if (!b_IsEnabled) GameObject.Find("GREEN_SCREEN").SetActive(false);
    }

    void InitializeIcons()
    {
        // Set Icons
        Icon_FirstBlood.go_Icon = GameObject.Find("Icon_FirstBlood");
        Icon_Dragon.go_Icon = GameObject.Find("Icon_Dragon");
        Icon_Tower.go_Icon = GameObject.Find("Icon_Tower");
        Icon_Baron.go_Icon = GameObject.Find("Icon_Baron");
        Icon_Inhib.go_Icon = GameObject.Find("Icon_Inhib");


        // Turn off Icons
        Icon_FirstBlood.go_Icon.GetComponent<SpriteRenderer>().enabled = false;
        Icon_FirstBlood.b_IsActive = false;

        Icon_Dragon.go_Icon.GetComponent<SpriteRenderer>().enabled = false;
        Icon_Dragon.b_IsActive = false;

        Icon_Tower.go_Icon.GetComponent<SpriteRenderer>().enabled = false;
        Icon_Dragon.b_IsActive = false;

        Icon_Baron.go_Icon.GetComponent<SpriteRenderer>().enabled = false;
        Icon_Dragon.b_IsActive = false;

        Icon_Inhib.go_Icon.GetComponent<SpriteRenderer>().enabled = false;
        Icon_Dragon.b_IsActive = false;
    }

    void SetTournamentLogo()
    {
        GameObject tournamentLogo = GameObject.Find("Overlay_Logo");
        GameObject tournamentText = GameObject.Find("Overlay_Text");

        // Reposition text based on bools
        if (NoIcon)
        {
            tournamentText.transform.localPosition = new Vector3(0, 0, 0);
            tournamentLogo.SetActive(false);
        }

        if(SmallText)
        {
            tournamentText.transform.localScale = new Vector3(0.001f, 0.001f, 1);
        }

        tournamentLogo.GetComponent<SpriteRenderer>().sprite = TournamentLogo;
        
        if(TournamentName.Contains("/"))
        {
            TournamentName = TournamentName.Replace("/", "\n");
        }

        tournamentText.GetComponent<TextMesh>().text = TournamentName;
    }

    // Used by Keyboard Input to apply Icon's to the screen
    void ActivateIcon(Enum_IconTypes iconType_, Enum_IconOwner iconOwner_, float f_DT)
    {
        // Pre-loading generic Icon for manipulation
        Icon currentIcon = Icon_FirstBlood;

        // Load icon to check against
        if (iconType_ == Enum_IconTypes.Baron) currentIcon = Icon_Baron;
        if (iconType_ == Enum_IconTypes.Dragon) currentIcon = Icon_Dragon;
        if (iconType_ == Enum_IconTypes.Tower) currentIcon = Icon_Tower;
        if (iconType_ == Enum_IconTypes.Inhib) currentIcon = Icon_Inhib;

        #region Enable the Icon
        // If the icon is still disabled...
        if (!currentIcon.b_IsActive)
        {
            // Figure out which position the icon will go in
            if(iconOwner_ == Enum_IconOwner.Left)
            {
                // Reposition the icon based on the current i_TeamIcons_Left/Right number
                // 520 - 90 * i_TeamIcons. Right Team *= -1;
                var finalPos = currentIcon.go_Icon.gameObject.transform.position;
                finalPos.x = -520 + (90 * i_TeamIcons_Left);
                currentIcon.finalPos_X = finalPos.x;

                currentIcon.currPos_X = currentIcon.finalPos_X + 90;
                currentIcon.startPos_X = currentIcon.currPos_X;
                currentIcon.go_Icon.transform.position = new Vector3(currentIcon.currPos_X, currentIcon.go_Icon.transform.position.y, currentIcon.go_Icon.transform.position.z);

                // Scale the current icon to double normal size
                var currScale = currentIcon.go_Icon.transform.localScale;
                currScale.x *= 1.4f;
                currScale.y *= 1.4f;
                currentIcon.go_Icon.transform.localScale = currScale;

                // Allow the icon to move
                currentIcon.b_CanMove = true;

                // Increment the i_TeamIcons
                ++i_TeamIcons_Left;
            }
            else if(iconOwner_ == Enum_IconOwner.Right)
            {
                var finalPos = currentIcon.go_Icon.gameObject.transform.position;
                finalPos.x = 520 - (90 * i_TeamIcons_Right);
                currentIcon.finalPos_X = finalPos.x;

                currentIcon.currPos_X = currentIcon.finalPos_X - 90;
                currentIcon.startPos_X = currentIcon.currPos_X;
                currentIcon.go_Icon.transform.position = new Vector3(currentIcon.currPos_X, currentIcon.go_Icon.transform.position.y, currentIcon.go_Icon.transform.position.z);

                // Scale the current icon to double normal size
                var currScale = currentIcon.go_Icon.transform.localScale;
                currScale.x *= 1.4f;
                currScale.y *= 1.4f;
                currentIcon.go_Icon.transform.localScale = currScale;

                // Allow the icon to move
                currentIcon.b_CanMove = true;

                // Increment the i_TeamIcons
                ++i_TeamIcons_Right;
            }
            
            // Enable & place the icon
            currentIcon.go_Icon.GetComponent<SpriteRenderer>().enabled = true;
            currentIcon.b_IsActive = true;

            // Change the icon's transparency
            var currAlpha = currentIcon.go_Icon.GetComponent<SpriteRenderer>().color;
            currAlpha.a = 0f;
            currentIcon.go_Icon.GetComponent<SpriteRenderer>().color = currAlpha;
        }
        #endregion

        #region Move the Enabled Icons
        if(currentIcon.b_CanMove && currentIcon.b_IsActive)
        {
            bool b_IsMoving = false;

            var currAlpha = currentIcon.go_Icon.GetComponent<SpriteRenderer>().color;
            currAlpha.a += f_DT * 5;
            currentIcon.go_Icon.GetComponent<SpriteRenderer>().color = currAlpha;

            if (iconType_ == Enum_IconTypes.FirstBlood) if (f_IconStopTimer_FirstBlood < 0.2f) f_IconStopTimer_FirstBlood += f_DT;
            if (iconType_ == Enum_IconTypes.Dragon) if (f_IconStopTimer_Dragon < 0.2f) f_IconStopTimer_Dragon += f_DT;
            if (iconType_ == Enum_IconTypes.Tower) if (f_IconStopTimer_Tower < 0.2f) f_IconStopTimer_Tower += f_DT;
            if (iconType_ == Enum_IconTypes.Inhib) if (f_IconStopTimer_Inhib < 0.2f) f_IconStopTimer_Inhib += f_DT;
            if (iconType_ == Enum_IconTypes.Baron) if (f_IconStopTimer_Baron < 0.2f) f_IconStopTimer_Baron += f_DT;

            if (f_IconStopTimer_FirstBlood >= 0.2 && iconType_ == Enum_IconTypes.FirstBlood) b_IsMoving = true;
            if (f_IconStopTimer_Dragon >= 0.2 && iconType_ == Enum_IconTypes.Dragon) b_IsMoving = true;
            if (f_IconStopTimer_Tower >= 0.2 && iconType_ == Enum_IconTypes.Tower) b_IsMoving = true;
            if (f_IconStopTimer_Baron >= 0.2 && iconType_ == Enum_IconTypes.Baron) b_IsMoving = true;
            if (f_IconStopTimer_Inhib >= 0.2 && iconType_ == Enum_IconTypes.Inhib) b_IsMoving = true;

            if (b_IsMoving)
            {
                // Rescale the icon down to normal size
                if(currentIcon.go_Icon.transform.localScale.x > 15)
                {
                    float currPos = currentIcon.go_Icon.transform.position.x;
                    float minPos = currentIcon.finalPos_X;
                    float maxPos = currentIcon.startPos_X;

                    float currPercent = (currPos - minPos) / (maxPos - minPos);

                    var currScale = currentIcon.go_Icon.transform.localScale;
                    currScale.x = 15 + (15 * currPercent);
                    currScale.y = 15 + (15 * currPercent);
                    currentIcon.go_Icon.transform.localScale = currScale;
                }

                if (iconOwner_ == Enum_IconOwner.Left) // If we're on the left side, move left over time
                {
                    if (currentIcon.go_Icon.transform.position.x > currentIcon.finalPos_X)
                    {
                        currentIcon.currPos_X = currentIcon.go_Icon.transform.position.x - (f_DT * 250);
                        currentIcon.go_Icon.transform.position = new Vector3(currentIcon.currPos_X, currentIcon.go_Icon.transform.position.y, currentIcon.go_Icon.transform.position.z);
                    }
                    else currentIcon.b_CanMove = false; // When we reached the limit, disable the ability to move (Stops updating the position number each frame)
                }
                else if (iconOwner_ == Enum_IconOwner.Right)
                {
                    if (currentIcon.go_Icon.transform.position.x < currentIcon.finalPos_X)
                    {
                        currentIcon.currPos_X = currentIcon.go_Icon.transform.position.x + (f_DT * 250);
                        currentIcon.go_Icon.transform.position = new Vector3(currentIcon.currPos_X, currentIcon.go_Icon.transform.position.y, currentIcon.go_Icon.transform.position.z);
                    }
                    else currentIcon.b_CanMove = false; // When we reached the limit, disable the ability to move (Stops updating the position number each frame)
                }
            }
            
            // currentIcon.go_Icon.transform.position = Vector3.Lerp(new Vector3(currentIcon.currPos_X, currentIcon.go_Icon.transform.position.y, currentIcon.go_Icon.transform.position.z), new Vector3(currentIcon.finalPos_X, currentIcon.go_Icon.transform.position.y, currentIcon.go_Icon.transform.position.z), 1.0f);
            
        }
        #endregion
    }

    // Update is called once per frame
    void Update ()
    {

        #region Scale the Icon's Alpha Channels
        if(f_TimeSinceLastButtonPress >= 5.0f)
        {
            if(Icon_FirstBlood.b_IsActive)
            {
                var currAlpha = Icon_FirstBlood.go_Icon.GetComponent<SpriteRenderer>().color;
                if (currAlpha.a >= 0.5f)
                {
                    currAlpha.a -= Time.deltaTime;
                }
                Icon_FirstBlood.go_Icon.GetComponent<SpriteRenderer>().color = currAlpha;
            }
            if (Icon_Dragon.b_IsActive)
            {
                var currAlpha = Icon_Dragon.go_Icon.GetComponent<SpriteRenderer>().color;
                if (currAlpha.a >= 0.5f)
                {
                    currAlpha.a -= Time.deltaTime;
                }
                Icon_Dragon.go_Icon.GetComponent<SpriteRenderer>().color = currAlpha;
            }
            if (Icon_Tower.b_IsActive)
            {
                var currAlpha = Icon_Tower.go_Icon.GetComponent<SpriteRenderer>().color;
                if (currAlpha.a >= 0.5f)
                {
                    currAlpha.a -= Time.deltaTime;
                }
                Icon_Tower.go_Icon.GetComponent<SpriteRenderer>().color = currAlpha;
            }
            if (Icon_Inhib.b_IsActive)
            {
                var currAlpha = Icon_Inhib.go_Icon.GetComponent<SpriteRenderer>().color;
                if (currAlpha.a >= 0.5f)
                {
                    currAlpha.a -= Time.deltaTime;
                }
                Icon_Inhib.go_Icon.GetComponent<SpriteRenderer>().color = currAlpha;
            }
            if (Icon_Baron.b_IsActive)
            {
                var currAlpha = Icon_Baron.go_Icon.GetComponent<SpriteRenderer>().color;
                if (currAlpha.a >= 0.5f)
                {
                    currAlpha.a -= Time.deltaTime;
                }
                Icon_Baron.go_Icon.GetComponent<SpriteRenderer>().color = currAlpha;
            }
        }
        else
        {
            f_TimeSinceLastButtonPress += Time.deltaTime;

            if (Icon_FirstBlood.b_IsActive)
            {
                var currAlpha = Icon_FirstBlood.go_Icon.GetComponent<SpriteRenderer>().color;
                if (currAlpha.a <= 1.0f)
                {
                    currAlpha.a += Time.deltaTime;
                }
                Icon_FirstBlood.go_Icon.GetComponent<SpriteRenderer>().color = currAlpha;
            }
            if (Icon_Dragon.b_IsActive)
            {
                var currAlpha = Icon_Dragon.go_Icon.GetComponent<SpriteRenderer>().color;
                if (currAlpha.a <= 1.0f)
                {
                    currAlpha.a += Time.deltaTime;
                }
                Icon_Dragon.go_Icon.GetComponent<SpriteRenderer>().color = currAlpha;
            }
            if (Icon_Tower.b_IsActive)
            {
                var currAlpha = Icon_Tower.go_Icon.GetComponent<SpriteRenderer>().color;
                if (currAlpha.a <= 1.0f)
                {
                    currAlpha.a += Time.deltaTime;
                }
                Icon_Tower.go_Icon.GetComponent<SpriteRenderer>().color = currAlpha;
            }
            if (Icon_Inhib.b_IsActive)
            {
                var currAlpha = Icon_Inhib.go_Icon.GetComponent<SpriteRenderer>().color;
                if (currAlpha.a <= 1.0f)
                {
                    currAlpha.a += Time.deltaTime;
                }
                Icon_Inhib.go_Icon.GetComponent<SpriteRenderer>().color = currAlpha;
            }
            if (Icon_Baron.b_IsActive)
            {
                var currAlpha = Icon_Baron.go_Icon.GetComponent<SpriteRenderer>().color;
                if (currAlpha.a <= 1.0f)
                {
                    currAlpha.a += Time.deltaTime;
                }
                Icon_Baron.go_Icon.GetComponent<SpriteRenderer>().color = currAlpha;
            }
        }
        #endregion

        // Return to Main Menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetTransparentBackground(false);
            GameObject.Destroy(GameObject.Find("TournamentOptions"));
            SceneManager.LoadScene("MainMenu");
        }

        // Left Team Input
        if (Input.GetKeyDown(KeyCode.Q) && !keyPressed_FirstBlood) { IO_FirstBlood = Enum_IconOwner.Left; keyPressed_FirstBlood = true; f_TimeSinceLastButtonPress = 0.0f; }
        if (Input.GetKeyDown(KeyCode.E) && !keyPressed_Dragon) { IO_Dragon = Enum_IconOwner.Left; keyPressed_Dragon = true; f_TimeSinceLastButtonPress = 0.0f; }
        if (Input.GetKeyDown(KeyCode.W) && !keyPressed_Tower) { IO_Tower = Enum_IconOwner.Left; keyPressed_Tower = true; f_TimeSinceLastButtonPress = 0.0f; }
        if (Input.GetKeyDown(KeyCode.T) && !keyPressed_Inhib) { IO_Inhib = Enum_IconOwner.Left; keyPressed_Inhib = true; f_TimeSinceLastButtonPress = 0.0f; }
        if (Input.GetKeyDown(KeyCode.R) && !keyPressed_Baron) { IO_Baron = Enum_IconOwner.Left; keyPressed_Baron = true; f_TimeSinceLastButtonPress = 0.0f; }

        // Right Team Input
        if (Input.GetKeyDown(KeyCode.P) && !keyPressed_FirstBlood) { IO_FirstBlood = Enum_IconOwner.Right; keyPressed_FirstBlood = true; f_TimeSinceLastButtonPress = 0.0f; }
        if (Input.GetKeyDown(KeyCode.I) && !keyPressed_Dragon) { IO_Dragon = Enum_IconOwner.Right; keyPressed_Dragon = true; f_TimeSinceLastButtonPress = 0.0f; }
        if (Input.GetKeyDown(KeyCode.O) && !keyPressed_Tower) { IO_Tower = Enum_IconOwner.Right; keyPressed_Tower = true; f_TimeSinceLastButtonPress = 0.0f; }
        if (Input.GetKeyDown(KeyCode.Y) && !keyPressed_Inhib) { IO_Inhib = Enum_IconOwner.Right; keyPressed_Inhib = true; f_TimeSinceLastButtonPress = 0.0f; }
        if (Input.GetKeyDown(KeyCode.U) && !keyPressed_Baron) { IO_Baron = Enum_IconOwner.Right; keyPressed_Baron = true; f_TimeSinceLastButtonPress = 0.0f; }

        // Control icons each frame
        if (IO_FirstBlood != Enum_IconOwner.None)
        {
            if (IO_FirstBlood == Enum_IconOwner.Left) ActivateIcon(Enum_IconTypes.FirstBlood, Enum_IconOwner.Left, Time.deltaTime);
            else ActivateIcon(Enum_IconTypes.FirstBlood, Enum_IconOwner.Right, Time.deltaTime);
        }
        if (IO_Dragon != Enum_IconOwner.None)
        {
            if (IO_Dragon == Enum_IconOwner.Left) ActivateIcon(Enum_IconTypes.Dragon, Enum_IconOwner.Left, Time.deltaTime);
            else ActivateIcon(Enum_IconTypes.Dragon, Enum_IconOwner.Right, Time.deltaTime);
        }
        if (IO_Tower != Enum_IconOwner.None)
        {
            if (IO_Tower == Enum_IconOwner.Left) ActivateIcon(Enum_IconTypes.Tower, Enum_IconOwner.Left, Time.deltaTime);
            else ActivateIcon(Enum_IconTypes.Tower, Enum_IconOwner.Right, Time.deltaTime);
        }
        if (IO_Inhib != Enum_IconOwner.None)
        {
            if (IO_Inhib == Enum_IconOwner.Left) ActivateIcon(Enum_IconTypes.Inhib, Enum_IconOwner.Left, Time.deltaTime);
            else ActivateIcon(Enum_IconTypes.Inhib, Enum_IconOwner.Right, Time.deltaTime);
        }
        if (IO_Baron != Enum_IconOwner.None)
        {
            if (IO_Baron == Enum_IconOwner.Left) ActivateIcon(Enum_IconTypes.Baron, Enum_IconOwner.Left, Time.deltaTime);
            else ActivateIcon(Enum_IconTypes.Baron, Enum_IconOwner.Right, Time.deltaTime);
        }

    }
}
