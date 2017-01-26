using UnityEngine;
using System.Collections;

public class Cs_PatrolPointLogic : MonoBehaviour
{
    [SerializeField] float f_WaitTime = 0.0f;

    // Use this for initialization
    void Start ()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    public float GetWaitTime()
    {
        return f_WaitTime;
    }
}
