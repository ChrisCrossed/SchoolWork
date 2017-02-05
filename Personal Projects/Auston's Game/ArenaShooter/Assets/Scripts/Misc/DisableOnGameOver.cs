using UnityEngine;
using System.Collections;

public class DisableOnGameOver : MonoBehaviour
{
    public MonoBehaviour[] toDisable;

	// Use this for initialization
	void Start ()
    {
        Events.GameOver.Subscribe(OnGameOver);
	}

    private void OnGameOver(int winningTeam)
    {
        for (int i = 0; i < toDisable.Length; i++)
        {
            if(toDisable[i] != null)
                toDisable[i].enabled = false;
        }
    }

    // Update is called once per frame
    void OnDestroy ()
    {
        Events.GameOver.Subscribe(OnGameOver);
    }
}
