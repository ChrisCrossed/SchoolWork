using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Targeting))]
public class AutomaticShooting : Shooting
{
    private Targeting targeting;
    private Transform currentTarget { get { return targeting.currentTarget; } }
    public float autoShootDelay = 0.75f;

    // Use this for initialization
    new void Start ()
    {
        base.Start();
        targeting = GetComponent<Targeting>();
	}
	
	// Update is called once per frame
	new void Update ()
    {
        base.Update();

        if (currentTarget == null)
        {
            timeSinceShot = 0;
        }

	    if(timeSinceShot > autoShootDelay)
        {
            if (shootFrom.Length < 1)
            {
                Debug.Log(gameObject.name + " has no transform to shoot from");
                return;
            }

            if (shootFrom.Length == 1) currentShootFrom = 0;
            else ++currentShootFrom;

            if (currentShootFrom >= shootFrom.Length)
                currentShootFrom -= shootFrom.Length;

            Transform _shootFrom = shootFrom[currentShootFrom];

            if (projectileObject == null)
                CreateAndShootProjectile(_shootFrom);
            else
                ShootProjectile(_shootFrom);
        }
	}
}
