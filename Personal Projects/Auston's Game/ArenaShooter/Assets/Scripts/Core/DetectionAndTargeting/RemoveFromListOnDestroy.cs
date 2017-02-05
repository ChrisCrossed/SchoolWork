using UnityEngine;
using System.Collections.Generic;

public class RemoveFromListOnDestroy<T> : MonoBehaviour
{
    [HideInInspector] public List<T> objectList = null;
    [HideInInspector] public T objectToRemove = default(T);
    [HideInInspector] private bool removalHandled = false;

    void DestroyWithoutRemoval()
    {
        removalHandled = true;
        Destroy(this);
    }

    void OnDestroy()
    {
        if (removalHandled) return;
        objectList.Remove(objectToRemove);
    }
}
