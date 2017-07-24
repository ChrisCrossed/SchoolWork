using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cs_PlaySongOnTrigger : MonoBehaviour
{
    GameObject go_Player;
    Cs_MusicPlayer go_MusicPlayer;
    [SerializeField] Enum_Song SongToPlay;

    private void Start()
    {
        go_Player = GameObject.Find("Player");
        go_MusicPlayer = go_Player.GetComponent<Cs_MusicPlayer>();
    }

    private void OnTriggerEnter(Collider collider_)
    {
        if(collider_.gameObject == go_Player)
        {
            // Disable collider
            gameObject.GetComponent<Collider>().enabled = false;

            // Play song
            go_MusicPlayer.RunningSong = SongToPlay;
        }
    }
}
