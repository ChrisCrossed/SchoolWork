using UnityEngine;
using System.Collections;

public class Cs_BaseColliderLogic : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
	
	}

    public void ApplyDamage(int i_Damage = 1)
    {
        if(gameObject.transform.root.gameObject.GetComponent<Cs_WallTowerLogic>())
        {
            gameObject.transform.root.gameObject.GetComponent<Cs_WallTowerLogic>().ApplyDamage(i_Damage);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
