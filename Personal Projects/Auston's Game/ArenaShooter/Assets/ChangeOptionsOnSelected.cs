using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public static class Options
{
    public class ControlSettings
    {
        private int sensitivity_ = GameManager.instance.sensitivityDefault;
        public int sensitivity { get { return sensitivity_; } set { sensitivity_ = value; if (sensitivity_ < 1) sensitivity_ += 10; if (sensitivity_ > 10) sensitivity_ -= 10; } }
        public bool aimInversion = false;

        public void Revert()
        {
            sensitivity = 3;
            aimInversion = false;
        }
    }

    public static ControlSettings[] playerControls = new ControlSettings[] { new ControlSettings(), new ControlSettings(), new ControlSettings(), new ControlSettings() };

    private static int volume_ = 5;
    public static int volume { get { return volume_; } set { volume_ = value; if (volume_ < 0) volume_ += 11; if (volume_ > 10) volume_ -= 11; } }
}

public class ChangeOptionsOnSelected : Selectable
{
    public enum OptionType { Sensitivity, AimInversion, Volume, Vibration, RevertAll}
    public OptionType option = OptionType.Sensitivity;
    public Text text;

    private bool showingValue;
    public int index;

    void Start()
    {
        if (text == null)
            text = GetComponentInChildren<Text>();
    }

    public override void OnSelected()
    {
        if(option == OptionType.RevertAll)
        {
            Options.playerControls[0].Revert();
            Options.playerControls[1].Revert();

            Options.volume = 5;
            Menu.GoToPrevious();
            return;
        }

        showingValue = !showingValue;
        if (!showingValue)
            if (text != null) text.text = name;

        OnAltDirectionalInput(0);
    }
    public override void OnEnterHover()
    {
        
    }
    public override void OnExitHover()
    {
        showingValue = false;
        if (text != null) text.text = name;
    }

    public override void OnAltDirectionalInput(int _positivity)
    {
        if (!showingValue) return;

        MenuAudio.PlayOnSelect();

        switch (option)
        {
            case OptionType.Sensitivity:
                Options.playerControls[index].sensitivity += _positivity;
                if (text != null) text.text = "< " + Options.playerControls[index].sensitivity + " >";
                break;
            case OptionType.AimInversion:
                if(_positivity != 0)
                    Options.playerControls[index].aimInversion = !Options.playerControls[index].aimInversion;
                if (text != null) text.text = "< " + Options.playerControls[index].aimInversion + " >";
                break;
            case OptionType.Volume:
                Options.volume += _positivity;
                if (text != null) text.text = "< " + Options.volume + " >";
                break;
        }
    }
}
