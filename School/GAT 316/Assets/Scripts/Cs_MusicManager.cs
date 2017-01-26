using UnityEngine;
using System.Collections;

public class Cs_MusicManager : MonoBehaviour
{
    Enum_EnemyState e_EnemyState = Enum_EnemyState.Patrol;
    AudioSource as_MusicSource;
    [SerializeField] float f_Pitch_Min = 1.0f;
    [SerializeField] float f_Pitch_Max = 1.2f;
    float f_Pitch_Curr;

    void Start()
    {
        as_MusicSource = gameObject.GetComponent<AudioSource>();

        f_Pitch_Curr = f_Pitch_Min;
    }

    public void Set_MusicState( Enum_EnemyState e_EnemyState_ )
    {
        e_EnemyState = e_EnemyState_;
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if(e_EnemyState != Enum_EnemyState.ChasePlayer)
        {
            // Reduce & Cap the current pitch
            if (f_Pitch_Curr > f_Pitch_Min) f_Pitch_Curr -= Time.deltaTime;
            if (f_Pitch_Curr < f_Pitch_Min) f_Pitch_Curr = f_Pitch_Min;

            // Reduce & cap the volume
            if( as_MusicSource.volume > 0.3f)
            {
                as_MusicSource.volume -= Time.deltaTime;
                if (as_MusicSource.volume < 0.3f) as_MusicSource.volume = 0.3f;
            }
        }
        else // State == ChasePlayer
        {
            // Increase & Cap the current pitch
            if (f_Pitch_Curr < f_Pitch_Max) f_Pitch_Curr += Time.deltaTime;
            if (f_Pitch_Curr > f_Pitch_Max) f_Pitch_Curr = f_Pitch_Max;

            // Increase & cap the volume
            if (as_MusicSource.volume < 0.5f)
            {
                as_MusicSource.volume += Time.deltaTime;
                if (as_MusicSource.volume > 0.5f) as_MusicSource.volume = 0.5f;
            }
        }

        as_MusicSource.pitch = f_Pitch_Curr;
	}
}
