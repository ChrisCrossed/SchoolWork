using UnityEngine;
using System.Collections;

public class DestroyAfterTime : MonoBehaviour
{
    public float lifetime = 5;
    public bool destroyAfterGameOver = false;
	
	// Update is called once per frame
	void Update ()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0 && (destroyAfterGameOver || !GameManager.instance.gameOver))
            Destroy(gameObject);
	}
}
