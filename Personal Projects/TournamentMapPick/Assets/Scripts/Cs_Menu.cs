using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum Enum_FormatType
{
    BestOf_3,
    BestOf_5,
    Background
}

public class Cs_Menu : MonoBehaviour
{
    Enum_TeamList e_Team_1;
    Enum_TeamList e_Team_2;
    Enum_FormatType e_FormatType;

    Cs_Data data;

    // Dropdown and UI
    Dropdown Drop_TeamOne;
    Dropdown Drop_TeamTwo;
    Dropdown Drop_FormatType;

    // Use this for initialization
    void Start ()
    {
        data = GameObject.Find("Data").GetComponent<Cs_Data>();

        Drop_TeamOne = GameObject.Find("Dropdown_Left").GetComponent<Dropdown>();
        Drop_TeamTwo = GameObject.Find("Dropdown_Right").GetComponent<Dropdown>();
        Drop_FormatType = GameObject.Find("Dropdown_Type").GetComponent<Dropdown>();

        data.TeamOne = Enum_TeamList.UW;
        data.TeamTwo = Enum_TeamList.UW;
        data.FormatType = Enum_FormatType.BestOf_3;

        // Dropdown
        Drop_TeamOne.onValueChanged.AddListener(delegate { Set_TeamOne(Drop_TeamOne); });
        Drop_TeamTwo.onValueChanged.AddListener(delegate { Set_TeamTwo(Drop_TeamTwo); });
        Drop_FormatType.onValueChanged.AddListener(delegate { Set_FormatType(Drop_FormatType); });
    }

    void Set_TeamOne( Dropdown target_ )
    {
        if(target_.value == 0)
        {
            data.TeamOne = Enum_TeamList.UW;
        }
        else if(target_.value == 1)
        {
            data.TeamOne = Enum_TeamList.DigiPen;
        }
        else if (target_.value == 2)
        {
            data.TeamOne = Enum_TeamList.CWU;
        }
        else
        {
            data.TeamOne = Enum_TeamList.WWU;
        }
    }

    void Set_TeamTwo( Dropdown target_ )
    {
        if (target_.value == 0)
        {
            data.TeamTwo = Enum_TeamList.UW;
        }
        else if (target_.value == 1)
        {
            data.TeamTwo = Enum_TeamList.DigiPen;
        }
        else if (target_.value == 2)
        {
            data.TeamTwo = Enum_TeamList.CWU;
        }
        else
        {
            data.TeamTwo = Enum_TeamList.WWU;
        }
    }

    void Set_FormatType( Dropdown target_ )
    {
        if (target_.value == 0)
        {
            data.FormatType = Enum_FormatType.BestOf_3;
        }
        else if (target_.value == 1)
        {
            data.FormatType = Enum_FormatType.BestOf_5;
        }
        else
        {
            data.FormatType = Enum_FormatType.Background;
        }

        print(target_.value);
    }

    public void StartPickBan()
    {
        SceneManager.LoadScene(1);
    }

    float f_QuitTimer;
    void Update()
    {
        #region Return to menu if Escape is double-tapped
        if (f_QuitTimer > 0f)
        {
            if (f_QuitTimer >= 0.5f) f_QuitTimer = -Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); print("WE QUIT"); }

            f_QuitTimer += Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && f_QuitTimer == 0f)
        {
            f_QuitTimer += Time.deltaTime;
        }
        #endregion
    }
}
