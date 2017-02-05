using UnityEngine;
using System.Collections;

public class EnableBehaviorOnAwake : MonoBehaviour
{
    public Behaviour[] toEnable = new Behaviour[0];

    void Awake()
    {
        for (int i = 0; i < toEnable.Length; i++)
            toEnable[i].enabled = true;
    }
}
