using UnityEngine;
using System.Collections;

public class Cs_RoadLogic : MonoBehaviour
{
    bool b_ObjectiveCollected;

    GameObject go_Car_1;
    GameObject go_Car_2;
    [SerializeField] GameObject go_Limo;

	// Use this for initialization
	void Start ()
    {
        go_Car_1    = GameObject.Find("Car_Black");
        go_Car_2    = GameObject.Find("Car_Red");
        // go_Limo     = GameObject.Find("Limo");
    }

    public void Set_SwitchToLimo()
    {
        // Set the cars to only run in the bottom lane
        GameObject.Destroy(go_Car_1);
        GameObject.Destroy(go_Car_2);
        // go_Car_1.GetComponent<Cs_CarLogic>().Set_BottomLaneOnly();
        // go_Car_2.GetComponent<Cs_CarLogic>().Set_BottomLaneOnly();

        // Enable and initiate the Limo
        go_Limo.SetActive(true);
        go_Limo.GetComponent<Cs_LimoLogic>().Init_Limo();
    }

    public bool ObjectiveCollected
    {
        set { b_ObjectiveCollected = value; }
        get { return b_ObjectiveCollected; }
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if(Input.GetKeyDown(KeyCode.P))
        {
            Set_SwitchToLimo();

            ObjectiveCollected = true;
        }
	}
}
