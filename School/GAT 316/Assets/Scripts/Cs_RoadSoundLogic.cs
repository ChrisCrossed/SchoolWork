using UnityEngine;
using System.Collections;

public class Cs_RoadSoundLogic : MonoBehaviour
{
    AudioSource as_AudioSource;
    float f_Volume;
    Vector3 v3_PlayerPosition;
    Vector3 v3_ThisPosition;

	// Use this for initialization
	void Start ()
    {
        as_AudioSource = gameObject.GetComponent<AudioSource>();
        f_Volume = 1.0f;
	}
	
	// Update is called once per frame
	void Update ()
    {
        v3_PlayerPosition = GameObject.Find("Player").transform.position;
        v3_ThisPosition = gameObject.transform.position;

        if (Vector3.Distance(v3_PlayerPosition, v3_ThisPosition) > 37f)
        {
            if(f_Volume > 0f)
            {
                f_Volume -= Time.deltaTime;
                if (f_Volume < 0f) f_Volume = 0f;
            }
        }
        else
        {
            if (f_Volume < 1f)
            {
                f_Volume += Time.deltaTime;
                if (f_Volume > 1.0f) f_Volume = 1f;
            }
        }

        as_AudioSource.volume = f_Volume;
	}
}
