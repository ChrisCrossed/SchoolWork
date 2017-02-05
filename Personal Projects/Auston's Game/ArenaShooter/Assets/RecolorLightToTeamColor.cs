using UnityEngine;
using System.Collections;

public class RecolorLightToTeamColor : MonoBehaviour
{
    public AffiliatedObject affiliatedObject;
    public int teamIndexOverride = -1;
    private int teamIndex { get { if (teamIndexOverride >= 0) return teamIndexOverride; else if (affiliatedObject != null) return affiliatedObject.team; else return -1; } }

    public Vector3 colorOffset = Vector3.zero;
    //public bool offsetUsesHSV = false;

    // Use this for initialization
    void Start()
    {
        if (affiliatedObject == null)
            affiliatedObject = GetComponent<AffiliatedObject>();
        if (affiliatedObject == null)
            affiliatedObject = transform.root.GetComponent<AffiliatedObject>();

        Color _teamColor = new Color(1, 0, 1, 1);
        if (teamIndex >= 0)
            _teamColor = PlayerManager.instance.playerColors[teamIndex];

        Light _light = GetComponent<Light>();

        _teamColor.r += colorOffset.x;
        _teamColor.g += colorOffset.y;
        _teamColor.b += colorOffset.z;

        _light.color = _teamColor;
    }
}
