using UnityEngine;
using System.Collections;

public class Cs_RadioLogic : MonoBehaviour
{
    AudioClip ac_Russia;
    AudioClip ac_Static;
    AudioClip ac_Mus_1;
    AudioClip ac_Mus_2;
    AudioClip ac_Mus_3;
    AudioClip ac_Mus_4;
    AudioClip ac_Mus_5;

    AudioSource as_Source;

    int i_SongNum;

    GameObject go_Player;
    float f_VolumeFade_DistMin = 5f;
    float f_VolumeFade_DistMax = 11f;

    float f_Volume_Min = 0f;
    float f_Volume_Max = 0.5f;

    // Use this for initialization
    void Start ()
    {
        ac_Mus_1 = Resources.Load("mus_1") as AudioClip;
        ac_Mus_2 = Resources.Load("mus_2") as AudioClip;
        ac_Mus_3 = Resources.Load("mus_3") as AudioClip;
        ac_Mus_4 = Resources.Load("mus_4") as AudioClip;
        ac_Mus_5 = Resources.Load("mus_5") as AudioClip;
        ac_Russia = Resources.Load("mus_Anthem") as AudioClip;
        ac_Static = Resources.Load("SFX_Static") as AudioClip;

        as_Source = gameObject.GetComponent<AudioSource>();

        go_Player = GameObject.Find("Player");

        Set_ResetRadio();
    }

    public void Set_ResetRadio( bool b_ActivateObjective_ = false )
    {
        i_SongNum = Random.Range(0, 5);
        b_IsMusic = false;

        if (b_ActivateObjective_)
        {
            gameObject.GetComponent<Cs_Objective>().Set_State = Enum_ObjectiveState.InProgress;
            if(transform.Find("RotArrow"))
            {
                transform.Find("RotArrow").GetComponent<Cs_RotArrow>().IsEnabled = true;
            }
        }
    }

    float f_Timer;
    bool b_IsMusic;
    bool b_StaticPlaying;
    public void Use()
    {
        if(i_SongNum < 5)
        {
            f_Timer = 0f;
            b_IsMusic = false;
            b_StaticPlaying = false;
            ++i_SongNum;
        }
    }

	// Update is called once per frame
	void Update ()
    {
        #region Reduce volume linearly if between/greater/less than these distances
        float f_DistToPlayer = Vector3.Distance( gameObject.transform.position, go_Player.transform.position );
        if (f_DistToPlayer > f_VolumeFade_DistMax) as_Source.volume = f_Volume_Min;
        else if (f_DistToPlayer < f_VolumeFade_DistMin) as_Source.volume = f_Volume_Max;
        else
        {
            float f_Perc = 1 - (f_DistToPlayer - f_VolumeFade_DistMin) / (f_VolumeFade_DistMax - f_VolumeFade_DistMin);
            as_Source.volume = f_Perc * f_Volume_Max;
        }
        #endregion

        #region Plays static sfx before moving into the music clip
        if (f_Timer < 0.25f)
        {
            f_Timer += Time.deltaTime;

            if(!b_StaticPlaying)
            {
                b_StaticPlaying = true;
                as_Source.clip = ac_Static;
                as_Source.Play();
            }
        }
        else
        {
            if(!b_IsMusic)
            {
                b_IsMusic = true;

                as_Source.time = Random.Range(0f, 30f);

                Color clr_CurrAlpha = transform.Find("ParticleSystem").GetComponent<ParticleSystemRenderer>().material.color;
                clr_CurrAlpha = Color.black;
                clr_CurrAlpha.a = 0.5f;

                // Find random song to play that isn't this one
                if (i_SongNum == 0) as_Source.clip = ac_Mus_1;
                else if (i_SongNum == 1) as_Source.clip = ac_Mus_2;
                else if (i_SongNum == 2) as_Source.clip = ac_Mus_3;
                else if (i_SongNum == 3) as_Source.clip = ac_Mus_4;
                else if (i_SongNum == 4) as_Source.clip = ac_Mus_5;
                else if (i_SongNum == 5)
                {
                    as_Source.clip = ac_Russia;
                    as_Source.time = 2f;

                    clr_CurrAlpha = Color.red;
                    clr_CurrAlpha.a = 1f;

                    // Connect to Objective and tell it we're done
                    if(gameObject.GetComponent<Cs_Objective>().Set_State == Enum_ObjectiveState.InProgress)
                    {
                        gameObject.GetComponent<Cs_Objective>().Set_State = Enum_ObjectiveState.Completed;

                        if (transform.Find("RotArrow"))
                        {
                            transform.Find("RotArrow").GetComponent<Cs_RotArrow>().IsEnabled = false;
                        }
                    }
                }
                
                transform.Find("ParticleSystem").GetComponent<ParticleSystemRenderer>().material.color = clr_CurrAlpha;

                as_Source.Play();
            }
        }
        #endregion
    }
}
