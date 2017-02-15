using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cs_Crosshair : MonoBehaviour
{
    // Object Connections
    GameObject go_Crosshair;
    GameObject go_Camera;
    Image img_Crosshair;
    
	// Use this for initialization
	void Start ()
    {
        // Object Connections
        go_Crosshair = GameObject.Find("Crosshair").gameObject;
        go_Camera = GameObject.Find("Main Camera").gameObject;
        img_Crosshair = go_Crosshair.GetComponent<Image>();
	}

    void CameraRaycast()
    {
        RaycastHit hit;
        int i_LayerMask = LayerMask.GetMask("Use", "Enemy");

        if(Physics.Raycast(go_Camera.transform.position, go_Camera.transform.forward, out hit, float.PositiveInfinity, i_LayerMask))
        {
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Use"))
            {
                // Because it is a button, we don't want to allow use until we're close
                if(hit.distance < 5f)
                {
                    img_Crosshair.color = Color.cyan;

                    go_CrosshairObject = hit.collider.gameObject;
                }
                else
                {
                    img_Crosshair.color = Color.white;

                    go_CrosshairObject = null;
                }
            }
            else if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                img_Crosshair.color = Color.red;

                go_CrosshairObject = hit.collider.gameObject;
            }
        }
        else
        {
            img_Crosshair.color = Color.white;

            go_CrosshairObject = null;
        }
    }

    GameObject go_CrosshairObject;
    public GameObject Get_CrosshairObject
    {
        get { return go_CrosshairObject; }
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        CameraRaycast();
	}
}
