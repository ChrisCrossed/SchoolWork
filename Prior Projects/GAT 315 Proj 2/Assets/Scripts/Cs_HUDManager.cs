using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Cs_HUDManager : MonoBehaviour
{
    public Camera hudCamera;

    public GameObject ui_Player;
    public GameObject ui_Primary;
    public GameObject ui_Secondary;

    public GameObject GO_Player;
    public GameObject GO_Primary;
    public GameObject GO_Secondary;

    // Use this for initialization
    void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Vector3 playerPos = GO_Player.transform.position;
        Vector3 playerPos = new Vector3(GO_Player.transform.position.x, ui_Player.transform.position.y, GO_Player.transform.position.z);
        Vector3 primaryPos = new Vector3(GO_Primary.transform.position.x, ui_Primary.transform.position.y, GO_Primary.transform.position.z);
        Vector3 secondaryPos = new Vector3(GO_Secondary.transform.position.x, ui_Secondary.transform.position.y, GO_Secondary.transform.position.z);

        Vector2 viewportPoint = hudCamera.WorldToViewportPoint(playerPos);
        Vector2 viewportPrimary = hudCamera.WorldToScreenPoint(primaryPos);
        Vector2 viewportSecondary = hudCamera.WorldToScreenPoint(secondaryPos);

        ui_Player.transform.position    = new Vector3(playerPos.x    * 0.15f * (650 / hudCamera.transform.position.y), ui_Player.transform.position.y,  playerPos.z * 0.15f * (650 / hudCamera.transform.position.y));
        ui_Primary.transform.position   = new Vector3(primaryPos.x   * 0.15f * (650 / hudCamera.transform.position.y), ui_Primary.transform.position.y, primaryPos.z * 0.15f * (650 / hudCamera.transform.position.y));
        ui_Secondary.transform.position = new Vector3(secondaryPos.x * 0.15f * (650 / hudCamera.transform.position.y), ui_Secondary.transform.position.y, secondaryPos.z * 0.15f * (650 / hudCamera.transform.position.y));

        // Rotates the UI_Player icon
        Vector3 newRot = GO_Player.transform.eulerAngles;
        ui_Player.transform.eulerAngles = new Vector3(newRot.x + 90f, newRot.y, 0);
    }

    public void SetGoldIcon(GoldType goldType_, bool b_IsEnabled_)
    {
        if (goldType_ == GoldType.Primary) ui_Primary.GetComponent<Image>().enabled = b_IsEnabled_;
        if (goldType_ == GoldType.Secondary) ui_Secondary.GetComponent<Image>().enabled = b_IsEnabled_;
    }
}
