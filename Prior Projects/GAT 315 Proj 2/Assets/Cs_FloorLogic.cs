using UnityEngine;
using System.Collections;

public class Cs_FloorLogic : MonoBehaviour
{
    float f_Timer;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        Renderer renderer = GetComponent<Renderer>();
        Material mat = renderer.material;

        float emission = Mathf.PingPong(Time.time, 1.0f);
        Color baseColor = Color.red; //Replace this with whatever you want for your base color at emission level '1'

        Color finalColor = baseColor * Mathf.LinearToGammaSpace(emission);

        mat.SetColor("_EmissionColor", finalColor);

        /*
        var matObj = gameObject.GetComponentInChildren<MeshRenderer>().material;

        matObj.shader = new Shader();

        gameObject.GetComponentInChildren<MeshRenderer>().material = matObj;
        */
    }
}
