using UnityEngine;
using System.Collections;

public class Cs_ObservatoryLogic : MonoBehaviour
{
    float f_Rotation = 45f;
    int i_NumberToLookAt = 2;

    public GameObject go_Sun;

    // Use this for initialization
    void Start ()
    {
	
	}

    public void SetPositionToLookAt(int i_Number_ = 0)
    {
        i_NumberToLookAt = i_Number_;
    }

    public int GetPositionToLookAt()
    {
        return i_NumberToLookAt;
    }
	
	// Update is called once per frame
	void Update ()
    {
        gameObject.GetComponent<Rigidbody>().MoveRotation(Quaternion.Lerp(gameObject.transform.rotation, Quaternion.Euler(new Vector3(0, i_NumberToLookAt * f_Rotation, 0)), Time.deltaTime / 2));

        Vector3 v3_SunRot = go_Sun.transform.eulerAngles;
        v3_SunRot.y = gameObject.transform.eulerAngles.y - 130f;
        go_Sun.transform.eulerAngles = v3_SunRot;
	}
}
