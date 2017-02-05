using UnityEngine;
using System.Collections;

public class ChangeMouseState : MonoBehaviour
{
    public CursorLockMode stateOnEnabled = CursorLockMode.None;
    public CursorLockMode stateOnDisabled = CursorLockMode.None;
    
    void OnEnable () { Cursor.lockState = stateOnEnabled; }
    void OnDisable () { Cursor.lockState = stateOnDisabled; }
}
