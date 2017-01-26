using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Cs_EnemyController : MonoBehaviour
{
    public GameObject player;
    public GameObject raycast_Left;
    public GameObject raycast_Right;

    GameObject go_LastHitObject;
    bool b_TurnLeft;
    bool b_IsObstructed;
    bool b_IsMovingForward = true;

    float f_EndGameTimer;
    bool b_IsGameOver;
    public GameObject Text_YouWin;

	// Use this for initialization
	void Start ()
    {
        Text_YouWin.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(!b_IsGameOver)
        {
            // Move Forward
            if (b_IsMovingForward)
            {
                gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 10;
            }
            else { gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 3; }

            b_IsMovingForward = true;

            if (b_IsObstructed)
            {
                if(b_TurnLeft)
                {
                    Vector3 newRot = gameObject.transform.eulerAngles;
                    newRot.y -= 60 * Time.deltaTime;
                    gameObject.transform.eulerAngles = newRot;
                }
                else
                {
                    Vector3 newRot = gameObject.transform.eulerAngles;
                    newRot.y += 60 * Time.deltaTime;
                    gameObject.transform.eulerAngles = newRot;
                }
            }
            RayCastForward();
        }
        else
        {
            f_EndGameTimer += Time.deltaTime;
            print(f_EndGameTimer);

            if (f_EndGameTimer >= 5.0f) SceneManager.LoadScene(0);
        }
    }

    void RayCastForward()
    {
        RaycastHit hit_Left = new RaycastHit();
        RaycastHit hit_Right = new RaycastHit();

        Debug.DrawRay(raycast_Left.transform.position, raycast_Left.transform.forward);
        Debug.DrawRay(raycast_Right.transform.position, raycast_Right.transform.forward);

        if (Physics.Raycast(raycast_Left.transform.position, raycast_Left.transform.forward, out hit_Left))
        {
            DetermineRayCastConclusion(hit_Left);
        }
        else if (Physics.Raycast(raycast_Right.transform.position, raycast_Right.transform.forward, out hit_Right))
        {
            DetermineRayCastConclusion(hit_Right);
        }
    }

    void DetermineRayCastConclusion(RaycastHit rayHitDir_)
    {
        if (rayHitDir_.distance < 10)
        {
            if((rayHitDir_.distance <= 5))
            {
                b_IsMovingForward = false;

                if (go_LastHitObject != rayHitDir_.collider.gameObject)
                {
                    // Set the new object we are getting close to
                    go_LastHitObject = rayHitDir_.collider.gameObject;

                    float coinFlip = Random.value;

                    if (coinFlip >= 0.5f) b_TurnLeft = true; else b_TurnLeft = false;
                    b_IsObstructed = true;
                }
            }
            else
            {
                b_IsMovingForward = true;

                if (go_LastHitObject != rayHitDir_.collider.gameObject)
                {
                    // Set the new object we are getting close to
                    go_LastHitObject = rayHitDir_.collider.gameObject;

                    float coinFlip = Random.value;

                    if (coinFlip >= 0.5f) b_TurnLeft = true; else b_TurnLeft = false;
                    b_IsObstructed = true;
                }
            }
        }
        else b_IsObstructed = false;
    }

    void OnTriggerEnter(Collider collider_)
    {
        if (collider_.gameObject.tag == "Wall_Player" || collider_.gameObject.tag == "Wall_Enemy" || collider_.gameObject.tag == "Wall")
        {
            EndGame();
        }
    }

    void EndGame()
    {
        // Destroy(gameObject);
        gameObject.GetComponent<Renderer>().enabled = false;
        gameObject.GetComponent<BoxCollider>().enabled = false;

        Text_YouWin.SetActive(true);

        b_IsGameOver = true;
    }
}
