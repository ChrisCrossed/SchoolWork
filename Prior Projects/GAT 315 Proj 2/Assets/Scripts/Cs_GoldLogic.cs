using UnityEngine;
using System.Collections;

public enum GoldType
{
    Primary,
    Secondary
}

public class Cs_GoldLogic : MonoBehaviour
{
    public GoldType goldType;

    float yPos;

	// Use this for initialization
	void Start ()
    {
        yPos = Random.Range(0, 180);

        gameObject.GetComponent<MeshRenderer>().enabled = false;

        GameObject.Find("HUDCanvas").GetComponent<Cs_HUDManager>().SetGoldIcon(goldType, false);
    }
	
	// Update is called once per frame
	void Update ()
    {
        yPos += Time.deltaTime;

        var currPos = gameObject.transform.position;
        currPos.y = Mathf.Sin(yPos * 4) / 2 + 1f;
        gameObject.transform.position = currPos;

        if (Input.GetKeyDown(KeyCode.P))
        {
            RespawnGold();
        }
    }

    public void StartGame()
    {
        RespawnGold();

        gameObject.GetComponent<MeshRenderer>().enabled = true;

        GameObject.Find("HUDCanvas").GetComponent<Cs_HUDManager>().SetGoldIcon(goldType, true);
    }

    void Score()
    {
        // Turn off material
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;

        // Turn off collider
        gameObject.GetComponent<Collider>().enabled = false;

        // Turn off HUD image
        GameObject.Find("HUDCanvas").GetComponent<Cs_HUDManager>().SetGoldIcon(goldType, false);
        
        // Give player score
        int currTimer = GameObject.Find("LevelManager").GetComponent<Cs_LevelManager>().GetCountdownTimer();
        if (goldType == GoldType.Secondary) currTimer *= 2;
        GameObject.Find("LevelManager").GetComponent<Cs_LevelManager>().SetPlayerScore(currTimer);

        // Play sound effects
        if (goldType == GoldType.Primary)
        {
            // Now handled in Level Manager
        }
        else GameObject.Find("Player").GetComponent<Cs_PlayerController>().PlaySFX(4);

        if (goldType == GoldType.Primary) GameObject.Find("LevelManager").GetComponent<Cs_LevelManager>().PlayerScoredPrimary();
    }

    public void RespawnGold()
    {
        // Move to random X/Y position
        int newSize = GameObject.Find("LevelManager").GetComponent<Cs_LevelManager>().GetFieldSize();
        Vector3 newLoc = gameObject.transform.position;
        newLoc.x = Random.Range(-(newSize - 5), (newSize - 5));
        newLoc.z = Random.Range(-(newSize - 5), (newSize - 5));
        gameObject.transform.position = newLoc;

        // Turn on material
        gameObject.GetComponent<MeshRenderer>().enabled = true;
        gameObject.GetComponentInChildren<MeshRenderer>().enabled = true;

        // Turn on collider
        gameObject.GetComponent<Collider>().enabled = true;

        // Turn on HUD image
        GameObject.Find("HUDCanvas").GetComponent<Cs_HUDManager>().SetGoldIcon(goldType, true);
    }

    void OnTriggerEnter(Collider collider_)
    {
        if (collider_.gameObject.tag == "Player")
        {
            print("Hit: " + collider_.gameObject.name);

            Score();
        }
    }
}
