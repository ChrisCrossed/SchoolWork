using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_Data : MonoBehaviour
{
	// Use this for initialization
	void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    Enum_FormatType e_FormatType;
    public Enum_FormatType FormatType
    {
        set { e_FormatType = value; }
        get { return e_FormatType; }
    }

    Enum_TeamList e_Team_1;
    public Enum_TeamList TeamOne
    {
        set { e_Team_1 = value; }
        get { return e_Team_1; }
    }

    Enum_TeamList e_Team_2;
    public Enum_TeamList TeamTwo
    {
        set { e_Team_2 = value; }
        get { return e_Team_2; }
    }
}
