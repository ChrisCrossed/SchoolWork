using UnityEngine;

public class DetachChildrenOnDestroy : MonoBehaviour
{
    public Transform[] childrenToDetach;
	
	void OnDestroy ()
    {
        for (int i = 0; i < childrenToDetach.Length; i++)
            childrenToDetach[i].parent = null;
	}
}
