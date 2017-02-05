using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class RecolorParticlesToTeamColor : MonoBehaviour
{
    public AffiliatedObject affiliatedObject;
    public int teamIndexOverride = -1;
    private int teamIndex { get { if (teamIndexOverride >= 0) return teamIndexOverride; else if (affiliatedObject != null) return affiliatedObject.team; else return -1; } }
    public bool preserveAlpha = true;

    public Vector3 colorOffset = Vector3.zero;
    //public bool offsetUsesHSV = false;

    // Use this for initialization
    void Start ()
    {
        if (affiliatedObject == null)
            affiliatedObject = GetComponent<AffiliatedObject>();
        if(affiliatedObject == null)
            affiliatedObject = transform.root.GetComponent<AffiliatedObject>();

        Color _teamColor = new Color(1, 0, 1, 1);
        if (teamIndex >= 0)
            _teamColor = PlayerManager.instance.playerColors[teamIndex];

        ParticleSystem _particleSystem = GetComponent<ParticleSystem>();
        if (preserveAlpha)
            _teamColor.a = _particleSystem.startColor.a;

        _teamColor.r += colorOffset.x;
        _teamColor.g += colorOffset.y;
        _teamColor.b += colorOffset.z;

        _particleSystem.startColor = _teamColor;
    }
}