using UnityEngine;
using System.Collections;

public class Cs_KeyLogic : MonoBehaviour
{
    [SerializeField]
    bool b_IsFake = false;
    bool b_KillKey = false;
    float f_StartingHeight;

	// Use this for initialization
	void Start ()
    {
	    if(b_IsFake)
        {
            gameObject.GetComponent<Collider>().enabled = false;
        }

        f_StartingHeight = gameObject.transform.position.y;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(!b_IsFake)
        {
            // Force Rotation
            RotateKey();
        }
        else
        {
            if(b_KillKey)
            {
                float f_Alpha = gameObject.GetComponent<MeshRenderer>().material.color.a - (Time.deltaTime * 3);

                if (f_Alpha <= 0) Destroy(gameObject);
                else
                {
                    Color tempColor = gameObject.GetComponent<MeshRenderer>().materials[0].color;
                    gameObject.GetComponent<MeshRenderer>().materials[0].color = new Color(tempColor.r, tempColor.g, tempColor.b, f_Alpha);

                    tempColor = gameObject.GetComponent<MeshRenderer>().materials[1].color;
                    gameObject.GetComponent<MeshRenderer>().materials[1].color = new Color(tempColor.r, tempColor.g, tempColor.b, f_Alpha);
                }
            }
        }
	}

    float f_Height;
    void RotateKey()
    {
        Vector3 currRot = gameObject.transform.eulerAngles;
        currRot.y = Mathf.LerpAngle(currRot.y, currRot.y + 90, Time.deltaTime);

        // currRot.y = f_StartingHeight + Mathf.Sin(Time.deltaTime * 5);

        Vector3 newPos = gameObject.transform.position;
        f_Height += Time.deltaTime;
        newPos.y = f_StartingHeight + Mathf.Sin(f_Height) / 2;

        gameObject.transform.eulerAngles = currRot;
        gameObject.transform.position = newPos;
    }

    [SerializeField] GameObject go_SFX;
    [SerializeField] AudioClip sfx_RealKey;
    [SerializeField] AudioClip sfx_FakeKey;
    public void DestroyKey()
    {
        b_KillKey = true;

        go_SFX.GetComponent<AudioSource>().PlayOneShot(sfx_FakeKey);
    }

    void OnTriggerEnter(Collider collision_)
    {
        if (collision_.gameObject.name == "Capsule")
        {
            // collision_.gameObject.GetComponent<Cs_FPSController>().IncrementKeyCounter();
            collision_.gameObject.transform.root.GetComponent<Cs_FPSController>().IncrementKeyCounter();

            go_SFX.GetComponent<AudioSource>().PlayOneShot(sfx_RealKey);

            Destroy(gameObject);
        }
    }
}
