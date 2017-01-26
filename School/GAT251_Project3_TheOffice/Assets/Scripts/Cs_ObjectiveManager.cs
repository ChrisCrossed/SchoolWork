using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Cs_ObjectiveManager : MonoBehaviour
{
    public bool b_JobTestEnvironment = false;

    bool b_ClockedIn;

    int i_NumTasks = 5;

    Text txt_JobList_1_Text;
    Text txt_JobList_2_Text;
    Text txt_JobList_3_Text;
    Text txt_JobList_4_Text;
    Text txt_JobList_5_Text;

    string[] s_JobList = new string[15];
    string s_TurnInJob = "[TURN IN]";
    Cs_RotArrow PhoneArrow;

    #region Task - 'Boss Kick Me'
    bool b_Job_BossKickMe;          // 0
    GameObject go_BossSign;
    #endregion

    #region Task - 'Change Radio Station'
    bool b_Job_ChangeRadioStation;  // 1
    [SerializeField] GameObject[] go_RadioList;
    #endregion

    #region Task - 'Punch In'
    public bool b_Job_PunchIn;
    GameObject go_PunchInClock;
    Text txt_TutorialText;
    #endregion

    #region Task - 'Fire People'
    bool b_Job_FirePeople;
    int i_PeopleFired;
    [SerializeField] GameObject[] go_Employees = new GameObject[6];
    #endregion

    #region Task - 'Give Communist Manifesto'
    bool b_Job_GiveManifesto;
    [SerializeField] GameObject[] go_Books = new GameObject[4];
    #endregion

    #region Task - 'Send Fax'
    bool b_Job_SendFax;
    GameObject go_FaxMachine;
    #endregion

    // Use this for initialization
    void Start ()
    {
        #region Initialize HUD Text
        txt_JobList_1_Text = GameObject.Find("JobList_1_Text").GetComponent<Text>();
        txt_JobList_2_Text = GameObject.Find("JobList_2_Text").GetComponent<Text>();
        txt_JobList_3_Text = GameObject.Find("JobList_3_Text").GetComponent<Text>();
        txt_JobList_4_Text = GameObject.Find("JobList_4_Text").GetComponent<Text>();
        txt_JobList_5_Text = GameObject.Find("JobList_5_Text").GetComponent<Text>();
        #endregion

        #region Task - 'Boss Kick Me'
        go_BossSign = GameObject.Find("Boss").transform.Find("BackMessage").gameObject;
        #endregion

        #region Task - 'Punch In'
        go_PunchInClock = GameObject.Find("PunchInClock");
        txt_TutorialText = GameObject.Find("TutorialText").GetComponent<Text>();
        #endregion

        #region Task - 'Fire People'
        i_PeopleFired = 0;
        #endregion

        #region Task - 'Send Fax'
        go_FaxMachine = GameObject.Find("FaxMachine");
        #endregion

        PhoneArrow = GameObject.Find("Phone").transform.Find("RotArrow").GetComponent<Cs_RotArrow>();

        // CreateNewJob();
        Init_PunchIn();

        Set_TaskText();
    }

    public void Set_IncrementPeopleFired()
    {
        ++i_PeopleFired;

        if(i_PeopleFired >= 5)
        {
            Complete_FirePeople();
        }
    }

    int i_NumAttempts;
    bool b_DayComplete = false;
    public void CreateNewJob()
    {
        int i_Hours = gameObject.GetComponent<Cs_LevelManager>().GetTime_Hours;
        if( i_Hours < 8 && i_Hours > 3)
        {
            if(i_Hours == 4 && !b_DayComplete)
            {
                txt_TutorialText.text = "Finish your work and\nclock out!";

                b_DayComplete = true;

                return;
            }
        }

        if (b_DayComplete) return;

        bool b_JobFound = false;
        i_NumAttempts = 0;

        while(!b_JobFound)
        {
            int i_JobNumber = Random.Range(0, i_NumTasks);

            switch(i_JobNumber)
            {
                // Boss Kick Me Sign
                case 0:
                    if(!b_Job_BossKickMe)
                    {
                        Init_BossKickMe();
                        b_JobFound = true;
                    }
                    break;

                // Change Radio Station
                case 1:
                    if (!b_Job_ChangeRadioStation)
                    {
                        Init_ChangeRadioStation();
                        b_JobFound = true;
                    }
                    break;

                // Fire 5 people
                case 2:
                    if (!b_Job_FirePeople)
                    {
                        Init_FirePeople();
                        b_JobFound = true;
                    }
                    break;

                // Communist Manifesto
                case 3:
                    if (!b_Job_GiveManifesto)
                    {
                        Init_Book();
                        b_JobFound = true;
                    }
                    break;

                // Communist Manifesto
                case 4:
                    if (!b_Job_SendFax)
                    {
                        Init_SendFax();
                        b_JobFound = true;
                    }
                    break;
            }

            ++i_NumAttempts;

            if (i_NumAttempts >= i_NumTasks) break;
        }
    }

    public bool ClockIn
    {
        set
        {
            b_ClockedIn = value;

            if (b_ClockedIn) gameObject.GetComponent<Cs_LevelManager>().Set_AllowedToContinue();
        }
        get
        {
            return b_ClockedIn;
        }
    }

    public bool ClockOutStatus
    {
        get { return b_DayComplete; }
    }

    public bool CheckForGameOver()
    {
        for(int i_ = 0; i_ < s_JobList.Length; ++i_)
        {
            if(s_JobList[i_] != "")
            {
                txt_TutorialText.text = "Comrade! Work must be done!";
                return false;
            }
        }

        return true;
    }

    #region Punch In
    int i_PunchIn_Number = -1;
    string s_PunchIn_Text = "Punch In Comrade!";
    void Init_PunchIn()
    {
        go_PunchInClock.GetComponent<Cs_Objective>().Set_State = Enum_ObjectiveState.InProgress;

        Set_TaskText(s_PunchIn_Text);
    }
    public void Complete_PunchIn()
    {
        if (!b_Job_PunchIn)
        {
            b_Job_PunchIn = true;

            // b_Job_PunchIn = false; (Handled within TurnInTasks)
            if (i_PunchIn_Number >= 0) s_JobList[ i_PunchIn_Number ] = s_TurnInJob;
            Set_TaskText();
            i_PunchIn_Number = -1;
            PhoneArrow.IsEnabled = true;

            txt_TutorialText.text = "Return to your Red Phone\nand use it between tasks!";
        }
    }
    #endregion

    #region 'Kick Me' On Boss
    int i_BossKickMe_Number = -1;
    string s_BossKickMe_Text = "Put Sign on Boss's Back";
    void Init_BossKickMe()
    {
        b_Job_BossKickMe = true;
        go_BossSign.GetComponent<Cs_Objective>().Set_State = Enum_ObjectiveState.InProgress;
        
        Set_TaskText(s_BossKickMe_Text);
    }
    public void Complete_BossKickMe()
    {
        if(b_Job_BossKickMe)
        {
            b_Job_BossKickMe = false;
            if(i_BossKickMe_Number >= 0) s_JobList[ i_BossKickMe_Number ] = s_TurnInJob;
            Set_TaskText();
            i_BossKickMe_Number = -1;
            PhoneArrow.IsEnabled = true;

            go_BossSign.GetComponent<Cs_Objective>().Set_State = Enum_ObjectiveState.Completed;
        }
    }
    #endregion

    #region Change Radio Station
    int i_ChangeRadioStation_Number = -1;
    string s_ChangeRadioStation_Text = "Play Soviet\nAppropriate Music";
    void Init_ChangeRadioStation()
    {
        b_Job_ChangeRadioStation = true;
        
        // Loop through all radios and reset them
        for(int i_ = 0; i_ < go_RadioList.Length; ++i_)
        {
            go_RadioList[i_].GetComponent<Cs_RadioLogic>().Set_ResetRadio( true );
        }
        
        Set_TaskText(s_ChangeRadioStation_Text);
    }
    public void Complete_ChangeRadioStation()
    {
        if(b_Job_ChangeRadioStation)
        {
            b_Job_ChangeRadioStation = false;
            if (i_ChangeRadioStation_Number >= 0) s_JobList[ i_ChangeRadioStation_Number ] = s_TurnInJob;
            Set_TaskText();
            i_ChangeRadioStation_Number = -1;
            PhoneArrow.IsEnabled = true;
        }
    }
    #endregion

    #region Fire People
    int i_FirePeople_Number = -1;
    string s_FirePeople_Text = "Fire 5 People";
    void Init_FirePeople()
    {
        b_Job_FirePeople = true;
        
        for(int i_ = 0; i_ < go_Employees.Length; ++i_)
        {
            if(go_Employees[i_] != null)
            {
                go_Employees[i_].GetComponent<Cs_Objective>().Set_State = Enum_ObjectiveState.InProgress;
            }
        }

        Set_TaskText(s_FirePeople_Text);
    }
    public void Complete_FirePeople()
    {
        if(b_Job_FirePeople)
        {
            b_Job_FirePeople = false;
            if (i_FirePeople_Number >= 0) s_JobList[ i_FirePeople_Number ] = s_TurnInJob;
            Set_TaskText();
            i_FirePeople_Number = -1;
            PhoneArrow.IsEnabled = true;

            for (int i_ = 0; i_ < go_Employees.Length; ++i_)
            {
                if(go_Employees[i_])
                {
                    go_Employees[i_].GetComponent<Cs_Objective>().Set_State = Enum_ObjectiveState.Disabled;
                }
            }

            i_PeopleFired = 0;
        }
    }
    #endregion

    #region Use Printer
    int i_SendFax_Number = -1;
    string s_SendFax_Text = "Send Very Legal Fax";
    void Init_SendFax()
    {
        b_Job_SendFax = true;

        go_FaxMachine.GetComponent<Cs_Objective>().Set_State = Enum_ObjectiveState.InProgress;

        Set_TaskText(s_SendFax_Text);
    }
    public void Complete_SendFax()
    {
        if(b_Job_SendFax)
        {
            b_Job_SendFax = false;
            if (i_SendFax_Number >= 0) s_JobList[i_SendFax_Number] = s_TurnInJob;
            Set_TaskText();
            i_SendFax_Number = -1;
            PhoneArrow.IsEnabled = true;

            go_FaxMachine.GetComponent<Cs_Objective>().Set_State = Enum_ObjectiveState.Disabled;
        }
    }
    #endregion

    #region Communist Manifesto
    int i_Book_Number = -1;
    string s_Book_Text = "Disperse Truth\nfrom Desk";
    void Init_Book()
    {
        b_Job_GiveManifesto = true;
        int i_RandomBookLocation = Random.Range(1, 4);

        for(int i_ = 0; i_ < go_Books.Length; ++i_)
        {
            if(go_Books[i_].GetComponent<Cs_BookLogic>())
            {
                // If the main book, activate it fully.
                if(go_Books[i_].GetComponent<Cs_BookLogic>().IsMainBook())
                {
                    go_Books[i_].GetComponent<Cs_BookLogic>().BookEnabled(true);
                    go_Books[i_].GetComponent<Cs_BookLogic>().ArrowState = true;
                }
                // Otherwise, keep transparent & keep arrows off for now until player picks up book
                else
                {
                    if(i_RandomBookLocation == i_)
                    {
                        go_Books[i_].GetComponent<Cs_BookLogic>().BookEnabled(true, true);
                        go_Books[i_].GetComponent<Cs_BookLogic>().ArrowState = false;
                    }
                    else
                    {
                        go_Books[i_].GetComponent<Cs_BookLogic>().BookEnabled( false );
                        go_Books[i_].GetComponent<Cs_BookLogic>().ArrowState = false;
                    }
                }
            }
        }

        Set_TaskText(s_Book_Text);
    }
    public void Complete_Book()
    {
        if(b_Job_GiveManifesto)
        {
            b_Job_GiveManifesto = false;
            if (i_Book_Number >= 0) s_JobList[i_Book_Number] = s_TurnInJob;
            Set_TaskText();
            i_Book_Number = -1;
            PhoneArrow.IsEnabled = true;

            for(int i_ = 0; i_ < go_Books.Length; ++i_)
            {
                if(go_Books[i_])
                {
                    if(go_Books[i_].GetComponent<Cs_Objective>())
                    {
                        if(go_Books[i_].GetComponent<Cs_Objective>().Set_State != Enum_ObjectiveState.Completed)
                        {
                            go_Books[i_].GetComponent<Cs_BookLogic>().BookEnabled(false);
                        }
                    }
                    else
                    {
                        go_Books[i_].GetComponent<Cs_Objective>().Set_State = Enum_ObjectiveState.Completed;
                    }
                }
            }
        }
    }
    #endregion

    public void Set_TurnInTasks()
    {
        if(ClockIn)
        {
            for(int i_ = 0; i_ < s_JobList.Length; ++i_)
            {
                if(s_JobList[i_] == s_TurnInJob)
                {
                    s_JobList[i_] = "";
                }
            }

            Set_TaskText();
        }
        else
        {
            if (b_Job_PunchIn)
            {
                b_Job_PunchIn = false;

                ClockIn = true;

                txt_TutorialText.text = "Now get to work!";

                for (int i_ = 0; i_ < s_JobList.Length; ++i_)
                {
                    if (s_JobList[i_] == s_TurnInJob)
                    {
                        s_JobList[i_] = "";
                    }
                }

                CreateNewJob();

                Set_TaskText();
            }
        }
    }

    void Set_TaskText( string s_Text_ = "")
    {
        // Bubble sort text
        for(int i_ = 0; i_ < s_JobList.Length; ++i_)
        {
            if (s_JobList[i_] == null) s_JobList[i_] = "";
            else if(s_JobList[i_] == "")
            {
                for(int j_ = i_ + 1; j_ < s_JobList.Length; ++j_)
                {
                    if(s_JobList[j_] != "" || s_JobList[j_] == null)
                    {
                        s_JobList[i_] = s_JobList[j_];
                        s_JobList[j_] = "";
                        break;
                    }
                }
            }
        }
        
        // Run through the text list to find the first open position. Assign text there.
        for(int i_ = 0; i_ < s_JobList.Length; ++i_)
        {
            if (s_JobList[i_] == "")
            {
                s_JobList[i_] = s_Text_;

                break;
            }
        }

        // Store new text positions for reference
        for(int i_ = 0; i_ < s_JobList.Length; ++i_)
        {
            if      (s_JobList[i_] == s_BossKickMe_Text)            i_BossKickMe_Number = i_;
            else if (s_JobList[i_] == s_ChangeRadioStation_Text)    i_ChangeRadioStation_Number = i_;
            else if (s_JobList[i_] == s_PunchIn_Text)               i_PunchIn_Number = i_;
            else if (s_JobList[i_] == s_FirePeople_Text)            i_FirePeople_Number = i_;
            else if (s_JobList[i_] == s_Book_Text)                  i_Book_Number = i_;
            else if (s_JobList[i_] == s_SendFax_Text)               i_SendFax_Number = i_;
        }

        // print("Kick Me: " + i_BossKickMe_Number + ", Radio: " + i_ChangeRadioStation_Number);

        // Set text on screen
        txt_JobList_1_Text.text = s_JobList[0];
        txt_JobList_2_Text.text = s_JobList[1];
        txt_JobList_3_Text.text = s_JobList[2];
        txt_JobList_4_Text.text = s_JobList[3];
        txt_JobList_5_Text.text = s_JobList[4];

        if (s_JobList[0] == s_TurnInJob) txt_JobList_1_Text.color = Color.red;
        else txt_JobList_1_Text.color = Color.black;

        if (s_JobList[1] == s_TurnInJob) txt_JobList_2_Text.color = Color.red;
        else txt_JobList_2_Text.color = Color.black;

        if (s_JobList[2] == s_TurnInJob) txt_JobList_3_Text.color = Color.red;
        else txt_JobList_3_Text.color = Color.black;

        if (s_JobList[3] == s_TurnInJob) txt_JobList_4_Text.color = Color.red;
        else txt_JobList_4_Text.color = Color.black;

        if (s_JobList[4] == s_TurnInJob) txt_JobList_5_Text.color = Color.red;
        else txt_JobList_5_Text.color = Color.black;
    }

    // Update is called once per frame
    float f_PunchInText_Timer;
	void Update ()
    {
        if(f_PunchInText_Timer < 15f)
        {
            if (ClockIn)
            {
                f_PunchInText_Timer += Time.deltaTime;

                if (f_PunchInText_Timer > 15f) txt_TutorialText.text = "";
            }
        }

	    if(Input.GetKeyDown(KeyCode.I))
        {
            Complete_Book();
            Complete_BossKickMe();
            Complete_ChangeRadioStation();
            Complete_FirePeople();
            Complete_SendFax();
        }

        if(Input.GetKeyDown(KeyCode.O))
        {
            Set_TurnInTasks();
        }
	}
}
