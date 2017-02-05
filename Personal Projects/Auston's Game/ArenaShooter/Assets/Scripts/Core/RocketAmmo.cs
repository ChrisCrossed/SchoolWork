using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RocketAmmo : MonoBehaviour
{
    public int maxRocketCount = 7;
    public int startingRocketCount = 3;
    public int currentRocketCount = 0;

    public AnimateGroup animateOnSuccess;
    public AnimateGroup animateOnFail;

    //public Animate

    public Text displayText;

    // Use this for initialization
    void Start ()
    {
        //AddRocket(startingRocketCount);
        if(GameManager.instance.startingRocketCountOverride > -1)
            currentRocketCount = GameManager.instance.startingRocketCountOverride;
        else
            currentRocketCount = startingRocketCount;
        displayText.text = currentRocketCount.ToString();
    }

    public bool SpendRocket(int _count = 1) //returns whether the rocket was able to be spent (if true, was spent)
    {
        if (_count == 0) return true;

        if (currentRocketCount - _count < 0)
        {
            animateOnFail.Play();
            return false;
        }

        animateOnSuccess.Play();

        currentRocketCount -= _count;
        displayText.text = currentRocketCount.ToString();
        return true;
    }

    public bool AddRocket(int _count = 1) //returns whether the rocket was able to be added (if true, was added)
    {
        if (_count == 0) return true;

        if (currentRocketCount + _count > maxRocketCount)
        {
            animateOnFail.Play();
            return false;
        }

        animateOnSuccess.Play();

        currentRocketCount += _count;
        displayText.text = currentRocketCount.ToString();
        return true;
    }

    // Update is called once per frame
    void Update ()
    {
        
	}
}
