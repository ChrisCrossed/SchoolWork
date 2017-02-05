using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class RecolorToTeamColor : MonoBehaviour
{
    public int colorIndex = -1;
    public Vector3 colorOffset = Vector3.zero;
    public bool useRootPlayerColor = false;
    public bool useOtherPlayerColor = false;

	// Use this for initialization
	void Start ()
    {
        Player _player = transform.root.GetComponent<Player>();

        if (_player == null)
            return;

        if (useRootPlayerColor)
            colorIndex = _player.index;

        else if (useOtherPlayerColor)
            colorIndex = System.Convert.ToInt32(!System.Convert.ToBoolean(_player.index));

        if (colorIndex < 0)
            colorIndex = transform.root.GetComponent<AffiliatedObject>().team;

        if (colorIndex < 0)
            return;

        GetComponent<Graphic>().color = PlayerManager.PlayerColors[colorIndex] + new Color(colorOffset.x, colorOffset.y, colorOffset.z, 0);
    }
}
