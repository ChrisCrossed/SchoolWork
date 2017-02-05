using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuObject : MonoBehaviour
{
    [System.Serializable]
    public struct Overrides
    {
        public MenuObject upObject;
        public MenuObject downObject;
        public MenuObject leftObject;
        public MenuObject rightObject;
    }

    public Text text;

    public Overrides overrides;
    public Animate[] animateOnEnterHover = new Animate[0];
    public Animate[] animateOnExitHover = new Animate[0];
    public Animate[] animateReversedOnExitHover = new Animate[0];

    private MenuObject[] overrideObjects;

    public Behaviour[] toggleOnSelect = new Behaviour[0];

    public Selectable selectable = null;

    // Use this for initialization
    void Start ()
    {
        MenuObject[] menuObjects = GetComponentsInChildren<MenuObject>();
        overrideObjects = new MenuObject[] { overrides.upObject, overrides.rightObject, overrides.downObject, overrides.leftObject };
        if (text) text.text = name;

        if (selectable == null) selectable = GetComponent<Selectable>();
    }

    public MenuObject GetAdjacentOverride(Direction _direction)
    {
        return overrideObjects[((int)_direction) - 1];
    }

    public void EnterHover()
    {
        for (int i = 0; i < animateOnEnterHover.Length; i++)
        {
            animateOnEnterHover[i].speed = Mathf.Abs(animateOnEnterHover[i].speed);
            animateOnEnterHover[i].Play();
        }
        if(selectable)
            selectable.OnEnterHover();
    }
    public void ExitHover()
    {
        for (int i = 0; i < animateOnExitHover.Length; i++)
        {
            animateOnExitHover[i].Play();
        }
        for (int i = 0; i < animateReversedOnExitHover.Length; i++)
        {
            animateReversedOnExitHover[i].speed *= -1;
            animateReversedOnExitHover[i].Play();
        }
        if (selectable)
            selectable.OnExitHover();
    }

    public void GiveAltDirectional(int _positivity)
    {
        selectable.OnAltDirectionalInput(_positivity);
    }

    public void SelectObject()
    {
        for (int i = 0; i < toggleOnSelect.Length; i++)
            toggleOnSelect[i].enabled = !toggleOnSelect[i].enabled;

        if (selectable != null) selectable.OnSelected();
    }
}
