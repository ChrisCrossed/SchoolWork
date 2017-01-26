using UnityEngine;
using System.Collections;

public class Cs_FakeGround : MonoBehaviour
{
    public GameObject LevelExit;

	public void OpenUnderground()
    {
        LevelExit.SetActive(true);

        gameObject.SetActive(false);
    }
}
