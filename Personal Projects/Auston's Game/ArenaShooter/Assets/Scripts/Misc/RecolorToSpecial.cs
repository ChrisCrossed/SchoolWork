using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class RecolorToSpecial : MonoBehaviour
{
    private enum ColorType { Negative, Positive }
    private Graphic graphic;
    [SerializeField]
    private ColorType colorType;

	// Use this for initialization
	void Start ()
    {
        graphic = GetComponent<Graphic>();

        if (colorType == ColorType.Negative)
            graphic.color = GameManager.instance.negativeColor;
        else
            graphic.color = GameManager.instance.positiveColor;
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
