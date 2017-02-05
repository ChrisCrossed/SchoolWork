using UnityEngine;
using System.Collections;

public class FaceCameras : MonoBehaviour
{
    int team = -1;

	// Use this for initialization
	void Start ()
    {
        //int _numberOfPlayers = PlayerManager.players.Count;
        team = transform.root.GetComponent<AffiliatedObject>().team;
	}
	
	// Update is called once per frame
	void Update ()
    {
        int _numberOfPlayers = PlayerManager.instance.startingPlayerCount;
        if (team <= _numberOfPlayers - 1 && team >= 0 && PlayerManager.players[team] != null)
            transform.rotation = Quaternion.LookRotation(-PlayerManager.players[team].camera.forward, -transform.root.up);
    }
}
