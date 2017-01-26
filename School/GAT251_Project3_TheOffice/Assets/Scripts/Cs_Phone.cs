using UnityEngine;
using System.Collections;

public class Cs_Phone : MonoBehaviour
{
    public void Use()
    {
        GameObject.Find("LevelManager").GetComponent<Cs_ObjectiveManager>().Set_TurnInTasks();

        transform.Find("RotArrow").GetComponent<Cs_RotArrow>().IsEnabled = false;
    }
}
