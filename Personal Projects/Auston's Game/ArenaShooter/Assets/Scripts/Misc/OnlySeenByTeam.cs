using UnityEngine;
using System.Collections;

public class OnlySeenByTeam : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
        int _teamIndex = transform.root.GetComponent<AffiliatedObject>().team;

        if (_teamIndex < 0)
            return;

        gameObject.layer = 12 + _teamIndex;
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
