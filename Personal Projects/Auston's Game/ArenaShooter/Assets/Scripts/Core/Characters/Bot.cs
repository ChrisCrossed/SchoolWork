using UnityEngine;
using System.Collections;

public class Bot : AffiliatedObject
{
    [System.NonSerialized]
    public int createdByIndex;

    private new void Start()
    {
        base.Start();
        Events.PlayerSpawned.Subscribe(OnPlayerSpawned);
    }
    
    private void OnPlayerSpawned(Player player)
    {
        if(player.index == createdByIndex)
            player.GetComponent<Creation>().createdObjects.Add(gameObject);
    }

    private void OnDestroy()
    {
        Events.PlayerSpawned.Unsubscribe(OnPlayerSpawned);
    }
}
