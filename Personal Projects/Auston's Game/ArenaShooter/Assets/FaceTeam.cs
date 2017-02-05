using UnityEngine;
using System.Collections;

public class FaceTeam : MonoBehaviour
{
    AffiliatedObject affiliatedObject;

    // Use this for initialization
    void Start ()
    {
        affiliatedObject = transform.root.GetComponent<AffiliatedObject>();
        GetComponent<Canvas>().enabled = false;

        for (int i = 0; i < 4; i++)
        {
            Player _player = PlayerManager.players[i];

            if (i % 2 != affiliatedObject.team || (_player != null && _player == affiliatedObject)) continue;

            GetComponent<Canvas>().enabled = true;

            gameObject.layer = i + 12;
            transform.GetChild(0).gameObject.layer = i + 12;
            transform.GetChild(1).gameObject.layer = i + 12;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        for (int i = 0; i < 4; i++)
        {
            Player _player = PlayerManager.players[i];

            if (_player == null || _player == affiliatedObject || _player.team != affiliatedObject.team) continue;

            transform.rotation = Quaternion.LookRotation(_player.camera.forward, transform.root.up);
        }
	}
}
